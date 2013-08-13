using UnityEngine;
using System.Collections;

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
	public int gunOverheatLvl = 100;
	
	// Use this for initialization
	void Start() 
	{
		inGame = true;
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
			if(Input.GetButton("ShowScoreboard") || Input.GetButton("ShowScoreboard2"))
			{
				paused = true;	
			}
			else paused = false;
			
			//Unlock the mouse if we're paused:
			if(inGame && paused) Screen.lockCursor = false;
				else Screen.lockCursor = true;
			
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
					myGunString = "Rocket Launcher";
					break;
				
				case(3) :
					myGunString = "Machine Gun";
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
				GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 70, 25), gunOverheatLvl + "%");
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
					
					//End headers:
					GUILayout.EndHorizontal();
					
				}else{
					Debug.Log("No scoreboard for game mode " + theNetwork.gameMode + ".");
				}
				//End Scoreboard:
				GUILayout.EndArea();
			}
			
			
		}		
	}
}
