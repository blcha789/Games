using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    [Header("Main")]
    public bool initializeOnLoadOnce = false;//true only on PreLoadScene

    private BannerView bannerView;
    private RewardBasedVideoAd rewardVideoAd;

    private string bannerId = "xxx";
    private string rewardVideoId = "xxx";

    private string appId = "xxx";

    private bool isRewarded = false;

    public Text wrenchText;

    private void Start()
    {
        if (initializeOnLoadOnce)
        {
            MobileAds.Initialize(appId);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            this.RequestRewardVideoAd();

            if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            {
                this.RequestBanner();
                ShowBanner();
            }
            else
            {
                if (this.bannerView != null)
                {
                    this.bannerView.Destroy();
                }
            }
        }
    }

    private void Update()
    {
        if (isRewarded)
        {
            PlayerPrefs.SetInt("AdsRemoved", 1);

            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

            int i = PlayerPrefs.GetInt("Wrench");
            i = i + 10;
            PlayerPrefs.SetInt("Wrench", i);

            wrenchText.text = PlayerPrefs.GetInt("Wrench").ToString();
            isRewarded = false;
        }
    }

    private void OnDisable()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }


    private void RequestBanner()
    {
        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        this.bannerView = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();

        this.bannerView.LoadAd(request);
    }

    private void RequestRewardVideoAd()
    {
        this.rewardVideoAd = RewardBasedVideoAd.Instance;

        this.rewardVideoAd.OnAdFailedToLoad += HandleRewardVideoAdOnAdFailedToLoad;
        this.rewardVideoAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
        this.rewardVideoAd.OnAdClosed += HandleRewardBasedVideoClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardVideoAd.LoadAd(request, rewardVideoId);
    }

    //-------------------------------------------------------------------------------------------

    public void ShowBanner()
    {
        this.bannerView.Show();
    }

    public void ShowRewardVideoAd()
    {
        if(this.rewardVideoAd.IsLoaded())
        {
            this.rewardVideoAd.Show();
        }
    }

    //-------------------------------------------------------------------------------------------
    
    //banner handlers
    public void HandleBannerAdOnAdLoaded(object sender, EventArgs args)
    {
        if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            ShowBanner();
    }

    public void HandleBannerAdOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        if (PlayerPrefs.GetInt("AdsRemoved") == 0)
            RequestBanner();
    }

    //RemoveAds Reward video handlers
    public void HandleRewardVideoAdOnAdFailedToLoad(object sender, EventArgs args)
    {
        this.RequestRewardVideoAd();
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        this.RequestRewardVideoAd();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        isRewarded = true;
    }
}
