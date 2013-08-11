using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

	//This is updated from the FPS Entity that holds this script
	//If the entity isLocal then it sets this to true:
	public bool isLocal = false;
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
	
	// Use this for initialization
	void Start() 
	{
		inGame = true;
		myController = GetComponent<FPSController>();
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
			}
			//Display score and pause menu:
			else if(inGame && paused)
			{
				
			}
			
			
		}		
	}
}
