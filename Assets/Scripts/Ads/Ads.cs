using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour
{
    private const string _RewardAdUnitId = "ca-app-pub-2287437722164523/1941909652";
    //private const string _BannerAdUnitId = "ca-app-pub-2287437722164523/3942107179";

    private BannerView bannerView;

    public enum RewardType { Diamand, Ressources, Resurection, BoostDamage, BoostLife, BoostXp, BoostRessource, BoostPrestige, None };

    public static Ads Instance;

    private RewardedAd _rewardedAd;

    private int tentative = 0;
    private bool videoShowed = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            CreateBanner();
        });
    }

    public float getBannerHeight()
    {
        if (bannerView != null)
            return ( bannerView.GetHeightInPixels()/ Screen.height) * 50f; ;
        return 0;
    }

    public void CreateBanner()
    {
        if (bannerView != null) return;

        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(
            AdSize.FullWidth
        );

        // Création avec position temporaire (0,0)
//        bannerView = new BannerView(_BannerAdUnitId, adSize, AdPosition.Bottom);
//#if UNITY_EDITOR
//        bannerView = new BannerView(_BannerAdUnitId, adSize, AdPosition.Bottom);
//#else
//    bannerView = new BannerView(_BannerAdUnitId, adSize, 0, 0);
//#endif

    }


    public void ShowBanner(bool show)
    {
        if (show)
        {
            bannerView?.Show();
            MainUi.Instance.adaptBanner(true);
        }
        else
        {
            bannerView?.Hide();
            MainUi.Instance.adaptBanner(false);
        }
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(_RewardAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Ad failed to load: " + error);
                return;
            }
            _rewardedAd = ad;
            Debug.Log("Rewarded ad loaded");

            _rewardedAd.OnAdFullScreenContentClosed += () => LoadRewardedAd();
            _rewardedAd.OnAdFullScreenContentFailed += (AdError err) => LoadRewardedAd();
        });
    }

    public void ShowRewardedAd(RewardType type)
    {
        if( tentative == 0) videoShowed = false;    

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {

            if (IAPManager.Instance.CheckAds())
                GetReward(type);
            else
            {
                _rewardedAd.Show((Reward reward) =>
                {
                    GetReward(type);
                });
            }
            tentative = 0;
        }
        else if(tentative < 10)
        {
            tentative++;
            StartCoroutine(ReloadAd(type, 0.5f));
            LoadAdsUI.Instance.Open();

        }
        else
        {
            // no found
            LoadAdsUI.Instance.SetError();
            videoShowed = true;
            tentative = 0;
        }
    }

    public void GetAdsReward()
    {

    }

    private IEnumerator ReloadAd(RewardType type, float delay)
    {
        LoadRewardedAd();
        yield return new WaitForSeconds(delay); // laisse le temps de charger
        if(!videoShowed) ShowRewardedAd(type);

    }

    public void GetReward(RewardType type)
    {
        switch (type)
        {
            case RewardType.Diamand:
                Stats.Instance.lastPub = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                MainUi.Instance.adsUI.Close();
                Stats.Instance.AddDiamand(5);
                break;
            case RewardType.Ressources:
                Stats.Instance.lastPub = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                Stats.Instance.AddIron(getIronAdsReward());
                if(Ship.Current.HaveUranium()) Stats.Instance.AddUranium(getUraniumAdsReward());
                MainUi.Instance.adsUI.Close();
                break;
            case RewardType.Resurection:
                Stats.Instance.ReduceLifeBoss = true;
                Stats.Instance.deadPubWatch++;
                ResurectionUI.Instance.Resurection();
                break;
            case RewardType.BoostDamage:
                setBoost(Boost.Type.damage);
                MainUi.Instance.shopUI.LoadBoost();
                break;
            case RewardType.BoostXp:
                setBoost(Boost.Type.xp);
                MainUi.Instance.shopUI.LoadBoost();
                break;
            case RewardType.BoostPrestige:
                setBoost(Boost.Type.prestige);
                MainUi.Instance.shopUI.LoadBoost();
                break;
            case RewardType.BoostLife:
                setBoost(Boost.Type.pvShield);
                MainUi.Instance.shopUI.LoadBoost();
                break;
            case RewardType.BoostRessource:
                setBoost(Boost.Type.ressources);
                MainUi.Instance.shopUI.LoadBoost();
                break;
        }

        videoShowed = true;
        LoadAdsUI.Instance.Close();
    }

    public void setBoost(Boost.Type type)
    {
        float coef = 1f;
        if (Stats.Instance.boosts[type].time <= 0f) coef = 1.5f;
        else coef = Stats.Instance.boosts[type].coef == 1f ? 1.5f : 2f; 
        Stats.Instance.boosts[type] = (coef, 3600);
    }


    public static BigNumber getIronAdsReward() => OfflineUI.calculOfflineIronEarn(150, false);
    public static BigNumber getUraniumAdsReward() => OfflineUI.calculOfflineUraniumEarn(150, false);
}