using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public int playerNumber = 8;
	public int gamePort = 25000;
	
	
	private const string typeName = "WALL";
	private const string gameName = "RoomName";
	
		
	private GameController gameController;
	
	public void Awake(){
	gameController = GetComponent<GameController>();	
	}
	
	private void StartServer()
	{
	    Network.InitializeServer(playerNumber, gamePort, !Network.HavePublicAddress());
	    MasterServer.RegisterHost(typeName, gameName);
		
	}
	
	void OnServerInitialized()
	{
	    gameController.selectPlayer();
	}
	
	void OnConnectedToServer()
	{
	    OnServerInitialized();
	}
	
	void OnGUI()
	{
	    if (!Network.isClient && !Network.isServer)
	    {
	        if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
	            StartServer();
	 
	        if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
	            RefreshHostList();
	 
	        if (hostList != null)
	        {
	            for (int i = 0; i < hostList.Length; i++)
	            {
	                if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
	                    JoinServer(hostList[i]);
	            }
	        }
	    }
	}
	
	private HostData[] hostList;
 
	private void RefreshHostList()
	{
	    MasterServer.RequestHostList(typeName);
	}
	 
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
	    if (msEvent == MasterServerEvent.HostListReceived)
	        hostList = MasterServer.PollHostList();
	}
	
	private void JoinServer(HostData hostData)
	{
	    Network.Connect(hostData);
	}
	 
	
	
}
