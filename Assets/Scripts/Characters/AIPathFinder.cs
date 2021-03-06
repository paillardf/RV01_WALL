﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

/** AI for following paths.
 * This AI is the default movement script which comes with the A* Pathfinding Project.
 * It is in no way required by the rest of the system, so feel free to write your own. But I hope this script will make it easier
 * to set up movement for the characters in your game. This script is not written for high performance, so I do not recommend using it for large groups of units.
 * \n
 * \n
 * This script will try to follow a target transform, in regular intervals, the path to that target will be recalculated.
 * It will on FixedUpdate try to move towards the next point in the path.
 * However it will only move in the forward direction, but it will rotate around it's Y-axis
 * to make it reach the target.
 * 
 * \section variables Quick overview of the variables
 * In the inspector in Unity, you will see a bunch of variables. You can view detailed information further down, but here's a quick overview.\n
 * The #repathRate determines how often it will search for new paths, if you have fast moving targets, you might want to set it to a lower value.\n
 * The #target variable is where the AI will try to move, it can be a point on the ground where the player has clicked in an RTS for example.
 * Or it can be the player object in a zombie game.\n
 * The speed is self-explanatory, so is turningSpeed, however #slowdownDistance might require some explanation.
 * It is the approximate distance from the target where the AI will start to slow down. Note that this doesn't only affect the end point of the path
 * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this.\n
 * #pickNextWaypointDist is simply determines within what range it will switch to target the next waypoint in the path.\n
 * #forwardLook will try to calculate an interpolated target point on the current segment in the path so that it has a distance of #forwardLook from the AI\n
 * Below is an image illustrating several variables as well as some internal ones, but which are relevant for understanding how it works.
 * Note that the #forwardLook range will not match up exactly with the target point practically, even though that's the goal.
 * \shadowimage{aipath_variables.png}
 * This script has many movement fallbacks.
 * If it finds a NavmeshController, it will use that, otherwise it will look for a character controller, then for a rigidbody and if it hasn't been able to find any
 * it will use Transform.Translate which is guaranteed to always work.
 */
[RequireComponent(typeof(Seeker))]

public class AIPathFinder : MonoBehaviour {

	/** Determines how often it will search for new paths. 
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
	public float repathRate = 0.5F;
	
	/** Target to move towards.
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
	public Transform target;
	
	
	
	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
	public bool canSearch = true;
	
	/** Enables or disables movement.
	  * \see #canSearch */
	public bool canMove = true;
	
	/** Maximum velocity.
	 * This is the maximum speed in world units per second.
	 */
	private float speed = 3;

	public float normalSpeed = 3;
	public float slowSpeed = 1;

	
	public float deadZone = 5f;             // The number of degrees for which the rotation isn't controlled by Mecanim.
	
	/** Rotation speed.
	 * Rotation is calculated using Quaternion.SLerp. This variable represents the damping, the higher, the faster it will be able to rotate.
	 */
	public float turningSpeed = 5;
	
	/** Distance from the target point where the AI will start to slow down.
	 * Note that this doesn't only affect the end point of the path
 	 * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this
 	 */
	public float slowdownDistance = 0.6F;
	
	/** Determines within what range it will switch to target the next waypoint in the path */
	public float pickNextWaypointDist = 1F;
	public float pickNextWaypointBlockDist = 0.2F;
	public float pickNextWaypointDistY = 0.2f;
		
	/** Target point is Interpolated on the current segment in the path so that it has a distance of #forwardLook from the AI.
	  * See the detailed description of AIPath for an illustrative image */
	public float forwardLook = 1;
	
	/** Distance to the end point to consider the end of path to be reached.
	 * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
	 */
	public float endReachedDistance = 0.2F;
	
	/** Do a closest point on path check when receiving path callback.
	 * Usually the AI has moved a bit between requesting the path, and getting it back, and there is usually a small gap between the AI
	 * and the closest node.
	 * If this option is enabled, it will simulate, when the path callback is received, movement between the closest node and the current
	 * AI position. This helps to reduce the moments when the AI just get a new path back, and thinks it ought to move backwards to the start of the new path
	 * even though it really should just proceed forward.
	 */
	public bool closestOnPathCheck = true;
	
	protected float minMoveScale = 0.05F;
	
	/** Cached Seeker component */
	protected Seeker seeker;
	
	/** Cached Transform component */
	protected Transform tr;
	
	/** Time when the last path request was sent */
	private float lastRepath = -9999;
	
	/** Current path which is followed */
	protected Path path;
	
	/** Cached CharacterController component */
	protected CharacterController controller;
	
