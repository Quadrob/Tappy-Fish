using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobScript : MonoBehaviour {

    string AppID = "ca-app-pub-8756328026300253~1952961562";//admob game ID
    string BannerAdID = "ca-app-pub-3940256099942544/6300978111";//test ID must get mine
    string InterstitialAdID = "ca-app-pub-3940256099942544/1033173712";//test ID must get mine

    private BannerView BannerView;
    private InterstitialAd interstitialAd;

    // Start is called before the first frame update
    void Start() {

        // Initialize the Google Mobile Ads SDK.

        //MobileAds.Initialize(initStatus => { });//this was suggested from google
        MobileAds.Initialize(AppID);//this was suggested by youtube

    }

    public void ShowBannerAd() {
        // Create a 320x50 banner at the top of the screen.
        this.BannerView = new BannerView(BannerAdID, AdSize.SmartBanner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.BannerView.LoadAd(request);
    }

    public void RemoveBannerAd() {
        //destroy the banner ad
        this.BannerView.Destroy();
    }

    public void LoadInterstitialAd() {
        // Initialize an InterstitialAd.
        this.interstitialAd = new InterstitialAd(InterstitialAdID);

        // Create an empty ad request.
        AdRequest requestInter = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        this.interstitialAd.LoadAd(requestInter);
    }

    public void ShowInterstitialAd() {

        if (this.interstitialAd.IsLoaded()) {
            this.interstitialAd.Show();
        }
    }
}
