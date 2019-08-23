using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class GameAdManager : MonoBehaviour
{
    private BannerView bannerView;
    private RewardBasedVideoAd rewardMoneyVideoAd;

    private string myAppId = "xxx";

    private string bannerId = "xxx";
    private string rewardMoneyId = "xxx";

    private bool isRewarded = false;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();

        this.RequestRewardMoneyVideoAd();

        if (PlayerPrefs.GetInt("RemoveAds") == 0)
            this.RequestBanner();
    }

    private void Update()
    {
        if (isRewarded)
        {
            PlayerPrefs.SetInt("RemoveAds", 1);

            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

            gameLogic.GetMoney(800);
            isRewarded = false;
        }
    }

    private void OnDisable()
    {
        this.bannerView.Destroy();
    }

    //-------------------------------------------------------------------------------------------

    public void ShowRewardMoneyVideoAd()
    {
        if (this.rewardMoneyVideoAd.IsLoaded())
        {
            this.rewardMoneyVideoAd.Show();
        }
        else
            Debug.Log("No Net Connection!");
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

    private void RequestRewardMoneyVideoAd()
    {
        this.rewardMoneyVideoAd = RewardBasedVideoAd.Instance;

        this.rewardMoneyVideoAd.OnAdFailedToLoad += HandleRewardMoneyVideoAdOnAdFailedToLoad;
        this.rewardMoneyVideoAd.OnAdClosed += HandleRewardMoneyBasedVideoClosed;
        this.rewardMoneyVideoAd.OnAdRewarded += HandleRewardMoneyBasedVideoRewarded;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardMoneyVideoAd.LoadAd(request, rewardMoneyId);
    }

    //-------------------------------------------------------------------------------------------

    public void HandleRewardMoneyVideoAdOnAdFailedToLoad(object sender, EventArgs args)
    {
        this.RequestRewardMoneyVideoAd();
    }

    public void HandleRewardMoneyBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardMoneyVideoAd();
    }

    public void HandleRewardMoneyBasedVideoRewarded(object sender, Reward args)
    {
        isRewarded = true;
    }
}
