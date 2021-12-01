using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour 
{
	public TorpedoMain torpedoMain;								//A link to the torpedo manager
	public PowerUpMain powerUpMain;								//A link to the power up manager
	
	public GameObject submarine;								//A link to the submarine
	public GameObject hangar;									//A link to the hangar
	
	public float scrollSpeed			= 0.4f;					//Starting scroll speed
	public float maxScrollSpeed 		= 0.7f;					//Maximum scroll speed
	public float maxScrollSpeedDist 	= 1500;					//Maximum scroll speed at thid distance
	
	public float distance;										//Current distance
	
	public Renderer Sky;										//The Sky renderer
	public Renderer Ground;										//The Ground renderer
	public Renderer Rempart;									//The Ground renderer

	public List<GameObject> Clouds;							//The list containing the obstacles

	
	public List<GameObject> obstacles;							//The list containing the obstacles
	
	public List<GameObject> secondLayer; 						//The list containing the second layer elements 
	public List<GameObject> thirdLayer; 						//The list containing the third layer elements 
	public List<GameObject> fourthLayer; 						//The list containing the fourth layer elements 

    static LevelGenerator myInstance;
    static int instances = 0;

    List<GameObject> activeElements = new List<GameObject>(); 	//A list containing the active elements
	
	Vector2 scrolling 					= new Vector2(0, 0);	//A vector for the seaBackground scrolling
	
	float defaultScroll;										//The default scrolling speed
	float scrollClouds;											//The clouds scrolling speed
	float scrollBackg;											//The background scrolling speed
	float scrollMiddle;											//The middleground scrolling speed
	float scrollForg;											//The foreground scrolling speed
	float scrollAtCrash;										//The scrolling speed before a crash
	
	float hangarPos							= 39;				//The starting x position of the hangar
	
	bool canGeneratePowerUp					= true;				//Power up generation switch
	bool canGenerateTorpedo					= true;				//Torpedo generation switch
	
	bool canGenerate						= false;			//Generation switch
	bool paused								= true;				//Pause switch
	
	bool scrollHangar						= true;				//Scroll hangar switch
	bool canModifySpeed						= true;				//Modify speed switch

    //Returns the instance
    public static LevelGenerator Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(LevelGenerator)) as LevelGenerator;

            return myInstance;
        }
    }

	//This function is called at the start of the game
	void Start()
	{
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Level Generator at the level");
        else
            myInstance = this;

		//Sets the starting values
		defaultScroll = scrollSpeed;

		scrollClouds = scrollSpeed * 13f;
		scrollBackg = scrollSpeed * 12.5f;
		scrollMiddle = scrollBackg * 3;
		scrollForg = scrollBackg * 8;
		
		//Generate a seconds and third layer in the middle
	//	GenerateSecondLayer(1);
	//	GenerateThirdLayer(1);
	}
	//This function is called every frame
	void FixedUpdate()
	{
		//If the generation is enabled, and the game is not paused
		if (canGenerate && !paused)
		{
			//If power up generation enabled
			if (canGeneratePowerUp)
				//Generate a power up
				StartCoroutine(GeneratePowerUp());
			//If a torpedo generation is enalbed
			if (canGenerateTorpedo)
				//Generate a torpedo
				StartCoroutine(GenerateTorpedo());
			
			//Scroll the level
			ScrollLevel();
		}
		
		//If not paused
		if (!paused)
		{
			//Increase the distance, and notify the mission manager
			distance += scrollSpeed * Time.deltaTime * 25;
            MissionManager.Instance.DistanceEvent((int)distance);
		}
	}
	//Scroll the level
	void ScrollLevel()
	{
		//If can modify speed
		if (canModifySpeed)
			//Calculate layer scrolling speed
			scrollSpeed = defaultScroll + (((maxScrollSpeedDist - (maxScrollSpeedDist - distance)) / maxScrollSpeedDist) * (maxScrollSpeed - defaultScroll));
		
		//Calculate the other scrolling speeds
		scrollClouds = scrollSpeed * 5f;
		scrollBackg = scrollSpeed * 12.5f;
		scrollMiddle = scrollBackg * 3;
		scrollForg = scrollBackg * 8;
		
		//Scroll the elements in the list activeElements, with a speed maching their layer
		for (int i = 0; i < activeElements.Count; i++)
		{
			switch(activeElements[i].tag)
			{
			case "Clouds":
				activeElements[i].transform.position -= Vector3.right * scrollClouds * Time.deltaTime;
				break;

			case "SecondLayer":
				activeElements[i].transform.position -= Vector3.right * scrollBackg * Time.deltaTime;
				break;
				
			case "ThirdLayer":
				activeElements[i].transform.position -= Vector3.right * scrollMiddle * Time.deltaTime;
				break;


			case "FourthLayer":
			case "Obstacles":
			case "Particle":
				activeElements[i].transform.position -= Vector3.right * scrollForg * Time.deltaTime;
				break;
			}
		}
		
		//If the hangar is scrollable
		if (scrollHangar)
		{
			//If it's position is lesser than -200
			if (hangar.transform.position.x < -19)
				//Disable it's scrolling
				scrollHangar = false;
			//Scroll the hangar
			hangar.transform.position -= Vector3.right * scrollForg * Time.deltaTime;
		}
		
		//Apply scroll speed to scroll vector
		scrolling.x = scrollSpeed;
		
		//Scroll the ground and rempart
		Ground.material.mainTextureOffset += new Vector2(scrolling.x * Time.deltaTime * 3.6f , 0f);
		Rempart.material.mainTextureOffset += new Vector2(scrolling.x * Time.deltaTime , 0f);


	}
	//Clear the level, and empty the active elements list
	void ClearMap()
	{
		//Stop all coroutines
		StopAllCoroutines();
		
		//Reset the power up and torpedo managers
		torpedoMain.ResetAll();
		powerUpMain.ResetAll();
		
		//Go while there is an element in activeElements
		while (activeElements.Count > 0)
		{
			//Reset and remove the element based on their layer
            switch (activeElements[0].tag)
			{
				case "SecondLayer":
                    secondLayer.Add(activeElements[0]);
                    activeElements[0].transform.localPosition = new Vector3(25, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
                    EnableDisable(activeElements[0], false);
                    activeElements.Remove(activeElements[0]);
					break;
				
				case "ThirdLayer":
                    thirdLayer.Add(activeElements[0]);
                    activeElements[0].transform.localPosition = new Vector3(30, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
                    EnableDisable(activeElements[0], false);
                    activeElements.Remove(activeElements[0]);
					break;
				
				case "FourthLayer":
                    fourthLayer.Add(activeElements[0]);
                    activeElements[0].transform.localPosition = new Vector3(35, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
                    EnableDisable(activeElements[0], false);
                    activeElements.Remove(activeElements[0]);
					break;

			case "Clouds":
				Clouds.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(35, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
				EnableDisable(activeElements[0], false);
				activeElements.Remove(activeElements[0]);
				break;
				
			case "Obstacles":
				obstacles.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(45, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
                    activeElements[0].GetComponent<Obstacles>().DeactivateChild();
                    activeElements.Remove(activeElements[0]);
					break;
			}
		}		
	}
	//Randomize the order of the obstacles in the obstacles list
	void RandomizeObstacles()  
	{
		//Get the number of obstacles in the array
	    int n = obstacles.Count;  
		GameObject temp;
		//Randomize them
	    while (n > 1) 
		{  
	        n--;  
	        int k = Random.Range(0, n + 1);
	        temp = (GameObject)obstacles[k];  
	        obstacles[k] = obstacles[n];  
	        obstacles[n] = temp;  
	    }  
	}
    //Enables/disables the object with childs based on platform
    void EnableDisable(GameObject what, bool state)
    {
        #if UNITY_3_5
            what.SetActiveRecursively(state);
        #else
            what.SetActive(state);
        #endif
    }
	//Generate a power up
	IEnumerator GeneratePowerUp()
	{	
		//Set power up generation switch to false
		canGeneratePowerUp = false;
		
		//Get a random number between 15 and 30
		int n = Random.Range(10, 30);
		
		//Wait for n seconds
		double waited = 0;
		while (waited <= n)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		//Generate a power up
		powerUpMain.GeneratePowerUp(scrollSpeed/defaultScroll);
		//Set power up generation switch to true
		canGeneratePowerUp = true;
	}
	//Generate a torpedo
	IEnumerator GenerateTorpedo()
	{
		//Set torpedo generation switch to false
		canGenerateTorpedo = false;
		
		//Get a random number between 15 and 35
		int n = Random.Range(5000, 6000);
		
		//Wait for n seconds
		double waited = 0;	
		while (waited <= n)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//If the distance is greater that 250
		if (distance < 250)
			//Get a random number between 10 and 30
			n = Random.Range(10, 30);
		else
			//Else, get a random number between 10 and 40
			n = Random.Range(10, 40);
		
		//Divide n by 10 to get a number between 1 and 2 or 1 and 3
		n = n / 10;
		
		//Generate n torpedoes
		for (int i = 0; i < n; i++)
		{
			//Generate a torpedo
			torpedoMain.LaunchTorpedo();
			
			//Wait for 1 seconds
			waited = 0;
			while (waited <= 1)
			{
				//If the game is not paused, increase waited time
				if (!paused)
					waited += Time.deltaTime;
				//Wait for the end of the frame
				yield return 0;
			}
		}
		//Set torpedo generation switch to true
		canGenerateTorpedo = true;
	}
	//Stop scrolling speed after a crash
	IEnumerator StopScrolling (float time)
	{
		//Disable speed modification
		canModifySpeed = false;
		
		//Set startValue and scrollAtCrash
		float startValue = scrollSpeed;
		scrollAtCrash = scrollSpeed;
		
		//Slow down to 0 in time
		var rate = 1.0f / time;
	    var t = 0.0f;
	    
	    while (t < 1.0f) 
	    {
	        t += Time.deltaTime * rate;
	        scrollSpeed = Mathf.Lerp(startValue, 0, t);
	        yield return new WaitForEndOfFrame();
	    }
		
		//Disable generation
		canGenerate = false;
		paused = true;
		
		//Notify the mission manager
        MissionManager.Instance.SinkEvent((int)distance);
		
		//Wait for a second
		yield return new WaitForSeconds(1);
		
		//If the player has a revive
        if (PlayerManager.Instance.HasRevive())
			//Show revive button
            StartCoroutine(GUIManager.Instance.ShowRevive());
		else
			//Else, show finish menu
            GUIManager.Instance.ShowEnd();
	}
	//Generate a second layer element
	public void GenerateSecondLayer(int placeAtLoc)
	{
		//Get a random element from the second layer
		int n = Random.Range(0, secondLayer.Count);
		GameObject go = secondLayer[n];
		
		//Remove it from the second layer list, and add it to the active elements
		secondLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			Vector3 newPos = go.transform.localPosition;

			newPos.x = 0;
			go.transform.localPosition = newPos;
		}
		//Activate the object
        EnableDisable(go, true);
	}
	//Generate a third layer element
	public void GenerateThirdLayer(int placeAtLoc)
	{
		//Get a random element from the third layer
		int n = Random.Range(0, thirdLayer.Count);
		GameObject go = thirdLayer[n];
		
		//Remove it from the third layer list, and add it to the active elements
		thirdLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			Vector3 newPos = go.transform.localPosition;
			
			newPos.x = 0;
			go.transform.localPosition = newPos;
		}
		//Activate the object
        EnableDisable(go, true);
	}
	//Generate a fourth layer element
	public void GenerateFourthLayer(int placeAtLoc)
	{	
		//Get a random element from the fourth layer
		int n = Random.Range(0, fourthLayer.Count);
		GameObject go = fourthLayer[n];
		
		//Remove it from the fourth layer list, and add it to the active elements
		fourthLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			Vector3 newPos = go.transform.localPosition;
			newPos.x = 0;
			
			go.transform.localPosition = newPos;
		}
		//Activate the object
        EnableDisable(go, true);
	}

	//Generate a fourth layer element
	public void GenerateCloudsLayer(int placeAtLoc)
	{	
		//Get a random element from the fourth layer
		//int n = Random.Range(0, Clouds.Count);
		GameObject go = Clouds[0];
		
		//Remove it from the clouds layer list, and add it to the active elements
		Clouds.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			Vector3 newPos = go.transform.localPosition;
			newPos.x = 0;
			
			go.transform.localPosition = newPos;
		}
		//Activate the object
		EnableDisable(go, true);
	}
	//Generate an obstacle element
	public void GenerateObstacles()
	{	
		//Get a random element from the obstacles
		int n = Random.Range(0, obstacles.Count);
		GameObject go = (GameObject)obstacles[n];
		//Debug.Log (go.name);
		//Remove it from the obstacles layer list, and add it to the active elements
		obstacles.Remove(go);
		activeElements.Add(go);

		//Activate the object

		go.GetComponent<Obstacles>().ActivateChild();
	}
	//Disable, remove and reset an object
	public void SleepGameObject(GameObject go)
	{
		//Disable and reset an object based on it's layer
		switch (go.tag)
		{
			case "Obstacles":
				activeElements.Remove(go);
				obstacles.Add(go);
//			print (go.name);
				go.transform.localPosition = new Vector3(100, go.transform.localPosition.y, go.transform.localPosition.z);
			break;

			case "Clouds":
				activeElements.Remove(go);
				Clouds.Add(go);
				go.transform.localPosition = new Vector3(100, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
				
			case "SecondLayer":
				activeElements.Remove(go);
				secondLayer.Add(go);
				go.transform.localPosition = new Vector3(100, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			
			case "ThirdLayer":
				activeElements.Remove(go);
				thirdLayer.Add(go);
                go.transform.localPosition = new Vector3(115, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			
			case "FourthLayer":
				activeElements.Remove(go);
				fourthLayer.Add(go);
                go.transform.localPosition = new Vector3(125, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			

		}

        if (go.tag != "Obstacles")
            EnableDisable(go, false);
	}
	//Add an explosion to the active elements
	public void AddExplosion(GameObject exp)
	{
		activeElements.Add(exp);
	}
	//Remove an explosion to the active elements
	public void RemoveExplosion(GameObject exp)
	{
		activeElements.Remove(exp);
	}
	//Restart the level generator
	public void Restart(bool startToScroll)
	{
		//Clear the map, and reset the hangar
		ClearMap();
        hangar.transform.localPosition = new Vector3(hangarPos, 1, hangar.transform.localPosition.z);
		
		//Reset speed modify, torpedo and power up generation switch
		canGenerateTorpedo = true;
		canGeneratePowerUp = true;
		canModifySpeed = true;
		
		//Reset distance and scroll speed
		distance = 0;
		scrollSpeed = defaultScroll;
		
		//Generate a second and third layer element at the middle
		GenerateSecondLayer(1);
		GenerateThirdLayer(1);
		
		//Enalbe hangar scroll
		scrollHangar = true;
		
		//If start scrolling now
		if (startToScroll)
			//Start to generate objects
			StartCoroutine(StartToGenerate(1.25f, 3));
	}
	//Pause the level generator
	public void Pause()
	{
		//Pause the generator
		canGenerate = false;	
		paused = true;
		//Pause the torpedo and power up manager
		torpedoMain.PauseAll();
		powerUpMain.PauseAll();
	}
	//Resume the level generator
	public void Resume()
	{
		//Resume the generator
		canGenerate = true;	
		paused = false;
		//Resume the torpedo and power up manager
		torpedoMain.ResumeAll();
		powerUpMain.ResumeAll();
	}
	//Disable speed modification
	public void ExtraSpeedEffect()
	{
		canModifySpeed = false;
	}
	//Enable speed modification
	public void ExtraSpeedOver()
	{
		canModifySpeed = true;
	}
	//Contiune to scroll the level
	public void ContinueScrolling()
	{
		//Reset scroll speed to before crash
		scrollSpeed = scrollAtCrash;
		
		//Enable generation and speed modification
		paused = false;
		canGenerate = true;
		canModifySpeed = true;
	}
    //Returns the speed multiplier for the powerup/torpedo
    public float SpeedMultiplier()
    {
        return scrollSpeed / defaultScroll;
    }
	//Sets the starting position for the hangar
	public void SetHangarPos(float pos)
	{
		hangarPos = pos;
	}
	//Scroll an explosion particle
	public IEnumerator ScrollExplosion(ParticleSystem particle)
	{
		//Get the particle gameobject, and add it to the active elements
		GameObject particleGO = particle.gameObject;	
		activeElements.Add(particleGO);
		
		//Wait for 2 seconds
		double waited = 0;	
		while (waited <= 2)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Remove the particle from the active elements
		activeElements.Remove(particleGO);
	}
	//Start the level generation system
	public IEnumerator StartToGenerate(float waitTime, float obstacleWaitTime)
	{
		//Randomize the obstalce list
		RandomizeObstacles();
		
		//Enable generation
		canGenerate = true;
		paused = false;
		
		//Wait for waitTime
		double waited = 0;
		while (waited <= waitTime)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Generate a fourth layer element
	//	GenerateFourthLayer(0);
		//Generate a clouds layer element
		GenerateCloudsLayer(0);
	}
}
