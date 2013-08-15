using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {
	
	public bool isLocal =  true;
	private CharacterController cc;
	private GUIScript GUICode;
	public GameObject camHolder;
	public int yCamSens = 30;
	public int xCamSens = 30;
	public float walkSpeed = 10f;
	public float sprintSpeed = 15f;
	public float maxLookHeight = 85f;
	public float minLookHeight = -85f;
	public Vector3 camAngle;
	public Vector3 moveVec;
	public GameObject myMesh;
	public GameObject myCam;
	public GameObject myCamHolder;
	public bool grounded = false;
	public bool wasGrounded = true;
	public AudioClip jumpSound;
	public AudioClip landSound;
	public bool sprinting = false;
	public float yMovement = 0f;
	public Vector3 lookDir;
	public bool canControl = true;
	//Used for networking
	private GameObject netHolder;
	private NetworkScript theNetwork;
	private WWW www;
	public bool hasSkin = false;
	public string skinURL;
	private bool skinChanged = false;
	public NetworkViewID viewID;
	public string myName;
	public float currentHealth = 100f;
	public float healthUpRate = 5f;
	public int kills;
	public int deaths;
	public bool hasShot = false;
	//Gun Variables:
	//Gun list:
	// 0 = None
	// 1 = Pistol
	// 2 = Rocket
	// 3 = Machine Gun
	// ifDone 4 = Shotgun
	public int currentGun = 0;
	public float lastShoot = 0f;
	public float overheat = 0.0f;
	public float ovrDownPerSec = 10;
	public AudioClip overheatSFX;
	//Pistol Stuff:
	public GameObject pistolShot;
	public AudioClip pistolSound;
	public float pistolROF = 0.3f;
	public float pistolOH = 30f;
	public float pistolShotWidth = 0.3f;
	//Machine Gun Stuff:
	public GameObject machineShot;
	public AudioClip machineSound;
	public float machineROF = 0.2f;
	public float machineOH = 7f;
	public float machineShotWidth = 0.2f;
	
	// Use this for initialization
	void Start() 
	{
		if(isLocal)
		{	
			//Setup Network shit:
			netHolder = GameObject.FindGameObjectWithTag("Network");
			theNetwork = netHolder.GetComponent<NetworkScript>();
			viewID = theNetwork.netviewID;
			//Set target frame rate:
			Application.targetFrameRate = 300;
			//Set the character controller:
			cc = GetComponent<CharacterController>();
			//Set GUIScript:
			GUICode = GetComponent<GUIScript>();
			//Turn on the GUI:
			GUICode.isLocal = true;
			//Make the camera ours bb:
			Camera.main.transform.parent = camHolder.transform;
			//Zero out the camera position:
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localEulerAngles = Vector3.zero;
			myCam = Camera.main.gameObject;
			if(PlayerPrefs.GetInt("ySens") != 0 && PlayerPrefs.GetInt("xSens") != 0)
			{
				yCamSens = PlayerPrefs.GetInt("ySens");
				xCamSens = PlayerPrefs.GetInt("xSens");
			}else{
				yCamSens = 75;
				xCamSens = 75;
				PlayerPrefs.SetInt("ySens", yCamSens);
				PlayerPrefs.SetInt("xSens", xCamSens);
			}
			//Set Current Gun
			currentGun = 1; //Pistol
		}
		else
		{
			Transform[] tempTransforms = gameObject.GetComponentsInChildren<Transform>();
			GameObject[] children = new GameObject[tempTransforms.Length - 1];
		
			for(int i = 0; i < tempTransforms.Length - 1; i++)
			{
				children[i] = tempTransforms[i+1].gameObject;	
			}
		
			foreach(GameObject child in children)
			{
				if(child.tag == "CamHolder") myCamHolder = child;	
			}	
		}
		
		if(hasSkin)
			www = new WWW(skinURL);
		
		camAngle = Vector3.zero;
		moveVec = Vector3.zero;
		lookDir = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(isLocal)
		{	
			hasShot = false;
			
			if(Input.GetKeyDown(KeyCode.F6)) currentGun--;
			if(Input.GetKeyDown(KeyCode.F7)) currentGun++;
			
			if(canControl)
			{
				//Camera Input:
				camAngle.x -= Input.GetAxis("Mouse Y") * 0.01f * yCamSens;
				camAngle.y += Input.GetAxis("Mouse X") * 0.01f * xCamSens;
				//Check that they're not look to high/low:
				camAngle.x = Mathf.Clamp(camAngle.x, minLookHeight, maxLookHeight);
				camAngle.z = 0f;
				//Apply camera movement:
				camHolder.transform.localEulerAngles = camAngle;
			}
			
			//Used for storing input data:
			Vector3 inputVector = Vector3.zero;
			
			if(canControl)
			{
				//Get inputs:
				if (Input.GetKey("s")) inputVector -= Camera.main.transform.forward;
				if (Input.GetKey("w")) inputVector += Camera.main.transform.forward;
				if (Input.GetKey("d")) inputVector += Camera.main.transform.right;
				if (Input.GetKey("a")) inputVector -= Camera.main.transform.right;
				//inputVector.y = 0f;
				inputVector.Normalize();
			}
			
			//Check if grounded:
			grounded = cc.isGrounded;
			//Check if sprinting:
			if (Input.GetButton("Sprint")) sprinting = true;
				else sprinting = false;
			
			//Apply movement:
			if(canControl)
			{
				if (!sprinting){
					cc.Move(inputVector * Time.deltaTime * walkSpeed);
				}else{
					cc.Move(inputVector * Time.deltaTime * sprintSpeed);
				}
			}
			
			//Falling/Gravity:
			if (yMovement <= 0f) {
				cc.Move(Vector3.up * -0.2f);
				grounded = cc.isGrounded;
				if(!grounded) cc.Move(Vector3.up * 0.2f);
			}else{
				grounded = false;
			}
				
			//Jump Input:
			if(grounded){
				yMovement = 0f;
				if(Input.GetKeyDown("space") && canControl)
				{
					yMovement = 4f;
				}
			}else{
				yMovement -= Time.deltaTime * 10f;
			}
			
			//Jump:
			cc.Move(Vector3.up * yMovement * Time.deltaTime * 5f);
			
			moveVec = inputVector;
			
			//Shoot:
			lastShoot += Time.deltaTime;
			overheat -= Time.deltaTime * ovrDownPerSec;
			if(overheat < 0) overheat = 0;
			if(Input.GetButton("Fire1") && canControl)
			{
				if(currentGun == 2) Shoot();
			}
			
			if(Input.GetButtonDown("Fire1") && canControl)
			{
				if(currentGun != 2) Shoot();	
			}
			
			if(currentHealth < 100)
				currentHealth += Time.deltaTime * healthUpRate;
			if(currentHealth > 100) currentHealth = 100;
			
			//Send player information:
			theNetwork.SendPlayer(viewID, transform.position, camAngle, moveVec, currentGun, hasShot, currentHealth);
		}
		
		if(!isLocal && hasShot)
		{
			Shoot();	
		}
		
		if(!grounded && wasGrounded)
		{
			wasGrounded = false;
			if(jumpSound != null) audio.PlayOneShot(jumpSound);
		}
		
		if(grounded && !wasGrounded)
		{
			wasGrounded = true;
			if(landSound != null) audio.PlayOneShot(landSound);
		}
		
		//Change Skin:
		if(hasSkin && www.isDone && !skinChanged)
		{
			myMesh.renderer.material.mainTexture = www.texture;
			skinChanged = true;
		}
		
		//Directional Animations:
		//Used by both 1st and 3rd person views.
		lookDir = new Vector3(270, 180 + camHolder.transform.eulerAngles.y, 0);
		myMesh.transform.localEulerAngles = lookDir;
	}
	
	//Used for updating someone elses player over the network:
	public void UpdatePlayer(Vector3 pos, Vector3 ang, Vector3 move, int gun, bool shot, float hp)
	{
		hasShot = shot;
		currentGun = gun;
		currentHealth = hp;
		transform.position = pos;
		camHolder.transform.eulerAngles = ang;
		moveVec = move;
		if(hasShot) Shoot();
	}
	
	void Shoot()
	{
		if(currentGun == 0)
		{
			Debug.Log("You're not holding a gun ;_;");
		}
		else if(currentGun == 1)
		{
			//Pistol:
			if(lastShoot > pistolROF && overheat + pistolOH <= 110f && isLocal)
			{ 
				if(overheat >= 100f && overheatSFX != null) audio.PlayOneShot(overheatSFX);
				if(overheat > 100f) overheat = 100f;
				lastShoot = 0f;
				hasShot = true;
				overheat += pistolOH;
				if(pistolSound) audio.PlayOneShot(pistolSound);
				//Raycasting
				Ray ray;
				RaycastHit hit;
				
				ray = new Ray(myCam.transform.position + new Vector3(0f, -0.1f, 0f), myCam.transform.forward);
				if(Physics.Raycast(ray, out hit, 1000))
				{
					GameObject shotEntity = (GameObject)GameObject.Instantiate(pistolShot);
					LineRenderer shot = shotEntity.GetComponent<LineRenderer>();
					
					shot.useWorldSpace = true;
					shot.SetWidth(pistolShotWidth, pistolShotWidth);
					shot.SetPosition(0, ray.origin);
					shot.SetPosition(1, hit.point);
					
					//If hit with player tag
					//Hit(viewID);
					if(hit.transform.gameObject.tag == "Player" && isLocal)
					{
						//Debug.Log("I DONE GOT ONE!");
						GameObject hitPlayer = hit.transform.gameObject;
						FPSController hitController = hitPlayer.GetComponent<FPSController>();
						NetworkViewID hitViewID = hitController.viewID;
						theNetwork.RegisterHit(viewID, hitViewID, currentGun);
					}
				}
			}
			else if(!isLocal)
			{
				if(pistolSound) audio.PlayOneShot(pistolSound);
				Ray ray;
				RaycastHit hit;
				
				ray = new Ray(myCamHolder.transform.position + new Vector3(0f, -0.1f, 0f), myCamHolder.transform.forward);
				if(Physics.Raycast(ray, out hit, 1000))
				{
					GameObject shotEntity = (GameObject)GameObject.Instantiate(pistolShot);
					LineRenderer shot = shotEntity.GetComponent<LineRenderer>();
					
					shot.useWorldSpace = true;
					shot.SetWidth(pistolShotWidth, pistolShotWidth);
					shot.SetPosition(0, ray.origin);
					shot.SetPosition(1, hit.point);
				}
			}
		}
		else if(currentGun == 2)
		{
			//Machine Gun:
			if(lastShoot > machineROF && overheat + machineOH <= 102f && isLocal)
			{ 
				if(overheat >= 100f && overheatSFX != null) audio.PlayOneShot(overheatSFX);
				if(overheat > 100f) overheat = 100f;
				lastShoot = 0f;
				hasShot = true;
				overheat += machineOH;
				if(pistolSound) audio.PlayOneShot(machineSound);
				//Raycasting
				Ray ray;
				RaycastHit hit;
				
				ray = new Ray(myCam.transform.position + new Vector3(0f, -0.1f, 0f), myCam.transform.forward);
				if(Physics.Raycast(ray, out hit, 1000))
				{
					GameObject shotEntity = (GameObject)GameObject.Instantiate(machineShot);
					LineRenderer shot = shotEntity.GetComponent<LineRenderer>();
					
					shot.useWorldSpace = true;
					shot.SetWidth(machineShotWidth, machineShotWidth);
					shot.SetPosition(0, ray.origin);
					shot.SetPosition(1, hit.point);
					
					//If hit with player tag
					//Hit(viewID);
					if(hit.transform.gameObject.tag == "Player" && isLocal)
					{
						//Debug.Log("I DONE GOT ONE!");
						GameObject hitPlayer = hit.transform.gameObject;
						FPSController hitController = hitPlayer.GetComponent<FPSController>();
						NetworkViewID hitViewID = hitController.viewID;
						theNetwork.RegisterHit(viewID, hitViewID, currentGun);
					}
				}
			}
			else if(!isLocal)
			{
				if(machineSound) audio.PlayOneShot(machineSound);
				Ray ray;
				RaycastHit hit;
				
				ray = new Ray(myCamHolder.transform.position + new Vector3(0f, -0.1f, 0f), myCamHolder.transform.forward);
				if(Physics.Raycast(ray, out hit, 1000))
				{
					GameObject shotEntity = (GameObject)GameObject.Instantiate(machineShot);
					LineRenderer shot = shotEntity.GetComponent<LineRenderer>();
					
					shot.useWorldSpace = true;
					shot.SetWidth(machineShotWidth, machineShotWidth);
					shot.SetPosition(0, ray.origin);
					shot.SetPosition(1, hit.point);
				}
			}	
		}
		else
		{
			Debug.Log("Gun ID" + currentGun + " is not implemented yet D:");	
		}
			
	}
}
