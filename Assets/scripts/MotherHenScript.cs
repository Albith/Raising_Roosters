using UnityEngine;
using System.Collections;

public class MotherHenScript : MonoBehaviour {

	bool playingAnimation=true;
	float animationDelay=1.0f;
	float happyDelay=2.0f;

	//This class handles the animations for the motherHen object.
		//Its animations are controlled by the gameManagerScript.
	void Start () {
	
		//Start the animation.
		StartCoroutine(henAnimation());
	}
	
	//The animation played here is a basic idle animation (running in place, I believe).
	IEnumerator henAnimation()
	{

		while(playingAnimation)
		{

			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("hen4");

			yield return new WaitForSeconds(animationDelay);

			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("hen1");
			
			yield return new WaitForSeconds(animationDelay);

			gameObject.GetComponent<SpriteRenderer>().sprite=
				Resources.Load<Sprite>("hen5");
			
			yield return new WaitForSeconds(animationDelay);

		}


	}

	//This next animation is played when a chick is succesfully handed over to the Mother Hen.
	IEnumerator happyHenAnimation()
	{

		gameObject.GetComponent<SpriteRenderer>().sprite=
			Resources.Load<Sprite>("hen_happy");
	
		yield return new WaitForSeconds(happyDelay);


		playingAnimation=true;
		StartCoroutine(henAnimation());
	}

	public void happyHen()
	{
		playingAnimation=false;

		GetComponent<AudioSource>().Play();

		StopAllCoroutines();
		StartCoroutine(happyHenAnimation());

	}

}
