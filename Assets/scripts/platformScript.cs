using UnityEngine;
using System.Collections;

public class platformScript : MonoBehaviour {

	//Platform class.
		//contains all the rules and checks that:
			//reset it,
			//activate gravity on it.

	//Collision Logic.
	bool isReadyForHatching=false;

	//Variables for Platform Manager lookup.
		public int unitSize=2;
		public int firstUnitOccupied=0;
	

	//Platform reset delay.
		public float platformResetDelay=1f;	
		
	//Fade out variables.
		float fadeInOut_Speed=0.03f;
		public float t=1;

		public int eggsOnPlatform=0;


	void Start () {


		unitSize=2;  //the smallest size for default.

	}
	


//-----Platform reset methods.------
	

	void resetPlatform()
	{

		eggsOnPlatform=0;

		GetComponent<BoxCollider>().isTrigger=false;
		isReadyForHatching=false;

		print ("--Resetting platform "+gameObject.name);

		//hide Platform.
		gameObject.GetComponent<SpriteRenderer>().enabled=false;
		GetComponent<Rigidbody>().useGravity=false;

		GetComponent<Rigidbody>().Sleep();
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().rotation=Quaternion.identity;

		gameObject.GetComponent<BoxCollider>().enabled=false;


		//grabbing platform number from the platform's name.
		int indexOfPlatform= (int)(gameObject.name[gameObject.name.Length- 1])-49;
		print ("launching platform with index "+indexOfPlatform);


		//Edit: updating column states, removing platform references..
		gameManager.gameInstance.myPlatform_andEgg_Manager.updateColumnStates_and_resize(indexOfPlatform);

	}

	//Launches the platform:
		//when platform is called by the platformEggManager.

	public void launchPlatform()
	{

		//enable Platform.
		gameObject.GetComponent<BoxCollider>().enabled=true;

		//shouldEggStartHatch=false;

		//resetting rigidbody.
		GetComponent<Rigidbody>().useGravity=true;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().rotation=Quaternion.identity;
		GetComponent<Rigidbody>().WakeUp();


		//raise opacity to max.
		gameObject.GetComponent<SpriteRenderer>().color = 
			new Color(255, 255, 255, 255);
		t=1;

		//show Platform.
		gameObject.GetComponent<SpriteRenderer>().enabled=true;


	}



//----------Collision methods.

	

	//Checks whether platform has touched one of the borders (top, bottom, sides)
		//and initiates a reset sequence if true.
	void OnTriggerEnter (Collider otherCollider)
	{

		if(otherCollider.tag== "Bottom2"|| 
		   otherCollider.tag== "Left"|| 
		   otherCollider.tag== "Right" )
				StartCoroutine(fadeOut_and_Reset_Sequence());
				

	}

	void OnCollisionEnter(Collision myCollision)
	{
		
		if(myCollision.collider.tag =="Rooster")
		{
			isReadyForHatching=true;
			print ("######Touched Rooster.#######");
		}


		
	}

	void OnCollisionStay(Collision myCollision)
	{

		if(myCollision.collider.tag =="Egg")
		{
			

			if(isReadyForHatching)
			{	
				myCollision.gameObject.GetComponent<eggScript>().startEggHatching();
				print ("START EGG HATCHING.");
				
			}
		}

	}

	public void toFadeOut()
	{
		StartCoroutine(fadeOut_and_Reset_Sequence());


	}


	IEnumerator fadeOut_and_Reset_Sequence()
	{

		//Do some disabling of physics first.
		GetComponent<Rigidbody>().useGravity=false;		
		GetComponent<Rigidbody>().Sleep();
		GetComponent<BoxCollider>().isTrigger=true;

		//Fade Out.
			
		while(t>0f)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(0,1,t));
			gameObject.GetComponent<SpriteRenderer>().color = newColor;
			t-=fadeInOut_Speed;
			yield return null;
		}	

		resetPlatform();

	}


	void Update()
	{




	}



}
