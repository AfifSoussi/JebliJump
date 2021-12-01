using UnityEngine;
using System.Collections;

public class PlayerJumping : MonoBehaviour {

//exposed variables for endless running
public LayerMask collide_layer = -5;
public LayerMask whatIsPlatform;
public PlayerOneWay PlatformChecker ;
public bool  can_glide = true;
public float jump_strength = 8;
public int multi_jumps = 0;
public float max_jump_speed = 6;
public float glide_max_fall_speed = -1;
public float jump_Gravity =10 ;
public float glide_Anti_Gravity =2 ;




//more variables for better control in free mode

	//jumping
private float air_control = 4;
private bool  rotate_in_air = false;
public int jumped = 0;
private Vector3 jump_forceVector = -Vector3.up; // added to the normal physics. useful so the jumps won't feel floaty

	//gliding
private float glide_air_control = 3;
private float glide_max_speed = 5;
private Vector3 glide_force_Vector = Vector3.up; // added while gliding
	
//internal variables
private float min_falling_speed = 0;
private float block_left = 0; // we will block the direction for a short time so the player won't "slow down" wile pressing against a wall
private float block_right = 0; 
private bool passingThrough = false;
static PlayerJumping myInstance;
static int instances = 0;


// internal raycasting and ground checking variables
private Vector3 playerCenter;
public CircleCollider2D mycollider;
public BoxCollider2D mycolliderBox;
private RaycastHit2D raycast_down;
private RaycastHit2D raycast_down_right;
private RaycastHit2D raycast_down_left;
private RaycastHit2D raycast_down_Platform;
private Collider2D Start_Jump_Platform;

private Vector2 raycast_left;
private Vector2 raycast_right;
private Vector2 raycast_Platform;
private float the_distance;
private float the_distance_right;
private float the_distance_left;
private float to_bottom;
private	float startJump;
private	float endJump;
private	float startDistance;
private	float endDistance;
	private	float startHeight;
	private	float endHeight;
	float stageSpeed ;

