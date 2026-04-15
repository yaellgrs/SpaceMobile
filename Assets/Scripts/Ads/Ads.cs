using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour
{
    private const string _RewardAdUnitId = "ca-app-pub-2287437722164523/1941909652";
    private const string _BannerAdUnitId = "ca-app-pub-2287437722164523/3942107179";

    private BannerView bannerView;

    public enum RewardType { Diamand, Ressources, Resurection, BoostDamage, BoostLife, BoostXp, BoostRessource, BoostPrestige, None };

    public static Ads Instance;

    private RewardedAd _rewardedAd;

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
        bannerView = new BannerView(_BannerAdUnitId, adSize, AdPosition.Bottom);
#if UNITY_EDITOR
        bannerView = new BannerView(_BannerAdUnitId, adSize, AdPosition.Bottom);
#else
    bannerView = new BannerView(_BannerAdUnitId, adSize, 0, 0);
#endif

        AdRequest request = new AdRequest();

        bannerView.OnBannerAdLoaded += () =>
        {
        #if !UNITY_EDITOR
                    int yPos = GetBannerYPositionDp();
                    bannerView.SetPosition(0, yPos);
        #endif
            if (Settings.Instance.showBanner)
            {
                bannerView.Show();
                MainUi.Instance.adaptBanner(true);
                BottomUI.Instance.AdaptBanner(true);
            }
            else
            {
                bannerView.Hide();
            }
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner failed to load: " + error);
        };

        bannerView.LoadAd(request);
    }

    private int GetNavBarHeightDp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var resources = activity.Call<AndroidJavaObject>("getResources"))
            using (var metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
            {
                float density = metrics.Get<float>("density");
                int resourceId = resources.Call<int>("getIdentifier",
                    "navigation_bar_height", "dimen", "android");

                if (resourceId > 0)
                {
                    int heightPx = resources.Call<int>("getDimensionPixelSize", resourceId);
                    return Mathf.RoundToInt(heightPx / density);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("GetNavBarHeightDp error: " + e);
        }
#endif
        return 0;
    }

    private int GetBannerYPositionDp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var resources = activity.Call<AndroidJavaObject>("getResources"))
            using (var metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
            {
                float density = metrics.Get<float>("density");

                int screenHeightPx = metrics.Get<int>("heightPixels");
                int screenHeightDp = Mathf.RoundToInt(screenHeightPx / density);

                int bannerHeightDp = Mathf.RoundToInt(bannerView.GetHeightInPixels() / density);

                int navBarDp = GetNavBarHeightDp();

                int yPos = screenHeightDp - bannerHeightDp - navBarDp;
                Debug.Log($"Banner Y = {yPos}dp (screen={screenHeightDp}, banner={bannerHeightDp}, nav={navBarDp})");
                return yPos;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("GetBannerYPositionDp error: " + e);
        }
#else
        // Fallback éditeur : simule une nav bar de 48dp
        int screenHeightDp = Mathf.RoundToInt(Screen.height / (Screen.dpi / 160f));
        int bannerHeightDp = Mathf.RoundToInt(bannerView.GetHeightInPixels() / (Screen.dpi / 160f));
        return screenHeightDp - bannerHeightDp - 48;
#endif

        return 0;
    }

    private IEnumerator ShowBannerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBanner(Settings.Instance.showBanner);
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
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) => GetReward(type));
        }
        else
        {
            Debug.Log("Ad not ready");
        }
    }

    public void GetReward(RewardType type)
    {
        switch (type)
        {
            case RewardType.Diamand:
                Stats.Instance.AddDiamand(5);
                break;
            case RewardType.Ressources:
                Stats.Instance.AddIron(getIronAdsReward());
                Stats.Instance.AddUranium(getUraniumAdsReward());
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
            case RewardType.BoostLife:
                setBoost(Boost.Type.pvShield);
                MainUi.Instance.shopUI.LoadBoost();
                break;
            case RewardType.BoostRessource:
                setBoost(Boost.Type.ressources);
                MainUi.Instance.shopUI.LoadBoost();
                break;
        }
    }

    public void setBoost(Boost.Type type)
    {
        Debug.Log("before boost : " + type + "time : " + Stats.Instance.boosts[type].time + " coef : " + Stats.Instance.boosts[type].coef);
        float coef = 1f;
        if (Stats.Instance.boosts[type].time <= 0f) coef = 1.5f;
        else coef = Stats.Instance.boosts[type].coef == 1f ? 1.5f : 2f; 
        Stats.Instance.boosts[type] = (coef, 3600);
        Debug.Log("after boost : " + type + "time : " + Stats.Instance.boosts[type].time + " coef : " + Stats.Instance.boosts[type].coef);
    }


    public static BigNumber getIronAdsReward() => OfflineUI.calculOfflineIronEarn(150, false);
    public static BigNumber getUraniumAdsReward() => OfflineUI.calculOfflineUraniumEarn(150, false);
}