using UnityEngine;
using System.Collections;

public class ResolutionManager : MonoBehaviour 
{
	static ResolutionManager myInstance;
    static int instances = 0;
	
	bool canModify 	= true;		//Can modify the resolution 
		
	float left		= 0;		//The left position for the left GUI elements
	float right		= 0;		//The right position for the right GUI elements
	float shop		= 0;		//The shop left/right position
	float mainHL	= 0;		//The main menu header left position
	float mainHR	= 0;		//The main menu header right position
	float scale		= 0;		//The level scale (sand, background, etc...)
	float hangar	= 0;		//The hangar location
	float subPos	= 0;		//The position of the submarine
	float subStartP	= 0;		//The starting position of the submarine
	
	//Retursn the instance
    public static ResolutionManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(ResolutionManager)) as ResolutionManager;

            return myInstance;
        }
    }
	
	// Use this for initialization
	void Start () 
	{
		//Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Player Manager at the level");
        else
            myInstance = this;
	}
	//Returns the aspect ratio
	string GetAspectRation(int a, int b)
	{
		int m = GetGreatestDivider(a, b);
		return (a/m) + ":" + (b/m);
	}
	//Returns the greatest divider of a and b
	int GetGreatestDivider(int a, int b)
	{
		int m;
 
		while (b != 0) 
		{
			m = a % b;
			a = b;
			b = m;
		}
		
		return a;
	}
	//Set target resolution
	public void SetResolutionSetting(GameObject[] scalable, GameObject[] shopElements, GameObject[] leftElements, GameObject[] rightElements)
//	public void SetResolutionSetting(GameObject[] scalable, GameObject[] shopElements, GameObject[] leftElements, GameObject[] rightElements, GameObject h)

	{
		//Calculate aspect ratio
		string ar = GetAspectRation(Screen.width, Screen.height);
		float SWidth = Screen.width;	float sHeight = Screen.height; 	float Ratio = SWidth/sHeight;
		if (Ratio >=1.2 && Ratio<=1.3)  //ratio 5/4
		{
			left = -2;
			right = 8;
			shop = -1f;
			mainHL = -2;
			mainHR = -1;
			scale = 4;
			//hangar = 39;
			//subPos = -30;
			//subStartP = -37;
		}
		else if (Ratio >1.3 && Ratio<=1.4)  //ratio 4/3
		{
			left = -3;
			right = 9;
			shop = 0;
			mainHL = 0;
			mainHR = 0;
			scale = 5;
			//hangar = 39;
			//subPos = -30;
			//subStartP = -37;
		}
		else if (Ratio >1.4 && Ratio<=1.59)  //ratio 3/2
		{
			left = -4;
			right = 10;
			shop = 1.5f;
			mainHL = -2;
			mainHR = 1;
			scale = 6;
		//hangar = 39;
		//subPos = -30;
		//subStartP = -37;
		}
		else if (Ratio >1.59 && Ratio<=1.9)  //ratio 16/9
		{
			left = -5;
			right = 11;
			shop = 2f;
			mainHL = -3;
			mainHR = 2;
			scale = 7;
			//hangar = 39;
			//subPos = -30;
			//subStartP = -37;
		}
		else if (Ratio > 1.9 && Ratio <= 2.2)  //ratio 16/9
		{
			left = -6;
			right = 12;
			shop = 3f;
			mainHL = -4;
			mainHR = 3;
			scale = 8;
			//hangar = 39;
			//subPos = -30;
			//subStartP = -37;
		}
		else 
		{
			canModify = false;
			print ("no supported resolution for GUI");
		}

	
		//If the aspect ratio is not supported, return to caller
		if (!canModify)
			return;
		
		//Declare temp values
		Vector3 temp;
		Vector2 offset;
			
		//Set left GUI elements
		foreach(GameObject element in leftElements)
		{
			temp = element.transform.position;
			
			if (element.name == "HeaderL")
				temp.x = mainHL;
			else
				temp.x = left;
			
			element.transform.position = temp;
		}
		
		//Set right GUI elements
		foreach(GameObject element in rightElements)
		{
			temp = element.transform.position;
			
			if (element.name == "HeaderR")
				temp.x = mainHR;
			else
				temp.x = right;
			element.transform.position = temp;
			
		}
		
		//Set position/scale of shop elements
		foreach(GameObject element in shopElements)
		{
			switch (element.name)
			{
				case "Bar":
				case "Header":
					temp = element.transform.localScale;
					temp.x = scale;
					element.transform.localScale = temp;
					break;
				
				case "Left":
					temp = element.transform.position;
					temp.x = -shop;
					element.transform.position = temp;
					break;
				
				case "Right":
					temp = element.transform.position;
					temp.x = shop;
					element.transform.position = temp;
					break;
			}
		}
		
		//Scale the background/GUI elements
		foreach(GameObject element in scalable)
		{
			temp = element.transform.localScale;
			temp.x = scale;
			element.transform.localScale = temp;
			
			offset = element.GetComponent<Renderer>().material.mainTextureScale;
			offset.x = scale/100;
			element.transform.GetComponent<Renderer>().material.mainTextureScale = offset;
		}
		
		//Set the hangar's current and starting position
		//temp = h.transform.position;
		//temp.x = hangar;
		//h.transform.position = temp;
		
		//LevelGenerator.Instance.SetHangarPos(hangar);
		
		//Set the submarine starting and main position
		//GameObject sub = PlayerManager.Instance.gameObject;
		//temp = sub.transform.position;
		//temp.x = subStartP;
		//sub.transform.position = temp;
		
		//PlayerManager.Instance.SetPositions(subStartP, subPos);
	}
	//Returns the right position
	public float RightPosition()
	{
		return right;
	}
}
