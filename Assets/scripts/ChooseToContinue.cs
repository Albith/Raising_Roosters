using UnityEngine;
using System.Collections;

public class ChooseToContinue : MonoBehaviour {

//This class manipulates the game menu.

	//Booleans that regulate the menu sequence access.
	public bool isLoadAutomatic=true, isReadyToCheckInput=false;

	public int firstChoice, secondChoice;
	private bool loadLock;

	//Variables to flash a message prompting player to press space.
	GameObject firstChoiceText, secondChoiceText;
	int currentChoice=0;
	bool hasChoiceChanged=false;

	float prompt_Show_Delay=0.5f;
	float prompt_Hide_Delay=0.5f;


// Initializing the prompt and setting up the flashing coroutine.
	void Start () {

		//start with first Choice highlighted.
		currentChoice= firstChoice;

		//initialize the flashingPrompt variable.
		firstChoiceText= GameObject.FindGameObjectWithTag("Next1");
		secondChoiceText= GameObject.FindGameObjectWithTag("Next2");

		if(isLoadAutomatic)
		{
			isReadyToCheckInput=true;

			//Call the coroutine that flashes it.
			InvokeRepeating("checkChoiceChanged", 0, 0.03f);
			StartCoroutine(flashCurrentChoice_Sequence());
		}
	}

	public void activateMenu()
	{

		isReadyToCheckInput=true;

		//Showing the menu options.
		firstChoiceText.GetComponent<MeshRenderer>().enabled=true;
		secondChoiceText.GetComponent<MeshRenderer>().enabled=true;

		//Call the coroutine that flashes it.
		InvokeRepeating("checkChoiceChanged", 0, 0.03f);
		StartCoroutine(flashCurrentChoice_Sequence());

	}


	void checkChoiceChanged()
	{

			if(hasChoiceChanged)
			{	
				//revert the previous choice to white text.
				//the flashing will stop for this choice as well.
				//also, color the current text yellow.
				
				if(currentChoice==firstChoice)
				{
					secondChoiceText.GetComponent<TextMesh>().color= Color.white;
					secondChoiceText.GetComponent<MeshRenderer>().enabled=true;

					firstChoiceText.GetComponent<TextMesh>().color= new Color32(251,255,0,255);
				}
				
				else				
				{
					firstChoiceText.GetComponent<TextMesh>().color= Color.white;
					firstChoiceText.GetComponent<MeshRenderer>().enabled=true;

					secondChoiceText.GetComponent<TextMesh>().color= new Color32(251,255,0,255);
				}
				
				
				hasChoiceChanged=false;
				
			}

	}

	IEnumerator flashCurrentChoice_Sequence()
	{

		while(true)
		{

				//show the prompt.
				if(currentChoice==firstChoice)
					firstChoiceText.GetComponent<MeshRenderer>().enabled=true;
				else 
					secondChoiceText.GetComponent<MeshRenderer>().enabled=true;

				yield return new WaitForSeconds(prompt_Show_Delay);


				//hide the prompt.
				if(currentChoice==firstChoice)
					firstChoiceText.GetComponent<MeshRenderer>().enabled=false;
				else 
					secondChoiceText.GetComponent<MeshRenderer>().enabled=false;

				yield return new WaitForSeconds(prompt_Hide_Delay);	

		}

	}


// Update checks for the space key being pressed.
	void Update () {

		if(isReadyToCheckInput)
			InputCheck();

	}


	void InputCheck()
	{
				
				//Detects the space key is being pressed and loads a new level.
				
				if (Input.GetKeyDown(KeyCode.Space)  && !loadLock)
					LoadScene ();
							
				//Detects one of the navigation keys is being pressed and 
				//changes the highlighted and selected choice.
				
				else{
					//Left or Up key was pressed.
					if (Input.GetKeyDown(KeyCode.LeftArrow)  || Input.GetKeyDown(KeyCode.UpArrow))
					{
						if(currentChoice!= firstChoice)
						{
							currentChoice= firstChoice;
							hasChoiceChanged=true;
						}
					}
					
					//Down or Right key was pressed.
					else if (Input.GetKeyDown(KeyCode.RightArrow)  || Input.GetKeyDown(KeyCode.DownArrow))
					{
						if(currentChoice!= secondChoice)
						{
							currentChoice= secondChoice;
							hasChoiceChanged=true;	
						}
					}
					
				}

	}


	void LoadScene(){
		loadLock = true;
		Application.LoadLevel (currentChoice);
	}


}
