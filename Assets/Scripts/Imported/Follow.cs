using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour 
{
	public Transform target;		//The target to follow
	
	Vector3 targetPos;				//The target position to follow
	
	//Called at every frame
	void Update () 
	{
		//Get the position
		targetPos = this.transform.position;
		targetPos.y = target.position.y;
		targetPos.x = target.position.x - 12;
		//Go to the position
		this.transform.position = targetPos;
	}
}