	/** Cached NavmeshController component */
	protected NavmeshController navController;
	
	
	/** Cached Rigidbody component */
	protected Rigidbody rigid;
	
	/** Current index in the path which is current target */
	protected int currentWaypointIndex = 0;
	
	/** Holds if the end-of-path is reached
	 * \see TargetReached */
	protected bool targetReached = false;
	
	/** Only when the previous path has been returned should be search for a new path */
	protected bool canSearchAgain = true;
	
	/** Returns if the end-of-path has been reached
	 * \see targetReached */
	public bool TargetReached {
		get {
			return targetReached;
		}
	}
	
	/** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
	private bool startHasRun = false;
	
	/** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	  * */
	protected virtual void Awake () {
		seeker = GetComponent<Seeker>();
		
		//This is a simple optimization, cache the transform component lookup
		tr = transform;
		
		//Make sure we receive callbacks when paths complete
		seeker.pathCallback += OnPathComplete;
		
		//Cache some other components (not all are necessarily there)
		controller = GetComponent<CharacterController>();
		navController = GetComponent<NavmeshController>();
		rigid = rigidbody;
		deadZone *= Mathf.Deg2Rad;
	}
	
	/** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see OnEnable
	 * \see RepeatTrySearchPath
	 */
	protected virtual void Start () {
		startHasRun = true;
		OnEnable ();
	}
	
	private Vector3[] updateTargets(){
		GameObject[] targetTab = GameObject.FindGameObjectsWithTag("Player");
		ArrayList targets = new ArrayList();
		for(int i = 0; i < targetTab.Length; i++){
			if(!targets.Contains(targetTab[i])){
				targets.Add(targetTab[i]);
			}
		}
		
		targetTab = GameObject.FindGameObjectsWithTag("Target");
		for(int i = 0; i < targetTab.Length; i++){
			if(!targets.Contains(targetTab[i])){
				targets.Add(targetTab[i]);
			}
		}

		Vector3[] endPoints = new Vector3[targets.Count];

        
		for(int c = 0; c<targets.Count; c++){
			endPoints[c] = ((GameObject) (targets[c])).transform.position;
		}
        
		return endPoints;
				
		/*
		
		if(targetIndex<targets.Count){
			target = ((GameObject) (targets[targetIndex])).transform;
		}else{
			if(newTargetSearch){ // no target atteignable
				Collider[] hitColliders = Physics.OverlapSphere(transform.position, 40, Constants.MaskBlock);
				if(hitColliders.Length>0) {
					
					target = hitColliders[0].transform;
					
						
					
				}
			}
			newTargetSearch = true;	
			targetIndex=0;
		}
		*/
		
	}
	
	/** Run at start and when reenabled.
	 * Starts RepeatTrySearchPath.
	 * 
	 * \see Start
	 */
	protected virtual void OnEnable () {
		if (startHasRun) StartCoroutine (RepeatTrySearchPath ());
	}
	
	/** Tries to search for a path every #repathRate seconds.
	  * \see TrySearchPath
	  */
	public IEnumerator RepeatTrySearchPath () {
		while (true) {
			TrySearchPath ();
			yield return new WaitForSeconds (repathRate);
		}
	}
	
