using UnityEngine;
using System.Collections;

public class platformAndEgg_Manager : MonoBehaviour {


	//This class:
		//spawns nests(platforms), hides them when no longer needed and resets them.

	//----Nests(platform) variables:
			public GameObject[] platformsArray;
			int numberOfPlatforms=3;

			//Platform X axis sizes, or scales.
				// Array includes: small_XScale, med_XScale= 2f, large_XScale;
				float[] platformScale = {1.35f,2f,2.7f};
			
			//Starting Positions for different platforms.
				// small, medium, large;
				float[] startingXPositions = {-3.93f, -3.45f, -2.99f};
				int[] platformSizes_inUnits= {3,4,2};
				int[] movableUnits_byPlatform= {7,6,5};
				float spawnYPosition=3.03f;		

			//Variables and flags to describe what's happening in the spaces.
				
				int numberOfUnits=10;

				public enum unitState
					{
						EMPTY,
						HAS_PLATFORM,
						HAS_EGGS,
						HAS_PLATFORM_AND_EGGS
					};

				//A column in this case refers to the 10 horizontal divisions
				//that each rooster occupies in the game.
					//Each of these 10 columns can have a platform or egg descending to the floor.
					//This is what this array keeps track of.
					public unitState[] columnStates;

//-------New variables for simple mode.----------

	public static float y_spawnPosition=1.93f;

	//Pointing to Egg Launcher.
	public eggLauncher myEggLauncher;

		//some values
		int LEFT=0;
		int MIDDLE=1;
		int RIGHT=2;

	public int currentLevel=-1;
	public int totalLevels=9;

//--------Method----------

void Start () {
	

				//initialize unitStates.
				columnStates= new unitState[numberOfUnits];

				for(int i=0; i< numberOfUnits; i++)
				{

					columnStates[i]=unitState.EMPTY;

				}
			
	}
	


//--------------Level management methods... 
	//these are launched by the game manager.
	public void triggerNextLevel()
	{

			currentLevel++;
			StartCoroutine(launchPlatformsSequence_forLevel(currentLevel));

	}

	public void restartLevel()
	{
	
			StartCoroutine(launchPlatformsSequence_forLevel(currentLevel));
		
	}


	public void resetAllPlatforms()
	{

		//Check what platforms are visible (the rest are already set),
			//and fade those.

		for(int i=0; i<numberOfPlatforms; i++)
		{

			if(!platformsArray[i].GetComponent<BoxCollider>().isTrigger)
				platformsArray[i].GetComponent<platformScript>().toFadeOut();

		}

	}


