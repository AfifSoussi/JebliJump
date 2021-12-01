using UnityEngine;
using System.Collections;

public class CoinMagnet : MonoBehaviour {
	public float magnitude =5f;
	public LayerMask whatIsCoinLayer;
	public string whatIsCoinTag = "Coin";
	private Collider2D theCoin;

	
	void FixedUpdate () {
	 	if (!PlayerManager.Instance.getMagnet() || PlayerManager.Instance.getDead()) return ;

		theCoin = Physics2D.OverlapCircle(transform.position, magnitude, whatIsCoinLayer);
		if (theCoin){
			if (theCoin.name=="Coin" || theCoin.name=="coin") // check if there is a coin inside the circle
			{ 
				//pull the coin toward the player
				//theCoin.transform.position = Vector3.MoveTowards(theCoin.transform.position, transform.position,1f);

				//Disable the coin's renderer and collider
				theCoin.GetComponent<Renderer>().enabled = false;
				theCoin.GetComponent<Collider2D>().enabled = false;
				
				//Play it's particle system, and increase coin ammount
				theCoin.transform.Find("CoinParticle").gameObject.GetComponent<ParticleSystem>().Play();
				PlayerManager.Instance.playSound("coinCollect");

				LevelManager.Instance.CoinGathered();





			}
		}
	}

}