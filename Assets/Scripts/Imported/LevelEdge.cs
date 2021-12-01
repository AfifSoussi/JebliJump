using UnityEngine;
using System.Collections;

public class LevelEdge : MonoBehaviour 
{
	//Called when something triggerer the object
	void OnTriggerEnter (Collider other) 
	{
		//If a spawn triggerer is collided with this
		if (other.name == "SpawnTriggerer")
		{
			//Spawn a proper object
			switch (other.tag)
			{/*
				case "SecondLayer":
                    LevelGenerator.Instance.GenerateSecondLayer(0);
					break;
				
				case "ThirdLayer":
					LevelGenerator.Instance.GenerateThirdLayer(0);
					break;
					
				case "FourthLayer":
                    LevelGenerator.Instance.GenerateFourthLayer(0);
					break;
*/
			case "Clouds":
				LevelGenerator.Instance.GenerateCloudsLayer(0);
				break;

			case "Obstacles":
				LevelGenerator.Instance.GenerateObstacles();
					break;
			}
		}
		//If a reset triggerer is collided with this
		else if (other.name == "ResetTriggerer")
		{
			//Reset the proper object
			switch (other.tag)
			{
				case "SecondLayer":
				case "ThirdLayer":
				case "FourthLayer":
				case "Clouds":
                    LevelGenerator.Instance.SleepGameObject(other.transform.parent.gameObject);
					break;

				case "Obstacles":
					Debug.Log("collided with obtacle : "+other.name);
					Obstacles child = other.transform.parent.GetComponent<Obstacles>();
					if (child)
					{
						Debug.Log(" obstacle numbver childs : " + child.elements.Count);

						child.DeactivateChild();
						LevelGenerator.Instance.SleepGameObject(other.transform.parent.gameObject);
					}
					
					break;
			}
		}
		//If a power up is collided with this
		else if (other.tag == "PowerUps")
		{
			print (" i got a power up ");
			//Reset the power up
			other.GetComponent<PowerUp>().ResetThis();
		}
		//If a torpedo is collided with this
		else if (other.name == "Torpedo")
		{
			print (" i got a torpedo");
			//Reset the torpedo
			other.transform.parent.gameObject.GetComponent<Torpedo>().ResetThis();
		}
	}
}
