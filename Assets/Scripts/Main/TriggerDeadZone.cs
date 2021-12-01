using UnityEngine;
using System.Collections;

public class TriggerDeadZone : MonoBehaviour {
	public GameObject HitEffect;


void Start (){
	gameObject.layer = 2;
	if(GetComponent<Renderer>())GetComponent<Renderer>().enabled=false;
}


                     

void OnTriggerEnter2D ( Collider2D other  ){
	if(other.transform.tag == "Player" && !PlayerManager.Instance.getDead())
	{		
			GameObject go = Instantiate (HitEffect, other.transform.position, Quaternion.identity) as GameObject;
			PlayerManager.Instance.playSound("hit");
			Destroy (go, 1);
			PlayerManager.Instance.StartCoroutine ("playerIsDead",1f);
			//LevelManager.Instance.Scrolling=false;
			//transform.collider2D.enabled=false;
	}
}

}