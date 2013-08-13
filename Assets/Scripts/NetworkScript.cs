using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkScript:MonoBehaviour 
{
	public bool isServer = false;
	public string uniqueGameName = "KDC7dfps2013";
	public string serverName;
	public bool connected = false;
	public string playerName;
	public string skinURL;
	public bool hasSkin;
	public GameObject fpsEntityPrefab;
	public List<FPSController> fpsEntities = new List<FPSController>();
	public NetworkViewID netviewID;
	public bool preppingLevel = false;
	public bool levelLoaded;
	public string levelString;
	public int lastLevelPrefix = 0;
	public bool useNat;
	public int port = 2300;
	public int gameMode = 0;
	public string gameModeString;
	public int level = 0;
	public int maxPlayers;
	public string comment;
	public HostData[] hostData;
	public bool hasRegistered = false;
	public bool hasSetGUI = false;
	public GUIScript myGUI;
	
	// Use this for initialization
	void Start() 
	{
		DontDestroyOnLoad(this);
		
		//Get Servers:
		MasterServer.RequestHostList(uniqueGameName);
		hostData = MasterServer.PollHostList();
		
		if(PlayerPrefs.GetString("skinURL") != null || PlayerPrefs.GetString("skinURL") != " ")
		{
			skinURL = PlayerPrefs.GetString("playerSkinURL");
			hasSkin = true;
		}else{
			hasSkin = false;
		}
		
		if(PlayerPrefs.GetString("playerName") != null || PlayerPrefs.GetString("playerName") != " ")
		{
			playerName = PlayerPrefs.GetString("playerName");	
		}
		else
		{
			playerName = "DefaultName";
			PlayerPrefs.SetString("playerName", "DefaultName");
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(levelLoaded && !hasSetGUI)
		{
			for(int i = 0; i < fpsEntities.Count; i++)
			{
				if(fpsEntities[i].viewID == netviewID)
				{
					myGUI = fpsEntities[i].gameObject.GetComponent<GUIScript>();	
				}
			}
			hasSetGUI = true;
		}
	}
	
	public void Connect(HostData connectData)
	{
		Debug.Log("Connecting to " + connectData.ToString() + ".");
		Network.Connect(connectData);
	}
	
	public void StartServer(string name, int max, int gm, int map)
	{
		serverName = name;
		level = map;
		gameMode = gm;
		maxPlayers = max;
		useNat = !Network.HavePublicAddress();
		Network.InitializeServer(max, port, useNat);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log ("Connected to server.");
		connected = true;
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server initialized.");
		switch(level)
		{
			case(0):
				levelString = "TestLevel";
				break;
		}
		switch(gameMode)
		{
			case(0):
				gameModeString = "FFA";
				break;
		}
		isServer = true;
		connected = true;
		comment = levelString + "|" + gameModeString;
		MasterServer.RegisterHost(uniqueGameName, serverName, comment);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.RegistrationSucceeded && !hasRegistered && isServer && connected)
		{
			Debug.Log("Server Registration Succeeded");
			networkView.RPC( "LoadLevel", RPCMode.AllBuffered, level, lastLevelPrefix + 1);
			lastLevelPrefix++;
			hasRegistered = true;
		}
		else if(msEvent == MasterServerEvent.RegistrationFailedNoServer || msEvent == MasterServerEvent.RegistrationFailedGameType)
		{
			Network.Disconnect();	
		}
	}
	
	[RPC]
	void LoadLevel(int level, int levelPrefix)
	{
		switch(level)
		{
			case(0):
				levelString = "TestLevel";
				break;
		}
		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = true;
		Network.SetLevelPrefix(levelPrefix);
		preppingLevel = true;
		Application.LoadLevel(levelString);
		//Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
	}
	
	void OnLevelWasLoaded()
	{
		if(connected)
		{
			Debug.Log("Level " + levelString + " was loaded.");
			levelLoaded = true;
			preppingLevel = false;
			netviewID = Network.AllocateViewID();
			InstantiateFPSEntity(true, netviewID, playerName, hasSkin, skinURL);
			networkView.RPC("NewPlayer", RPCMode.OthersBuffered, netviewID, playerName, hasSkin, skinURL);			
		}else{
			Debug.Log("The main menu was loaded.");
		}
	}
	
	[RPC]
	void NewPlayer(NetworkViewID viewID, string name, bool hasSkin, string skinURL)
	{
		InstantiateFPSEntity(false, viewID, name, hasSkin, skinURL);
	}
	
	void InstantiateFPSEntity(bool isLocal, NetworkViewID anID, string name, bool hasSkin, string skinURL)
	{
		if(levelLoaded)
		{
			Debug.Log("Make a new player, isLocal " + isLocal);
			GameObject newEntity = (GameObject)GameObject.Instantiate(fpsEntityPrefab);
			FPSController entity = newEntity.GetComponent<FPSController>();
			entity.hasSkin = hasSkin;
			entity.skinURL = skinURL;
			entity.myName = name;
			
			if(isLocal)
			{
				entity.viewID = netviewID;
				entity.isLocal = true;
			}else{
				entity.isLocal = false;
				entity.viewID = anID;	
			}
			
			fpsEntities.Add(entity);
		}else{
			StartCoroutine(WaitMakeNewPlayer(isLocal, anID, name, hasSkin, skinURL));
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		if(connected)
		{
			Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);	
		}
	}
	
	//Send my player shit:
	public void SendPlayer(NetworkViewID viewID, Vector3 pos, Vector3 ang, Vector3 moveVec, int myKills, int myDeaths)
	{
		if(connected)
		{
			//Send everyone else an RPC with my shit:
			networkView.RPC("SendPlayerRPC", RPCMode.Others, viewID, pos, ang, moveVec, myKills, myDeaths);	
		}	
	}
	
	//Recive player information:
	[RPC]
	void SendPlayerRPC(NetworkViewID viewID, Vector3 pos, Vector3 ang, Vector3 moveVec, int kills, int deaths)
	{
		if(connected)
		{
			for(int i = 0; i < fpsEntities.Count; i++)
			{
				if(viewID == fpsEntities[i].viewID)
				{
					//Update this players values:
					fpsEntities[i].UpdatePlayer(pos, ang, moveVec, kills, deaths);
				}
			}
		}
	}
	
	public void RefreshServers()
	{
		//Get Servers:
		Debug.Log("Refreshing servers.");
		MasterServer.RequestHostList(uniqueGameName);
		hostData = MasterServer.PollHostList();
	}
	
	void OnApplicationQuit()
	{
		if(connected)
		{
			if(isServer)
			{
				networkView.RPC("ServerLeave", RPCMode.OthersBuffered);
			}else{
				networkView.RPC("PlayerLeave", RPCMode.OthersBuffered, netviewID);
			}
			if(isServer) MasterServer.UnregisterHost();
			Network.Disconnect();
		}
	}
	
	[RPC]
	void PlayerLeave(NetworkViewID viewID)
	{
		Debug.Log("Player " + viewID.ToString() + " has left.");
		for(int i = 0; i < fpsEntities.Count; i++)
		{
			if(fpsEntities[i].viewID == viewID)
			{
				Destroy(fpsEntities[i].gameObject);
				fpsEntities.RemoveAt(i);
			}
		}
	}
	
	[RPC]
	void ServerLeave()
	{
		
		if(isServer) MasterServer.UnregisterHost();
		else Debug.Log("Server disconnected D:");
		connected = false;
		isServer = false;
		Network.Disconnect();
	}
	
	void OnDisconnectedFromServer()
	{
		Debug.Log("Loading main menu.");
		for(int i = 0; i < fpsEntities.Count; i++)
		{
			Destroy(fpsEntities[i].gameObject);
			fpsEntities.RemoveAt(i);
		}
		hasSetGUI = false;
		myGUI = null;
		isServer = false;
		connected = false;
		levelLoaded = false;
		fpsEntities = new List<FPSController>();
		hasRegistered = false;
		
		Application.LoadLevel("Menu");	
	}
	
	public void Kick(int playerIndex)
	{
		Network.CloseConnection(fpsEntities[playerIndex].viewID.owner, false);
		networkView.RPC("KickedPlayer", RPCMode.AllBuffered, fpsEntities[playerIndex].viewID);
	}
	
	[RPC]
	void KickedPlayer(NetworkViewID viewID)
	{
		string kickedName = "???";
		
		for(int i = 0; i < fpsEntities.Count; i++)
		{
			if(fpsEntities[i].viewID == viewID)
			{
				kickedName = fpsEntities[i].myName;
				if(fpsEntities[i] != null) Destroy(fpsEntities[i].gameObject);
				fpsEntities.RemoveAt(i);
			}
		}
		//Send chat message saying "name" was kicked:
		SendChatMessage("_SERVER", kickedName + " was kicked by the host.");
	}
	
	public void DisconnectMe()
	{
		if(connected)
		{
			Debug.Log("Disconnecting from server.");
			if(isServer)
			{
				networkView.RPC("ServerLeave", RPCMode.OthersBuffered);
			}else{
				networkView.RPC("PlayerLeave", RPCMode.OthersBuffered, netviewID);
			}
			if(isServer) MasterServer.UnregisterHost();
			levelLoaded = false;
			Network.Disconnect();
			connected = false;
		}
		
	}
	
	IEnumerator WaitMakeNewPlayer(bool isLocal, NetworkViewID anID, string name, bool hasSkin, string skinURL)
	{
		Debug.Log("Waiting 5 seconds then trying to make a new player again.");
		yield return new WaitForSeconds(5);
		InstantiateFPSEntity(isLocal, anID, name, hasSkin, skinURL);
	}
	
	//Chat
	public void SendChatMessage(string sender, string msg)
	{
		networkView.RPC("SendMessageRPC", RPCMode.All, sender, msg);
	}
	
	[RPC]
	void SendMessageRPC(string name, string msg)
	{
		Debug.Log("MSG: " + name + " " + msg);
		ChatMessage newMessage = new ChatMessage();
		newMessage.sender = name;
		newMessage.message = msg;
		myGUI.messages.Add(newMessage);
		myGUI.textDisplayTime = Time.time + myGUI.chatFadeTime;
	}
}
