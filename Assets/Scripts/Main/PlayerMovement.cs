using UnityEngine;
using System.Collections;
public enum playerStates {Standing,	Idle , Jumping , Falling ,Gliding , Dead ,Reviving}

public class PlayerMovement : MonoBehaviour {
//public Transform player_graphic_pointer;
public PlayerOneWay GroundChecker;  //check if we are touching the ground
public GameObject RunningDust ;    //particle to emit (dust) when we run on the ground


//variables to use with free movement (not in endless running)
private float max_walking_speed = 6; // the maximal speed we can walk and/or switch to running
private float max_running_speed = 8; // the maximal running speed
private float gain_move_speed = 12; // how fast will we get to the maximum?
private float lose_move_speed=13; // how fast will we lose the speed if we won't press a button
private float stopping_lose_move_speed=15; // how quick do we stop if we are going against movement direction?


//Internal variables (do not modify !)
private Vector2 startPosition;
private bool restarted=false;
public playerStates current_mode = playerStates.Standing; // the current and start mode we're in
public float current_speed = 0; // this is our calculated speed, the velocity of the rigidbody2D can get in the way
private float max_speed = 0; // internal max speed for looking the speed
public int movement_direction = 1; // the last direction we moved
private float last_max_speed = 0; // needed to slowly decrease the max_speed
public Vector3 last_velocity; // for now a workaround for the stuck-in-wall problem
private string x_axis_name = "Horizontal"; // the buttons and/or controller-analogstick for left&right.
private string y_axis_name = "Vertical";// the buttons and/or controller-analogstick for Up&Down
private Vector3 start_pos; // needed for respawning
static PlayerMovement myInstance;
static int instances = 0;

//animations variables (internal)
public Animator anim;
private string myAnim;
private int sizeAnims=playerStates.GetValues(typeof(playerStates)).Length;
private string[] anims = new string[8];


//temporary variables
Vector2 real_axis ;
Vector2 tmp_direction;
Vector2 tmp_velocity;


//Retursn the instance
public static PlayerMovement Instance
{
	get
	{
		if (myInstance == null)
			myInstance = FindObjectOfType(typeof(PlayerMovement)) as PlayerMovement;
			return myInstance;
	}
}

void Start (){

	//Calibrates the myInstance static variable
	instances++;
	
	if (instances > 1)
		Debug.Log("Warning: There are more than one Player Movement at the level");
	else
		myInstance = this;


	tag = "Player"; // WE are the player, okaaaay?!

	anim= GetComponent<Animator>();
	//boost_pointer=transform;
	//get the animations names into an array for better performance while iterating
	int i=0;
	foreach (playerStates p in playerStates.GetValues(typeof(playerStates)) )
	{
		anims[i]=p.ToString();
		i++;
	}

	start_pos = transform.position;

	//start using space key
	GameEventManager.GameStart += GameStart;
	GameEventManager.GameOver += GameOver;
	startPosition = transform.localPosition;
	//renderer.enabled = false;
	//rigidbody2D.isKinematic = true;
	
	enabled = false;

	//hide the running dust system particle
	RunningDust.GetComponent<Renderer>().enabled=false;


}

private void GameStart () {
	//	transform.localPosition = startPosition;
//		renderer.enabled = true;
//		rigidbody2D.isKinematic = false;
		enabled = true;
		if (restarted)
		{
			restarted=false;
		}
	}
	
private void GameOver () {
	//	renderer.enabled = false;
//		rigidbody2D.isKinematic = true;
		enabled = false;
		restarted=true;

		//hide the running dust system particle
		RunningDust.GetComponent<Renderer>().enabled=false;
	}


// Set the bool values to play the current animation
void playAnim (){
		//Debug.Log (sizeAnims);
	

	for (int i=0;i<sizeAnims;i++)
	{
		if (anims[i]==current_mode.ToString())
				anim.SetBool(anims[i],true);
		else
			anim.SetBool(anims[i],false);
	}

}

//// this functions returns the correct button that is needed right now. this is needed for mobile versions of the game. if use_mobile_input is true, it will return the mobile inputs ////////
public Vector2 return_real_axis (){
if (!LevelManager.Instance.Scrolling){
	if(InputManager.Instance.useTouch)
	{
		//return mobile_joystick.position;
		return real_axis;
	}
	else
	{
			real_axis.x = Input.GetAxis(x_axis_name);
			real_axis.y = Input.GetAxis(y_axis_name);
			return real_axis;
	
	}
	}
	else 
		return(new Vector2(0,0));
}

//play the standing animation when we are not playing
public void playStanding (){

	current_mode=playerStates.Standing;
	for (int i=0;i<sizeAnims;i++)
	{
		if (anims[i]==current_mode.ToString()){

			anim.SetBool(anims[i],true);
//				print (" i played stand animation");
			}
		else
			anim.SetBool(anims[i],false);
	}
}

public void playDead (){
	if (current_mode!=playerStates.Dead){

		//make sure the player stay on the ground
		GetComponent<Rigidbody2D>().velocity=Vector2.zero;
		GetComponent<Rigidbody2D>().velocity=new Vector2 (0,-30);
		current_mode=playerStates.Dead;

		for (int i=0;i<sizeAnims;i++)
		{
			if (anims[i]==current_mode.ToString())
				anim.SetBool(anims[i],true);
			else
				anim.SetBool(anims[i],false);
		}
	}
}

public void playRevive (){
	if (current_mode==playerStates.Dead){
		
		//make sure the player stay on the ground
		current_mode=playerStates.Reviving;
		
		for (int i=0;i<sizeAnims;i++)
		{
			if (anims[i]==current_mode.ToString())
				anim.SetBool(anims[i],true);
			else
				anim.SetBool(anims[i],false);
		}
	}
}




///////////////////////////////////////////////
void FixedUpdate (){

if(!PlayerManager.Instance.getPlayerEnabled())
		{
			return;
		}	
	playAnim();
	last_velocity = GetComponent<Rigidbody2D>().velocity;


	//control the running dust when we are grounded
		if((current_mode==playerStates.Idle) && !GroundChecker.bUp && GroundChecker.bGrounded && PlayerManager.Instance.getPlayerEnabled() && current_mode!=playerStates.Standing)
	{RunningDust.GetComponent<Renderer>().enabled=true;}
	else
	{RunningDust.GetComponent<Renderer>().enabled=false;}
/*
	if(current_mode==playerStates.Hit)
	{
			current_mode=playerStates.Idle;
	}
	*/
	if(current_mode==playerStates.Jumping|| current_mode==playerStates.Falling || current_mode==playerStates.Gliding) // in some cases, we can't use our movement code
		return;
/*
	if((current_mode==playerStates.Walking) || (current_mode==playerStates.Running) || (current_mode==playerStates.Boosting)) // check the direction we are moving
	{
		if(return_real_axis().x!=0.0f)
		{
			if(Mathf.Sign(return_real_axis().x)!=movement_direction) // we changed the direction while walking, let's stop
			{
				current_mode=playerStates.Stopping;
			}
			movement_direction = (int)Mathf.Sign(return_real_axis().x); // -1 = left; 1 = right;
				if(current_mode!=playerStates.Jumping && current_mode!=playerStates.Falling) {
					if (movement_direction==1)
						transform.eulerAngles= new Vector2 (transform.eulerAngles.x,0);
					else
						transform.eulerAngles= new Vector2 (transform.eulerAngles.x,180);
					//Debug.Log ("direction="+movement_direction+"-----tmp dir = "+tmp_direction.y);
				}
		}
	}
	*/
	if((Mathf.Abs(return_real_axis().x)<0.01f || !PlayerManager.Instance.getPlayerEnabled())  && current_mode!=playerStates.Idle) // no joystick input? then idle
	{
		current_mode=playerStates.Idle;
		last_max_speed=0;
	}	
		/*
	if(current_mode==playerStates.Stopping) // we are stopping
	{
		if(Mathf.Sign(return_real_axis().x)==movement_direction && PlayerManager.Instance.getPlayerEnabled()) // we are still trying to get in the stopping direction
		{
			current_speed = Mathf.Max(0,current_speed-(stopping_lose_move_speed*Time.deltaTime));
		}
		else // if not then stop the stopping
		{	
			current_mode=playerStates.Idle;
			movement_direction = movement_direction*-1;
		}
		if(current_speed==0)
				current_mode=playerStates.Idle;
			
		tmp_velocity.x = current_speed*movement_direction*-1;
		tmp_velocity.y = rigidbody2D.velocity.y;
		rigidbody2D.velocity=tmp_velocity;
		return;
	}
	if(current_mode==playerStates.Sliding) // we slow down
	{
		if(current_speed<0.1f)
		{
			current_mode=playerStates.Idle;
		}
		max_speed = current_speed;
	}

	if(current_mode==playerStates.Idle) // if we idle, the max speed is the maximun, so we can decrease
	{
		if(current_speed>0.1f)
		{
			
			if(current_speed<max_running_speed)
				current_mode=playerStates.Sliding;
			else
				current_mode=playerStates.RunSliding;
		}
		max_speed = current_speed;
	}
	if(PlayerManager.Instance.getPlayerEnabled())
	{
		if(current_mode==playerStates.Walking)
			max_speed = max_walking_speed*Mathf.Abs(return_real_axis().x);
		if(current_mode==playerStates.Running)
		{
			max_speed = max_running_speed*Mathf.Abs(return_real_axis().x);	
		}

	}
	if(max_speed < last_max_speed)
	{
		max_speed = last_max_speed - lose_move_speed*Time.deltaTime;
	}
	else
	{
		last_max_speed = max_speed;
	}
	if(Mathf.Abs(return_real_axis().x)>0.01f && PlayerManager.Instance.getPlayerEnabled() && (current_mode==playerStates.Idle || current_mode==playerStates.Sliding))
	{
		//	max_speed=0;
		last_max_speed=0;
		current_mode=playerStates.Walking;
	}

	if(current_mode!=playerStates.Stopping)
	{
		if(Mathf.Abs(return_real_axis().x)>0.02f)
			current_speed = Mathf.Clamp(current_speed+(gain_move_speed*Time.deltaTime),max_speed*-1,max_speed);
		else
		{
			current_speed = Mathf.Clamp(current_speed-(lose_move_speed*Time.deltaTime),0,max_speed);
		}
		tmp_velocity.x = current_speed*movement_direction;
		tmp_velocity.y = rigidbody2D.velocity.y;
		rigidbody2D.velocity=tmp_velocity;
	}
	
	if((current_speed<=max_running_speed && current_mode == playerStates.Boosting))
	{
		current_mode=playerStates.Running;
	}

	if((current_speed<=max_walking_speed && current_mode == playerStates.Running))
	{
		current_mode=playerStates.Walking;
	}
	if((current_speed>=max_walking_speed && current_mode == playerStates.Walking) && current_mode == playerStates.Walking && current_speed>=max_walking_speed)
	{
		current_mode=playerStates.Running;
	}
	if(current_mode!=playerStates.Idle)
		last_max_speed = max_speed;

*/
	//move the player a little back if he get moved by epsilon while landing
	if ((transform.position.x != start_pos.x)  && LevelManager.Instance.getScrolling() && !PlayerManager.Instance.getDead())
		//reset position if moved by Epsilon
	if (start_pos.x-0.2f < transform.position.x  && transform.position.x < start_pos.x+0.2f){
		//print (transform.position.x-start_pos.x);		
		transform.position=new Vector2(start_pos.x,transform.position.y);
	}
		//kill him if he move too much ! (obselete)
	else if (start_pos.x-0.2f < transform.position.x  && transform.position.x < start_pos.x+0.2f)
	{
		//	StartCoroutine(restart_level());
		//	GameEventManager.TriggerGameOver();
		}



}
	/*

	void OnCollisionEnter2D ( Collision2D collisionInfo  ){
		if(current_mode==playerStates.Dead)
		{
			foreach(ContactPoint2D contact in collisionInfo.contacts) {
				//print (collisionInfo.gameObject.name);
				//Debug.DrawRay(contact.point, contact.normal * 10, Color.white);

				//collisionInfo.gameObject.collider2D.enabled=false;
				//collisionInfo.gameObject.renderer.enabled=false;
			}
		}
	}
	*/
/*	
	void OnCollisionStay2D ( Collision2D collisionInfo  ){
		if(current_mode==playerStates.Dead)
		{
			foreach(ContactPoint2D contact in collisionInfo.contacts) {
				print (collisionInfo.gameObject.name);
				Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
			}
		}
	}
	*/
	void OnCollisionExit2D ( Collision2D collisionInfo  ){
	}


}