using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	
	public string currentMenu = "main";
	
	//Button Positions:
	public Rect next;
	public Rect back;
	
	//Menu button variables:
	private int top = 10;
	private int left = 10;
	private int width = 150;
	private int height = 25;
	public int buttonIncrement = 10;
	
	//Main box size:
	public Rect mainBox;
	
	//Player Variables:
	public string playerName;
	public string playerSkinURL;
	
	public string dcIP = "192.0.0.1";
	
	public bool isFullscreen = false;
	public Resolution selectedResolution;
	public Resolution currentResolution;
	public Resolution[] resolutions;
	public int selectedGraphics;
	public int currentGraphics;
	public string isFullscreenString;
	public string curGraphicString;
	private string gLoopString;
	
	// Use this for initialization
	void Start()
	{
		currentMenu = "main";
		mainBox = new Rect(180, 20, Screen.width - 200, Screen.height - 65);
		back = new Rect(180, Screen.height - 35, 100, 25);
		next = new Rect(Screen.width - 120, Screen.height - 35, 100, 25);
		
		//Get Player Prefs:
		playerName = PlayerPrefs.GetString("playerName");
		playerSkinURL = PlayerPrefs.GetString("playerSkinURL");
		
		//Set up resolution variables:
		resolutions = Screen.resolutions;
		currentResolution = Screen.currentResolution;
		selectedResolution = currentResolution;
		//Set up graphic quality variables:
		currentGraphics = QualitySettings.GetQualityLevel();
		Debug.Log(currentGraphics);
		selectedGraphics = currentGraphics;
		//Setup fullscreen variable:
		isFullscreen = Screen.fullScreen;
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		mainBox = new Rect(180, 20, Screen.width - 200, Screen.height - 65);
		back = new Rect(180, Screen.height - 35, 100, 25);
		next = new Rect(Screen.width - 120, Screen.height - 35, 100, 25);
		
		if(isFullscreen)
			isFullscreenString = "Fullscreen";
		else
			isFullscreenString = "Windowed";
		
		switch(currentGraphics)
		{
			case(0):
				curGraphicString = "Basic";
				break;
			case(1):
				curGraphicString = "Low";
				break;
			case(2):
				curGraphicString = "Medium";
				break;
			case(3):
				curGraphicString = "High";
				break;
			case(4):
				curGraphicString = "Ultra";
				break;
			case(5):
				curGraphicString = "MEGA";
				break;
		}
	}
	
	void OnGUI()
	{
		//Constant Menu Buttons:
		//Quick Match:
		if(GUI.Button(new Rect(left, top, width, height), "Quick Match"))
		{
			FindQuickMatch();
		}
		//Host Game:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 1), width, height), "Host Game"))
		{
			currentMenu = "startServer";
		}
		//Server List:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 2), width, height), "Server List"))
		{
			currentMenu = "serverList";
		}
		//Direct connect:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 3), width, height), "Direct Connect"))
		{
			currentMenu = "directConnect";
		}
		//Player Customization:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 4), width, height), "Player Customization"))
		{
			currentMenu = "customization";
		}
		//Settings:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 5), width, height), "Settings"))
		{
			currentMenu = "settings";
		}
		//Credits:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 6), width, height), "Credits"))
		{
			currentMenu = "credits";
		}
		//Exit:
		if(GUI.Button(new Rect(left, top + ((height + buttonIncrement) * 7), width, height), "Exit Game"))
		{
			//Exit();
		}
		
		//Main area:
		GUILayout.BeginArea(mainBox);
			//Menu options:
			if(currentMenu == "main")
			{
				GUILayout.Label("Welcome to Killer Death Cubes (KDC)");
				GUILayout.Label("This game was made for #7dfps by Joe Cooper");
				GUILayout.Label("Find me on twitter @themisfit25");
			}
			else if(currentMenu == "startServer")
			{
				
			}
			else if(currentMenu == "serverList")
			{
				//Headings:
				GUILayout.BeginHorizontal();
				GUILayout.Label("Server Name:");
				GUILayout.Label("Players:");
				GUILayout.Label("Map:");
				GUILayout.Label("Gamemode:");
				GUILayout.Label ("Ping:");
				GUILayout.Label ("Connect:");
				GUILayout.EndHorizontal();
				GUILayout.Space(5);
				//NetworkScript.GetServers()
				//for every server in NetwrokScript.ServerList
				//{
				//	BeginHorizontal()
				//	make label(name)
				//	make label(curPlayers + / + maxPlayers)
				//	make label(map)
				//	make label(gm)
				//	make label(ping)
				//	make button(connect)
				//	EndHorizontal()
				//}
				//Test:
//				GUILayout.BeginHorizontal();
//				GUILayout.Label("TestServer");
//				GUILayout.Label("12/16");
//				GUILayout.Label("MapName");
//				GUILayout.Label("DeathMatch");
//				GUILayout.Label ("31");
//				GUILayout.Button("Connect");
//				GUILayout.EndHorizontal();
			}
			else if(currentMenu == "directConnect")
			{
				GUILayout.Label("Direct Connect:");
				dcIP = GUILayout.TextField(dcIP);
				if(GUILayout.Button("Connect")) DirectConnect();
			}
			else if(currentMenu == "customization")
			{
				//Name:
				GUILayout.Label("Player Name:");
				playerName = GUILayout.TextField(playerName);
				//Skin:
				GUILayout.Label("Player Skin URL:");
				playerSkinURL = GUILayout.TextField(playerSkinURL);
				//Save:
				if(GUILayout.Button("Save Settings!")) savePlayerCustomizations();
			}
			else if(currentMenu == "options")
			{
				
			}
			else if(currentMenu == "credits")
			{
				
			}
			else if(currentMenu == "settings")
			{
				//Fullscreen:
				GUILayout.Label("Fullscreen:");
				if(GUILayout.Button (isFullscreenString))
					isFullscreen = !isFullscreen;
				GUILayout.Space(15);
				//Resolution Options:
				GUILayout.Label("Resolution:");
				GUILayout.BeginHorizontal();
				int count = 1;
				foreach(Resolution res in resolutions)
				{
					if(GUILayout.Button(res.width + "x" + res.height))
						selectedResolution = res;
					count++;
					if(count > 5)
					{
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
						count = 1;	
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Label("The current resolution is " + currentResolution.width + "x" + currentResolution.height + ".");
				GUILayout.Space(15);
				//Graphic Options:
				GUILayout.Label("Graphic Quality:");
			
				GUILayout.BeginHorizontal();
				for(int i = 0; i < 6; i++)
				{
					switch(i)
					{
						case(0):
							gLoopString = "Basic";
							break;
						case(1):
							gLoopString = "Low";
							break;
						case(2):
							gLoopString = "Medium";
							break;
						case(3):
							gLoopString = "High";
							break;
						case(4):
							gLoopString = "Ultra";
							break;
						case(5):
							gLoopString = "MEGA";
							break;
					}
					
					if(GUILayout.Button(gLoopString))
						selectedGraphics = i;
				}
				GUILayout.EndHorizontal();
				GUILayout.Label("The current graphic quality is " + curGraphicString + ".");
				//Apply:
				GUILayout.Space(30);
				if(GUILayout.Button("Apply Changes!"))
				{
					Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen);
					currentResolution = Screen.currentResolution;
					selectedResolution = currentResolution;
					if(selectedGraphics != currentGraphics)
					{
						QualitySettings.SetQualityLevel(selectedGraphics);
						currentGraphics = QualitySettings.GetQualityLevel();
						selectedGraphics = currentGraphics;
					}
				}
			}
		GUILayout.EndArea();
		
		if(currentMenu == "customization" || currentMenu == "options" || currentMenu == "credits" || currentMenu == "settings" || currentMenu == "directConnect" || currentMenu == "serverList")
		{
			if(GUI.Button(back, "Back")) currentMenu = "main";
		}
		else if(currentMenu == "startServer")
		{
			if(GUI.Button(back, "Back")) currentMenu = "main";
			if(GUI.Button(next, "Start")) StartServer();
		}
		
	}
	
	void savePlayerCustomizations()
	{
		//Save player name and url to player prefs
		PlayerPrefs.SetString("playerName", playerName);
		PlayerPrefs.SetString("playerSkinURL", playerSkinURL);
		Debug.Log("Saved Details");
	}
	
	void StartServer()//server perameters
	{
		//Send server info to NetworkScript
		//NetworkScript.StartServer(server perameters);
	}
	
	void DirectConnect()//IP perameter
	{
		//NetworkScript.DirectConnect(ip);	
	}
	
	void FindQuickMatch()
	{
		//NetworkScript.FindQuickMatch();
	}
}
