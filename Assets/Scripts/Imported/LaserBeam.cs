using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour 
{
	public float scrollSpeed = 0.0f;			//Laser scroll speed
	
	Vector2 scrolling = new Vector2(0, 0);		//Scrolling vector
	
	//Called at every frame
	void Update () 
	{
		//Apply scrolling speed to scrolling vector
		scrolling.x = scrollSpeed;
		//Set the texture offset based on scrolling vector
		this.GetComponent<Renderer>().material.mainTextureOffset += scrolling * Time.deltaTime;
	}
}
