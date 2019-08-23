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

    private string myAppId = "xxx";

    private string bannerId = "xxx";
    private string rewardFuelId = "xxx";

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
