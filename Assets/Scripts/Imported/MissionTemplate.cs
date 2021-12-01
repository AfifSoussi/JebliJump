using UnityEngine;
using System.Collections;

[System.Serializable]
public class MissionTemplate
{
	//Enumeration definitions
	public enum MissionType {Distance, DistanceWithNoCoins, DistanceWithNoPowerUps, SinkBetween, Coin, PowerUps, ExtraSpeed, Shield, SonicWave, Obstacles, Revive, Mine, Torpedo, Laser};
	public enum GoalType {InOneRun, InMultipleRun, InShop, Other};
	
	public string name 						= "";						//Mission name
	
	public MissionType missionType 			= MissionType.Distance;		//Mission type selection
	public GoalType goalType 				= GoalType.InOneRun;		//Mission goal selection
	
	public string description 				= "";						//Mission description
	
	public int valueA 						= 0;						//Goal value A
	public int valueB 						= 0;						//Goal value B
	
	int storedValue 						= 0;						//Stored value
	int startingValue 						= 0;						//Starting Value
	
	bool completed 							= false;					//Completition status
	
	//Sets the stored value
	public void SetStoredValue(int v)
	{
		storedValue = v;
		startingValue = v;
	}
	//Returns stored value
	public int StoredValue()
	{
		return storedValue;
	}
	//Modifies stored value
	public void ModifyStoredValue(bool addValue, int ammount)
	{
		if (addValue)
			storedValue += ammount;
		else
			storedValue = startingValue + ammount;
	}
	//Reset stored value
	public void ResetThis()
	{
		storedValue = 0;
	}
	//Returns completion status
	public bool Completed()
	{
		return completed;
	}
	//Set the mission to completed status
	public void Complete()
	{
		completed = true;
	}
}
