using UnityEngine;
using System.Collections;

public class ScrollingObject : MonoBehaviour
{	
	private Vector3 start_position;
	public bool bMainScene=false;
	public float speed;
	public bool bObjects = true ;
	public float reload_position;
	public float distance_Traveled=0;

	

	public void Start(){
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		start_position= transform.position;
		enabled = false;
		distance_Traveled=0;
		if (bMainScene){
		}
		//find game manager object
		//GameObject go = GameObject.Find("GameManager");
		//other = (GameMaster) go.GetComponent(typeof(GameMaster));

	}

	private void GameStart(){
	//	slideObjects = true;
		enabled = true;
		distance_Traveled=0;
		if (bMainScene)
			transform.position =start_position;
	}
	
	private void GameOver () {
	//	slideObjects= false;
		enabled = false;
	}


void FixedUpdate ()
{	
	//if the stage is scrolling
	if (LevelManager.Instance.getScrolling() && !PlayerManager.Instance.getDead())
	{
		if (!bMainScene)
		{
				//the walked distance
				distance_Traveled+=Time.deltaTime * 2.5f ;

				//moving the main scene by GameManager control
				//if (bMainScene)
			//		transform.position= GameManager.position_Main;
				//a moving object
			if (bObjects)
				{
					if (start_position.x-transform.position.x<reload_position)
						transform.position -= speed * Vector3.right  * Time.deltaTime;
					else
					{
						transform.position = new Vector3(start_position.x,transform.position.y,transform.position.z);
					}
				}
				//a moving background texture
			else if (!bObjects)
			{
				GetComponent<Renderer>().material.mainTextureOffset += new Vector2(Time.deltaTime * speed/50 , 0f);
			}
		}
	}
}
}
