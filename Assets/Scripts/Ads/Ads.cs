using GoogleMobileAds;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour
{
    /*
    private const string _RewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private const string _BannerAdUnitId = "ca-app-pub-2287437722164523/3942107179";*/

    private const string _RewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private const string _BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

    //ca-app-pub-2287437722164523~4568072994
    //ca-app-pub-2287437722164523/3942107179
    //ca-app-pub-2287437722164523/8457751465

    private BannerView bannerView;


    public enum RewardType { Diamand, Ressources, Resurection ,None};

    public static Ads Instance;

    private RewardedAd _rewardedAd;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialisation de l'AdMob SDK
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            CreateBanner();
        });
    }

    public void CreateBanner()
    {
        /*
        if (bannerView != null)
            return;

        int bannerWidth = Screen.width;

        // Optionnel : limiter à une largeur max raisonnable pour le SDK
        bannerWidth = Mathf.Min(bannerWidth, 10);


        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(bannerWidth);

        bannerView = new BannerView(_BannerAdUnitId, adSize, AdPosition.Bottom);

        AdRequest request = new AdRequest();



        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("✅ Banner loaded");
            if (Settings.Instance.showBanner)
                bannerView.Show();
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("❌ Banner failed to load: " + error);
        };

        bannerView.LoadAd(request);

        ShowBannerDelayed(1f);*/
    }

    private IEnumerator ShowBannerDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBanner(true);
    }

    public void ShowBanner(bool show)
    {
        /*
        if (show)
        {
            bannerView?.Show();
            MainUi.Instance.adaptBanner(true);
        }
        else
        {
            bannerView?.Hide();
            MainUi.Instance.adaptBanner(false);
        }*/
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }


    public void LoadRewardedAd()
    {
        // Détruit l'ancienne pub si elle existe
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
                return;
            }
            _rewardedAd = ad;

            // Abonnements aux événements utiles
            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadRewardedAd(); // Recharge pour plus tard
            };
        });
    }



    public void ShowRewardedAd(RewardType type)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // Ajoute ici la récompense pour le joueur
                GetReward(type);
            });
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
                break;
        }
    }

    public static BigNumber getIronAdsReward()
    {
        return OfflineUI.calculOfflineIronEarn(150, false);
    }

    public static BigNumber getUraniumAdsReward()
    {
         return OfflineUI.calculOfflineUraniumEarn(150, false);
    }
}