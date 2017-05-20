using UnityEngine;
using System.Collections;

public class gameManager : MonoBehaviour {

	//This class manages the game state,
		//and keeps track of game logic variables.

	//Static variable so every object in the game can call the gameManager.
	public static gameManager gameInstance;
	public static bool isSimpleMode=true;

	//Platform manager that manages both the egg and the nests(platforms).
	public platformAndEgg_Manager myPlatform_andEgg_Manager;
	public eggLauncher myEggLauncher;

	//Game Variables.
		public GameObject[] roostersArray; //used to access rooster sprites+other things.
		public int numberOfRoosters=10;
		public static bool isGameRunning=true;
		public bool inPositionForLevelChange=true;	
		float rooster_fadeInOut_Speed= 0.017f;
		float rooster_show_Delay= 1f;
		int[] roosterShuffleArray={0,1,2,3,4,5,6,7,8,9};
		bool hasShuffled=false;	
		//The following variable is used to randomize the rooster's position in the last section.
			int shuffleAmount=4;

	//Variables used to control the difficulty in each level:
		//keeps track of how many chicks to save for this level.
		//eggs lost in the game are used to determine when the level should restart.
			//Note: the game is lenient.  
			//If at least one egg in the level is safely transported,
				//the level is cleared (all other eggs can be broken)
		public int howManyChicksSaved=0;
		public int howManyEggsLost=0;
		int totalChicksSaved=0;
		int totalEggsLost=0;
		public int totalChicksInLevel=-1;
		
			//Variables for the user interface at the top left corner of the screen.
			public TextMesh chicksSavedMultiplier;
			public TextMesh eggsLostMultiplier;
			public TextMesh chicksSavedText;
			public TextMesh eggsLostText;
			public GameObject chickUISprite;
			public GameObject eggsLostSprite;

			//Variables used to determine the fade in/out of the interface.
			float UIshowDelay=2f;
			float fadeInOut_Speed=0.017f;
			public float t=0;

			//Variables used to control the transitions between levels.	
			float UImessageDelay=4f;
			public GameObject grayBackground;
			public TextMesh UI_message;

			//Game state constants.
			int NEXT_LEVEL=0;
			int RESTART=1;
			int LAST=2;
			int WIN=3;
			int PLAY_AGAIN=4;
			int INTRO=5;
			int BEGIN=6;
			int SHUFFLED=7;

			//Status messages displayed on screen.
			string[] UI_message_strings= {"To Next Level!", "Try Again!", "To Last Level!", 
								 "You Finished!\nCongratulations!", "What would you\nlike to do next?", 
								 "Ready to Start?\n\nProtect the eggs\nand return them\nto Mother Hen!",
									"Begin!", "Watch out!\nThe roosters \nhave changed positions!"};

			//Mother Hen object reference.
			public GameObject MotherHen;

	//Positional variables.
		//The gameManager sets the starting position for all the roosters.
		public static float X_position_Offset= 0.986f;
		Vector2 roosterStartPosition;

	//Important: here the gameManager determines chicken movement.
		public float ChickenFlightForce= 120f;// 
		public int maxButtonPressTime= 60;		
		
	//--------Sounds and sound playing method.-----------

	//Method for playing sounds on objects that will be destroyed:  
		//This will not happen much in this particular game,
		//as the roosters are not dynamically generated, but the eggs are.
		
		//Here's an example of how you would play the sound in another script.
			//gameManager.gameInstance.playSound(gameManager.soundIndexes.JUMP);

		//To add sounds to the soundArray.
			//add a name for the sound in the enum below.
			//keep track of what order the enum entry is.  
				//for example, if the sound WIN is placed 3rd in the list below,
				//make sure to add the WIN sound effect in the 3rd place of the soundArray in the Unity editor.

		public AudioClip[] soundArray;

		public enum soundIndexes
		{
	
			EGG_CRACK,
			CHICK_SAVED,
			EGG_LAUNCH
			
		}
		
		public void playSound(soundIndexes whichSound)
		{
			
			GetComponent<AudioSource>().clip=soundArray[(int)whichSound];
			GetComponent<AudioSource>().Play();
			
		}
		
	//---------------------
			
