using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {

	//This script is attached to the camera, and updates it during the main mode of gameplay.
	//Basically, the camera is stationary -until- any of the player's chickens
		//fly up beyond a certain threshold. Starting from and beyond that Y value,
		//the camera will follow the flying chickens,
		//until they reach another threshold (when Mother Hen and the rafters are visible).

	float minimumY=-1.75f;
	float maximumY=2.4f;

	//Booleans activated by high-flying roosters.
	bool isFollowingRoosters=false;
	public static bool isAtMaxPosition=false;
	bool touchedHighestPosition=false;

	float roosters_BaseYPosition= -4.35f;
	float roosters_FollowYThreshold=  -1.75f;

	//Variables to record each rooster's position, and their logic.
	float[] rooster_YPosition_Array= {-5,-5,-5,-5,-5,-5,-5,-5,-5,-5};
	public bool[] isRoosterHighEnough={false,false,false,false,false,false,false,false,false,false};

	//Used to regulate the frequency of the rooster's Y position check.
	int frameCount=0;
	int cameraCheckFrequency=2;


	// Update is called once per frame
	void Update()
	{

		//Note: this cameraCheck is performed on every frame.
				roosterCameraCheck();

	}


	//The main function in this class, it  checks the rooster's Y position and 
		//changes camera state accordingly.
	void roosterCameraCheck () {
	
		int whichRoostersAreHighEnough=0;

		//Checking all the roosters' Y positions,
			//to determine if they should be followed.
		for(int i=0; i<gameManager.gameInstance.numberOfRoosters; i++)
		{

			//If any rooster has gone over the threshold,
				//set the camera status boolean to true.
				//prepare to update the camera.	
			if(gameManager.gameInstance.roostersArray[i].transform.position.y > roosters_FollowYThreshold)
			{ 
				rooster_YPosition_Array[i]= gameManager.gameInstance.roostersArray[i].transform.position.y;
				isRoosterHighEnough[i]=true;
				whichRoostersAreHighEnough++;
			}

			else
				isRoosterHighEnough[i]=false;
		
		}


		//Checking whether to Follow Roosters. 
			//Are there any roosters that are high enough on the Y axis?
			if(whichRoostersAreHighEnough>0)
				isFollowingRoosters=true;
			else 
				isFollowingRoosters=false;

	    //If true,
			//set the camera's position to follow the highest rooster.
		if(isFollowingRoosters)
		{

			gameManager.gameInstance.inPositionForLevelChange=false;


			float highestPosition=-1.75f;

			for(int i=0; i<gameManager.gameInstance.numberOfRoosters; i++)
			{

				if(isRoosterHighEnough[i])
					if(rooster_YPosition_Array[i]>highestPosition )
						highestPosition= gameManager.gameInstance.roostersArray[i].transform.position.y;
			
			}

			//The camera must not go over the maximum Y position value.
			if(highestPosition>= maximumY)
				{
					highestPosition=maximumY;
					isAtMaxPosition=true;
					touchedHighestPosition=true;
				}

			else 
				isAtMaxPosition=false;

			//Updating the camera position to follow the highest rooster.
			gameObject.transform.position= new Vector3
			(
				0,
				highestPosition,		//getting the highest rooster Position.
				-10
			);

		}

		//If no rooster is high enough, the camera stays at the floor level.
		//Note: once the camera has reached a high point,
			//we test to see if the level has been cleared.  
			//the reason being, the player -has- to reach the highest point
			//in order to transport a chick.
		else
		{
			isAtMaxPosition=false;

			gameManager.gameInstance.inPositionForLevelChange=true;

			if(touchedHighestPosition)
			{
				gameManager.gameInstance.checkForNextLevel();
				touchedHighestPosition=false;
			}

			gameObject.transform.position= new Vector3
				(
					0,
					-1.75f,		//getting the lowest rooster Position.
					-10
					);

		}


	}
}
