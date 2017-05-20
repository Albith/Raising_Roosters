using UnityEngine;
using System.Collections;

public class eggScript : MonoBehaviour {


	//Set current Unit Index the egg is occupying now.
	public int unitIndex=0;
	public int whichPlatform=0;
	//Change these variables to change the duration of the hatching sequence.
		public float eggHatchDuration;
		//number of Egg hatching sprites (including the initial egg picture).
		int numberOfEggSprites=6;  
		float eggHatch_timeStep;  //equals hatchDuration/ numberOfSprites.


	//Status variables to facilitate hatching.
		bool isHatchingStarted=false;
		bool isEggHatched=false;
		bool isEggCollided=false;
		bool isEggDelivered=false;
	//Start position of egg.
		Vector3 startPosition;

	//Other variables.
		float eggSplatDuration=2f;


	// Use this for initialization
	void Start () {
	
		eggHatch_timeStep= eggHatchDuration/numberOfEggSprites;
		launchEgg();
		startPosition=transform.position;
		//unitIndex=0;
	
	}
	

	//activates the gravity for the egg.
	void launchEgg()
	{
		GetComponent<Rigidbody>().useGravity=true;

	}

	//Method is called by the platform that the egg rests on,
		//only when a rooster touches said platform.
	public void startEggHatching()
	{
		if(!isHatchingStarted)
		{
			isHatchingStarted=true;
			//print ("STARTING EGG HATCHING.");
			StartCoroutine(eggHatchSequence());
		}

	}

	//Used when resetting the egg.
	public void stopEggHatching()
	{
		StopAllCoroutines();
	}


	//Sequence that:
		//calls a delay (how long is based on number of hatch frames and total duration),
		//these variables are created at the top of the file.

		//sets a boolean 'isEggHatched' to true at the end of the sequence,
			//only then can the bird be lifted to the top, 
			//and the win score increased.

	IEnumerator eggHatchSequence()
	{

		for(int i=1; i<numberOfEggSprites; i++)
		{
			yield return new WaitForSeconds(eggHatch_timeStep);

			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("egg"+i);

			if(i==5)	
				{
					isEggHatched=true;
					StartCoroutine(birdChirpSequence());
				}
				
				gameManager.gameInstance.playSound(gameManager.soundIndexes.EGG_CRACK);
		}
		
	}


	IEnumerator birdChirpSequence()
	{

		while(isEggHatched)
		{

			float stagger_delay= Random.Range (1f, 3f);

			yield return new WaitForSeconds(stagger_delay);

			gameObject.GetComponent<AudioSource>().Play();

		}


	}

//-----Collision methods.-----

	void OnCollisionEnter(Collision myCollision)
	{

		if(myCollision.collider.tag =="Rooster")
				{
					startEggHatching();
					print ("START EGG HATCHING.");
		
				}

	}

	void OnTriggerEnter (Collider otherCollider)
	{
		
		if(otherCollider.tag== "Bottom2"|| 
		   otherCollider.tag== "Left"|| 
		   otherCollider.tag== "Right" )
			{
				if(!isEggCollided)
				{
					isEggCollided=true;
					//one egg is decremented from the # of eggs on screen.
					gameManager.gameInstance.myEggLauncher.EggRemovedFromScreen(unitIndex);
					
					//decrementing from platform counter.
					gameManager.gameInstance.myPlatform_andEgg_Manager.
					platformsArray[whichPlatform].GetComponent<platformScript>().eggsOnPlatform--;

					StopAllCoroutines();
					
					gameManager.gameInstance.addEggLost();
					StartCoroutine(eggSplatRoutine());
					
				}
			}
	}
		

	void OnTriggerStay(Collider otherCollider)
	{

	if(otherCollider.tag== "Goal" && isEggHatched  && !isEggDelivered)
			{	
				isEggDelivered=true;
				if(gameObject.name.Contains("Egg_Heavy"))
			   		gameManager.gameInstance.addChickSaved(true);
			   	else
					gameManager.gameInstance.addChickSaved(false);
				//fade out platform.
				
				gameManager.gameInstance.myPlatform_andEgg_Manager.
				platformsArray[whichPlatform].GetComponent<platformScript>().eggsOnPlatform--;
			   
				if(gameManager.gameInstance.myPlatform_andEgg_Manager.
					platformsArray[whichPlatform].GetComponent<platformScript>().eggsOnPlatform<=0)
						gameManager.gameInstance.myPlatform_andEgg_Manager.
							platformsArray[whichPlatform].GetComponent<platformScript>().toFadeOut();

				Destroy(gameObject);
	
			}	
	}

void Update()
	{





	}


//----Egg reset sequence.---------


	IEnumerator eggSplatRoutine()
	{
			gameManager.gameInstance.playSound(gameManager.soundIndexes.EGG_CRACK);
			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("kerSplat1");
			
			gameObject.GetComponent<Rigidbody>().useGravity=false;
			gameObject.GetComponent<Rigidbody>().isKinematic=true;

			yield return new WaitForSeconds(eggSplatDuration);

			Destroy(gameObject);

	}



}
