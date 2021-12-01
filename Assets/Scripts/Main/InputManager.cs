using UnityEngine;
using System.Collections;

public enum Swipe { None, Up, Down, Left, Right };





public class InputManager : MonoBehaviour 
{
	public bool useTouch = false;				//Use touch based controls
	
	public LayerMask mask = -1;					//Set input layer mask
	
	Ray ray;									//The hit ray
	RaycastHit2D hit;								//The hit raycast
	
	Transform button;							//The triggered button

	//input checkers controller by  inputmanager
	public bool inputJump =false;
	public bool inputGlideStart =false;
	public bool inputGlideStop =false;
	public bool inputDownJump=false;

	//swype variables
	public float minSwipeLength = 200f;
	Vector2 firstPressPos;
	Vector2 secondPressPos;
	Vector2 currentSwipe;
	public static Swipe swipeDirection;
	private Touch t;



	static InputManager myInstance;
	static int instances = 0;
	GameObject playTriggerer;


	//Retursn the instance
	public static InputManager Instance
	{
		get
		{
			if (myInstance == null)
				myInstance = FindObjectOfType(typeof(InputManager)) as InputManager;
			return myInstance;
		}
	}


	
	void Start (){
		
		//Calibrates the myInstance static variable
		instances++;
		
		if (instances > 1)
			Debug.Log("Warning: There are more than one Input Manager at the level");
		else
			myInstance = this;

		playTriggerer = GameObject.Find("PlayTriggerer");
	}

	//Called at every frame
	void Update () 
	{
		if (useTouch)
			GetTouches();
		else
			GetClicks();
	}

