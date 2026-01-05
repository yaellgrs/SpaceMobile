using System.Collections.Generic;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine;

public class Ads : MonoBehaviour
{
    private const string _RewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private const string _BannerAdUnitId = "ca-app-pub-2287437722164523/3942107179";

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
            CreatBanner();
        });
    }

    public void CreatBanner()
    {
        if (bannerView != null)
            return;

        bannerView = new BannerView(
            _BannerAdUnitId,
            AdSize.Banner,
            AdPosition.Bottom
        );

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);

        ShowBanner();
    }

    public void ShowBanner()
    {
        bannerView?.Show();
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
                Stats.Instance.upDiamand(5, true);
                break;
            case RewardType.Ressources:
                Stats.Instance.upIron(getIronAdsReward(), true);
                Stats.Instance.upUranium(getUraniumAdsReward(), true);
                break;
            case RewardType.Resurection:
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