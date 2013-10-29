using UnityEngine;
using System.Collections;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
    public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
    public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
 
   	private Seeker seeker;
    
    //The calculated path
    public Path path;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 1;
 
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
	
	private BotAnimation animController;
    
    void Awake ()
    {
		
		seeker = GetComponent<Seeker>();
        animController = GetComponent<BotAnimation>();
        
        
		
		
   }
    
    RaycastHit hit = new RaycastHit();
    void Update ()
    {
		if (Input.GetKey(KeyCode.Mouse2)){
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit ,40.0f)){
				//Start a new path to the targetPosition, return the result to the OnPathComplete function
        		seeker.StartPath (transform.position,hit.point, OnPathComplete);
			
			}
		}
		
		if(path != null){
			 Chasing();
		}
        
    }
    
    
    public void OnPathComplete (Path p) {
        Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
 
   
       
    
    void Chasing ()
    {
		
	
		
		
         if (path == null) {
            //We have no path to move after yet
            return;
        }
        
        if (currentWaypoint >= path.vectorPath.Count) {
            Debug.Log ("End Of Path Reached");
			animController.SimpleMove (Vector3.zero);
            return;
        }
        
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
        dir *= chaseSpeed;
        animController.SimpleMove (dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
        
    }

    /*
    void Patrolling ()
    {
        // Set an appropriate speed for the NavMeshAgent.
        nav.speed = patrolSpeed;
        
        // If near the next waypoint or there is no destination...
        if(nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        {
            // ... increment the timer.
            patrolTimer += Time.deltaTime;
            
            // If the timer exceeds the wait time...
            if(patrolTimer >= patrolWaitTime)
            {
                // ... increment the wayPointIndex.
                if(wayPointIndex == patrolWayPoints.Length - 1)
                    wayPointIndex = 0;
                else
                    wayPointIndex++;
                
                // Reset the timer.
                patrolTimer = 0;
            }
        }
        else
            // If not near a destination, reset the timer.
            patrolTimer = 0;
        
        // Set the destination to the patrolWayPoint.
        nav.destination = patrolWayPoints[wayPointIndex].position;
    }
    */
}