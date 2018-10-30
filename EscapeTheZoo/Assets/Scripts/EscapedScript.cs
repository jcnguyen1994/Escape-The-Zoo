﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapedScript : MonoBehaviour {

    bool winGame = false;
    public GameObject winGameHud;
    int winGameTime;
	// Use this for initialization
	void Start () {
        winGame = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (winGame)
        {
            print(Time.time - winGameTime);
            if ((int)Time.time - winGameTime > 3)
            {
                print("Shifting scene now");
                SceneManager.LoadScene("MainMenu");
                winGame = false;
                return;
            }
            return;
        }
    }


    private void endGame()
    {
        winGame = true;
        winGameTime = (int)Time.time;
        Animator youEscaped = winGameHud.GetComponent<Animator>();
        youEscaped.Play("WinGameClip");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TouchMeCube")
        {
            endGame();
        }
    }
}
