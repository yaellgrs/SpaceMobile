using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.UIElements;



public class Boost
{
    public ShopUI shopUI;

    private VisualElement boost;
    private Button buy;
    private Label Lbl_time;
    private Label Lbl_diamand;
    private Label Lbl_coef;
    //private Label Lbl_price;

    public int time; // in hour
    public int price; // in diamand
    public string name;

    public enum Type { time, damage, xp, pvShield, prestige, ressources };
    public Type type;

    public void load(UIDocument ShopDocument)
    {
        var root = ShopDocument.rootVisualElement;

        boost = root.Q<VisualElement>(name);
        buy = boost.Q<Button>("buy");
        //Lbl_price = boost.Q<Label>("diamand");
        Lbl_time = boost.Q<Label>("time");
        Lbl_diamand = boost.Q<Label>("diamand");
        Lbl_coef = boost.Q<Label>("coef");

        if (type == Type.time) Lbl_diamand.text = price.ToString();

        loadBonusActive();
  
        buy.clicked += Buy;

        buy.SetEnabled(CanPay());
    }

    private void loadBonusActive()
    {
        if (checkActive())
        {
            Utility.setBorderColor(boost, Color.green);
            Lbl_time.style.display = DisplayStyle.Flex;
            Lbl_time.text = getTime();
            Lbl_coef.style.display = DisplayStyle.Flex;
            Lbl_coef.text = "x" + Stats.Instance.boosts[type].coef.ToString("F1");
        }
        else
        {
           
            Utility.setBorderColor(boost, Color.white);
            if(Lbl_time != null) Lbl_time.style.display = DisplayStyle.None;
            if(Lbl_coef != null ) Lbl_coef.style.display = DisplayStyle.None;
        }

        //Lbl_price.text = price.ToString();
    }

    private bool checkActive()
    {
        if(type == Type.time) return false;
        return Stats.Instance.boosts[type].time > 0f; 
    }

    private string getTime()
    {
        return Utility.TimeToString_hm((long)Stats.Instance.boosts[type].time);
    }

    private void Buy()
    {
        if(CanPay())
        {
            if (type == Type.time)
            {
                    shopUI.Close();
                    Stats.Instance.lastConnection = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (time * 3600);
                    MainUi.Instance.offlineUI.Load(false);
                
            }
            Pay();
        }

    }

    public void Pay()
    { 
        if (type == Type.damage)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostDamage);
        if (type == Type.xp)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostXp);
        if (type == Type.pvShield)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostLife);
        if (type == Type.prestige)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostPrestige);
        if (type == Type.ressources)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostRessource);
        if (type == Type.time)
            Stats.Instance.AddDiamand(-price);
    }

    public bool CanPay()
    {
        if(type == Type.time) return price <= Stats.Instance.diamand;
        return Stats.Instance.boosts[type].coef != 2f || (Stats.Instance.boosts[type].coef == 2f && Stats.Instance.boosts[type].time <= 0f);

    }   //oui si 1f ( 0f ) so 1.5f( xf ) ou 2f( 0f)
}
