using UnityEngine;
//using UnityEditor;
using System.Collections;

public class LevelManager: MonoBehaviour {
	public string CurrentState = "Waiting";
	public GameObject GameplayLayer;
	public float ReloadPosition;
	public bool Scrolling;
	public float SpeedCurrent = 0;
	float DistanceTraveled = 0;
	Vector3 GameplayLayerInitial;
	bool ScrollingInitial;
	private float SpeedAdding = 1;
	int coins = 0;
	Object[] collectingObjects;
	object[] particalObjects;
	private float speedInitial;
	static int instances = 0;
	static LevelManager myInstance;

	public static LevelManager Instance {
		get {
			if (myInstance == null)
				myInstance = FindObjectOfType(typeof(LevelManager)) as LevelManager;
			
			return myInstance;
		}
	}

	void Awake() {
		//limit framerate to 30/60fps
		// 0 for no sync, 1 for panel refresh rate, 2 for 1/2 panel rate
		QualitySettings.vSyncCount = 0;
		//control the fixed time updates to have a faster game on slower machines
		//Time.fixedDeltaTime=0.03f;
		Application.targetFrameRate = 200;
		PlayerManager.Instance.DisableControls();
		GUIManager.Instance.DeactivateMainGUI();
	}

	void Start() {
		//Calibrates the myInstance static variable
		instances++;
		
		if (instances > 1)
			Debug.Log("Warning: There are more than one Level Manager at the level");
		else
			myInstance = this;
		
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		enabled=false;
		
		SaveManager.CreateAndLoadData();		        //Create or load the saved stats
		GUIManager.Instance.UpdateBestDistance();		//Update best distance at the hangar
		GUIManager.Instance.SetLevelResolution();		//Set the level for the current resolution
		MissionManager.Instance.LoadStatus();			//Load mission status
		
		//variables
		speedInitial=SpeedCurrent;
		ScrollingInitial=Scrolling;
		GameplayLayerInitial=GameplayLayer.transform.position;
	}



	
	private void GameOver() {
		CurrentState="Dying";
		enabled = false;
	}
	
	private void GameStart() {
		CurrentState="Starting";
		enabled = true;
	}

	public void StartLevel() {
	//	print ("start level");
		CurrentState="Starting";
		GameEventManager.TriggerGameStart();


		StartCoroutine(LevelGenerator.Instance.StartToGenerate(1.25f, 3));	//Start the level generator
		PlayerManager.Instance.ResetStatus(true);							//Reset player status, and move the submarine to the starting position
		PlayerManager.Instance.EnableControls();
		GUIManager.Instance.ShowStartPowerUps();								//Show the power up activation GUI
		GUIManager.Instance.ActivateMainGUI();								//Activate main GUI
		
		//variables
		DistanceTraveled=0;
		SpeedAdding =1;
		SpeedCurrent=speedInitial;
		if (ScrollingInitial)
			Scrolling=true;
		else
			Scrolling=false;
	}

	public void PauseGame() {




	//	print ("pause level");
		CurrentState="Pausing";
		Scrolling=false;

		PlayerManager.Instance.DisableControls();				//Disable sub controls
		LevelGenerator.Instance.Pause();							//Pause the level generator
	}

	public void QuitToMain() {
	//	print ("quit to main");
		CurrentState="Quiting";
		LevelGenerator.Instance.Restart(false);				//Disable level generator
		MissionManager.Instance.Save();						//Save progress

		GUIManager.Instance.DeactivateMainGUI();				//Deactivate the main GUI
		GUIManager.Instance.ActivateMainMenu();				//Activate main menu
		GUIManager.Instance.UpdateBestDistance();			//Update best distance at the hangar
		PlayerManager.Instance.DisableControls();
		PlayerManager.Instance.ResetStatus(false);			//Reset player status
		PlayerMovement.Instance.RunningDust.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled=false;


		//variables
		GameplayLayer.transform.position=GameplayLayerInitial;
		DistanceTraveled=0;
		SpeedAdding =1;
		SpeedCurrent=speedInitial;
		Scrolling=false;
	}

	public void Restart() {
	//	print ("Restart");
		if (CurrentState=="Restarting")
			CurrentState="Starting";
		else
			CurrentState="Restarting";
		coins = 0;										//Reset coin numbers

		LevelGenerator.Instance.Restart(true);					//Restart level generator
		PlayerManager.Instance.ResetStatus(true);				//Reset player status
		PlayerManager.Instance.EnableControls();
		MissionManager.Instance.Save();							//Save mission status

		GUIManager.Instance.ShowStartPowerUps();					//Show the power up activation GUI
		GUIManager.Instance.ActivateMainGUI();					//Activate main GUI
		GUIManager.Instance.UpdateBestDistance();				//Update best distance at the hangar

		//variables
		GameplayLayer.transform.position=GameplayLayerInitial;
		DistanceTraveled=0;
		SpeedAdding =1;
		SpeedCurrent=speedInitial;
		if (ScrollingInitial)
			Scrolling=true;
		else
			Scrolling=false;
	}

	public void ResumeGame() {
	//	print ("resume game");
		CurrentState="Resuming";
		if (ScrollingInitial)
			Scrolling=true;
		else
			Scrolling=false;
			LevelGenerator.Instance.Resume();						//Resume level generation
	}

	public void Revive() {
		CurrentState="Reviving";
		PlayerManager.Instance.DisableControls();
		StartCoroutine(PlayerManager.Instance.Revive());			//Revive the player
	}




	public void CoinGathered() {
		coins++;										//Increase coin number
		MissionManager.Instance.CoinEvent(coins);				//Notify the mission manager
	}
	
	public int Coins() {
		return coins;
	}

	public float getDistanceTraveled() {
		return DistanceTraveled;
	}

	public bool getScrolling() {
		return Scrolling;
	}

	public float getSpeed() {
		return SpeedCurrent;
	}

	/*


	private void ScrollingLevel() {
		if (Scrolling && !PlayerManager.Instance.getDead())
		{
			//the walked distance
			DistanceTraveled+=Time.deltaTime * SpeedCurrent	/ 2 ;

			//add +1 to speed each 10 unit traveled
			if (DistanceTraveled>SpeedAdding*10 && DistanceTraveled<SpeedAdding*11 && SpeedCurrent<16)
			{
				SpeedCurrent++;
				SpeedAdding++;
			}

			if (DistanceTraveled<ReloadPosition)  // end not reached yet
				GameplayLayer.transform.position -= SpeedCurrent * Vector3.right  * Time.deltaTime;
			else   // end reached ,start the stage again
			{
				print ("start again");
				//GameplayLayer.transform.position -= SpeedCurrent * Vector3.right  * Time.deltaTime;
				//GameEventManager.TriggerGameStart();
				LevelManager.Instance.Restart();
			}
		}
	}

*/

	void Update() {
		//ScrollingLevel();
	}
	
}
