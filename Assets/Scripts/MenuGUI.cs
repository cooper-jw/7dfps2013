using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {
	
	public string currentMenu = "main";
	public GameObject networkObject;
	private NetworkScript myNetwork;
	
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
	
	//Setting Variables:
	public bool isFullscreen = false;
	public Resolution selectedResolution;
	public Resolution currentResolution;
	public Resolution[] resolutions;
	public int selectedGraphics;
	public int currentGraphics;
	public string isFullscreenString;
	public string curGraphicString;
	private string gLoopString;
	public int mouseSens;
	public float mouseSensf;
	
	//Server Setup Variables:
	public string serverName = "Default Name";
	public int maxPlayers;
	public string maxPlayersString = "16";
	public int selectedGM = 0;
	public string selectedGMString;
	public string loopGMString;
	public int selectedMap = 0;
	public string selectedMapString;
	public string loopMapString;
	
	// Use this for initialization
	void Start()
	{
		//Get the network script:
		myNetwork = networkObject.GetComponent<NetworkScript>();
		//Set current menu:
		currentMenu = "main";
		//Setup GUI things:
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
		selectedGraphics = currentGraphics;
		//Setup fullscreen variable:
		isFullscreen = Screen.fullScreen;
		//Mouse Sensitivy Setup:
		if(PlayerPrefs.GetInt("mouseSens") != 0)
			mouseSens = PlayerPrefs.GetInt("mouseSens");
		else
		{
			mouseSens = 100;
			PlayerPrefs.SetInt("mouseSens", mouseSens);
			PlayerPrefs.SetInt("ySens", mouseSens);
			PlayerPrefs.SetInt("xSens", mouseSens);
		}
		mouseSensf = (float)mouseSens;
		//Server Setup Variables:
		selectedMap = 0;
		selectedGM = 0;
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
		
		maxPlayers = int.Parse(maxPlayersString);
		
		mouseSens = (int)mouseSensf;
		
		if(Input.GetKeyDown(KeyCode.F12))
			Application.LoadLevel("TestLevel");
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
			Application.Quit();
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
			GUILayout.Label("Start Server:");
			GUILayout.Space(10);
			//Server Name:
			GUILayout.Label("Server Name (this needs to be changed!):");
			serverName = GUILayout.TextField(serverName);
			GUILayout.Space(10);
			//Max Players:
			GUILayout.Label ("Max Players:");
			maxPlayersString = GUILayout.TextField(maxPlayersString);
			GUILayout.Space(10);
			//Gamemode:
			GUILayout.Label("Gamemode:");
			GUILayout.BeginHorizontal();
			for(int i = 0; i < 1; i++)
			{
				switch(i)
				{
					case(0):
						loopGMString = "Free For All";
						break;
				}
				if(GUILayout.Button(loopGMString))
				{
					selectedGM = i;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			//Map:
			GUILayout.Label("Map:");
			GUILayout.BeginHorizontal();
			for(int i = 0; i < 1; i++)
			{
				switch(i)
				{
					case(0):
						loopMapString = "TestMap";
						break;
				}
				if(GUILayout.Button(loopMapString))
				{
					selectedMap = i;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		}
		else if(currentMenu == "serverList")
		{
			//Headings:
			GUILayout.BeginHorizontal();
			GUILayout.Label("Server Name:");
			GUILayout.Label("Players:");
			GUILayout.Label("Map|Gamemode:");
			GUILayout.Label ("Connect:");
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			foreach(HostData hd in myNetwork.hostData)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(hd.gameName);
				GUILayout.Label(hd.connectedPlayers.ToString() + "/" + hd.playerLimit.ToString());
				GUILayout.Label(hd.comment);
				if(GUILayout.Button("Connect"))
				{
					currentMenu = "connecting";
					myNetwork.Connect(hd);
				}
				
				GUILayout.EndHorizontal();
			}
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
			GUILayout.Space(15);
			//Mouse Sensitivity:
			GUILayout.Label("Mouse Sensitivity:");
			mouseSensf = GUILayout.HorizontalSlider(mouseSensf,10,300);
			GUILayout.Label(mouseSens.ToString());
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
				if(mouseSens != PlayerPrefs.GetInt("mouseSens"))
				{
					PlayerPrefs.SetInt("mouseSens", mouseSens);
					PlayerPrefs.SetInt("ySens", mouseSens);
					PlayerPrefs.SetInt("xSens", mouseSens);
				}
			}
		}
		else if(currentMenu == "connecting")
		{
			GUILayout.Label("Connecting to server...");
		}
		else if(currentMenu == "startserver")
		{
			GUILayout.Label("Starting server...");
		}
		GUILayout.EndArea();
		
		if(currentMenu == "customization" || currentMenu == "options" || currentMenu == "credits" || currentMenu == "settings" || currentMenu == "directConnect" || currentMenu == "serverList")
		{
			if(GUI.Button(back, "Back")) currentMenu = "main";
		}
		else if(currentMenu == "startServer")
		{
			if(GUI.Button(back, "Back")) currentMenu = "main";
			if(GUI.Button(next, "Start"))
			{
				StartServer(serverName, maxPlayers, selectedGM, selectedMap);
				currentMenu = "startserver";
			}
		}
		
		if(currentMenu == "serverList")
		{
			if(GUI.Button(next, "Refresh")) RefreshServers();
		}
	}
	
	void savePlayerCustomizations()
	{
		//Save player name and url to player prefs
		if(playerName != null || playerName != " ")
		{
			myNetwork.playerName = playerName;
		}else{
			myNetwork.playerName = "DefaultName";
			playerName = myNetwork.playerName;
		}
		PlayerPrefs.SetString("playerName", playerName);
		PlayerPrefs.SetString("playerSkinURL", playerSkinURL);
		if(playerSkinURL != null || playerSkinURL != " ")
		{
			myNetwork.skinURL = playerSkinURL;
			myNetwork.hasSkin = true;
		}else{
			myNetwork.skinURL = playerSkinURL;
			myNetwork.hasSkin = false;
		}
		Debug.Log("Saved Details");
	}
	
	void StartServer(string name, int max, int gm, int map)//server perameters
	{
		if(max > 32) max = 32;
		if(max < 2) max = 2;
		Debug.Log("Server information " + name + " " + max + " " + gm + " " + map);
		//Send server info to NetworkScript
		myNetwork.StartServer(name, max, gm, map);
	}
	
	void DirectConnect()//IP perameter
	{
		//NetworkScript.DirectConnect(ip);	
	}
	
	void FindQuickMatch()
	{
		//NetworkScript.FindQuickMatch();
	}
	
	void RefreshServers()
	{
		myNetwork.RefreshServers();
	}
}
