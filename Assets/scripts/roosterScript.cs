using UnityEngine;
using System.Collections;

public class roosterScript : MonoBehaviour {

	//This script is attached to each of the 8 roosters in the main game mode.

	//Movement Key related variables.
		public string movementKey;
		bool isMovingStarted=false;
		bool isMoveKeyAssigned=false;
		
	//States for the Audio and Graphics:
		float flapUpDelay=0.3f;
		float flapDownDelay=0.6f;
		float currentFlapDelay;
		roosterSounds currentSoundIndex=roosterSounds.FLAP_UP;

	//Sound Array added.
		public AudioClip[] soundArray;
		public enum roosterSounds
		{
			FLAP_UP,
			FLAP_DOWN		
		}
	
		public void playSound(roosterSounds whichSound)
		{
			
			GetComponent<AudioSource>().clip=soundArray[(int)whichSound];
			GetComponent<AudioSource>().Play();
			
		}


		//Data structure to describe the rooster's state.
		public enum roosterState
		{
			
			IN_FLOOR,
			GOING_UP,
			GOING_DOWN
			
		}	

		roosterState currentState;

	//Force and Movement variables, determined in the Game Manager.
		//They are very important for the feel of the game.
			float upForce=0f;
			int maxPressTime=0;
			int currentPressTime=0;

	//The Y starting position for roosters is constant,
		//and it's stored here.
		float startingY_position=-4.35f;

//----------Class Methods---
	void Start () {
	
		isMovingStarted=false;
		currentFlapDelay=0f;
		gameObject.tag="Rooster";
	}

	
	//------Animation and Sound Methods.
	void setRoosterState(roosterState newState)
	{

		//This function plays the animation and sound sequences required 
			//for each state.

		if(currentState == newState)
		{ 
			//Do nothing.    
		}

		else
		{
		
			if(newState == roosterState.GOING_UP)
			{

				currentFlapDelay=flapUpDelay;
				currentSoundIndex=roosterSounds.FLAP_UP;

				//Play the audio/video sequence until no longer going up.
				if(currentState == roosterState.IN_FLOOR)
					StartCoroutine(flap_Sequence());

				currentState= newState;

			}


			else if(newState == roosterState.GOING_DOWN)
			{

				//When descending, the flapping animation is the same as the upward one,
					//but the delay between animations is longer
					//(so the flapping looks slower.)
				currentFlapDelay=flapDownDelay;
				currentSoundIndex=roosterSounds.FLAP_DOWN;			

				//Play audio/video sequence.
				if(currentState == roosterState.IN_FLOOR)
					StartCoroutine(flap_Sequence());

				currentState= newState;

			}

			else 	//go to the Idle state.
			{

				//Stop all animation sequences.
				StopAllCoroutines();

				//Set the rooster's graphic to the idle one.
				gameObject.GetComponent<SpriteRenderer>().sprite=
					Resources.Load<Sprite>("roosterIdle");

				currentState= newState;


			}

		}


	}

	//The flap animation sequence is described below.
	IEnumerator flap_Sequence()
	{

		//The flap animation continues until the rooster lands.
			//In the meantime, the flapDelay is modified
			//depending on whether the rooster is flying up or descending.
		while(true)
		{

			//set graphic to flap up graphic.
			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("roosterFlapUp");

			//play Sound Effect.
			playSound(currentSoundIndex);

			yield return new WaitForSeconds(currentFlapDelay);

			//set graphic to flap down graphic.
			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("roosterFlapDown");

			//play Sound Effect.
			playSound(currentSoundIndex);

			yield return new WaitForSeconds(currentFlapDelay);


		}




	}

	//This method is called from the Game Manager, assigning parameters to the rooster:
		//1st argument:movement Key assigned to the rooster
		//2nd argument:chicken jump or flight force.
		//3rd argument:the maximum time the move Key can be pressed, 
		//before player must press the Key again.

	//The 2nd or 3rd arguments are pretty much constant, 
		//but the 1st argument can be modified in the game's last section, 
		//when the roosters' positions are randomized.

	public void assignParameters(string whatKey, float howMuchForce, int howMuchPressTime)
	{

		//Quick check: reassigning space button in string.
		if(whatKey== " ")
			whatKey="space";

		//Setting object name.
		gameObject.name= "rooster"+whatKey;
		print (gameObject.name+" was created.");

		//Assigning a Movement Key.
		movementKey=whatKey;
		isMoveKeyAssigned=true;

		//Setting the upward force for the rooster.
		upForce= howMuchForce;

		//Setting the maximum time the flight button can be pressed.
			//This forces the player to press the flight button again,
			//and have to plan out how the rooster will fly.
		maxPressTime=howMuchPressTime;

	}


	// Update checks for the movement Key status.
		//if starting to press, apply a Force to the rooster.

	void Update () {
	
		//Moving the character, only if the MoveKey has been assigned.
		if(isMoveKeyAssigned)
			{
				if(Input.GetKey(movementKey) )
					{	
					
						//The player can hold down the flight button and have the rooster
							//fly, but only for a certain period (maxPressTime).
							//After this, the rooster will stop moving up and start descending.
						//The player must let go of the flight button and 
							//press it again for the rooster to start flying again.
						if(currentPressTime<maxPressTime)
						{
							//if the rooster is on the ground,
								//turn on the rigidbody and its gravity.
							if(!isMovingStarted) 
								{ 	
									isMovingStarted=true;		
				 					GetComponent<Rigidbody>().useGravity=true;
									GetComponent<Rigidbody>().WakeUp();
									GetComponent<Rigidbody>().isKinematic=false;
								}

							GetComponent<Rigidbody>().AddForce(Vector3.up * upForce);
							
							setRoosterState(roosterState.GOING_UP);

							currentPressTime++;

						}
					}

				else
					{
						currentPressTime=0;
					
						if(isMovingStarted)
							{
								//print ("setting going down state.");
								setRoosterState(roosterState.GOING_DOWN);
							}
					}
			}

		//Checking if the rooster has reached the bottom of the screen, 
			//If this is true, turn off the rigidbody to stop descending.
		if(transform.position.y < startingY_position)
			if(isMovingStarted)
				{
					setRoosterState(roosterState.IN_FLOOR);

					isMovingStarted=false;
					GetComponent<Rigidbody>().Sleep();
					
					//resetting position.
					transform.position= 
						new Vector3 (transform.position.x,
						             startingY_position,
						             transform.position.z);
					
					//resetting rigidbody.
					GetComponent<Rigidbody>().useGravity=false;
					GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
					GetComponent<Rigidbody>().rotation=Quaternion.identity;
					
					GetComponent<Rigidbody>().isKinematic=true;
					
				}


	}
	

}
