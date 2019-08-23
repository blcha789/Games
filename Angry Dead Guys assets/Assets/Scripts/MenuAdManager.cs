using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;

public class MenuAdManager : MonoBehaviour
{
    public bool isPreLoadScene = false;

    private BannerView bannerView;
    private RewardBasedVideoAd rewardFuelVideoAd;
    private RewardBasedVideoAd removeAds;

    private string testAppId = "ca-app-pub-3940256099942544~3347511713";
    private string myAppId = "ca-app-pub-8372432902600459~7932228748";

    private string testBannerId = "ca-app-pub-3940256099942544/6300978111";
    private string testRewardVideoId = "ca-app-pub-3940256099942544/5224354917";

    private string bannerId = "ca-app-pub-8372432902600459/7200143108";
    private string rewardFuelId = "ca-app-pub-8372432902600459/6427575384";

    private bool isRewarded = false;

    public MainMenu menuLogic;

    private void Start()
    {
        if (isPreLoadScene)
            MobileAds.Initialize(myAppId);

        if (!isPreLoadScene)
        {      
            this.RequestRewardFuelVideoAd();

            if(PlayerPrefs.GetInt("RemoveAds") == 0)
                this.RequestBanner();
        }
    }

    private void Update()
    {
        if(isRewarded)
        {
            PlayerPrefs.SetInt("RemoveAds", 1);

            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

            menuLogic.RefreshFuelCanister(500);
            isRewarded = false;
        }
    }

    private void OnDisable()
    {
        this.bannerView.Destroy();
    }

    //-------------------------------------------------------------------------------------------

    public void ShowRewardFuelVideoAd() // button
    {
        if (this.rewardFuelVideoAd.IsLoaded())
        {
            this.rewardFuelVideoAd.Show();
        }
    }

    //-------------------------------------------------------------------------------------------

    private void RequestBanner()
    {
        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the bottom of the screen.
        this.bannerView = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();

        // Load a banner ad.
        this.bannerView.LoadAd(request);
    }

    private void RequestRewardFuelVideoAd()
    {
        this.rewardFuelVideoAd = RewardBasedVideoAd.Instance;

        this.rewardFuelVideoAd.OnAdFailedToLoad += HandleRewardFuelVideoAdOnAdFailedToLoad;
        this.rewardFuelVideoAd.OnAdRewarded += HandleRewardFuelBasedVideoRewarded;
        this.rewardFuelVideoAd.OnAdClosed += HandleRewardFuelBasedVideoClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardFuelVideoAd.LoadAd(request, rewardFuelId);
    }

    //-------------------------------------------------------------------------------------------

    public void HandleRewardFuelVideoAdOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        this.RequestRewardFuelVideoAd();
    }

    public void HandleRewardFuelBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardFuelVideoAd();
    }

    public void HandleRewardFuelBasedVideoRewarded(object sender, Reward args)
    {
        isRewarded = true;
    }
}