	//The all-important level generation sequence.
		//To change the level difficulty or layout,
		//modify this section.
	IEnumerator launchPlatformsSequence_forLevel(int whichLevel)
	{

		//I need this variable here.
		int chance=0, chance1, chance2;
		int whichPlatform=0, whichPlatform1, whichPlatform2;

			switch(whichLevel)
			{
				case(0):
					//launch Platform in the middle.
					print ("-----Starting First Level.------");
					launchPlatform_single(4,MIDDLE);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(MIDDLE);
					break;
				
				case(1):
					//launch One egg in another platform.
					print ("-----Starting Second Level.------");
					yield return new WaitForSeconds(2f);
					
					 chance=Random.Range(0,2);
					 whichPlatform=MIDDLE;
					
					if(chance==0)
						whichPlatform=LEFT;
					else 
						whichPlatform=RIGHT;
		
					launchPlatform_single(4,whichPlatform);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(whichPlatform);
					break;

				case(2):
					//launch two eggs in one platform.
					print ("-----Starting Third Level.------");
					yield return new WaitForSeconds(2f);
					
					 chance=Random.Range(0,3);
					 whichPlatform=chance;

					launchPlatform_single(4,whichPlatform);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(whichPlatform);
					yield return new WaitForSeconds(2f);
					myEggLauncher.launchEgg_atLocation(whichPlatform);
					break;

				case(3):
					//launch two eggs in two platforms.
					print ("-----Starting Fourth Level.------");
					yield return new WaitForSeconds(2f);
					
		//Was initially randomizing the platform spawning here,
			//but there were bugs, so I decided to hard-code the spawn locations.
					// chance=Random.Range(0,3);
					// chance1=Random.Range (0,3);

					// while(chance1 == chance)
					// 	chance1=Random.Range (0,3);

					
					// whichPlatform=chance;
					// whichPlatform1=chance1;

					launchPlatform_multiple(LEFT);
					launchPlatform_multiple(RIGHT);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(LEFT);
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atLocation(RIGHT);				
					break;


				case(4):
					//launch three eggs in two platforms.
					print ("-----Starting Fifth Level.------");
					yield return new WaitForSeconds(2f);


					//Randomization works better this way.
						//Have a preset group of spawn combinations,
						//and use random to choose between them.

					chance=Random.Range (0,1);
					if (chance==1)
						{
							whichPlatform=RIGHT;
							whichPlatform1=LEFT;
							whichPlatform2=whichPlatform;

						}
					else
						{
							whichPlatform=LEFT;
							whichPlatform1=RIGHT;
							whichPlatform2=whichPlatform;
						}

						
					//Platforms.
					launchPlatform_multiple(whichPlatform);
					yield return new WaitForSeconds(1f);
					launchPlatform_multiple(whichPlatform1);
					
					//Eggs.
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atLocation(whichPlatform);
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atCenterOfNest(whichPlatform1);
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atLocation(whichPlatform2);		
					break;


				case(5):
					//launch three eggs in three platforms.
					print ("-----Starting Fifth Level.------");
					yield return new WaitForSeconds(2f);
					
					//Calculating chance.
					chance=Random.Range(0,3);
					chance1=Random.Range (0,3);
					chance2=Random.Range(0,3);


					while(chance1 == chance)
						chance1=Random.Range (0,3);
					
					while(chance2 == chance1 || chance2==chance)
						chance2=Random.Range (0,3);
					
					whichPlatform=chance;
					whichPlatform1=chance1;
					whichPlatform2=chance2;
					
					
					//Platforms.
					launchPlatform_static(2,MIDDLE,4);
					yield return new WaitForSeconds(1f);
					launchPlatform_static(3,LEFT,0);
					launchPlatform_static(3,RIGHT,7);

					//Eggs.
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atCenterOfNest(whichPlatform);
					yield return new WaitForSeconds(Random.Range (1f,3f) );
					myEggLauncher.launchEgg_atLocation(whichPlatform1);
					yield return new WaitForSeconds(Random.Range (0.5f,2.5f) );
					myEggLauncher.launchEgg_atLocation(whichPlatform2);		
					break;	


				case(6):
					//reshuffled, launch one Platform and one egg in the middle.
					print ("-----Starting First Level.------");
					launchPlatform_single(4,MIDDLE);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(MIDDLE);
					break;	
			
				case(7):
					//reshuffled, launch two eggs in two platforms.
					print ("-----Starting Last Level, with shuffled roosters.------");
					yield return new WaitForSeconds(2f);
					
					
					launchPlatform_multiple(LEFT);
					launchPlatform_multiple(RIGHT);
					yield return new WaitForSeconds(3f);
					myEggLauncher.launchEgg_atCenterOfNest(LEFT);
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atLocation(RIGHT);				
					break;
				

				case(8):
					//reshuffled, launch three eggs in three platforms.
					print ("-----Starting Fifth Level.------");
					yield return new WaitForSeconds(2f);
					
					//Calculating chance.
					chance=Random.Range(0,3);
					chance1=Random.Range (0,3);
					chance2=Random.Range(0,3);
					
					
					while(chance1 == chance)
						chance1=Random.Range (0,3);
					
					while(chance2 == chance1 || chance2==chance)
						chance2=Random.Range (0,3);
					
					whichPlatform=chance;
					whichPlatform1=chance1;
					whichPlatform2=chance2;
					
					
					//Platforms.
					launchPlatform_static(3,MIDDLE,3);
					yield return new WaitForSeconds(1f);
					launchPlatform_static(2,LEFT,0);
					launchPlatform_static(4,RIGHT,6);
					
					//Eggs.
					yield return new WaitForSeconds(1f);
					myEggLauncher.launchEgg_atLocation(whichPlatform);
					yield return new WaitForSeconds(Random.Range (1f,3f) );
					myEggLauncher.launchEgg_atCenterOfNest(whichPlatform1);
					yield return new WaitForSeconds(Random.Range (0.5f,2.5f) );
					myEggLauncher.launchEgg_atLocation(whichPlatform2);						
					break;	


				case(9):
					print ("You win the game!!");
					gameManager.isGameRunning=false;
					break;


				default:
					print ("launchPlatformSequence_forLevel(): Invalid level entered, default case.");
					break;
			}
		
	}

