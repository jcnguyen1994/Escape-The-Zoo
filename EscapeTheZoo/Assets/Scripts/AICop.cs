﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AICop : MonoBehaviour {
    public GameObject player;
    public GameObject key;
    public GameObject[] waypoints;
    public GameObject gameOverHud;
    public GameObject spawnPoint;
    public Rigidbody rB;
    NavMeshAgent nav_mesh;
    Animation anim;
    VelocityReporter vReporter;
    int currWaypoint = -1;
    public enum AIStates { Patrol, Chase, Flying, Eating }
    AIStates AIstate;
    public float chaseDistance,stealDistance;
    static int gameOverTime;
    float flyTime = 0, startTime = 0;
    public bool hasKey = false;
    
    //public bool canFire;
    // Use this for initialization
    void Start()
    {
        nav_mesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();
        vReporter = player.GetComponent<VelocityReporter>();
        rB = GetComponent<Rigidbody>();
        AIstate = AIStates.Patrol;
        startTime = 0;
        SetNextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(NotificationScreen.gameOver)
        {
            if( ((int)Time.time - gameOverTime) > 5)
            {
                print(Time.time);
                print("Shifting scene now");
                SceneManager.LoadScene("MainMenu");
                NotificationScreen.gameOver = false;
                return;
            }
            return;
        }

        switch (AIstate)
        {
            case AIStates.Patrol:
                setVelocities();
                patrol();
                break;
            case AIStates.Chase:
                setVelocities();
                chase();
                break;
            case AIStates.Flying:
                fly();
                break;
            case AIStates.Eating:
                setState(AIStates.Flying);
                break;
        }

    }
    void setVelocities()
    {
        rB.velocity = Vector3.zero;
        rB.angularVelocity = Vector3.zero;
    }
    private void fly()
    {
        flyTime += Time.deltaTime;
        
        if (flyTime > 5)
        {
            this.transform.rotation = Quaternion.Euler(0,0,0);
            this.transform.position = spawnPoint.transform.position;
            nav_mesh.enabled = true;
            nav_mesh.isStopped = false;
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            setState(AIStates.Patrol);
            this.GetComponent<Rigidbody>().useGravity = true;
            SetNextWaypoint();

            print("Spawning new cop");
        }
    }
    private void ChaseWaypoint()
    {
        Vector3 targetVel = vReporter.Velocity;
        Vector3 predictedPosition = vReporter.prevPos + vReporter.Velocity * Time.deltaTime;
        nav_mesh.SetDestination(predictedPosition);
        nav_mesh.speed = 7 + Mathf.Min((Time.time - startTime)/30,7);
    }
    private void SetNextWaypoint()
    {
        if (waypoints.Length == 0)
        {
            print("Error: Length of waypoints zero");
            return;
        }
        nav_mesh.speed = 3;
        currWaypoint = (currWaypoint + 1) % waypoints.Length;
        nav_mesh.SetDestination(waypoints[currWaypoint].transform.position);

    }
    private void endGame()
    {
        if (!NotificationScreen.gameOver)
        {
            nav_mesh.isStopped = true;
            //player.GetComponent<Animator>().SetBool("caught",true);
            print("Game Over");
            NotificationScreen.gameOver = true;
            gameOverTime = (int)Time.time;
            Debug.Log("GO time" + gameOverTime);
            Animator gameOverAnimator = gameOverHud.GetComponent<Animator>();
            anim.Play("Whistle");
            gameOverAnimator.Play("GameOver");
            GameData gd = new GameData();
            gd.Load();
            gd.updateScore(ScoreSystem.getInstance().curScore);
            print(gd.scores.ToArray()[0]);
            gd.Save();
        }
    }

    private void patrol()
    {
        
        Vector3 distanceToPlayer = (player.transform.position - this.transform.position);
        float dir = Vector3.Dot(this.transform.forward, distanceToPlayer);

        if (distanceToPlayer.magnitude < 3)
        {
            print("Calling endgame");
            endGame();
            return;
        }
        if (!nav_mesh.pathPending && nav_mesh.remainingDistance < 0.5f)
        {
            SetNextWaypoint();
        }
        if (distanceToPlayer.magnitude < chaseDistance - 10 && dir > 0)
        {
            setState(AIStates.Chase);
        }
        else if (hasKey && distanceToPlayer.magnitude < stealDistance && dir < 0)
        {
            NotificationScreen.getInstance().displayNotification("Press F to pay respects", Time.time, 2);

            if (Input.GetKeyDown(KeyCode.F))
            {
                stealKey();
            }
        }
        anim.Play("Walk");
    }
    void stealKey()
    {
        key.transform.position = player.transform.position;

        hasKey = false;
    }
    public void setState(AIStates state)
    {
        switch (state)
        {
            case AIStates.Flying:
                flyTime = 0;
                nav_mesh.isStopped = true;
                nav_mesh.enabled = false;
                
                break;
            case AIStates.Chase:
                AudioManager.getInstance().playAlert();
                break;
            case AIStates.Eating:
                AudioManager.getInstance().playEat();
                anim.Play("Eating");
                this.GetComponent<Rigidbody>().useGravity = false;
                break;
        }
        AIstate = state;

    }
    private void chase()
    {
        Vector3 distanceToPlayer = (player.transform.position - this.transform.position);
        float dir = Vector3.Dot(this.transform.forward, distanceToPlayer);

        if (distanceToPlayer.magnitude < 3)
        {
            print("Calling endgame");
            endGame();
            return;
        }
        if (distanceToPlayer.magnitude > chaseDistance || dir < 0)
        {
            AIstate = AIStates.Patrol;
            return;
        }
        if (!nav_mesh.pathPending && nav_mesh.remainingDistance < 1.5f)
        {
            AIstate = AIStates.Patrol;
            SetNextWaypoint();
        }
        else
        {
            ChaseWaypoint();
        }
        anim.Play("Run");
    }
}