	//If playing with mouse
	void GetClicks()
	{
		//If we pressed the mouse
		if(Input.GetMouseButtonDown(0))
		{
			//Cast a ray
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			//save began touch 2d point

			//If the ray hit something in the set layer
			if (hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, mask))
			{
				//Register it, and send it to the GUI manager
				button = hit.transform;
//				print (button);
				GUIManager.Instance.ButtonDown(button);
			}
			//If the ray didn't hit a GUI object
			else
			{
				//Set the button to null, and jump the player
				button = null;
				inputJump =true;
				inputGlideStart=false;
				inputGlideStop =false;
				inputDownJump=false;
				//get the cooridnates of the first click for swype control
				firstPressPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);

			}
		}
		//If the click was released
		else if (Input.GetMouseButtonUp(0))
		{
			//If there is a button registered previousely
			if (button != null)
				//Send it to the GUI manager
				GUIManager.Instance.ButtonUp(button);
			//If there is a button registered
			else
			{
				//Stop the gliding (if we are gliding)
				inputGlideStop =true;
				inputGlideStart=false;
				inputJump=false;
				inputDownJump=false;
				//verify if we did a swype , (if it's down then jump down)
				DetectSwipeMouse();
			}

		}
		//If the click was maintained
		else if (Input.GetMouseButton(0))
		{
			//If there is no button registered previousely
			if (button == null){
				//start the gliding
				inputGlideStart =true;
				inputGlideStop =false;
				inputDownJump=false;
			}
		}
	
	}


	//If playing with touch screen
	void GetTouches()
	{
		//Loop through the touches
		foreach (Touch touch in Input.touches) 
		{
			//If a touch has happened
			if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
			{
				//If the ray hit something in the set layer
				if (hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero, Mathf.Infinity, mask))
				{
					//Register it, and send it to the GUI manager
					button = hit.transform;
					//print (button);
					GUIManager.Instance.ButtonDown(button);
				}
				//If the ray didn't hit a GUI object
				else
				{
					//Set the button to null, and jump the player
					button = null;
					inputJump =true;
					inputGlideStart=false;
					inputGlideStop =false;
					inputDownJump=false;
					//get the cooridnates of the first click for swype control
					firstPressPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
				}
			}
			//If a touch has ended
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				//If there is a button registered previousely
				if (button != null)
					//Send it to the GUI manager
					GUIManager.Instance.ButtonUp(button);
				//If there is a button registered
				else{
					//Stop the gliding (if we are gliding)
					inputGlideStop =true;
					inputGlideStart=false;
					inputJump=false;
					inputDownJump=false;
					//verify if we did a swype , (if it's down then jump down)
					t= touch;
					DetectSwipeTouch();
				}
			}
			//If a touch is maintained
			else if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
			{
				//If there is no button registered previousely
				if (button == null)
					//start the gliding
					inputGlideStart =true;
				inputGlideStop =false;
				inputDownJump=false;
			}

		}
	}


	public void DetectSwipeMouse()
	{
		//save ended touch 2d point
		secondPressPos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		//create vector from the two points
		currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y); 
		// Make sure it was a legit swipe, not a tap
		if (currentSwipe.magnitude < minSwipeLength) {
			swipeDirection = Swipe.None;
			inputDownJump=false;
			return;
		}
		//normalize the 2d vector
		currentSwipe.Normalize();
		
		//swipe upwards
		if(currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
		{
			Debug.Log("up swipe");
			swipeDirection = Swipe.Up;
		}
		//swipe down
		 else if(currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
		{
			Debug.Log("down swipe");
			inputDownJump=true;
			swipeDirection = Swipe.Down;
		}
		//swipe left
		else if(currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
		{
			Debug.Log("left swipe");
			swipeDirection = Swipe.Left;
		}
		//swipe right
		else if(currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
		{
			Debug.Log("right swipe");
			swipeDirection = Swipe.Right;
		}
		else
		{
			Debug.Log("None swipe");
			swipeDirection = Swipe.None;
		}
	}


	public void DetectSwipeTouch()
	{
		//save ended touch 2d point
		secondPressPos = new Vector2(t.position.x, t.position.y);
		
		currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

		// Make sure it was a legit swipe, not a tap
		if (currentSwipe.magnitude < minSwipeLength) {	
			swipeDirection = Swipe.None;
			return;
		}
		currentSwipe.Normalize();
		// Swipe up
		if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {	
			swipeDirection = Swipe.Up;
			// Swipe down	
		} else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
			inputDownJump=true;
			swipeDirection = Swipe.Down;
			// Swipe left
		} else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
			swipeDirection = Swipe.Left;
			// Swipe right
		} else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
			swipeDirection = Swipe.Right;
		}
		else
		{
			Debug.Log("None swipe");
			swipeDirection = Swipe.None;
		}
	}

	//playing with buttons (not complete)
	/*
	//If playing with keyboard
	void GetButtons()
	{
		//If we pressed the jump button
		if(Input.GetButtonDown("Jump"))
		{
			print ("jump down");
			//If the ray hit something in the set layer
			if (LevelManager.Instance.CurrentState=="Waiting" || LevelManager.Instance.CurrentState=="Pausing"|| LevelManager.Instance.CurrentState=="Quiting")
			{
				//Register it, and send it to the GUI manager
				button = playTriggerer.transform;
				print (playTriggerer);
				GUIManager.Instance.ButtonDown(button);
				print ("jump down GUI");
			}
			//If the ray didn't hit a GUI object
			else
			{
				//Set the button to null, and jump the player
				button = null;
				inputGlideStart =false;
				inputGlideStop =false;
				inputJump =true;
				print ("jump down Player");
			}
		}
		//If the click was released
		else if (Input.GetButtonUp("Jump"))
		{
			//If there is a button registered previousely
			if (button != null)
				//Send it to the GUI manager
				GUIManager.Instance.ButtonUp(button);
			//If there is a button registered
			else
				//Stop the gliding (if we are gliding)
				inputGlideStart =false;
			inputJump =false;
			inputGlideStop =true;
		}
		//If the click was maintained
		else if (Input.GetButton("Jump"))
		{
			//If there is no button registered previousely
			if (button == null)
				//start the gliding
				inputJump =false;
			inputGlideStop =false;
			inputGlideStart =true;
		}
		//no click is done ,stop all jumping inputs
		else 
		{
			inputJump =false;
			inputGlideStart =false;
			inputGlideStop =false;
		}
	}
	*/



	
	
	




}

