using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour 
{
	public MissionTemplate[] missions;						//List of missions

    static MissionManager myInstance;
    static int instances = 0;

	int[] activeMissionIDs = new int[3];					//The ID of the active missions
	bool[] activeMissionComplete = new bool[3];				//Store which mission is completed from the active missions
	string data = "";										//The data string containing the saved status

    //Returns the instance
    public static MissionManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(MissionManager)) as MissionManager;

            return myInstance;
        }
    }

    //Called at the biginning of the game
    void Start()
    {
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Mission Manager at the level");
        else
            myInstance = this;
    }

	//Loads the saved status
	public void LoadStatus()
	{
		//Loads the data string
		
		//If a mission is removed, reset the data string
		if (data.Length > missions.Length)
			ResetDataString();
		//If a mission was added, update the data string
		else if (data.Length < missions.Length)
			UpdateDataString();
		
		//Get the active missions
		activeMissionIDs[0] = SaveManager.GetMission1();
		activeMissionIDs[1] = SaveManager.GetMission2();
		activeMissionIDs[2] = SaveManager.GetMission3();
		
		//If a mission slot is empty, look for a new mission, if possible
		for (int i = 0; i < 3; i++)
		{
			if (activeMissionIDs[i] == -1)
				GetNewMission(i);
		}
		
		//If a mission slot is empty, set it to complete status
		for (int i = 0; i < 3; i++)
		{
			if (activeMissionIDs[i] != -1)
				activeMissionComplete[i] = false;
			else
				activeMissionComplete[i] = true;
		}
		
		//Get the mission data for active missions
		if (!activeMissionComplete[0] && missions[activeMissionIDs[0]].goalType != MissionTemplate.GoalType.InOneRun)
			missions[activeMissionIDs[0]].SetStoredValue(SaveManager.GetMission1Data());
		
		if (!activeMissionComplete[1] && missions[activeMissionIDs[1]].goalType != MissionTemplate.GoalType.InOneRun)
			missions[activeMissionIDs[1]].SetStoredValue(SaveManager.GetMission2Data());
		
		if (!activeMissionComplete[2] && missions[activeMissionIDs[2]].goalType != MissionTemplate.GoalType.InOneRun)
			missions[activeMissionIDs[2]].SetStoredValue(SaveManager.GetMission3Data());
		
		//Update mission GUI texts
		UpdateGUITexts();
	}
	//Save progress
	public void Save()
	{
		//Loop trought the missions
		for (int i = 0; i < 3; i++)
		{
			//If the mission is completed
			if (activeMissionComplete[i] && activeMissionIDs[i] != -1)
			{
				print ("Mission " + (i + 1) + " Completed!");
				//Modify the data string
				char[] newData = data.ToCharArray();
				newData[activeMissionIDs[i]] = '1';
				data = new string (newData);
				
				//Reset mission data
				if (i == 0)
					SaveManager.SetMission1Data(0);
				else if (i == 1)
					SaveManager.SetMission2Data(0);
				else
					SaveManager.SetMission3Data(0);
				
				//Save the modifies
				SaveManager.SetMissionData(data);
				
				//Find a new mission
				GetNewMission(i);
			}
			//If the mission is not completed
			else if (!activeMissionComplete[i])
			{
				//Set a reference
				MissionTemplate mission = missions[activeMissionIDs[i]];
				
				//If the mission requires data save
				if (mission.goalType == MissionTemplate.GoalType.InMultipleRun || mission.goalType == MissionTemplate.GoalType.InShop)
				{
					//Save the data
					if (i == 0)
						SaveManager.SetMission1Data(mission.StoredValue());
					else if (i == 1)
						SaveManager.SetMission2Data(mission.StoredValue());
					else
						SaveManager.SetMission3Data(mission.StoredValue());
				}
			}
		}
		
		UpdateGUITexts();
	}
	//Called on distance based events
	public void DistanceEvent (int number)					
	{
		MissionTemplate mission;
		
		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplate.MissionType.Distance)
					CheckDistanceIn(mission, number, i);
				else if (mission.missionType == MissionTemplate.MissionType.DistanceWithNoCoins)
					CheckDistanceNoCoin(mission, number, i);
				else if (mission.missionType == MissionTemplate.MissionType.DistanceWithNoPowerUps)
					CheckDistanceNoPowerUp(mission, number, i);
			}
		}
	}
	//Called on sink based events
	public void SinkEvent(int number)
	{
		MissionTemplate mission;
		
		for (int i = 0; i < 3; i++)
		{	
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplate.MissionType.SinkBetween)
					CheckSindBetween(mission, number, i);
			}
		}
	}
	//Called on coin based events
	public void CoinEvent(int number)
	{
		MissionTemplate mission;

		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplate.MissionType.Coin)
					CheckCoinIn(mission, number, i);
			}
		}
	}
	//Called on power up based events
	public void PowerUpEvent(string name)
	{
		MissionTemplate mission;
		
		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplate.MissionType.PowerUps)
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.ExtraSpeed && name == "Extra Speed")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.Shield && name == "Shield")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.SonicWave && name == "Sonic Wave")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.Revive && name == "Revive")
					CheckPowerUpBased(mission, i);
			}
		}
	}
	//Called on obstacle based events
	public void ObstacleEvent(string name)
	{
		MissionTemplate mission;
		
		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplate.MissionType.Obstacles)
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.Mine && name == "Mine")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.Laser && name == "Laser")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplate.MissionType.Torpedo && name == "Torpedo")
					CheckObstacleBased(mission, i);
			}
		}
	}
	//Called on shop based events
	public void ShopEvent(string name)
	{
		MissionTemplate mission;
		
		for (int i = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.goalType == MissionTemplate.GoalType.InShop)
				{
					if (mission.missionType == MissionTemplate.MissionType.PowerUps)
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplate.MissionType.ExtraSpeed && name == "Extra Speed")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplate.MissionType.Shield && name == "Shield")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplate.MissionType.SonicWave && name == "Sonic Wave")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplate.MissionType.Revive && name == "Revive")
						CheckShopBased(mission, i);
				}
			}
		}
	}
	
	//Checks DistanceIn based mission status
	void CheckDistanceIn(MissionTemplate mission, int number, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun)
		{
            GUIManager.Instance.UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == MissionTemplate.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
            GUIManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks DistanceNoCoin based mission status
	void CheckDistanceNoCoin(MissionTemplate mission, int number, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun)
		{
            GUIManager.Instance.UpdateMissionStatus(i, number, mission.valueA);

            if (mission.valueA <= number && LevelManager.Instance.Coins() == 0)
				MissionCompleted(mission, i);
		}
	}
	//Checks DistanceNoPowerUp based mission status
	void CheckDistanceNoPowerUp(MissionTemplate mission, int number, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun)
		{
            GUIManager.Instance.UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number && !PlayerManager.Instance.PowerUpUsed())
				MissionCompleted(mission, i);
		}
	}
	//Checks SinkBetween based mission status
	void CheckSindBetween(MissionTemplate mission, int number, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun)
		{
            GUIManager.Instance.UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number && number <= mission.valueB)
				MissionCompleted(mission, i);
		}
	}
	//Checks Coin based mission status
	void CheckCoinIn(MissionTemplate mission, int number, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun)
		{
            GUIManager.Instance.UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == MissionTemplate.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
            GUIManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks Power Up based mission status
	void CheckPowerUpBased(MissionTemplate mission, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun || mission.goalType == MissionTemplate.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
            GUIManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks Obstalce based mission status
	void CheckObstacleBased(MissionTemplate mission, int i)
	{
		if (mission.goalType == MissionTemplate.GoalType.InOneRun || mission.goalType == MissionTemplate.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
            GUIManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Check shop based mission status
	void CheckShopBased(MissionTemplate mission, int i)
	{
		mission.ModifyStoredValue(true, 1);
        GUIManager.Instance.UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
		
		if (mission.valueA == mission.StoredValue())
		{
			MissionCompleted(mission, i);
			Save();
		}
	}
	
	//Completes the mission
	void MissionCompleted(MissionTemplate mission, int missionID)
	{
		//Flag the mission as completed
		activeMissionComplete[missionID] = true;
		//Show the GUI notification
        StartCoroutine(GUIManager.Instance.ShowMissionComplete(mission.description));
	}
	//Get new mission
	void GetNewMission(int i)
	{
		//Loop trought the mission data
		for (int j = 0; j < data.Length; j++)
		{
			//If a mission is uncompleted
			if (data[j] == '0')
			{
				//If we are at mission slot 1
				if (i == 0)
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManager.SetMission1(j);
						
						return;
					}
				}
				//If we are at mission slot 2
				else if (i == 1)
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManager.SetMission2(j);
						
						return;
					}
				}
				//If we are at mission slot 3
				else
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManager.SetMission3(j);
						
						return;
					}
				}
			}
		}
		
		//If we did not find a suitable mission, we set the slot inactive
		activeMissionIDs[i] = -1;
		
		if (i == 0)
			SaveManager.SetMission1(-1);
		else if (i == 1)
			SaveManager.SetMission2(-1);
		else
			SaveManager.SetMission3(-1);
	}
	//Gets new missions
	void GetNextMissions()
	{
		//Loop trought the mission slots
		for (int i = 0; i < 3; i++)
		{
			//If the mission is completed in the mission slot
			if (activeMissionComplete[i])
			{
				bool found = false;
				//Loop trought the mission data
				for (int j = 0; j < data.Length; j++)
				{
					//If we found an empty mission
					if (data[j] == '0')
					{
						//If we are at mission slot 1
						if (i == 0)
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManager.SetMission1(j);
							}
						}
						//If we are at mission slot 2
						else if (i == 1)
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManager.SetMission2(j);
							}
						}
						//If we are at mission slot 3
						else
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManager.SetMission3(j);
							}
						}
						
						found = true;
					}
				}
				
				//If there is no next mission
				if (!found)
				{
					//Flag the mission inactive
					if (i == 0)
						SaveManager.SetMission1(-1);
					else if (i == 1)
						SaveManager.SetMission2(-1);
					else
						SaveManager.SetMission3(-1);
				}
			}
		}
	}
	//Updates the GUI mission texts
	void UpdateGUITexts()
	{
		//Declare 3 string
		string text1;
		string text2;
		string text3;
		
		//If mission 1 is active, give it's description to text1
		if (!activeMissionComplete[0])
		{
			text1 = missions[activeMissionIDs[0]].description;
            GUIManager.Instance.UpdateMissionStatus(0, missions[activeMissionIDs[0]].StoredValue(), missions[activeMissionIDs[0]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text1 = "Completed";
            GUIManager.Instance.UpdateMissionStatus(0, 0, 0);
		}
		
		//If mission 2 is active, give it's description to text2
		if (!activeMissionComplete[1])
		{
			text2 = missions[activeMissionIDs[1]].description;
            GUIManager.Instance.UpdateMissionStatus(1, missions[activeMissionIDs[1]].StoredValue(), missions[activeMissionIDs[1]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text2 = "Completed";
            GUIManager.Instance.UpdateMissionStatus(0, 0, 0);
		}
		
		//If mission 3 is active, give it's description to text3
		if (!activeMissionComplete[2])
		{
			text3 = missions[activeMissionIDs[2]].description;
            GUIManager.Instance.UpdateMissionStatus(2, missions[activeMissionIDs[2]].StoredValue(), missions[activeMissionIDs[2]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text3 = "Completed";
            GUIManager.Instance.UpdateMissionStatus(0, 0, 0);
		}
		
		//Update the GUI texts
        GUIManager.Instance.UpdateMissionTexts(text1, text2, text3);
	}
	//Updates the data string
	void UpdateDataString()
	{
		//Update the data string
		for (int i = data.Length; i < missions.Length; i++)
			data += "0";
		
		//Save the data string
		SaveManager.SetMissionData(data);
	}
	//Resets the data string
	public void ResetDataString()
	{
		//Create a new data string
		string s = "";
		for (int i = 0; i < missions.Length; i++)
			s += "0";
		
		//Assign the new data string to the data, and save it
		data = s;
		SaveManager.SetMissionData(data);
		
		//Reassign the missions
		ResetMissions();
	}
	//Reset missions
	void ResetMissions()
	{
		//Reset missions
		SaveManager.SetMission1(0);
		SaveManager.SetMission2(1);
		SaveManager.SetMission3(2);
		
		//Reset mission data
		SaveManager.SetMission1Data(0);
		SaveManager.SetMission2Data(0);
		SaveManager.SetMission3Data(0);
	}
}