	private float FixedDeltaTime;

//Return the instance
public static PlayerJumping Instance
{
	get
	{
		if (myInstance == null)
			myInstance = FindObjectOfType(typeof(PlayerJumping)) as PlayerJumping;
		return myInstance;
	}
}


void Start (){

	//Calibrates the myInstance static variable
	instances++;
		
	if (instances > 1)
		Debug.Log("Warning: There are more than one Player Jumping at the level");
	else
		myInstance = this;
	
	GameEventManager.GameStart += GameStart;
	GameEventManager.GameOver += GameOver;
	enabled=false;

	mycollider = GetComponent<CircleCollider2D>() as CircleCollider2D;	
	mycolliderBox = GetComponent<BoxCollider2D>() as BoxCollider2D;	
}


private void GameStart () {
		enabled = true;
		mycolliderBox.enabled=true;
		mycollider.enabled=true;
	}
	
private void GameOver () {
		enabled = false;
		mycolliderBox.enabled=false;
		mycollider.enabled=false;
	}

//disable the platform below the player so he can jump down the enable it again
private IEnumerator platformDisable (RaycastHit2D thePlatform){
	thePlatform.collider.isTrigger=true;
	passingThrough=true;
	yield return new WaitForSeconds(0.7f);
	thePlatform.collider.isTrigger=false;
	passingThrough=false;
}

public bool getPassingThrough (){
		return passingThrough;
	}



void FixedUpdate (){

		if(!PlayerManager.Instance.getPlayerEnabled() || PlayerMovement.Instance.current_mode==playerStates.Dead)return;

	//to_bottom = mycollider.radius*1.2f;
	to_bottom = 1.2f;


	stageSpeed = LevelGenerator.Instance.scrollSpeed *10;


	if (stageSpeed < 1.1f && jump_strength!=23 && jump_Gravity!=50 && glide_max_fall_speed!= -3){
	//	print ("level one");
		jump_strength=23;
		jump_Gravity=50;
		glide_max_fall_speed= -3;
	} else 	if (stageSpeed >= 1.1f && stageSpeed < 1.35f && jump_strength!=27 && jump_Gravity!=75 && glide_max_fall_speed!= -4){
	//	print ("level two");
		jump_strength=27;
		jump_Gravity=75;
		glide_max_fall_speed= -4;
	}else 	if (stageSpeed >= 1.35f && stageSpeed < 1.55f && jump_strength!=32 && jump_Gravity!=110 && glide_max_fall_speed!= -5){
		print ("level three");
		jump_strength=32;
		jump_Gravity=110;
		glide_max_fall_speed= -5;
	}else 	if (stageSpeed >= 1.55f && stageSpeed < 1.75f && jump_strength!=34 && jump_Gravity!=135 && glide_max_fall_speed!= -7){
	///	print ("level four");
		jump_strength=34;
		jump_Gravity=135;
		glide_max_fall_speed= -7;
	}else 	if (stageSpeed >= 1.75f && stageSpeed < 1.95f && jump_strength!=38 && jump_Gravity!=170 && glide_max_fall_speed!= -10){
	//	print ("level four");
		jump_strength=38;
		jump_Gravity=170;
		glide_max_fall_speed= -10;
	}else 	if (stageSpeed >= 1.95f && jump_strength!=40 && jump_Gravity!=200 && glide_max_fall_speed!= -15){
	//	print ("level five");
		jump_strength=40;
		jump_Gravity=200;
		glide_max_fall_speed= -15;
	}






	bool  gliding = false;
	
	block_left = Mathf.Max(0,block_left-Time.deltaTime);
	block_right = Mathf.Max(0,block_right-Time.deltaTime);
	//transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0);
	
		//jumping down from a the air
		if (InputManager.Instance.inputDownJump && (PlayerMovement.Instance.current_mode==playerStates.Jumping || PlayerMovement.Instance.current_mode==playerStates.Falling || PlayerMovement.Instance.current_mode==playerStates.Gliding)){
			//print ("get Down baby");
			PlayerMovement.Instance.GetComponent<Rigidbody2D>().velocity=new Vector2 (0,-30);
		}


		if(InputManager.Instance.inputJump && ((PlayerMovement.Instance.current_mode!=playerStates.Jumping && PlayerMovement.Instance.current_mode!=playerStates.Falling) || jumped<=multi_jumps) && PlayerMovement.Instance.current_mode!=playerStates.Gliding)
		{
			//reset input
			InputManager.Instance.inputJump=false;

			//jumping
			min_falling_speed=0;
			if(jumped==0 && PlayerMovement.Instance.current_mode!=playerStates.Falling){
				PlayerManager.Instance.playSound("jump");
				jumped++;
			}
			else
				jumped++;
			PlayerMovement.Instance.current_mode=playerStates.Jumping;
			if (PlatformChecker.getPlatform()){
				Start_Jump_Platform=PlatformChecker.getPlatform();
			}
			if(jumped==1){
					PlayerMovement.Instance.current_mode=playerStates.Jumping;
			}
			else
				{
					PlayerMovement.Instance.current_mode=playerStates.Jumping;
					PlayerMovement.Instance.anim.CrossFade("MultiJumping",0f);
				}




			//CONSTANT_VALUE_1 = 4 * maxJumpHeight / jumpDistance
			//	CONSTANT_VALUE_2 = 0.5 / maxJumpHeight
					
			//		jumpForce.y = SpeedManager.speed * CONSTANT_VALUE_1
			//		gravity = jumpForce.y * jumpForce.y * CONSTANT_VALUE_2
	
			/*
			//start generating
			startJump=Time.time;
			startDistance = LevelGenerator.Instance.distance;

			LevelGenerator.Instance.Resume();
			 */
			/*		float CONSTANT_VALUE_1 = 4f * 5f / 10f;
			float 	CONSTANT_VALUE_2 = 0.5f / 5f;
			Vector2 gravity ;
			gravity.x=0;



			jump_strength = LevelManager.Instance.getSpeed() * CONSTANT_VALUE_1;
			gravity.y = ((jump_strength * jump_strength * CONSTANT_VALUE_2)+10)*-1;

			print ("jump strengh="+jump_strength+"  gravity="+gravity.y);

			Physics2D.gravity = gravity;
*/
			GetComponent<Rigidbody2D>().velocity=new Vector2 (GetComponent<Rigidbody2D>().velocity.x,jump_strength);

/*
		//stopping
		if(PlayerMovement.Instance.current_mode==playerStates.Stopping){
			rigidbody2D.velocity=new Vector2 (0,rigidbody2D.velocity.y);
			PlayerMovement.Instance.current_speed=0;
			}
*/
}
		//gliding
		if(InputManager.Instance.inputGlideStart && (PlayerMovement.Instance.current_mode==playerStates.Falling)  && PlayerMovement.Instance.current_mode!=playerStates.Gliding && can_glide)
		{
			//reset input
			InputManager.Instance.inputGlideStart =false;


				//print ("gliding");
			PlayerMovement.Instance.current_mode=playerStates.Gliding;
			GetComponent<Rigidbody2D>().velocity=new Vector2 (GetComponent<Rigidbody2D>().velocity.x,0);

			PlayerMovement.Instance.anim.CrossFade("StartGliding",0f);
		}
		//StopGliding
		if(InputManager.Instance.inputGlideStop && PlayerMovement.Instance.current_mode==playerStates.Gliding )
		{
			//reset input
			InputManager.Instance.inputGlideStop =false;
	
				PlayerMovement.Instance.current_mode=playerStates.Falling;
				PlayerMovement.Instance.anim.CrossFade("StopGliding",0f);
		}
		
		if(PlayerMovement.Instance.current_mode==playerStates.Gliding) // set the bool  true, it's faster than check everytime the strings
				gliding = true;

		//falling from jump
		if(GetComponent<Rigidbody2D>().velocity.y<0)
		{			startHeight = transform.position.y;
			if(PlayerMovement.Instance.current_mode==playerStates.Jumping)
			{
//				print (transform.position.y);
				PlayerMovement.Instance.current_mode=playerStates.Falling;
			}
		}
		if(PlayerMovement.Instance.current_mode==playerStates.Jumping || PlayerMovement.Instance.current_mode == playerStates.Falling || PlayerMovement.Instance.current_mode == playerStates.Gliding)
		{
			if(jumped==0)
			{
				jumped=1;
			}
			if(PlayerMovement.Instance.current_mode == playerStates.Falling)
			{
				min_falling_speed=GetComponent<Rigidbody2D>().velocity.y;
			}
			jump_forceVector.y=jump_Gravity*-1;
			glide_force_Vector.y=glide_Anti_Gravity;
			if(!gliding)
			{
				GetComponent<Rigidbody2D>().AddForce(jump_forceVector);
			}
			else
				GetComponent<Rigidbody2D>().AddForce(glide_force_Vector);

			if(PlayerMovement.Instance.return_real_axis().x < -0.2f && block_left==0) // moving left
			{
				if(PlayerMovement.Instance.movement_direction==1) // want to move left but facing right
				{
					if(PlayerMovement.Instance.current_speed==0)
						PlayerMovement.Instance.movement_direction=-1;
					if(!gliding)
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed - (air_control);
					else 
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed - (glide_air_control);
				}
				else
				{
					if(!gliding) 
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed + (air_control);
					else
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed + (glide_air_control);
				}
			}
			if(PlayerMovement.Instance.return_real_axis().x > 0.2f  && block_right==0) // moving right
			{
				if(PlayerMovement.Instance.movement_direction==-1) // want to move right but facing left
				{
					if(PlayerMovement.Instance.current_speed==0)
						PlayerMovement.Instance.movement_direction=1;
					if(!gliding)
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed - (air_control);
					else	
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed - (glide_air_control);
				}
				else
				{
					if(!gliding)
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed + (air_control);
					else 
							PlayerMovement.Instance.current_speed = PlayerMovement.Instance.current_speed + (glide_air_control);
				}
			}	
			
			if(!gliding)
				PlayerMovement.Instance.current_speed = Mathf.Clamp(PlayerMovement.Instance.current_speed,0,max_jump_speed);
			else 
				PlayerMovement.Instance.current_speed = Mathf.Clamp(PlayerMovement.Instance.current_speed,0,glide_max_speed);
			//Debug.Log(PlayerMovement.Instance.current_speed + " / " + PlayerMovement.Instance.movement_direction);
			
			Vector2 tmp_velocity = GetComponent<Rigidbody2D>().velocity;
			tmp_velocity.x = PlayerMovement.Instance.current_speed*PlayerMovement.Instance.movement_direction;
			GetComponent<Rigidbody2D>().velocity=tmp_velocity;

			//falling
			if(!gliding && PlayerMovement.Instance.current_mode == playerStates.Falling)
			{
				tmp_velocity = GetComponent<Rigidbody2D>().velocity;
				tmp_velocity.y = Mathf.Min(min_falling_speed,GetComponent<Rigidbody2D>().velocity.y);
				GetComponent<Rigidbody2D>().velocity=tmp_velocity;
			}
			//gliding
			if(gliding){
				tmp_velocity = GetComponent<Rigidbody2D>().velocity;
				tmp_velocity.y = Mathf.Max(GetComponent<Rigidbody2D>().velocity.y,glide_max_fall_speed);
				GetComponent<Rigidbody2D>().velocity=tmp_velocity;
			}
			//rotate player
			if(rotate_in_air &&PlayerMovement.Instance.return_real_axis().x!=0)
			{
					if (PlayerMovement.Instance.movement_direction==1)
						transform.eulerAngles= new Vector2 (transform.eulerAngles.x,0);
					else
						transform.eulerAngles= new Vector2 (transform.eulerAngles.x,180);
			}
		}


////////////// calculate if we are on the ground //////////////////////////////

	// raycast down, we need the information!
	raycast_down.point=Vector3.zero;
	raycast_right=transform.position;	raycast_right.x+=mycollider.radius;	raycast_right.y-=1;
	raycast_left=transform.position;	raycast_left.x-=mycollider.radius;	raycast_left.y-=1;

	if(Physics2D.Raycast(raycast_right,Vector3.up*-1,10,collide_layer) || Physics2D.Raycast(raycast_left,Vector3.up*-1,10,collide_layer))
	{
		raycast_down_right=Physics2D.Raycast(raycast_right,Vector3.up*-1,10,collide_layer);
		raycast_down_left=Physics2D.Raycast(raycast_left,Vector3.up*-1,10,collide_layer);
		the_distance_right = Vector3.Distance(raycast_down_right.point,transform.position);
		the_distance_left = Vector3.Distance(raycast_down_left.point,transform.position);
		Debug.DrawLine (raycast_right, raycast_down_right.point,Color.green);
		Debug.DrawLine (raycast_left, raycast_down_left.point,Color.green);


		if (the_distance_right<to_bottom)
			{
				the_distance=the_distance_right;
				raycast_down=raycast_down_right;
				raycast_Platform = raycast_right;
			}
		else
			{
				the_distance=the_distance_left;
				raycast_down=raycast_down_left;
				raycast_Platform = raycast_left;
			}



			raycast_down_Platform=Physics2D.Raycast(raycast_Platform,Vector3.up*-1,10,whatIsPlatform);
			//	print (raycast_down_Platform);

			// if the is a platform beneath the player and he jump down then disable it for a second so he can pass through
			if (InputManager.Instance.inputDownJump && raycast_down_Platform ){
			
			//	if (raycast_down_Platform.collider.gameObject.GetInstanceID() == Start_Jump_Platform.gameObject.GetInstanceID()){
					//print (raycast_down_Platform.collider.gameObject.GetInstanceID()+"  "+Start_Jump_Platform.gameObject.GetInstanceID());
					StartCoroutine(platformDisable(raycast_down_Platform));

			
			}

			//reset jump down  input
			InputManager.Instance.inputDownJump=false;

		//Debug.Log("to bottom = "+to_bottom+"     -----    distance 2d = "+the_distance);
		
		if(((the_distance>to_bottom) || (raycast_down.point == Vector2.zero)) && PlayerMovement.Instance.current_mode!=playerStates.Jumping && PlayerMovement.Instance.current_mode!=playerStates.Falling  && PlayerMovement.Instance.current_mode!=playerStates.Gliding)
		{
			PlayerMovement.Instance.current_mode=playerStates.Falling;
		}
		if(Vector3.Distance(raycast_down.point,transform.position)<to_bottom && (PlayerMovement.Instance.current_mode==playerStates.Falling || PlayerMovement.Instance.current_mode==playerStates.Gliding))
		{/*
				//stop generating
				endJump=(Time.time-startJump)*1000;
				endDistance = (LevelGenerator.Instance.distance-startDistance)*10;
				startHeight = startHeight*-1;
				LevelGenerator.Instance.Pause();
				print ("distance "+endDistance+" m"+" height "+ startHeight+" time= "+endJump+" ms");
				*/
			PlayerMovement.Instance.current_mode=playerStates.Idle;
			jumped=0;
			min_falling_speed=0;
		}
	}
	else
	{
		if(transform.parent!=null)
			transform.parent=null;
		if(PlayerMovement.Instance.current_mode!=playerStates.Jumping && PlayerMovement.Instance.current_mode!=playerStates.Falling && PlayerMovement.Instance.current_mode!=playerStates.Gliding)
		{
			PlayerMovement.Instance.current_mode=playerStates.Falling;
		}
	}


}
	/*
void OnCollisionEnter2D ( Collision2D collisionInfo  ){
	if(!PlayerManager.Instance.getPlayerEnabled())return;
	if(PlayerMovement.Instance.current_mode==playerStates.Jumping || PlayerMovement.Instance.current_mode==playerStates.Falling || PlayerMovement.Instance.current_mode==playerStates.Gliding)
	{
	    foreach(ContactPoint2D contact in collisionInfo.contacts) {
	        Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
	    }
	}
}

void OnCollisionStay2D ( Collision2D collisionInfo  ){
	if(!PlayerManager.Instance.getPlayerEnabled())return;
	
    foreach(ContactPoint2D contact in collisionInfo.contacts) {
 		if(	transform.InverseTransformPoint(contact.point).y>0)
		{
		Debug.DrawRay(contact.point, contact.normal * 10, Color.red);
		}
    }
}
void OnCollisionExit2D ( Collision2D collisionInfo  ){
}
*/
}