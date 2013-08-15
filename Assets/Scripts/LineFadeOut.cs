using UnityEngine;
using System.Collections;

public class LineFadeOut : MonoBehaviour {
	
	public float fadeOutSpeed = 20f;
	public Color colour;
	private float alpha = 10f;
	private LineRenderer line;
	
	// Use this for initialization
	void Start() 
	{
		line = gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update() 
	{
		alpha -= Time.deltaTime * fadeOutSpeed;
		Color cl = colour;
		cl.a = alpha;
		line.SetColors(cl, cl);
		
		if(alpha < 0) Destroy(this.gameObject);
	}
}
