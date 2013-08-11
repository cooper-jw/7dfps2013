using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {
	
	public bool isLocal =  true;
	private CharacterController cc;
	private GUIScript GUICode;
	public GameObject camHolder;
	public float yCamSens = 30f;
	public float xCamSens = 30f;
	public float walkSpeed = 10f;
	public float sprintSpeed = 15f;
	public float maxLookHeight = 85f;
	public float minLookHeight = -85f;
	public Vector3 camAngle;
	public Vector3 moveVec;
	public GameObject myMesh;
	public bool grounded = false;
	public bool sprinting = false;
	public float yMovement = 0f;
	public Vector3 lookDir;
	public bool canControl = true;
	
	//Gun list:
	// 0 = None
	// 1 = Pistol
	// 2 = Rocket
	// 3 = Machine Gun
	// ifDone 4 = Shotgun
	public int currentGun = 0;
	
	// Use this for initialization
	void Start() 
	{
		if(isLocal)
		{
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
		}else{
			
		}
		
		camAngle = Vector3.zero;
		moveVec = Vector3.zero;
		lookDir = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(isLocal)
		{	
			//Can we control
			canControl = !GUICode.paused;
			
			//Camera Input:
			camAngle.x -= Input.GetAxis("Mouse Y") * Time.deltaTime * yCamSens;
			camAngle.y += Input.GetAxis("Mouse X") * Time.deltaTime * xCamSens;
			//Check that they're not look to high/low:
			camAngle.x = Mathf.Clamp(camAngle.x, minLookHeight, maxLookHeight);
			camAngle.z = 0f;
			//Apply camera movement:
			if(canControl)
				camHolder.transform.localEulerAngles = camAngle;
			
			//Used for storing input data:
			Vector3 inputVector = Vector3.zero;
			
			//Get inputs:
			if (Input.GetKey("s")) inputVector -= Camera.main.transform.forward;
			if (Input.GetKey("w")) inputVector += Camera.main.transform.forward;
			if (Input.GetKey("d")) inputVector += Camera.main.transform.right;
			if (Input.GetKey("a")) inputVector -= Camera.main.transform.right;
			//inputVector.y = 0f;
			inputVector.Normalize();
			
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
			
			//Send player information:
			//theNetwork.SendPlayer(viewID, transform.position, camAngle, moveVec);
		}
		
		//Directional Animations:
		//Used by both 1st and 3rd person views.
		lookDir = new Vector3(270, 180 + camHolder.transform.eulerAngles.y, 0);
		myMesh.transform.localEulerAngles = lookDir;
	}
	
	//Used for updating someone elses player over the network:
	public void UpdatePlayer(Vector3 pos, Vector3 ang, Vector3 move)
	{
		transform.position = pos;
		camHolder.transform.eulerAngles = ang;
		moveVec = move;
	}
}
