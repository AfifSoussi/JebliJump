using UnityEngine;
using System.Collections;

public static class SaveManager : object 
{
	static int coinAmmount 		= 1500;					//The ammount of coins the player has
	static int bestDistance 	= 0;					//The best distance the player has reached
	
	static int extraSpeed 		= 0;					//The ammount of extra speed power ups the player has
	static int shield 			= 0;					//The ammount of shield power ups the player has
	static int Magnet	 		= 0;					//The ammount of Magnet wave power ups the player has
	static int revive 			= 0;					//The ammount of revive power ups the player has
	
	static int mission1 		= 0;					//Mission 1 ID
	static int mission2 		= 1;					//Mission 2 ID
	static int mission3 		= 2;					//Mission 3 ID
	
	static int mission1Data 	= 0;					//Mission 1 saved data
	static int mission2Data 	= 0;					//Mission 2 saved data
	static int mission3Data 	= 0;					//Mission 3 saved data
	
	static string missionData	= ""; 					//Saved mission data string
	
	//If there is no save data, create it, else, load the existing
	public static void CreateAndLoadData() 
	{
		//If found the coin ammount data, load the datas
		if (PlayerPrefs.HasKey("Coin ammount"))
			LoadData();
		//Else, create the datas
		else
			CreateData();
	}
	//Creates a blank save
	public static void CreateData()
	{
		PlayerPrefs.SetInt("Coin ammount", 1500);
		PlayerPrefs.SetInt("Best Distance", 0);
		
		PlayerPrefs.SetInt("Extra Speed", 0);
		PlayerPrefs.SetInt("Shield", 0);
		PlayerPrefs.SetInt("Magnet", 0);
		PlayerPrefs.SetInt("Revive", 0);
		
		PlayerPrefs.SetInt("Mission1", 0);
		PlayerPrefs.SetInt("Mission2", 1);
		PlayerPrefs.SetInt("Mission3", 2);
		
		PlayerPrefs.SetInt("Mission1Data", 0);
		PlayerPrefs.SetInt("Mission2Data", 0);
		PlayerPrefs.SetInt("Mission3Data", 0);
		
		PlayerPrefs.SetString("Missions", "");
		
		PlayerPrefs.Save();
	}
	//Loads the save
	static void LoadData()
	{
		coinAmmount = PlayerPrefs.GetInt("Coin ammount");
		bestDistance = PlayerPrefs.GetInt("Best Distance");
		
		extraSpeed = PlayerPrefs.GetInt("Extra Speed");
		shield = PlayerPrefs.GetInt("Shield");
		Magnet = PlayerPrefs.GetInt("Magnet");
		revive = PlayerPrefs.GetInt("Revive");
		
		mission1 = PlayerPrefs.GetInt("Mission1");
		mission2 = PlayerPrefs.GetInt("Mission2");
		mission3 = PlayerPrefs.GetInt("Mission3");
		
		mission1Data = PlayerPrefs.GetInt("Mission1Data");
		mission2Data = PlayerPrefs.GetInt("Mission2Data");
		mission3Data = PlayerPrefs.GetInt("Mission3Data");
		
		missionData = PlayerPrefs.GetString("Missions");
	}
	//Return the coin ammounts
	public static int GetCoins()
	{
		return coinAmmount;
	}
	//Returns the best distance
	public static int GetBestDistance()
	{
		return bestDistance;
	}
	//Returns the number of extra speed power ups
	public static int GetExtraSpeed()
	{
		return extraSpeed;
	}
	//Returns the number of shield power ups
	public static int GetShield()
	{
		return shield;
	}
	//Returns the number of Magnet blast power ups
	public static int GetMagnet()
	{
		return Magnet;
	}
	//Returns the number of revive power ups
	public static int GetRevive()
	{
		return revive;
	}
	//Returns mission 1 ID
	public static int GetMission1()
	{
		return mission1;
	}
	//Returns mission 2 ID
	public static int GetMission2()
	{
		return mission2;
	}
	//Returns mission 3 ID
	public static int GetMission3()
	{
		return mission3;
	}
	//Returns mission 1 data
	public static int GetMission1Data()
	{
		return mission1Data;
	}
	//Returns mission 2 data
	public static int GetMission2Data()
	{
		return mission2Data;
	}
	//Returns mission 2 data
	public static int GetMission3Data()
	{
		return mission3Data;
	}
	//Returns the mission data
	public static string GetMissionData()
	{
		return missionData;
	}
	//Modifies and saves the coin ammount
	public static void SetCoins(int ammount)
	{
		coinAmmount = ammount;
		PlayerPrefs.SetInt("Coin ammount", coinAmmount);
		PlayerPrefs.Save();
	}
	//Modifies and saves the best distance
	public static void SetBestDistance(int distance)
	{
		bestDistance = distance;
		PlayerPrefs.SetInt("Best Distance", distance);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of extra speed power ups
	public static void ModifyExtraSpeedBy(int modifyBy)
	{
        extraSpeed += modifyBy;
		PlayerPrefs.SetInt("Extra Speed", extraSpeed);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of shield power ups
    public static void ModifyShieldBy(int modifyBy)
	{
        shield += modifyBy;
		PlayerPrefs.SetInt("Shield", shield);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of Magnet wave power ups
	public static void ModifyMagnetBy(int modifyBy)
	{
		Magnet += modifyBy;
		PlayerPrefs.SetInt("Magnet", Magnet);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of revive power ups
	public static void ModifyReviveBy(int modifyBy)
	{
		revive += modifyBy;
		PlayerPrefs.SetInt("Revive", revive);
		PlayerPrefs.Save();
	}
	//Sets mission 1 ID
	public static void SetMission1(int id)
	{
		mission1 = id;
		PlayerPrefs.SetInt("Mission1", id);
		PlayerPrefs.Save();
	}
	//Sets mission 2 ID
	public static void SetMission2(int id)
	{
		mission2 = id;
		PlayerPrefs.SetInt("Mission2", id);
		PlayerPrefs.Save();
	}
	//Sets mission 3 ID
	public static void SetMission3(int id)
	{
		mission3 = id;
		PlayerPrefs.SetInt("Mission3", id);
		PlayerPrefs.Save();
	}
	//Sets mission 1 data
	public static void SetMission1Data(int id)
	{
		mission1Data = id;
		PlayerPrefs.SetInt("Mission1Data", id);
		PlayerPrefs.Save();
	}
	//Sets mission 2 data
	public static void SetMission2Data(int id)
	{
		mission2Data = id;
		PlayerPrefs.SetInt("Mission2Data", id);
		PlayerPrefs.Save();
	}
	//Sets mission 3 data
	public static void SetMission3Data(int id)
	{
		mission3Data = id;
		PlayerPrefs.SetInt("Mission3Data", id);
		PlayerPrefs.Save();
	}
	//Modifies the mission data
	public static void SetMissionData(string s)
	{
		missionData = s;
		PlayerPrefs.SetString("Missions", missionData);
		PlayerPrefs.Save();
	}
}