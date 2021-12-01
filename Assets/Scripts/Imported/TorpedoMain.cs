
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorpedoMain : MonoBehaviour 
{
	public float movementSpeed  = 0;								//The movement speed of the torpedoes
	
	List<Torpedo> inactive 	 	= new List<Torpedo>();				//A list of the inactive torpedoes
	List<Torpedo> activated	 	= new List<Torpedo>();				//A list of the active torpedoes
	
	//Called at the beginning of the game
	void Start()
	{
		//Loop trought the children
		foreach (Transform child in transform)
		{
			//Add child to inactive
			inactive.Add(child.GetComponent<Torpedo>());
		}
	}
	//Called from the level generator, launches a torpedo
	public void LaunchTorpedo()
	{
		//Get the first torpedo from the inactive list
		Torpedo t = inactive[0];
		
		//Remove it from the inactived, and add it to the actives
		inactive.Remove(t);
		activated.Add(t);
		
		//Launch the torpedo
        t.Launch(movementSpeed);
	}
	//Reset the torpedo, called from the actual torpedo
	public void ResetTorpedo(Torpedo sender)
	{
		//Remove it from the actives, and add it to the inactive
		activated.Remove(sender);
		inactive.Add(sender);
	}
	//Resets the torpedoes
	public void ResetAll()
	{
		gameObject.BroadcastMessage ("ResetThis");
	}
	//Pause the torpedoes
	public void PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}
	//Resume the torpedoes
	public void ResumeAll()
	{
		this.gameObject.BroadcastMessage ("Resume");
	}
}