		//Setting values at the start of the game's execution.
		void Awake ()
		{
			
			//Initializing the static variable.
			gameInstance= this;
			
			//Setting the start position vector.
			roosterStartPosition= new Vector2 (-4.43f, -4.35f);
			
			//Spawn the roosters.
			spawnRoosters();
			
		}
		
		void Start()
		{
			hideUI();
			StartCoroutine(startNextLevelSequence());
		}


		void shuffleRoosters()
		{
			//5.1.2015.
			//This method changes the positions of the roosters in the array.
				//This is run only in the last section of the game (last 3 levels).
			
			//1. Shuffling the array that keeps positions.
				// Knuth shuffle algorithm :: courtesy of Wikipedia.
				for (int i = 0; i < shuffleAmount; i++ )
				{
					int temp = roosterShuffleArray[i];
					int j = Random.Range(i, numberOfRoosters);
					roosterShuffleArray[i] = roosterShuffleArray[j];
					roosterShuffleArray[j] = temp;
				}

			//2. Performing the position change.
			for (int i = 0; i < numberOfRoosters; i++)
			{
		
					//Switching the rooster to the new position.
					roostersArray[i].transform.position= 
							new Vector3(roosterStartPosition.x + roosterShuffleArray[i]* X_position_Offset ,
					            		roostersArray[i].transform.position.y,
					           			 0);
			}

		}
		

	void spawnRoosters () {
			
			//This method is run at the start of the game.

			//It instantiates all 10 roosters in a loop.
				//The roosters are labelled following the traditional layout 
					//of numbers on the keyboard (1-9, then 0). 
				//Therefore: the last rooster is labeled '0', not '10', 

			roostersArray= new GameObject[numberOfRoosters];

			for(int i=0; i<numberOfRoosters; i++)
				{

					roostersArray[i] = Instantiate(Resources.Load("Rooster", typeof(GameObject)),

					                                   new Vector3(roosterStartPosition.x + i* X_position_Offset ,
					            								   roosterStartPosition.y,
					            								   0) ,

					                                   Quaternion.identity

					                                   ) as GameObject;

					//assigning a random color to each rooster.
					roostersArray[i].GetComponent<SpriteRenderer>().color= 
						new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), 1.0f);


					//setting movement Key to use.
					string movementKey=(i+1).ToString();
					if(i==9)
						movementKey="0";

					//setting number to be displayed on the rooster.
					roostersArray[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite= 
							Resources.Load("roosterBanner"+movementKey, typeof(Sprite)) as Sprite;
			
			//Assigning parameters to the rooster:
						//1st argument:movement Key
						//2nd argument:chicken jump or flight force.
						//3rd argument:the maximum time the move Key can be pressed, 
							//before player must press the Key again.
						//The first argument can vary between roosters,
							//the rest are the same throughout.
					roostersArray[i].GetComponent<roosterScript>().
							assignParameters(movementKey, ChickenFlightForce, maxButtonPressTime);
			
				}


		}//end of the SpawnRoosters() method.



//-------Status message methods.-------------

	//When the egg has hatched and it reaches the Goal area at the top,
		//increment the howManyChicksSavedCounter,
		//and instantiate a ChickSprite to display on the interface.

	public void addChickSaved(bool isHeavyEgg)
	{
			//1.Add a new Chick sprite that will be placed on the rafters.
			GameObject newPlayer = Instantiate(Resources.Load("ChickSprite", typeof(GameObject)),
		                                   
		                                   new Vector3(4.48f - (totalChicksSaved)* 0.7f ,
		            								   3.99f,
		            								   0f) ,
		                                   
		                                   Quaternion.identity
		                                   
		                                   ) as GameObject;


			if(isHeavyEgg)
				newPlayer.GetComponent<SpriteRenderer>().color= new Color32(19,59,255,255);

		howManyChicksSaved++;
		totalChicksSaved++;

		//Mother Hen is happy, trigger the animaiton.
		MotherHen.GetComponent<MotherHenScript>().happyHen();

		//Display the Interface with the updated saved chick values.
				StopCoroutine(showUI_FadeInOut());
				StartCoroutine(showUI_FadeInOut());

	
	}

	//This method is triggered when one of Mother Hen's eggs fall off.
	public void addEggLost()
	{

		howManyEggsLost++;
		totalEggsLost++;

		//Display the Interface with the updated saved chick values.
			StopCoroutine(showUI_FadeInOut());
			StartCoroutine(showUI_FadeInOut());

		checkForRestartLevel();

	}

	//Note: I'm not using this method in the game at this time, apparently.
	public void incrementEggsInLevel()
	{

		if(totalChicksInLevel==-1)
			totalChicksInLevel=1;
		else 
			totalChicksInLevel++;

	}

