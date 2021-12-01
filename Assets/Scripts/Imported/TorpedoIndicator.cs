using UnityEngine;
using System.Collections;

public class TorpedoIndicator : MonoBehaviour 
{
	public float speed = 5.0f;							//Vertical speed
	public float distance = 1.0f;						//Vertical distance
	
	float offset = 0.0f;								//Offset
	
	float originalPos = 0;								//The original position of the indicator
	Vector3 nextPos = new Vector3();					//The next position of the indicator
	
	bool paused = false;								//Is the game paused
	
	//Called when the object is enabled
	void OnEnable()
	{
		//Set original position, and set pause to false
		originalPos = this.transform.position.y;
		paused = false;
	}
	//Called at every frame
	void Update () 
	{	
		//If the game is not paused
		if (!paused)
		{
			//Calculate offset
			offset = (1 + Mathf.Sin(Time.time * speed)) * distance / 2.0f;
			
			//Modify next position
			nextPos = this.transform.position;
			nextPos.y = originalPos + offset;
			
			//Apply next position
			this.transform.position = nextPos;
		}
	}
	//Pause the indicator
	public void Pause()
	{
		paused = true;
	}
	//Resume the indicator
	public void Resume()
	{
		paused = false;
	}
	
	
}
