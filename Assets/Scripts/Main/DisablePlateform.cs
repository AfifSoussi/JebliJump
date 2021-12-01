using UnityEngine;
using System.Collections;

public class DisablePlateform : MonoBehaviour {

	BoxCollider2D PlateformCollider;

void OnCollisionStay2D ( Collision2D collisionInfo  ){
	

	if(collisionInfo.transform.tag=="Player")
		{
			PlateformCollider.isTrigger=true;
			print("disable plateform");
		}
		else
		{
			print ("enable plateform");
			PlateformCollider.isTrigger=false;

		}

}
void OnCollisionExit2D ( Collision2D collisionInfo  ){
		
		
		if(collisionInfo.transform.tag=="Player")
		{
			PlateformCollider.isTrigger=true;
			print("disable plateform");
		}
		else
		{
			print ("enable plateform");
			PlateformCollider.isTrigger=false;
			
		}
		
	}

	// Use this for initialization
	void Start () {
		PlateformCollider = GetComponent<BoxCollider2D>() as BoxCollider2D;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
