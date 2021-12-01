using UnityEngine;
using System.Collections;

public class TriggerCamera : MonoBehaviour {

public bool  move_with_player = true;
public bool  lock_x_axis = false;
public float locked_x = 0;
public bool  lock_y_axis = false;
public float locked_y = 0;
public Vector2 extra_position;
public float z_distance = 0; // if zero then the current Z distance will be used
public float smoothness = 0.4f;
public float max_speed = 2;

void Start (){
	gameObject.layer = 2;
	if(GetComponent<Renderer>())GetComponent<Renderer>().enabled=false;
}
void OnTriggerEnter2D ( Collider2D other  ){
	if(other.transform.tag!="Player")return;
	PlayerCamera playercamera = other.GetComponent<PlayerCamera>() as PlayerCamera;
	if(playercamera)
	{
			print ("trigger camera");
		playercamera.move_with_player = move_with_player;
		playercamera.lock_x_axis = lock_x_axis;
		playercamera.locked_x = locked_x;
		playercamera.lock_y_axis = lock_y_axis;
		playercamera.locked_y = locked_y;
		playercamera.extra_position = extra_position;
		if(z_distance!=0)playercamera.z_distance = z_distance;
		playercamera.smoothness = smoothness;
		playercamera.max_speed = max_speed;
	}
	else
	{
		Debug.LogError("No PlayerCamera component found");
	}
}
}