using UnityEngine;
using System.Collections;

public class PlayerOneWay : MonoBehaviour {
	public bool bUp;
	private Collider2D thePlatform;
	public bool bGrounded;
	public LayerMask whatIsPlatform;
	public LayerMask whatIsGround;

	IEnumerator ActivatePlatform(Collider2D PlatformName)
	{
		//Hide the pause elements

		
		//Wait for the pause elements to move out of the screen
		yield return new WaitForSeconds(2f);
		if (!PlayerJumping.Instance.getPassingThrough() )
			PlatformName.isTrigger=false;


	}

	public Collider2D getPlatform (){
		return thePlatform;
	}
	
		
		void FixedUpdate () {
			
		if (bUp) 
			thePlatform=Physics2D.OverlapCircle(transform.position, 1.3f, whatIsPlatform);
		else//check if the is a ground beneath the player
		{		
		thePlatform=Physics2D.OverlapCircle(transform.position, 0.3f, whatIsPlatform);
		bGrounded=Physics2D.OverlapCircle(transform.position, 0.3f, whatIsGround);
		}

		if (thePlatform) // check if there is one way platform beneath the player
		{
			if (bUp){  // if the player is below platform ,disable it so he can pass
				thePlatform.isTrigger=true;
				StartCoroutine(ActivatePlatform(thePlatform));
			}
			if (!bUp && !PlayerJumping.Instance.getPassingThrough() && thePlatform.isTrigger){
				thePlatform.isTrigger=false;
			}
		}
	}
	//Checking the collison of the gameobject we created in step 2 for checking if the player is just below the platform and nedded to ignore the collison to the platform
	void OnTriggerStay2D(Collider2D other ) {
	//	print (other.name);
	}
	
	//Just to make sure that the platform's Box Collider does not get permantly disabled and it should be enabeled once the player get its through
	void OnTriggerExit2D(Collider2D other) {
	//	print (other.name);
	}
}