//----------Next Level or Game Progress methods------//

//Verifying that all conditions are met before going to the next level.
public void checkForNextLevel()
{

		//inPositionForLevelChange is activated when at least 
			//one chick is sent to the Mother Hen succesfully.
		if(inPositionForLevelChange && howManyChicksSaved+howManyEggsLost==totalChicksInLevel )
		{

			//Resetting platforms here.
			myPlatform_andEgg_Manager.resetAllPlatforms();
			StartCoroutine(startNextLevelSequence());

		}

}

//Verifying whether the level must be restarted.
void checkForRestartLevel()
	{

		if(howManyChicksSaved+howManyEggsLost==totalChicksInLevel)
		{

			//Resetting platforms here.
			myPlatform_andEgg_Manager.resetAllPlatforms();

			StartCoroutine(restartLevelSequence());

		}


	}


//This function advances the game to the next level,
	//prepares the roosters for the last 3 levels,
	//and checks for an ending condition.
IEnumerator startNextLevelSequence()
	{

		//Show next level message.
			//Wait and Hide the message.
		if(myPlatform_andEgg_Manager.currentLevel < myPlatform_andEgg_Manager.totalLevels-1)
		{

			if(myPlatform_andEgg_Manager.currentLevel==-1)
			{
				show_UI_Message(INTRO);
				yield return new WaitForSeconds(UImessageDelay*1.2f);
				show_UI_Message(BEGIN);
				yield return new WaitForSeconds(UImessageDelay);
				hide_UI_Message();

				//game time.
				myPlatform_andEgg_Manager.triggerNextLevel();
			}

			else
			{

				if(myPlatform_andEgg_Manager.currentLevel==5)
				{	
					//Shuffling roosters.
					show_UI_Message(SHUFFLED);

					if(!hasShuffled)
						{
							StartCoroutine(shuffleRoostersSequence());
							hasShuffled=true;
						}

						yield return new WaitForSeconds(UImessageDelay*1.5f);


				}

				if(myPlatform_andEgg_Manager.currentLevel==myPlatform_andEgg_Manager.totalLevels-2)
				{	
					//Shuffling roosters.
					show_UI_Message(LAST);
					yield return new WaitForSeconds(UImessageDelay);
							
				}

				else 
					{
						show_UI_Message(NEXT_LEVEL);
						yield return new WaitForSeconds(UImessageDelay);
					}

				hide_UI_Message();

				//reset UI counters.
				howManyEggsLost=0;
				howManyChicksSaved=0;
				totalChicksInLevel=-1;

				//Show UI again.
				StartCoroutine( showUI_FadeInOut());

				//Call the platform manager.
				myPlatform_andEgg_Manager.triggerNextLevel();
			}
		
		}

		else
		{

			show_UI_Message(WIN);
			yield return new WaitForSeconds(UImessageDelay);


			show_UI_Message(PLAY_AGAIN);

			//call the Menu to change Scenes.
			gameObject.GetComponent<ChooseToContinue>().activateMenu();


		}
	}

IEnumerator restartLevelSequence()
	{

		//Show restart level message.
			//Wait and Hide the message.

		show_UI_Message(RESTART);
		yield return new WaitForSeconds(UImessageDelay);
		hide_UI_Message();


		//reset UI counters.
		howManyEggsLost=0;
		howManyChicksSaved=0;
		totalChicksInLevel=-1;


		//Call the platform manager.
		myPlatform_andEgg_Manager.restartLevel();

	}


//This sequence describes the transition to the last 3 levels of the game.
	//In this transition, the locations of the roosters are shuffled, so that 
	//they no longer map to the keyboard's number key layout. 
