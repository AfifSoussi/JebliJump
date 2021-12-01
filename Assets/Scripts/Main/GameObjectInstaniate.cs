using UnityEngine;
using System.Collections;

public class GameObjectInstaniate : MonoBehaviour {

	public bool Started =false;
	public string OldState;
	public Animator anim;

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		enabled = false;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		anim= GetComponent<Animator>();
	}

	private void GameStart(){
		GetComponent<Renderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		enabled = true;
	}
	
	private void GameOver () {
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		enabled = false;
		
	}


	// Update is called once per frame
	void Update () {
		if (LevelManager.Instance.CurrentState=="Starting" || LevelManager.Instance.CurrentState=="Restarting")
		{
			if (Started){
			Started=false;
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider2D>().enabled = true;
			OldState=LevelManager.Instance.CurrentState;
			}
			if (OldState != LevelManager.Instance.CurrentState)
				Started=true;
		}
	}



		
}