	//The 10 columns that divide the play are are clustered into categories.
		//The platforms take this into account, and they launch from basically
		//three regions: the left side of the screen, the middle and the right.
			//This method returns a random launch point within these 3 regions.
	int getLocationIndex(int where)
	{

		//1. determine what Platform to Launch.
		if(where==MIDDLE)
		{
			
			return Random.Range (3, 6);
			
		}
		
		else if(where==LEFT)
		{
			
			return Random.Range (0, 3);
			
		}
		
		else //if where==RIGHT
		{
			
			return Random.Range (6, 8);
			
		}


	}

//--------More platform setup methods.-------->

	//This one launches a nest in which all the parameters are supplied by the designer.
	void launchPlatform_static(int whatSize, int whichPlatform, int whichIndex)
	{
		
		//1.assign a Position and a Scale.
		platformsArray[whichPlatform].transform.position= 
			new Vector3(
				(startingXPositions[whatSize-2]+ 
			 whichIndex*gameManager.X_position_Offset),
				y_spawnPosition
				);
		
		platformsArray[whichPlatform].transform.localScale= new Vector3( 
		                                                                platformScale[whatSize-2],
		                                                                1.37f 
		                                                                );	
		
		//2. assigning other values.
		platformsArray[whichPlatform].GetComponent<platformScript>().unitSize=whatSize;
		platformsArray[whichPlatform].GetComponent<platformScript>().firstUnitOccupied=whichIndex;
		
		
		//update unitState values.
		for(int i=whichIndex; i<whichIndex+whatSize; i++)
		{
			if(columnStates[i]== unitState.EMPTY)
				columnStates[i] = unitState.HAS_PLATFORM;
			
			else if(columnStates[i] == unitState.HAS_EGGS)
				columnStates[i] = unitState.HAS_PLATFORM_AND_EGGS;
			
		}
		
		
		//3.resetting the other internal values for platform.
		platformsArray[whichPlatform].GetComponent<platformScript>().launchPlatform();
		
		
	}


	//This method launches a single platform from a random location inside
		//one of 3 regions described by the user.
	void launchPlatform_single(int whatSize, int whichPlatform)
	{

		int locationIndex=0;

				//1. determine what Platform to Launch.
					//already determined from the 'where' value.
					
					//determine what Index to Launch from.
					locationIndex= getLocationIndex(whichPlatform);


					//checking Right bounds if in last platform;
					if(whichPlatform==2 && locationIndex+whatSize> numberOfUnits )
						{
							whatSize=numberOfUnits-locationIndex;  //in the last row, platform is too big for right bound.

						}			

					//2.assign a Position and a Scale.
						platformsArray[whichPlatform].transform.position= 
													new Vector3(
														(startingXPositions[whatSize-2]+ 
													 	locationIndex*gameManager.X_position_Offset),
														y_spawnPosition
														);
						
						platformsArray[whichPlatform].transform.localScale= new Vector3( 
						                                                        platformScale[whatSize-2],
						                                                        1.37f 
						                                                        );

					//3. assigning other values.
						platformsArray[whichPlatform].GetComponent<platformScript>().unitSize=whatSize;
						platformsArray[whichPlatform].GetComponent<platformScript>().firstUnitOccupied=locationIndex;


						//update unitState values.
						for(int i=locationIndex; i<locationIndex+whatSize; i++)
						{
							if(columnStates[i]== unitState.EMPTY)
								columnStates[i] = unitState.HAS_PLATFORM;
							
							else if(columnStates[i] == unitState.HAS_EGGS)
								columnStates[i] = unitState.HAS_PLATFORM_AND_EGGS;
							
						}


					//4.resetting the other internal values for platform.
						platformsArray[whichPlatform].GetComponent<platformScript>().launchPlatform();

	}

//----------Launching Platforms END.------>

