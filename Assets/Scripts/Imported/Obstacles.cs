using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacles : MonoBehaviour 
{
	public List<GameObject> elements		= new List<GameObject>();		//A list that holds the obstacle childs
	
	//Called at the beginning of the game
	void Start()
	{
		//Loop through children
		foreach (Transform child in transform)
		{
			//If the child is not a triggerer
			if (child.name != "SpawnTriggerer" && child.name != "ResetTriggerer")
			{
				//Add it to the elements list, and deactivate it
	            elements.Add(child.gameObject);
                EnableDisable(child.gameObject, false);
			}
		}
	}
    //Enables/disables the object with childs based on platform
    void EnableDisable(GameObject what, bool activate)
    {
        #if UNITY_3_5
            what.SetActiveRecursively(activate);
        #else
            what.SetActive(activate);
        #endif
    }
	//Called when the obstacle is reseting
	public void DeactivateChild()
	{
		//Loop through the elements list
		foreach (GameObject child in elements)
		{			
			//Look for the child's explosion
			Transform go = child.transform.Find("ExplosionParticle");
			
			//If we found it
			if (go != null)
				//Stop the particle
				go.GetComponent<ParticleSystem>().Stop();
			//Deactivate the child
            EnableDisable(child, false);
		}
	}
	//Called when the obstacle is spawned
	public void ActivateChild()
	{
		//Loop through the elements array
		foreach (GameObject child in elements)
		{
			//Activate the child
            EnableDisable(child, true);
			
			//Active the child's renderer and collider
			child.GetComponent<Renderer>().enabled = true;
			if (child.GetComponent<Collider>() != null)
				child.GetComponent<Collider>().enabled = true;
			else if (child.GetComponent<Collider2D>() != null)
				child.GetComponent<Collider2D>().enabled = true;

		}
	}
}
