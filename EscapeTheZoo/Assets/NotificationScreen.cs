﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : MonoBehaviour {

    string notifyText;
    float startTime;
    float duration;
    public Text textBox;
    static NotificationScreen instance;
    // Use this for initialization
    public static NotificationScreen getInstance()
    {
        return instance;
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        notifyText = "Hello Mr. Elephant \n This is what you have to do: \n 1.Avoid the security guard \n 2.Free your friends \n3.Escape the zoo";
        startTime = Time.time;
        duration = 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime & startTime + duration > Time.time)
        {
            textBox.text = notifyText;
        }
        else
            textBox.text = "";
    }

    public void displayNotification(string msg, float sTime, float length)
    {
        notifyText = msg;
        duration = length;
        startTime = sTime;
    }
}
