using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
public Transform camera_pointer;

public bool  move_with_player = true;
public bool  lock_x_axis = false;
public float locked_x = 0;
public bool  lock_y_axis = false;
public float locked_y = 0;
public Vector2 extra_position;
public float z_distance = 0; // if zero then the start Z distance will be used
public float smoothness = 0.4f;
public float max_speed = 2;
private Vector3 velocity= Vector3.zero;
private float velocity1d = 0;
void Start (){
	if(!camera_pointer)
	{
		Debug.LogError("Need a camera pointer!!");
		return;
	}
	if(z_distance==0)
	{
		if(!camera_pointer.GetComponent<Camera>().orthographic)z_distance=camera_pointer.position.z;
		else z_distance = camera_pointer.GetComponent<Camera>().orthographicSize;
	}
	if(locked_x==0) locked_x = camera_pointer.position.x;
	if(locked_y==0) locked_y = camera_pointer.position.y;
}

private Vector3 target_position;
void FixedUpdate (){
	if(move_with_player && camera_pointer)
	{
			Vector3 target_position;
			target_position.x=transform.position.x+extra_position.x;
			target_position.y=transform.position.y+extra_position.y;

		if(!camera_pointer.GetComponent<Camera>().orthographic){
			target_position.z=z_distance;
			}
		else
		{
			target_position.z=camera_pointer.position.z;
			camera_pointer.GetComponent<Camera>().orthographicSize = Mathf.SmoothDamp(camera_pointer.GetComponent<Camera>().orthographicSize,z_distance,ref velocity1d,smoothness,max_speed);
		}
		if(lock_x_axis) target_position.x = locked_x;
		if(lock_y_axis) target_position.y = locked_y;
		if(smoothness>0)camera_pointer.position = Vector3.SmoothDamp(camera_pointer.position,target_position,ref velocity,smoothness,max_speed);
		else camera_pointer.position = target_position;
		return;
	}

}
}