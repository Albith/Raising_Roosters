using UnityEngine;
using System.Collections;

public class scriptForMusic : MonoBehaviour {

	static bool AudioBegin=false;

	//A simple script that turns on the main theme loop
		//at the beginning of the game.

	void Start () {
	
		if(!AudioBegin)
		{	
			GetComponent<AudioSource>().Play();
			DontDestroyOnLoad(gameObject);
			AudioBegin=true;
		}
	}

}
