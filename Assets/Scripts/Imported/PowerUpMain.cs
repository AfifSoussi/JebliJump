using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpMain : MonoBehaviour 
{
	public float verticalSpeed = 5.0f;							//The vertical speed of the generated power ups
	public float verticalDistance = 1.0f;						//The vertical movement distance of the generated power ups
	
	public float horizontalSpeed  = 0;							//The horizontal speed of the generated power up
	
	List<PowerUp> inactive 	 	= new List<PowerUp>();			//A list of the active power ups
	List<PowerUp> activated	 	= new List<PowerUp>();			//A list of the deactivated power ups
	
	bool sonicBlastFirst		= false;						//Generate sonic blast first switch
	bool canGenerateRevive		= true;							//Can generate revive switch
	
	//Called at the start of the game
	void Start()
	{
		//Loop through children
		foreach (Transform child in transform)
		{
			//Add child to the inactive list
			inactive.Add(child.GetComponent<PowerUp>());
		}
	}
	//Find and returns a compatible power up to spawn
	PowerUp FindCompatiblePowerUp()
	{
		int n = 0;
		//If set to spawn sonic blast first
		if (sonicBlastFirst)
		{
			//Loop trought the power ups
			foreach (PowerUp item in inactive) 
			{
				//If the item is the sonic wave, return in
				if (item.name == "SonicWave")
					return item;
			}
			
			sonicBlastFirst = false;
		}
		//If it is not set to spawn the sonic blast first
		else
		{
			//If cant generate revive
			if (!canGenerateRevive)
			{
				//Get a power up, which is not the revive
				PowerUp powerUp	= null;
				
				do
				{
					n = Random.Range(0, inactive.Count);	
					powerUp = inactive[n];
				} while (powerUp.name == "Revive");
				
				//Return it
				return powerUp;
			}
			//If can generate revive
			else
			{
				//Get a random power up, and return it
				n = Random.Range(0, inactive.Count);	
				return inactive[n];
			}
		}
		
		return null;
	}
	//Called from the level generator, spawns a power up
	public void GeneratePowerUp(float multiplyValue)
	{
		//Find a compatible power up, and remove it from the inactive list
		PowerUp powerUp = FindCompatiblePowerUp();
		inactive.Remove(powerUp);
		//Find a new y position for the power up randomly
		Vector3 newPos = powerUp.transform.position;
		newPos.y = Random.Range(-5, 3);
		powerUp.transform.position = newPos;
		//Activate and launch the power up
		activated.Add(powerUp);
		powerUp.Setup(verticalSpeed, verticalDistance, horizontalSpeed * multiplyValue);
	}
	//Resets a power up, called from the actual power up
	public void ResetPowerUp(PowerUp sender)
	{
		//Deactivate the power up, and add it to the inactive list
        sender.DisableTrail();
		activated.Remove(sender);
		inactive.Add(sender);
	}
	//Sets the sonic blast as the first power up to spawn
	public void SetSonicBlastFirst()
	{
		sonicBlastFirst = true;
	}
	//Disables revive generation
	public void DisableReviveGeneration()
	{
		canGenerateRevive = false;
	}
	//Resets the manager, and notify the power ups to reset
	public void ResetAll()
	{
		sonicBlastFirst = false;
		canGenerateRevive = true;
		
		gameObject.BroadcastMessage ("ResetThis");
	}
	//Notifies the power ups to pause
	public void PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}
	//Notifies the power ups to resume
	public void ResumeAll()
	{
		this.gameObject.BroadcastMessage ("Resume");
	}
}
