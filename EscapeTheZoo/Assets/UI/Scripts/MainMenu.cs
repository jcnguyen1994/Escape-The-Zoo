﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	
	// Use this for initialization
	public void PlayGame () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

	public void ExitGame () {
		Debug.Log ("Exiting Game");
		Application.Quit ();
	}

	public void SetVolume(float value){
		Debug.Log ("Volume Set: "+ value);
		AudioListener.volume = value;
		Prefs.GeneralVolume = value;
	}

}
