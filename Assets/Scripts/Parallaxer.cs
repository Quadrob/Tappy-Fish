using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

//this script is used to move objects
public class Parallaxer : MonoBehaviour {
    //create a pool for objects 
    class PoolObject {
        public Transform transform;
        public bool objInUse;
        public PoolObject(Transform t) { transform = t; }
        public void use() { objInUse = true; }
        public void Dispose() { objInUse = false; }
    }

    [System.Serializable]
    //get the variation in up down movement
    public struct YSpawnRange {
        public float miny;
        public float maxy;
    }
    //get the game objects
    public GameObject Prefab1;
    public GameObject Prefab2;
    public GameObject Prefab3;
    public GameObject Prefab4;
    public GameObject Prefab5;
    public GameObject Prefab6;
    //get other values used
    public int poolSize;
    public float shiftSpeed;
    public float gameSpeedUp;
    public float spawnRate;
    public bool visible;
    //declare variables
    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPosition;
    public bool spawnImmediate;//particale preware
    public Vector3 immediateSpawnPosition;
    public Vector2 targetAspectRatio;

    GameObject[] inGameObjects;
    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;
    float shiftSpeedOriginal;

    //this method calls config to fill arrays
    void Awake() {
        Configure();
    }
    //start the script
    void Start() {
        game = GameManager.Instance;
        shiftSpeedOriginal = shiftSpeed;

    }
    //switch event listeners on and off
    void OnEnable() {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    //on game over empty arrays and call config to repopulate and reset speed
    void OnGameOverConfirmed() {
        for (int i = 0; i < poolObjects.Length; i++) {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
            Destroy(inGameObjects[i]);
        }
        shiftSpeed = shiftSpeedOriginal;
        Configure();
    }
    //continueously update 
    void Update() {
        //check if game over
        if (game.GameOver) { return; }
        //timer to spawn objects
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) {
            //speed up shift speed to increase difficulty
            float score = GameManager.Instance.Score;
            if (score == 10) { shiftSpeed += gameSpeedUp; }
            else if (score == 20) { shiftSpeed = shiftSpeed + gameSpeedUp; }
            else if (score == 40) { shiftSpeed = shiftSpeed + gameSpeedUp; }
            else if (score == 80) { shiftSpeed = shiftSpeed + gameSpeedUp; }
            else if (score == 150) { shiftSpeed = shiftSpeed + gameSpeedUp; }
            else if (score == 200) { shiftSpeed = shiftSpeed + gameSpeedUp; }
            Spawn();
            spawnTimer = 0;
        }
        //call shift to move object
        Shift();
    }
    //spawning pool objects
    void Configure() {
        //set target aspect and initialise empty arrays
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        inGameObjects = new GameObject[poolSize];
        //fill array
        GameObject[] prefabs = { Prefab1, Prefab2, Prefab3, Prefab4, Prefab5, Prefab6};
        //for loop to fill the other arrays
        for (int i = 0; i < poolObjects.Length; i++) {
            //get random gameobject and make if visible
            int RandomOption = Random.Range(0, prefabs.Length);
            GameObject gameGo = Instantiate(prefabs[RandomOption]) as GameObject;
            gameGo.SetActive(visible);
            inGameObjects[i] = gameGo;//save game object to be destroyed for new game
            //set other properties of object
            Transform t = gameGo.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }
        //spawn immediately
        if (spawnImmediate) {
            SpawnImmediate();
        }
    }
    //spawn method
    void Spawn() {
        Transform t = GetPoolObject();
        if (t == null) { return; }//if true this indicates that poolSize is too small
        Vector3 position = Vector3.zero;
        position.y = Random.Range(ySpawnRange.miny, ySpawnRange.maxy);
        position.x = (defaultSpawnPosition.x * Camera.main.aspect) / targetAspect;
        position.z = 90;
        t.position = position;
    }

    void SpawnImmediate() {
        Transform t = GetPoolObject();
        if (t == null) { return; }//if true this indicates that poolSize is too small
        Vector3 position = Vector3.zero;
        position.y = Random.Range(ySpawnRange.miny, ySpawnRange.maxy);
        position.x = (immediateSpawnPosition.x * Camera.main.aspect) / targetAspect;
        position.z = 90;
        t.position = position;
        Spawn();
    }
    //move object
    void Shift() {
        for (int i = 0; i < poolObjects.Length; i++) {
            poolObjects[i].transform.localPosition += (-Vector3.right * shiftSpeed * Time.deltaTime);
            CheckDisposeObject(poolObjects[i]);
        }
    }
    //check if object is avalible to use
    void CheckDisposeObject(PoolObject poolObject) {
        if (poolObject.transform.position.x < (-defaultSpawnPosition.x * Camera.main.aspect) / targetAspect) {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }
    //turn object to in use
    Transform GetPoolObject() {
        for (int i = 0; i < poolObjects.Length; i++) {
            if (!poolObjects[i].objInUse) {
                poolObjects[i].use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