	//Updates the column state variables after a nest(platform) disappears.
	public void updateColumnStates_and_resize(int index)
	{	
		print ("---removing platform states for platform #"+index+".-----");
		
		int platformSize= platformsArray[index].GetComponent<platformScript>().unitSize;
		int startIndex= platformsArray[index].GetComponent<platformScript>().firstUnitOccupied;
		
		//0.clear the columnStates that area has.		
		for(int i=startIndex; i<startIndex+platformSize; i++)
		{
			if(columnStates[i] == unitState.HAS_PLATFORM)
				columnStates[i]= unitState.EMPTY;
			
			else if(columnStates[i] == unitState.HAS_PLATFORM_AND_EGGS )
				columnStates[i]= unitState.HAS_EGGS;
			
		}		
		
	}


//-------------Older method I'm referencing.-----------

	//I'm not using this function currently, it is only here for reference.
	void launchPlatform_multiple (int whichPlatform)
	{
			print ("Launching Platform multiple.");
			
			int index=0;

			//1. Generate a nest size.
	
				bool mustChangeSize=true;
				int newSize=2;		//default value.
				int indexForNewSize;

				//temp variables used further down.
				int endIndex=0;

			

				//2. Resizing the platform.
					//We check for out of bounds or conflicting cases here.
				
				int iterCount=0;				

				//while(mustChangeSize || iterCount<40)
				//{
					
					mustChangeSize=false;
					newSize= Random.Range (2,5);  //2 (small) through 4 (large) sizes.
					index= getLocationIndex(whichPlatform);
					print ("platform "+whichPlatform+" has index "+index);
						//check Size is not conflicting with other
						endIndex=index+newSize;

						//Checking that the new platform is not spawning in an area 
							//that already has platforms.
						if(whichPlatform==0 && (columnStates[endIndex]==unitState.HAS_PLATFORM || 
			                			columnStates[endIndex]==unitState.HAS_PLATFORM_AND_EGGS) )
						{
							index=0;  //in the last row, platform is too big for right bound.
							newSize=platformsArray[1].GetComponent<platformScript>().firstUnitOccupied-index;
							endIndex=index+newSize;
							
						}


						if(whichPlatform==2 && (endIndex > numberOfUnits) )
							{
								newSize=2;  //in the last row, platform is too big for right bound.
								endIndex=index+newSize;

							}

						//if middle platform, check new Size doesn't clash with the right platform.			
						else if(whichPlatform==1 &&
							        (columnStates[endIndex]==unitState.HAS_PLATFORM || 
							 		 columnStates[endIndex]==unitState.HAS_PLATFORM_AND_EGGS) )
							
								{	newSize=2;
									index= platformsArray[2].GetComponent<platformScript>().firstUnitOccupied-newSize;
									endIndex=index+newSize;
								}

						else if(whichPlatform==1 &&
									 (columnStates[index]==unitState.HAS_PLATFORM || 
									 columnStates[index]==unitState.HAS_PLATFORM_AND_EGGS) )
							
								{	
									index= platformsArray[1].GetComponent<platformScript>().firstUnitOccupied+
										   platformsArray[1].GetComponent<platformScript>().unitSize;
									endIndex=index+newSize;
								}
						
					iterCount++;

				//}		//end of while loop.


				if(iterCount>=40)
					print("-----------iterCount is"+iterCount+" warning in determineNewSize for platform #"+index+".------------");


				indexForNewSize=newSize-2;
			
				//assign a Position and a Scale and UnitSize property based on the Size.
				platformsArray[whichPlatform].transform.position= 
													new Vector3(
																	(startingXPositions[indexForNewSize]+ 
																	 index*gameManager.X_position_Offset),
																	 3.03f
																);
				
				platformsArray[whichPlatform].transform.localScale= new Vector3( 
			                                                        platformScale[indexForNewSize],
				                                                     1.37f 
				                                                        );


				platformsArray[whichPlatform].GetComponent<platformScript>().unitSize=newSize;
				platformsArray[whichPlatform].GetComponent<platformScript>().firstUnitOccupied=index;

			
			//3.resetting the other internal values for platform.
				platformsArray[whichPlatform].GetComponent<platformScript>().launchPlatform();


				//update unitState values.
				print ("---adding platform states for platform #"+index+".-----");

				for(int i=index; i<endIndex; i++)
				{
					if(columnStates[i]== unitState.EMPTY)
						columnStates[i] = unitState.HAS_PLATFORM;
						
					else if(columnStates[i] == unitState.HAS_EGGS)
						columnStates[i] = unitState.HAS_PLATFORM_AND_EGGS;

				}

	}


}	//end of class.
