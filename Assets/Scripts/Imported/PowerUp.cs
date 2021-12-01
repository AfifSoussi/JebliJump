using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour 
{
	public PowerUpMain parent;					//The power up manager parent object
	public GameObject trail;					//The trail renderer gameobject
	
	float verticalSpeed = 5.0f;					//Vertical speed
	float verticalDistance = 1.0f;				//Vertical distance
	
	float horizontalSpeed  = 0;					//Horizontal speed
	
	float offset = 0.0f;						//Vertical offset
	float originalPos = 0;						//Original y position
	
	Vector3 nextPos = new Vector3();			//Stores the next position
	Vector3 startingPos;						//The starting position of the object
	
	bool paused = false;						//Is the game paused
	bool canMove = false;						//Can this object move
	
	//Called at the beginning of the game
	void Start()
	{
		//Saves the starting position
		startingPos = this.transform.position;
	}
	//Called at every frame
	void Update () 
	{	
		//If the game is not paused, and the power up can move
		if (!paused && canMove)
		{
			//Get current position
			nextPos = this.transform.position;
			
			//Calculate new vertical position
			offset = (1 + Mathf.Sin(Time.time * verticalSpeed)) * verticalDistance / 2.0f;
			nextPos.y = originalPos + offset;
			
			//Calculate new horizontal position
			nextPos.x -= horizontalSpeed * Time.deltaTime;
			
			//Apply new position
			this.transform.position = nextPos;
		}
	}
    //Enables/disables the object with childs based on platform
    void EnableDisable(GameObject what, bool state)
    {
        #if UNITY_3_5
            what.SetActiveRecursively(state);
        #else
            what.SetActive(state);
        #endif
    }
	//Called when the power up manager activates this power up
	public void Setup(float vSpeed, float vDist, float hSpeed)
	{
		//Set speed related variables
		this.verticalSpeed = vSpeed;
		this.verticalDistance = vDist;
		this.horizontalSpeed = hSpeed;
		
		//Get original y position
		originalPos = this.transform.position.y;
		
		//Activate trail particle
        EnableDisable(trail, true);
		
		//Enable movement
		canMove = true;
		paused = false;
	}
    //Disable trail
    public void DisableTrail()
    {
        EnableDisable(trail, false);
    }
	//Pause the power up
	public void Pause()
	{
		paused = true;
	}
	//Resume the power up
	public void Resume()
	{
		paused = false;
	}
	//Reset the power up
	public void ResetThis()
	{
		//Disable movement and deactivate trail effect
		canMove = false;
        EnableDisable(trail, false);
        
		//Reset position and notify the power up manager
		this.transform.position = startingPos;
		parent.ResetPowerUp(this);
	}
}
