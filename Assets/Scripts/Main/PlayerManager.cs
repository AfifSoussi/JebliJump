using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour 
{
	public LayerMask whatIsDestroyable;
	public GameObject[] bonusParticles;									//the power up activation
	public AudioClip[] sounds; 										//the played sounds

	public Texture2D[] subTextures;										//The array containing the sub and sub damaged textures
	public Renderer subMaterial;										//A link to the sub material
	
	public Transform shield;											//The sub's shield
	public SphereCollider shieldCollider;								//The shield's collider
	public GameObject speedParticle;									//The speed effect
	public GameObject speedTrail;										//The speed trail effect
	public GameObject Magnet;										//The magnet particle


	public ParticleSystem smoke;										//The smoke particle
	public ParticleSystem bubbles;										//The Player bubble particle system
	public ParticleSystem reviveParticle;								//The revive particle



	
	static PlayerManager myInstance;
	static int instances = 0;
	
	float xPos								= -30;						//The x position of the Player
	Vector3 startingPos						= Vector3.zero	;					//The starting position of the Player
	Collision2D crashedObstacle	;										// the object who caused the player death
	bool playerEnabled						= false;					//The Player control enabled/disabled
	bool canDie								= true;						//Can the player die?
	bool Dead								= false;					//The Player Dead
	bool firstObstacleGenerated				= false;					//The first obstacle generated
	bool hasRevive							= false;					//Can the player revive?
	bool inRevive							= false;					//The player is currently reviving
	bool shieldActive						= false;					//Shield enabled/disabled
	bool magnetActive						= false;					//magnet enabled/disabled
	bool inExtraSpeed						= false;					//The Player is using the extra speed power up
	bool paused								= false;					//The game is paused/unpaused
	bool shopReviveUsed						= false;					//The shop revive is used/unused
	Collider2D destroyableObject ;										//the object the disable when shield is active
	
	bool powerUpUsed						= false;					//A power up is used/unused
	
	Transform thisTransform;											//The transform of this object stored
	
	//Retursn the instance
	public static PlayerManager Instance
	{
		get
		{
			if (myInstance == null)
				myInstance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
			
			return myInstance;
		}
	}


	void Awake(){
		//disable the render of all particles
		for (int i=0;i<bonusParticles.Length;i++){
		bonusParticles[i].GetComponent<Renderer>().enabled=false;
		}
	}

	//Called at the beginning the game
	void Start()
	{
	//	print ("player start ");
		//Calibrates the myInstance static variable
		instances++;
		
		if (instances > 1)
			Debug.Log("Warning: There are more than one Player Manager at the level");
		else
			myInstance = this;
		
		//Set transform and rotationDiv
		thisTransform = this.GetComponent<Transform>();
		startingPos = thisTransform.position;
	//	PlayerMovement.Instance.anim.speed=1f;
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().Play();
		this.GetComponent<Rigidbody2D>().isKinematic=true;


	}



	//play a sound
	public void playSound(string sound){
		switch (sound){
		case "coinCollect" :
			GetComponent<AudioSource>().PlayOneShot(sounds[0]);
			break;
		case "jump" :
			GetComponent<AudioSource>().PlayOneShot(sounds[1]);
			break;
		case "hit" :
			GetComponent<AudioSource>().PlayOneShot(sounds[2]);
			break;
		case "menuButton" :
			GetComponent<AudioSource>().PlayOneShot(sounds[3]);
			break;
		case "powerUpActivate" :
			GetComponent<AudioSource>().PlayOneShot(sounds[4]);
			break;
		case "powerUpCollected" :
			GetComponent<AudioSource>().PlayOneShot(sounds[5]);
			break;
		case "powerUpBuy" :
			GetComponent<AudioSource>().PlayOneShot(sounds[6]);
			break;



		}
		
	}
	

	//Pause the Player/particles
	public void Pause()
	{
	//	print ("player pause ");
		paused = true;
		PlayerMovement.Instance.anim.speed=0f;
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().Pause();
		for (int i=0;i<bonusParticles.Length;i++){
			bonusParticles[i].GetComponent<ParticleSystem>().Pause();
		}
		this.GetComponent<Rigidbody2D>().isKinematic=true;

	}
	//Resume the Player/particles
	public void Resume()
	{
	//	print ("player resume ");
		paused = false;
		PlayerMovement.Instance.anim.speed=1f;
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().Play();
		for (int i=0;i<bonusParticles.Length;i++){
			bonusParticles[i].GetComponent<ParticleSystem>().Play();
		}
		if (!inExtraSpeed){
			this.GetComponent<Rigidbody2D>().isKinematic=false;
			EnableControls();
		}

	}
	//Returns Dead state
	public bool getDead()
	{
//		print ("player dead  : "+Dead);
		return Dead;
	}


	public IEnumerator Revive()
	{
	//	print ("player revive ");
		//If the Player is not reviving
		if (!inRevive)
		{
			//Set revive based variables
			inRevive = true;
			//powerUpUsed = true;
			Dead = false;
			//canDie = true;

			//disable the collider in front of the player and hide it
			//print (crashedObstacle.gameObject.name);


			GameObject go = Instantiate (bonusParticles[5], transform.position, Quaternion.identity) as GameObject;
			Destroy (go, 1);

			// disable all the object that can kill the player in front of him
			Collider2D[] Destroyable=Physics2D.OverlapCircleAll(transform.position, 15f, whatIsDestroyable);
			if (Destroyable.Length>0)
				for (int i=0 ;i<Destroyable.Length;i++){
					//print ("destroyable "+i+"   "+Destroyable);
					Destroyable[i].gameObject.GetComponent<Collider2D>().enabled=false;
					Destroyable[i].gameObject.GetComponent<Renderer>().enabled=false;
				}


			//reset the player stats and play revive animations
			PlayerMovement.Instance.anim.speed=1f;
			this.transform.position = startingPos;
			PlayerMovement.Instance.playRevive();
			yield return new WaitForSeconds(1f);

			//bring back control and scrolling
			EnableControls();
			LevelGenerator.Instance.ContinueScrolling();
			PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().Play();



			//If the player has collected a revive
			if (hasRevive)
			{
				//Set variable, and disable it's hearth
				hasRevive = false;
				GUIManager.Instance.DisableReviveGUI(0);
			}
			//If the player has not collected a revive, but has a revive from the store
			else
			{
				//Set variable, remove revive from account, and disable it's hearth
				shopReviveUsed = true;
				SaveManager.ModifyReviveBy(-1);
				GUIManager.Instance.DisableReviveGUI(1);
			}
			
			//Reset speed, play revive particle, and change texture to intact
			//reviveParticle.Play();
			//subMaterial.material.mainTexture = subTextures[0];
			

			//Wait for 0.4 seconds, and move to starting position
			//yield return new WaitForSeconds(0.4f);
			//StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, -23, thisTransform.position.z), 1.0f, false));
			
			//Wait for 1.2 seconds, and restart level scrolling
			//yield return new WaitForSeconds(1.2f);

			//set variables

			playerEnabled = true;
			inRevive = false;	
		}
		
		//If the player is already in revive, wait for the end of frame, and return to caller
		yield return 0;
	}


	//Reset Player status
	public void ResetStatus(bool moveToStart)
	{ //print ("player reset counter start ? "+moveToStart);

		//Stop the coroutines
		StopAllCoroutines();

		//disable all particles
		for (int i=0;i<bonusParticles.Length;i++){
			bonusParticles[i].GetComponent<Renderer>().enabled=false;
		}
		
		//Disable the bubble particles
		//		bubbles.Clear();
		//		bubbles.Stop();
		
		//Reset variables
		Dead = false;
		paused = false;
		canDie = true;
		PlayerMovement.Instance.anim.speed=1f;
		//PlayerMovement.Instance.RunningDust.renderer.enabled=true;
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().Play();
		this.GetComponent<Rigidbody2D>().isKinematic=false;
		
		inRevive = false;
		hasRevive = false;
		shopReviveUsed = false;
		inExtraSpeed = false;
		shieldActive = false;
		powerUpUsed = false;
		
		firstObstacleGenerated = false;
		
		//Reset rotation and position
		
		this.transform.position = startingPos;


		PlayerMovement.Instance.playStanding();


		//Reset texture

		//If moveToStart, move the Player from the resting position to the starting position
	//	if (moveToStart)
	//	{
	//		StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, -23, thisTransform.position.z), 1.0f, true));
	//	}
	}


	//Enalbe Player controls
	public void EnableControls()
	{
//		print ("player enable controls ");
		playerEnabled = true;
	}

	//Disable Player controls
	public void DisableControls()
	{
	//	print ("player disable controls ");
		playerEnabled = false;
	}

	//return the current status of controls
	public bool getPlayerEnabled()
	{
		return playerEnabled ;
	}

	public bool getPause(){
	   return paused;
	}





	IEnumerator playerIsDead ( float timing){
	//	print (" kill player");

		Dead = true;
		DisableControls();
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled=false;
		LevelGenerator.Instance.StartCoroutine("StopScrolling", 0f);
		yield return new WaitForSeconds(0.2f);
		PlayerMovement.Instance.playDead();
		//yield return new WaitForSeconds(1f);
	//	print ("dead ");
	}




	//Play an explosion			
	void PlayExplosion(Transform expParent)
	{
		//If the sub collided with a torpedo
		if (expParent.name == "Torpedo")
		{
			//Notify torpedo manager
				expParent.transform.parent.gameObject.GetComponent<Torpedo>().TargetHit(true);
		}
		//If the sub collided with something else
		else
		{
			//Find the particle child, and play it
			ParticleSystem explosion = expParent.Find("ExplosionParticle").gameObject.GetComponent("ParticleSystem") as ParticleSystem;
			explosion.Play();
			//Disable the object's renderer and collider
			expParent.GetComponent<Renderer>().enabled = false;
			expParent.GetComponent<Collider>().enabled = false;
		}
	}


	//Scale object to scale under time
	IEnumerator ScaleObject (Transform obj, Vector3 scale, float time, bool deactivate)
	{
		
		//Set the object active
		//EnableDisable(obj.gameObject, true);
		//Get it's current scale
		Vector3 startScale = obj.localScale;
		
		//Scale the object
		var rate = 1.0f / time;
		var t = 0.0f;
		
		while (t < 1.0f) 
		{
			//If the game is not paused, increase t, and scale the object
			if (!paused)
			{
				t += Time.deltaTime * rate;
				//obj.localScale = Vector3.Lerp(startScale, scale, t);
			}
			
			yield return new WaitForEndOfFrame();
		}
		

			

	}

	//Activate extra speed effects for time
	IEnumerator ExtraSpeedEffect(float time)
	{
		//freeze the player
		DisableControls();
		transform.GetComponent<Rigidbody2D>().isKinematic=true;
		PlayerJumping.Instance.mycollider.enabled=false;
		PlayerJumping.Instance.mycolliderBox.enabled=false;



		//enable the electricity effect
		bonusParticles[2].GetComponent<Renderer>().enabled=true;
		//enable the fire effect
		//bonusParticles[3].renderer.enabled=true;
		//activation effect



		//Get the current scroll speed, then set scroll speed to 3
		float newSpeed = LevelGenerator.Instance.scrollSpeed;
		LevelGenerator.Instance.scrollSpeed = 2;
		
		//Wait for time
		double waited = 0;
		while (waited <= time)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Set variabler for extra speed
		LevelGenerator.Instance.scrollSpeed = newSpeed;
		inExtraSpeed = false;
		canDie = true;

		// disable all the object that can kill the player in front of him
		Collider2D[] Destroyable=Physics2D.OverlapCircleAll(transform.position, 15f, whatIsDestroyable);
		if (Destroyable.Length>0)
		for (int i=0 ;i<Destroyable.Length;i++){
			//print ("destroyable "+i+"   "+Destroyable);
			Destroyable[i].gameObject.GetComponent<Collider2D>().enabled=false;
			Destroyable[i].gameObject.GetComponent<Renderer>().enabled=false;
		}

		//enable the player
		EnableControls();
		transform.GetComponent<Rigidbody2D>().isKinematic=false;
		PlayerJumping.Instance.mycollider.enabled=true;
		PlayerJumping.Instance.mycolliderBox.enabled=true;


		
		//disable the electricity effect
		bonusParticles[2].GetComponent<Renderer>().enabled=false;
		//disable the fire effect
		//bonusParticles[3].renderer.enabled=false;
		
		//Allow the level generator to increase scrolling speed
		LevelGenerator.Instance.ExtraSpeedOver();
	}
	//The sink effects
	IEnumerator SinkEffects()
	{
		//Wait for 0.5 seconds, and stop the level scrolling in 2.5 seconds
		yield return new WaitForSeconds(0.5f);
		LevelGenerator.Instance.StartCoroutine("StopScrolling", 2.5f);
		
		//Wait for 2.75 seconds, and disable smoke emission
		yield return new WaitForSeconds(2.75f);
		smoke.enableEmission = false;
	}
	//Move obj to endPos in time
	IEnumerator MoveToPosition (Transform obj, Vector3 endPos, float time, bool enableSub) 
	{

		//Declare variables, get the starting position, and move the object
		float i = 0.0f;
		float rate = 1.0f / time;
		
		Vector3 startPos = obj.position;
		
		while (i < 1.0) 
		{
			//If the game is not paused, increase t, and scale the object
			if (!paused)
			{
				i += Time.deltaTime * rate;
				obj.position = Vector3.Lerp(startPos, endPos, i);
			}
			
			//Wait for the end of frame
			yield return 0;
		}
		
		//The the sub is set to enable, activate it, and decrease emission rate
		if (enableSub)
		{
			playerEnabled = true;
		}
	}
	//Called when the player collects/activates an extra speed
	public void ExtraSpeed()
	{
		//If the player is already using an extra speed, or sinking, or the controls are not enabled, return
		if (inExtraSpeed || !playerEnabled)
			return;
		
		//Set power up based variables
		powerUpUsed = true;
		inExtraSpeed = true;
		canDie = false;
		
		//Activate particles
	//	EnableDisable(speedParticle, true);
	//	EnableDisable(speedTrail, true);
		
		//Set level generator for extra speed, and activate it
		LevelGenerator.Instance.ExtraSpeedEffect();
		StartCoroutine (ExtraSpeedEffect(3));
	}


	IEnumerator ActivatedestroyableObject(Collider2D objectName)
	{
		//Wait for the pause elements to move out of the screen
		yield return new WaitForSeconds(2f);
		objectName.isTrigger=false;
	}


	//Called when a magnet power up is picked up, launches the magnet particle
	public void  RaiseMagnet()
	{
		 print ("magnet activated");
		//If a magnet is already activates,  or dying, or the controls are not enabled, return
		if (magnetActive || !playerEnabled)
			return;
		
		//Set power up based variables, and raise the shield
		powerUpUsed = true;
		magnetActive = true;
		//enable the particle effect
		bonusParticles[0].GetComponent<Renderer>().enabled=true;
		StartCoroutine(DisableMagnet());
		//StartCoroutine(ScaleObject(shield.transform, new Vector3(18, 16.2f, 1), 0.35f, false));
	}

	//Disable magnet
	IEnumerator DisableMagnet()
	{

		//Wait for 10 seconds
		double waited = 0;
		while (waited <= 15f)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Mark magnet as inactive
		magnetActive = false;
		//disable the particle effect
		bonusParticles[0].GetComponent<Renderer>().enabled=false;

		print ("disable magnet");
	}

	public bool getMagnet(){
		return (magnetActive);
	}


	//Called when the player collects/activate a shield power up
	public void RaiseShield()
	{ print ("shield activated");
		//If a shield is already activates,  or dying, or the controls are not enabled, return
		if (shieldActive || !playerEnabled)
			return;
		//activate the particle effect
		bonusParticles[1].GetComponent<Renderer>().enabled=true;

		//Set power up based variables, and raise the shield
		powerUpUsed = true;
		shieldActive = true;
		StartCoroutine(DisableShield());
		//StartCoroutine(ScaleObject(shield.transform, new Vector3(18, 16.2f, 1), 0.35f, false));
	}



	//Disable shield
	IEnumerator DisableShield()
	{

		//Wait for 10 seconds
		double waited = 0;
		while (waited <= 15f)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}


		// disable all the object that can kill the player in front of him
		Collider2D[] Destroyable=Physics2D.OverlapCircleAll(transform.position, 10f, whatIsDestroyable);
		if (Destroyable.Length>0)
		for (int i=0 ;i<Destroyable.Length;i++){
			//print ("destroyable "+i+"   "+Destroyable);
			Destroyable[i].gameObject.GetComponent<Collider2D>().enabled=false;
			Destroyable[i].gameObject.GetComponent<Renderer>().enabled=false;
		}

		//Mark shield as inactive
		shieldActive = false;
		//disable particle effect
		bonusParticles[1].GetComponent<Renderer>().enabled=false;
	//	print ("disable shield");
	}

	//Return revive state
	public bool HasRevive()
	{	//	print (" i have revive ?? = "+hasRevive);

		if (hasRevive || (SaveManager.GetRevive() > 0 && !shopReviveUsed))
			return true;
		else
			return false;
	}
	//Return power up used state
	public bool PowerUpUsed()
	{
		return powerUpUsed;
	}
	

	//Called when a revive is collected
	void ReviveCollected()
	{
		//Register revive, Show hearth on the GUI, and disable additional revive generation
		hasRevive = true;
		GUIManager.Instance.RevivePicked();
		LevelGenerator.Instance.powerUpMain.DisableReviveGeneration();
	}



	//Update mission manager based on name
	void UpdateMission(string name)
	{
		//Obstacle based update
		if (name == "Mine" || name == "MineChain" || name == "Chain")
			MissionManager.Instance.ObstacleEvent("Mine");
		else if (name == "Torpedo")
			MissionManager.Instance.ObstacleEvent("Torpedo");
		else if (name == "Laser" || name == "LaserBeam")
			MissionManager.Instance.ObstacleEvent("Laser");
		//Power up based update
		else if (name == "ExtraSpeed")
			MissionManager.Instance.PowerUpEvent("Extra Speed");
		else if (name == "Shield")
			MissionManager.Instance.PowerUpEvent("Shield");
		else if (name == "SonicWave")
			MissionManager.Instance.PowerUpEvent("Sonic Wave");
		else if (name == "Revive")
			MissionManager.Instance.PowerUpEvent("Revive");
	}


	//Called when the Player trigger with something
	void OnTriggerEnter2D (Collider2D other)
	{
		//If the sub triggered with an obstacle
		if (other.tag == "Obstacles" || other.tag == "Bonus") 
		{	//print ("obstacles"+other.name+firstObstacleGenerated);
			//If it is a coin
			if (other.transform.name == "Coin" || other.transform.name == "coin")
			{
				//Disable the coin's renderer and collider
				other.GetComponent<Renderer>().enabled = false;
				other.GetComponent<Collider2D>().enabled = false;
				
				//Play it's particle system, and increase coin ammount
				other.transform.Find("CoinParticle").gameObject.GetComponent<ParticleSystem>().Play();
				playSound("coinCollect");
				LevelManager.Instance.CoinGathered();
				
			}
			//If the sub triggered with a hazard
			//if (other.transform.name == "Mine" || other.transform.name == "Chain" || other.transform.name == "MineChain" || other.transform.name == "Torpedo" || other.transform.name == "Laser" || other.transform.name == "LaserBeam")
			/*		{
				//get the object who might have killed the player"


			
				//Notify the mission manager
				UpdateMission(other.transform.name);
				//If the sub is not sinking, and doesn't have a protection
				if (!Dead && canDie && !shieldActive)
				{
					//kill him, disable controls, and play animation it
					Dead = true;
					DisableControls();
					//WreckSub();
				}


				//Play explosion particle
				PlayExplosion (other.transform); 
			}  */
			//If the sub triggered with the obstacle generation triggerer, and it is not triggered before
			else if (other.name == "ObstacleGenTriggerer" && !firstObstacleGenerated)
			{
			//	print ("start generating obstacles");
				//Trigger it, and start obstacle generation
				firstObstacleGenerated = true;
				LevelGenerator.Instance.GenerateObstacles();
			}  


		}
		//If the sub triggered with a power up
		else if (other.tag == "PowerUps")
		{
			PlayerManager.Instance.playSound("powerUpActivate");
			//Notify mission manager
			UpdateMission(other.transform.name);
			
			//Activate proper function based on name
			switch (other.transform.name)
			{
			case "ExtraSpeed":
				if (playerEnabled)
					ExtraSpeed();
				break;
				
			case "Shield":
				if (playerEnabled)
					RaiseShield();
				break;
				
			case "Revive":
				if (playerEnabled)
					ReviveCollected();
				break;

			case "Magnet":
				if (playerEnabled)
					RaiseMagnet();
				break;

			}
			
			//Reset the power up
			other.GetComponent<PowerUp>().ResetThis();
		}
	}


	void OnCollisionEnter2D ( Collision2D other  ){
		//crashedObstacle = other;
	}


	//Called at every frame
	void Update()
	{
		//disable all destroyable object when shield is activated
		if (shieldActive)
		{	
			destroyableObject =Physics2D.OverlapCircle(transform.position, 3f, whatIsDestroyable);
			if (destroyableObject) // check if there is one way platform beneath the player
			{
				if (destroyableObject.isTrigger==false){
				//	print (destroyableObject.gameObject.name);
					destroyableObject.isTrigger=true;
					StartCoroutine(ActivatedestroyableObject(destroyableObject));
				}
			}
		}
		
	}
}