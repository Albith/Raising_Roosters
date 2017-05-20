using UnityEngine;
using System.Collections;

public class eggLauncher : MonoBehaviour {


	float typesOfEggs=  2;
	public float eggHatchAverageFrequency=5f;
	public float frequencyOffset=2f;

	int spawnLocations=10;
	Vector2 eggStartPosition;

	int previousLocation_inPlatform=-1;
	int previousPlatform=-1;
	int newPlatform=0;
	int newLocation_inPlatform=0;

	//egg Management stuff.

	public int maxNumberOfEggsOnScreen= 4;
	public int numberOfEggsOnScreen=0;

	public int [] eggsOnColumn= {0,0,0,0,0,0,0,0,0,0};	


	//Egg Launching variables.
		bool canLaunchEgg=false;
		bool doneTryingToLaunch=false;
		bool isDoingShortDelay=false;
	
	//for launching at the beginning.
	bool isStarting=true;

	// Use this for initialization
	void Start () {
	
		previousLocation_inPlatform=1;
		newLocation_inPlatform=0;

		isStarting=true;
		eggStartPosition=new Vector2( -4.43f, 4f);
		numberOfEggsOnScreen=0;

	
	}

	public void launchEgg_atCenterOfNest(int whatLocation)
	{
		
		//updating the eggs per level for the game manager.
		gameManager.gameInstance.incrementEggsInLevel();
		
		int whatIndex=0;
		
		//1.get the index from the currently launched location.
		int platformStartIndex= 
			gameManager.gameInstance.myPlatform_andEgg_Manager.
				platformsArray[whatLocation].GetComponent<platformScript>().firstUnitOccupied;
		int platformEndIndex=
			gameManager.gameInstance.myPlatform_andEgg_Manager.
				platformsArray[whatLocation].GetComponent<platformScript>().unitSize + platformStartIndex;
		

		int platformSize= platformEndIndex-platformStartIndex;
		if(platformSize==3)
			whatIndex=platformStartIndex+1;
		if(platformSize==4)
			whatIndex=Random.Range (platformStartIndex+1,platformStartIndex+3) ;
		else 
			whatIndex= Random.Range (platformStartIndex, platformEndIndex);
		
		//while(eggsOnColumn[whatIndex]>0)	
		//whatIndex= Random.Range (platformStartIndex, platformEndIndex);
		
		
		//2.providing a chance for a special egg to appear.
		int specialEggChance= Random.Range (0,13);
		string eggToLaunch= "Egg";
		if(specialEggChance==8)
			eggToLaunch="Egg_Heavy";
		
		
		GameObject newEgg = Instantiate(Resources.Load(eggToLaunch, typeof(GameObject)),
		                                
		                                new Vector3(eggStartPosition.x + whatIndex* gameManager.X_position_Offset ,
		            platformAndEgg_Manager.y_spawnPosition,
		            0) ,
		                                
		                                Quaternion.identity
		                                
		                                ) as GameObject;
		
		
		//3.assigning the index value to fetch later.
		newEgg.GetComponent<eggScript>().unitIndex= newLocation_inPlatform;
		newEgg.GetComponent<eggScript>().whichPlatform= whatLocation;
		gameManager.gameInstance.myPlatform_andEgg_Manager.platformsArray[whatLocation].GetComponent<platformScript>().eggsOnPlatform++;
		
		
		//3b.setting columnState value.
		if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform] == platformAndEgg_Manager.unitState.EMPTY)
			gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform]= platformAndEgg_Manager.unitState.HAS_EGGS;
		
		else if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform] == platformAndEgg_Manager.unitState.HAS_PLATFORM)
			gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform]= platformAndEgg_Manager.unitState.HAS_PLATFORM_AND_EGGS;
		
		
		
		//Playing sound.
		GetComponent<AudioSource>().Play();
		
		//updating previous location values.
		previousLocation_inPlatform=newLocation_inPlatform;
		previousPlatform=newPlatform;
		
		//incrementing counter.
		numberOfEggsOnScreen++;
		eggsOnColumn[newLocation_inPlatform]++;
		
		
		
	}

	public void launchEgg_atLocation(int whatLocation)
	{

		//updating the eggs per level for the game manager.
		gameManager.gameInstance.incrementEggsInLevel();

		int whatIndex=0;

		//1.get the index from the currently launched location.
		int platformStartIndex= 
			gameManager.gameInstance.myPlatform_andEgg_Manager.
				platformsArray[whatLocation].GetComponent<platformScript>().firstUnitOccupied;
		int platformEndIndex=
			gameManager.gameInstance.myPlatform_andEgg_Manager.
				platformsArray[whatLocation].GetComponent<platformScript>().unitSize + platformStartIndex;


		whatIndex= Random.Range (platformStartIndex, platformEndIndex);

		//while(eggsOnColumn[whatIndex]>0)	
			//whatIndex= Random.Range (platformStartIndex, platformEndIndex);


		//2.providing a chance for a special egg to appear.
		int specialEggChance= Random.Range (0,13);
		string eggToLaunch= "Egg";
		if(specialEggChance==8)
			eggToLaunch="Egg_Heavy";
		
		
		GameObject newEgg = Instantiate(Resources.Load(eggToLaunch, typeof(GameObject)),
		                                
		                                new Vector3(eggStartPosition.x + whatIndex* gameManager.X_position_Offset ,
		            					platformAndEgg_Manager.y_spawnPosition,
		           						 0) ,
		                                
		                                Quaternion.identity
		                                
		                                ) as GameObject;
		
		
		//3.assigning the index value to fetch later.
		newEgg.GetComponent<eggScript>().unitIndex= newLocation_inPlatform;
		newEgg.GetComponent<eggScript>().whichPlatform= whatLocation;
		gameManager.gameInstance.myPlatform_andEgg_Manager.platformsArray[whatLocation].GetComponent<platformScript>().eggsOnPlatform++;


			//3b.setting columnState value.
			if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform] == platformAndEgg_Manager.unitState.EMPTY)
				gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform]= platformAndEgg_Manager.unitState.HAS_EGGS;
			
			else if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform] == platformAndEgg_Manager.unitState.HAS_PLATFORM)
				gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[newLocation_inPlatform]= platformAndEgg_Manager.unitState.HAS_PLATFORM_AND_EGGS;


			
			//Playing sound.
			GetComponent<AudioSource>().Play();
			
			//updating previous location values.
			previousLocation_inPlatform=newLocation_inPlatform;
			previousPlatform=newPlatform;
			
			//incrementing counter.
			numberOfEggsOnScreen++;
			eggsOnColumn[newLocation_inPlatform]++;



	}

	


	public void EggRemovedFromScreen(int eggLocation)
	{


		numberOfEggsOnScreen--;
		eggsOnColumn[eggLocation]--;


		//resetting the column state for the departed egg, if there's no eggs left on column.
		if(eggsOnColumn[eggLocation] ==0)
		{
		if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[eggLocation] == platformAndEgg_Manager.unitState.HAS_PLATFORM_AND_EGGS)
			gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[eggLocation]= platformAndEgg_Manager.unitState.HAS_PLATFORM;

		else if(gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[eggLocation] == platformAndEgg_Manager.unitState.HAS_EGGS)
			gameManager.gameInstance.myPlatform_andEgg_Manager.columnStates[eggLocation]= platformAndEgg_Manager.unitState.EMPTY;

		}
	}

}
