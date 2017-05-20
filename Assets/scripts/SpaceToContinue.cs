using UnityEngine;
using System.Collections;

public class SpaceToContinue : MonoBehaviour {
	
	//This class checks for the player's press of the space key 
		//inside the main menu & tutorial modes.
	//Note: this is an older form of the class 'ChooseToContinue.cs'.

	public int levelNumber;
	private bool loadLock;

	//Variables to flash a message prompting player to press space.
	GameObject flashingPrompt;
	float prompt_ShowHide_Delay=0.6f;


// Initializing the prompt and setting up the flashing coroutine.

	void Start () {

		//initialize the flashingPrompt variable.
		flashingPrompt= GameObject.FindGameObjectWithTag("Next");

		//Call the coroutine that flashes it.
		StartCoroutine(flashPromptSequence());

	}

	IEnumerator flashPromptSequence()
	{

		while(true)
		{

			//show the prompt.
			flashingPrompt.GetComponent<MeshRenderer>().enabled=true;

			yield return new WaitForSeconds(1f);


			//hide the prompt.
			flashingPrompt.GetComponent<MeshRenderer>().enabled=false;

			yield return new WaitForSeconds(0.8f);

		}

	}


// Update checks for the space key being pressed.
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)  && !loadLock)
			LoadScene ();
	}
	
	void LoadScene(){
		loadLock = true;
		Application.LoadLevel (levelNumber);
	}
}