	/** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true.
	 * Otherwise will start WaitForPath function.
	 */
	public void TrySearchPath () {
		if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch) {
			SearchPath ();
		} else {
			StartCoroutine (WaitForRepath ());
		}
	}
	
	/** Is WaitForRepath running */
	private bool waitingForRepath = false;
	
	/** Wait a short time til Time.time-lastRepath >= repathRate.
	  * Then call TrySearchPath
	  * 
	  * \see TrySearchPath
	  */
	protected IEnumerator WaitForRepath () {
		if (waitingForRepath) yield break; //A coroutine is already running
		
		waitingForRepath = true;
		//Wait until it is predicted that the AI should search for a path again
		yield return new WaitForSeconds (repathRate - (Time.time-lastRepath));
		
		waitingForRepath = false;
		//Try to search for a path again
		TrySearchPath ();
	}
	
	/** Requests a path to the target */
	public virtual void SearchPath () {
		
		//if (target == null) { Debug.Log ("Target is null, aborting all search"); canSearch = false; return; }
		
		lastRepath = Time.time;
		//This is where we should search to
		//Vector3 targetPosition = target.position;
		
		canSearchAgain = false;
		
		//Alternative way of requesting the path
		//Path p = PathPool<Path>.GetPath().Setup(GetFeetPosition(),targetPoint,null);
		//seeker.StartPath (p);
		
		//We should search from the current position
		seeker.StartMultiTargetPath(GetFeetPosition(), updateTargets() ,true);//.StartPath (GetFeetPosition(), targetPosition);
	}
	
	public virtual void OnTargetReached () {
		//End of path has been reached
		//If you want custom logic for when the AI has reached it's destination
		//add it here
		//You can also create a new script which inherits from this one
		//and override the function in that script
	}
	
	public void OnDestroy () {
		if (path != null) path.Release (this);
	}
	
	/** Called when a requested path has finished calculation.
	  * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	  * Finally it is returned to the seeker which forwards it to this function.\n
	  */
	public virtual void OnPathComplete (Path p) {
		
		
        
        if (p.error) {
            Debug.Log ("Ouch, the path returned an error\nError: "+p.errorLog);
			canSearchAgain = true;
			path = null;
			return;
        }
        
        MultiTargetPath mp = p as MultiTargetPath;
        if (mp == null) {
            Debug.LogError ("The Path was no MultiTargetPath");
          	path = null;
			canSearchAgain = true; return;
        }
        
        //All paths
        List<Vector3>[] paths = mp.vectorPaths;
        bool noPath = true;
		for (int i=0;i<paths.Length;i++) {
            //Plotting path i
            List<Vector3> mpath = paths[i];
            
            if (mpath == null) {
                Debug.Log ("Path number "+i+" could not be found");
                continue;
            }
			
			//Claim the new path
			mp.Claim (this);
		
			//Replace the old path
			path = mp;
		
			//mpset some variables
			currentWaypointIndex = 0;
			targetReached = false;
			canSearchAgain = true;
		
			noPath = false;
		//next row can be used to find out if the path could be found or not
		//If it couldn't (error == true), then a message has probably been logged to the console
		//however it can also be got using p.errorLog
		//if (p.error)
		
			if (closestOnPathCheck) {
				Vector3 p1 = mp.startPoint;
				Vector3 p2 = GetFeetPosition ();
				float magn = Vector3.Distance (p1,p2);
				Vector3 dir = p2-p1;
				dir /= magn;
				int steps = (int)(magn/pickNextWaypointDist);
				for (int j=0;j<steps;j++) {
					CalculateVelocity (p1);
					p1 += dir;
				}
			}
			break;
 
            
        }
		
		if(noPath){
			path = null;	
		}
		
		/*p = _p as ABPath;
		if (p == null) throw new System.Exception ("This function only handles ABPaths, do not use special path types");
		
		//Release the previous path
		if (path != null) path.Release (this);
		*/
		
		
	}
	
	public virtual Vector3 GetFeetPosition () {
		//if (controller != null) {
		//	print ("tr.position"+tr.position);
		//	return tr.position - Vector3.up*controller.height*0.5F;
		//}
	
		return tr.position;
	}
	
	public virtual void Update () {
		
		if (!canMove) { return; }
		
		Vector3 dir = CalculateVelocity (GetFeetPosition());
		if (targetDirection != Vector3.zero) {
			RotateTowards (targetDirection);
		}
	
		
		
		if (navController != null) {
			navController.SimpleMove (GetFeetPosition(),dir);
		} else if (controller != null) {
			controller.SimpleMove (dir);
		} else if (rigid != null) {
			rigid.AddForce (dir);
		} else {
			transform.Translate (dir*Time.deltaTime, Space.World);
		}
	}
	
	/** Point to where the AI is heading.
	  * Filled in by #CalculateVelocity */
	protected Vector3 targetPoint;
	/** Relative direction to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	protected Vector3 targetDirection;
	
	protected float XZSqrMagnitude (Vector3 a, Vector3 b) {
		float dx = b.x-a.x;
		float dz = b.z-a.z;
		return dx*dx + dz*dz;
	}
	
	/** Calculates desired velocity.
	 * Finds the target path segment and returns the forward direction, scaled with speed.
	 * A whole bunch of restrictions on the velocity is applied to make sure it doesn't overshoot, does not look too far ahead,
	 * and slows down when close to the target.
	 * /see speed
	 * /see endReachedDistance
	 * /see slowdownDistance
	 * /see CalculateTargetPoint
	 * /see targetPoint
	 * /see targetDirection
	 * /see currentWaypointIndex
	 */
	protected Vector3 CalculateVelocity (Vector3 currentPosition) {
		Vector3 targetPosition;
		Vector3 dir;
		speed = normalSpeed;
		if (path == null || path.vectorPath == null || path.vectorPath.Count == 0){
			Collider[] hitColliders = Physics.OverlapSphere(transform.position+transform.up, 1f, Constants.MaskBlock);
			if(hitColliders.Length>0) {	
				targetPosition = hitColliders[0].transform.position;	
					
			}else{
				GameObject g = GameObject.FindGameObjectWithTag("Player");
				if(g!=null){
					targetPosition = g.transform.position;
				}else{
					return Vector3.zero;
				}
			}
			
			
		} else{
			
			List<Vector3> vPath = path.vectorPath;
			//Vector3 currentPosition = GetFeetPosition();
			
			if (vPath.Count == 1) {
				vPath.Insert (0,currentPosition);
			}
			
			if (currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count-1; }
			
			if (currentWaypointIndex <= 1) currentWaypointIndex = 1;
			
			while (true) {
				if (currentWaypointIndex < vPath.Count-1) {
					//There is a "next path segment"
					float dist = XZSqrMagnitude (vPath[currentWaypointIndex], currentPosition);
						//Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
					
					float nextWD = pickNextWaypointDist;
					RaycastHit hit = new RaycastHit();
					int mask  = Constants.MaskGround;
					Ray ray = new Ray(tr.position, -tr.up);
					if (!Physics.Raycast (ray, out hit ,0.1F, mask)){
						nextWD = pickNextWaypointBlockDist;
						speed = slowSpeed;
					}
					
					float distY = Mathf.Abs(vPath[currentWaypointIndex].y-currentPosition.y);
					
					
					if (distY<pickNextWaypointDistY&&dist< nextWD*nextWD) {
						currentWaypointIndex++;
					} else {
						break;
					}
				} else {
					break;
				}
			}
			targetPosition = vPath[currentWaypointIndex];
			dir = targetPosition-currentPosition;
			dir.y = 0;
			
			if (currentWaypointIndex == vPath.Count-1 && dir.magnitude <= endReachedDistance) {
			if (!targetReached) { targetReached = true; OnTargetReached (); }
			
			//Send a move request, this ensures gravity is applied
			return Vector3.zero;
		}
		
			
		}
		// = vPath[currentWaypointIndex] - vPath[currentWaypointIndex-1];
		
		//Vector3 targetPosition = CalculateTargetPoint (currentPosition,vPath[currentWaypointIndex-1] , vPath[currentWaypointIndex]);
			//vPath[currentWaypointIndex] + Vector3.ClampMagnitude (dir,forwardLook);
		
		
		dir = targetPosition-currentPosition;
		
		float verticalDir = dir.y;
		dir.y = 0;
		float targetDist = dir.magnitude;
		
		float slowdown = Mathf.Clamp01 (targetDist / slowdownDistance);
		
		this.targetDirection = dir;
		this.targetPoint = targetPosition;
		
		
		Vector3 forward = tr.forward;
		float dot = Vector3.Dot (dir.normalized,forward);




		float sp = speed * Mathf.Max (dot,minMoveScale) * slowdown;
		
		
		if (Time.deltaTime	> 0) {
			sp = Mathf.Clamp (sp,0,targetDist/(Time.deltaTime*2));
		}
		Vector3 result = forward*sp;
		result.y = verticalDir;
		return result;
	}
	
	protected Vector3 CalculateTargetPoint (Vector3 p, Vector3 a, Vector3 b) {
		a.y = p.y;
		b.y = p.y;
		
		float magn = (a-b).magnitude;
		if (magn == 0) return a;
		
		float closest = Mathfx.Clamp01 (Mathfx.NearestPointFactor (a, b, p));
		Vector3 point = (b-a)*closest + a;
		float distance = (point-p).magnitude;
		
		float lookAhead = Mathf.Clamp (forwardLook - distance, 0.0F, forwardLook);
		
		float offset = lookAhead / magn;
		offset = Mathf.Clamp (offset+closest,0.0F,1.0F);
		return (b-a)*offset + a;
	}
	
	/** Rotates in the specified direction.
	 * Rotates around the Y-axis.
	 * \see turningSpeed
	 */
	protected virtual float RotateTowards (Vector3 dir) {
		Quaternion rot = tr.rotation;
		Quaternion toTarget = Quaternion.LookRotation (dir);
		
		rot = Quaternion.Slerp (rot,toTarget,turningSpeed*Time.fixedDeltaTime);
		Vector3 euler = rot.eulerAngles;
		euler.z = 0;
		euler.x = 0;
		rot = Quaternion.Euler (euler);
		
		tr.rotation = rot;
		return euler.y;
	}
	
	protected float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        // If the vector the angle is being calculated to is 0...
        if(toVector == Vector3.zero)
            // ... the angle between them is 0.
            return 0f;
        
        // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
        float angle = Vector3.Angle(fromVector, toVector);
        
        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        
        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        
        // We need to convert the angle we've found from degrees to radians.
        angle *= Mathf.Deg2Rad;

        return angle;
    }
	
	
}
