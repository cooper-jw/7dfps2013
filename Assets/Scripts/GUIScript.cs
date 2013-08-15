using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIScript : MonoBehaviour {

	//This is updated from the FPS Entity that holds this script
	//If the entity isLocal then it sets this to true:
	public bool isLocal = false;
	//Network Variables:
	private GameObject networkHolder;
	private NetworkScript theNetwork;
	//Crosshair texture:
	public Texture crosshair;
	
	//FPS Controller:
	public FPSController myController;
	
	//Control Booleans:
	public bool inGame = false;
	public bool paused = false;
	public bool showScore = false;
	
	public int myGun = 0;
	public string myGunString = " ";
	public int overheatInt;
	public int healthInt;
	
	//Chat Variables:
	public List<ChatMessage> messages = new List<ChatMessage>();
	public float chatFadeTime = 7.5f;
	public float textDisplayTime = -100f;
	public bool isWriting = false;
	public string myMessage = "";
	
	// Use this for initialization
	void Start() 
	{
		myController = GetComponent<FPSController>();
		//Network shit:
		networkHolder = GameObject.FindGameObjectWithTag("Network");
		theNetwork = networkHolder.GetComponent<NetworkScript>();
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(isLocal)
		{
			inGame = theNetwork.connected;
			
			if(isWriting || paused)
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				myController.canControl = false;
			}
			else 
			{
				Screen.showCursor = false;
				Screen.lockCursor = true;
				myController.canControl = true;
			}
			
			
			if(Input.GetKeyDown("t") && !isWriting)
			{
				isWriting = true;
			}
			
			if(Input.GetKeyDown("escape"))
			{
				isWriting = false;
			}
				
			if(Input.GetButton("ShowScoreboard"))
			{
				paused = true;	
			}
			else paused = false;
			
			//Unlock the mouse if we're paused:
			if(inGame && paused) Screen.lockCursor = false;
				else if(!isWriting) Screen.lockCursor = true;
			//Get overheat level:
			overheatInt = Mathf.RoundToInt(myController.overheat);
			//Get health level:
			healthInt = Mathf.RoundToInt(myController.currentHealth);
			
			//Get current gun:
			myGun = myController.currentGun;
			
			//Set gun string:
			switch(myGun)
			{
				case(0) :
					myGunString = "Nothing! ;_;";
					break;
				
				case(1) :
					myGunString = "Pistol";
					break;
				
				case(2) :
					myGunString = "Machine Gun";
					break;
				
				case(3) :
					myGunString = "Rocket Launcher";
					break;
				
				case(4) :
					myGunString = "Shotgun";
					break;
			}
		}
	}
	
	void OnGUI()
	{
		if(isLocal)
		{
			//If we're playing:
			if(inGame && !paused)
			{
				//Display crosshair:
				GUI.DrawTexture(new Rect((Screen.width/2)-8, (Screen.height/2)-8, 16, 16), crosshair);
				//Dipslay gun type:
				GUI.Label(new Rect((Screen.width/2) - 100, 15, 200, 30), myGunString);
				//Display overheat level:
				GUI.Label(new Rect(Screen.width - 140, Screen.height - 65, 130, 25), "Overheat Level:");
				GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 70, 25), overheatInt + "%");
				//Display health:
				GUI.Label(new Rect(10, 10, 150, 30), healthInt.ToString());
				//Display chat messages:
				if(Time.time < textDisplayTime)
				{
					for(int i = 1; i < 15; i++)
					{
						if(i < messages.Count)
						{
							GUI.Label(new Rect(10,Screen.height - 55 - (i*15), 700, 20), messages[messages.Count-i].sender + " " + messages[messages.Count-i].message);
						}
					}
				}
				//Write chat messages:
				if(isWriting)
				{
					Screen.lockCursor = false;
					Screen.showCursor = true;
					GUI.SetNextControlName("ChatBox");
					myMessage = GUI.TextField(new Rect(10,Screen.height - 35, 500, 20), myMessage);
					GUI.FocusControl("ChatBox");
					if(GUI.Button(new Rect(520, Screen.height - 35, 60, 20), "Send"))
					{
						isWriting = false;
						if(myMessage != "" && theNetwork.connected)
						{
							theNetwork.SendChatMessage(theNetwork.playerName, myMessage);
						}
						myMessage = "";
					}
				}else if(!paused){
					Screen.lockCursor = true;
					Screen.showCursor = false;
				}
			}
			//Display score and pause menu:
			else if(inGame && paused)
			{
				//Disconnect Button:
				if(GUI.Button(new Rect(10, 10, 150, 25), "Disconnect"))
				{
					theNetwork.DisconnectMe();
				}
				//Scoreboard:
				if(GUI.Button(new Rect(10, 45, 150, 25), "Exit to Windows"))
				{
					theNetwork.DisconnectMe();
					Application.Quit();
				}
				
				//Scoreboard:
				GUILayout.BeginArea(new Rect(170, 20, Screen.width - 190, Screen.height - 40));
				//Free for all scores:
				if(theNetwork.gameMode == 0)
				{
					//Headers:
					GUILayout.BeginHorizontal();
					GUILayout.Label("Player:");
					GUILayout.Label("Kills:");
					GUILayout.Label("Deaths:");
					//If you're the host:
					if(theNetwork.isServer)
						//Add a kick colum:
						GUILayout.Label("Kick:");
					//End headers:
					GUILayout.EndHorizontal();
					
					//Populate Scoreboard:
					for(int i = 0; i < theNetwork.fpsEntities.Count; i++)
					{
						GUILayout.BeginHorizontal();
						
						GUILayout.Label(theNetwork.fpsEntities[i].myName);
						GUILayout.Label(theNetwork.fpsEntities[i].kills.ToString());
						GUILayout.Label(theNetwork.fpsEntities[i].deaths.ToString());
						
						
						if(theNetwork.isServer)
						{
							if(theNetwork.fpsEntities[i].viewID != theNetwork.netviewID)
							{
								if(GUILayout.Button("Kick"))
								{
									Debug.Log("Kicking player " + theNetwork.fpsEntities[i].myName + "/" + theNetwork.fpsEntities[i].viewID.owner.ipAddress.ToString());
									theNetwork.Kick(i);
								}
							}else{
								GUILayout.Label("Can't Kick!");	
							}
						}
						
						GUILayout.EndHorizontal();
					}
					
				}else{
					Debug.Log("No scoreboard for game mode " + theNetwork.gameMode + ".");
				}
				//End Scoreboard:
				GUILayout.EndArea();
			}
			
			
		}		
	}
}
