using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
//import to use score text
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //initialise events
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    //get ad manager game object
    public GameObject AdManagerObject;

    //access other script
    private AdMobScript adMobScript;

    //game pages
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countDownPage;
    public Text scoreText;
    public Text currentScore;
    public AudioSource ButtonAudio;
    public AudioSource SpeedUpAudio;
    public AudioSource HeyAudio;

    //counter to show ads every second round
    public int counter = 0;

    //page states
    enum PageState {
        None, 
        Start, 
        GameOver, 
        Countdown
    }

    //score value and gameover
    int score = 0;
    bool gameOver = true;

    //get gameover value
    public bool GameOver { get { return gameOver; } }
    public int Score { get { return score; } }
    
    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            adMobScript = AdManagerObject.GetComponent<AdMobScript>();
            SetPageState(PageState.Start);
            HeyAudio.Play();
        }
    }

    void OnEnable() {
        CountDownText.OnCountDownFinished += OnCountDownFinished;
        TapControl.OnPlayerDied += OnPlayerDied;
        TapControl.OnPlayerScored += OnPlayerScored;
    }

    void OnDisable() {
        CountDownText.OnCountDownFinished -= OnCountDownFinished;
        TapControl.OnPlayerDied -= OnPlayerDied;
        TapControl.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountDownFinished() {
        SetPageState(PageState.None);
        score = 0;
        OnGameStarted();//event sent to tapcontrol
        gameOver = false;
    }

    void OnPlayerDied() {
        gameOver = true;
        currentScore.text = ("Score: " + score.ToString());
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
        adMobScript.LoadInterstitialAd();
    }

    void OnPlayerScored() {
        score++;
        scoreText.text = score.ToString();
        if (score == 10) { SpeedUpAudio.Play(); }
        else if (score == 20) { SpeedUpAudio.Play(); }
        else if (score == 40) { SpeedUpAudio.Play(); }
        else if (score == 80) { SpeedUpAudio.Play(); }
        else if (score == 150) { SpeedUpAudio.Play(); }
        else if (score == 200) { SpeedUpAudio.Play(); }

    }

    // switch page states
    void SetPageState(PageState state) {
        switch (state) {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                adMobScript.RemoveBannerAd();
                break;

            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(false);
                adMobScript.ShowBannerAd();

                break;

            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countDownPage.SetActive(false);
                break;

            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countDownPage.SetActive(true);
                break;
        }
    }

    public void AdCounter() {
        counter += 1;
        if (counter > 1) {
            counter = 0;
            adMobScript.ShowInterstitialAd();
        }
    }

    public void ConfirmGameOver() {
        //activated when restart button is hit
        ButtonAudio.Play();
        scoreText.text = "0";
        SetPageState(PageState.Start);
        OnGameOverConfirmed();//event sent to tapcontrol
        AdCounter();
    }

    public void StartGame() {
        ButtonAudio.Play(); 
        //activated when play button is hit
        SetPageState(PageState.Countdown);
    }

}
