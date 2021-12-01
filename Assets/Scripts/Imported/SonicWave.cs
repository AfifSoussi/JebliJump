using UnityEngine;
using System.Collections;

public class SonicWave : MonoBehaviour 
{
	bool canDisable	= false;								//Can the power up disable itself?
	
	//If the wave collides with something
	void OnTriggerEnter (Collider other)
	{
		//If it is an obstacle
		if (other.transform.name == "Mine" || other.transform.name == "Chain" || other.transform.name == "MineChain" || other.transform.name == "Laser" || other.transform.name == "LaserBeam")
		{
			//Explode it
			PlayExplosion (other.transform);
		}
		//If it is a torpedo
		else if (other.name == "Torpedo")
		{
			//If the sonic wave is in the screen, disable it
			if (!canDisable)
				other.transform.parent.GetComponent<Torpedo>().TargetHit(true);
		}
		//If the sonic wave is collided with and obstacle reset triggerer, and the wave can disable itself
		else if (other.name == "ResetTriggerer" && other.tag == "Obstacles" && canDisable)
		{
			//Reset the wave
			ResetThis();
		}
	}
	//Called when the wave collides with an obstacle
	void PlayExplosion(Transform expParent)
	{	
		//Disable obstacle's renderer and collider
		expParent.GetComponent<Renderer>().enabled = false;
		expParent.GetComponent<Collider>().enabled = false;
		
		//If the wave can disable, return
		if (canDisable)
			return;
		
		//Plays the explosion effect
		ParticleSystem explosion = expParent.Find("ExplosionParticle").gameObject.GetComponent("ParticleSystem") as ParticleSystem;
		explosion.Play();
	}
	//Reset the sonic wave
	void ResetThis()
	{
		//Reset variable and position, then disable
		canDisable = false;
		this.transform.localPosition = new Vector3(-70, 0, -5);
		EnableDisable(this.gameObject, false);
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
	//Prepare for disable
	public void Disable()
	{
		canDisable = true;
	}
}
