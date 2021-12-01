using UnityEngine;
using System.Collections;

public class HelperJumpPad : MonoBehaviour {

public float power = 20;
public bool  only_from_top = true; // if true, only if the player jumps on the pad it will work
public Vector2 tmp_velocity;
private bool  not_on_top = false;
void OnCollisionEnter2D ( Collision2D collisionInfo  ){
	if(collisionInfo.transform.tag=="Player" || collisionInfo.transform.tag=="Enemy")
	{
		if(only_from_top)
		{
			not_on_top = true;
			    foreach(ContactPoint2D contact in collisionInfo.contacts) 
				{
					if(contact.normal.y!=1 && contact.normal.y<-0.9f)
					{
						not_on_top=false;
					}
				}
				if(not_on_top)return;
		}
		if(collisionInfo.transform.tag=="Enemy")
		{	
			tmp_velocity=collisionInfo.rigidbody.velocity;
			tmp_velocity.y=power;
			collisionInfo.rigidbody.velocity=tmp_velocity;
			return;
		}
		PlayerJumping jump_component = collisionInfo.transform.GetComponent<PlayerJumping>() as PlayerJumping;
		PlayerMovement.Instance.current_mode=playerStates.Jumping;
			if(jump_component.jumped==0 && PlayerMovement.Instance.current_mode!=playerStates.Falling)
			jump_component.jumped++;
		else
			jump_component.jumped=2;
		
		tmp_velocity=collisionInfo.rigidbody.velocity;
		tmp_velocity.y=power;
		collisionInfo.rigidbody.velocity=tmp_velocity;

		//jump_component.playerMovement.play_animation(jump_component.animation_jump);
		PlayerMovement.Instance.current_mode=playerStates.Jumping;
		
	}
}
}