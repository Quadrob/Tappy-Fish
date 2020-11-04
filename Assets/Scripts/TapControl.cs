using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

//make sure theres a rigidbody
[RequireComponent(typeof(Rigidbody2D))]
//method to control tap
public class TapControl : MonoBehaviour {
    //initiating events
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerScored;
    public static event PlayerDelegate OnPlayerDied;

    //declare variables
    public float tapForce = 220;
    public float tiltSmooth = 2;
    public Vector3 startPostion;


    //declare audio variables
    public AudioSource TapAudio;
    public AudioSource ScoreAudio;
    public AudioSource DieAudio;

    Rigidbody2D rigidBody2D;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    //function to control rotation
    void Start() {
        rigidBody2D = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -40);
        forwardRotation = Quaternion.Euler(0, 0, 40);
        game = GameManager.Instance;
        rigidBody2D.simulated = false;
    }
    //switch event listeners on and off
    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    //event method
    void OnGameStarted() {
        rigidBody2D.velocity = Vector3.zero;
        rigidBody2D.simulated = true;
    }
    //event method
    void OnGameOverConfirmed() {
        transform.localPosition = startPostion;
        transform.rotation = Quaternion.identity;
    }
    //continuose update 
    void Update() {
        //check if game over
        if (game.GameOver) { return; }

        //if method to pick up tap on screen
        if (Input.GetMouseButtonDown(0)) {
            TapAudio.Play();
            //rotate fish and push up
            transform.rotation = forwardRotation;
            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }
        //rotate fish down
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    //score or dead zones
    void OnTriggerEnter2D(Collider2D col) {

        if (col.gameObject.tag == "ScoreZone") {
            //register a score event and play sound
            OnPlayerScored(); //event sent to gameManager
            ScoreAudio.Play();
        }
        if (col.gameObject.tag == "DeadZone") {
            //stop fish movement
            rigidBody2D.simulated = false;
            //register a dead event and play sound
            OnPlayerDied(); //event sent to gameManager
            DieAudio.Play();
        }
    }

}

