using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class CountDownText : MonoBehaviour {

    public delegate void CountDownFinished();
    public static event CountDownFinished OnCountDownFinished;
    public AudioSource CountDownAudio;
    public AudioSource StartAudio;
    Text countDown;

    void OnEnable() {
        CountDownAudio.Play();
        countDown = GetComponent<Text>();
        countDown.text = "3";
        StartCoroutine("CountDown");
    }

    IEnumerator CountDown() {
        int count = 3;
        for (int i = 0; i < count; i++) {
            countDown.text = (count - i).ToString();
            CountDownAudio.Play();
            yield return new WaitForSeconds(1);
        }

        countDown.text = "SWIM!";
        StartAudio.Play();
        yield return new WaitForSeconds(1);
        OnCountDownFinished();
    }
}