IEnumerator shuffleRoostersSequence()
{

		float fadeRooster=1;

		//1. First, fade out all the roosters.
		while(fadeRooster>0f)
		{
			fadeRooster-=rooster_fadeInOut_Speed;
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(0,1,fadeRooster));
				for(int i=0; i<numberOfRoosters; i++)
					{
						roostersArray[i].GetComponent<SpriteRenderer>().material.color = newColor;
						roostersArray[i].transform.GetChild(0).
											GetComponent<SpriteRenderer>().material.color = newColor;
					}
			yield return null;
		}	
		
		
		//2.Second, perform the shuffle.
		shuffleRoosters();


		//3.Third, add a delay that ensures the shuffling is completed,
			//before the roosters reappear.
		yield return new WaitForSeconds(rooster_show_Delay);
		

		//4.Fourth, fade in the roosters.
		while(fadeRooster<1f)
		{
			fadeRooster+=rooster_fadeInOut_Speed;
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(0,1,fadeRooster));
				for(int i=0; i<numberOfRoosters; i++)
					{
						roostersArray[i].GetComponent<SpriteRenderer>().material.color = newColor;
						roostersArray[i].transform.GetChild(0).
											GetComponent<SpriteRenderer>().material.color = newColor;
					}
			yield return null;
		}	


}


//----------User Interface methods.------------

//The next  two methods show and hide the status message
	//that appears in the middle of the screen.

void show_UI_Message(int whatMessage)
{
		grayBackground.GetComponent<SpriteRenderer>().enabled=true;
		UI_message.text= UI_message_strings[whatMessage];


}

void hide_UI_Message()
{

		grayBackground.GetComponent<SpriteRenderer>().enabled=false;
		UI_message.text=" ";

}

	
IEnumerator showUI_FadeInOut()
	{

		//1. First of all, update the UI values.
		chicksSavedMultiplier.text="x "+totalChicksSaved.ToString();
		eggsLostMultiplier.text="x "+totalEggsLost.ToString();

		//2.Fade in the Interface.
		while(t<1f)
		{

			Color newColor = new Color(1, 1, 1, Mathf.Lerp(0,1,t));
			chicksSavedMultiplier.GetComponent<MeshRenderer>().material.color = newColor;
			eggsLostMultiplier.GetComponent<MeshRenderer>().material.color= newColor;
			chicksSavedText.GetComponent<MeshRenderer>().material.color = newColor;
			eggsLostText.GetComponent<MeshRenderer>().material.color = newColor;
			chickUISprite.GetComponent<SpriteRenderer>().color = newColor;
			eggsLostSprite.GetComponent<SpriteRenderer>().color = newColor;
			t+=fadeInOut_Speed;
			yield return null;
		}	


		//3.Show the elements at full opacity, for a time.
			yield return new WaitForSeconds(UIshowDelay);



		//4. Fade out the elements.
		while(t>0f)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(0,1,t));
			chicksSavedMultiplier.GetComponent<MeshRenderer>().material.color = newColor;
			eggsLostMultiplier.GetComponent<MeshRenderer>().material.color= newColor;
			chicksSavedText.GetComponent<MeshRenderer>().material.color = newColor;
			eggsLostText.GetComponent<MeshRenderer>().material.color = newColor;
			chickUISprite.GetComponent<SpriteRenderer>().color = newColor;
			eggsLostSprite.GetComponent<SpriteRenderer>().color = newColor;
			t-=fadeInOut_Speed;
			yield return null;
		}	

	}

	void hideUI()
	{
		//Setting all UI elements' opacity to 0.

		chicksSavedMultiplier.GetComponent<MeshRenderer>().material.color = 
					new Color(255, 255, 255, 0);
		eggsLostMultiplier.GetComponent<MeshRenderer>().material.color = 
			new Color(255, 255, 255, 0);
		chicksSavedText.GetComponent<MeshRenderer>().material.color = 
			new Color(255, 255, 255, 0);
		eggsLostText.GetComponent<MeshRenderer>().material.color = 
			new Color(255, 255, 255, 0);
		chickUISprite.GetComponent<SpriteRenderer>().color = 
			new Color(255, 255, 255, 0);
		eggsLostSprite.GetComponent<SpriteRenderer>().color = 
			new Color(255, 255, 255, 0);


	}

}
