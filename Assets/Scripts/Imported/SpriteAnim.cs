using UnityEngine;
using System.Collections;

public class SpriteAnim : MonoBehaviour 
{
	public Texture2D frameA;					//The first frame of the animation
	public Texture2D frameB;					//The second frame of the animation
	
	int currentId = 0;							//The ID of the current frame
	
	bool canAnimate = false;					//Animation enabled/disabled
	
	//Called when the object is enabled
	void OnEnable () 
	{
		//Enable the animation
		canAnimate = true;
	}
	//Called when the object is disabled
	void OnDisable()
	{
		//Stop the animation coroutine, and disable the animation
		StopCoroutine("Animate");
		canAnimate = false;
	}
	//Called on every frame
	void Update()
	{
		//If the animation is enabled
		if (canAnimate)
		{
			//Start the animation coroutine
			StartCoroutine(Animate());
		}
	}
	//The animation coroutine
	IEnumerator Animate()
	{
		//Disable the calling of additional coroutines
		canAnimate = false;
		
		//Wait for 0.1 seconds
		yield return new WaitForSeconds(0.1f);
		
		//If the current animation frame is 0
		if (currentId == 0)
		{
			//Go to animation frame 1
			this.GetComponent<Renderer>().material.mainTexture = frameB;
			currentId = 1;
		}
		//If the current animation frame is 1
		else
		{
			//Go to animation frame 0
			this.GetComponent<Renderer>().material.mainTexture = frameA;
			currentId = 0;
		}
		
		//Enable the calling of a new coroutine
		canAnimate = true;
	}
}
