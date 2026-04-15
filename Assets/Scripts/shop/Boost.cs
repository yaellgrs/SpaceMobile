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
    private Label Lbl_coef;
    //private Label Lbl_price;

    public int time; // in hour
    public int price; // in diamand
    public string name;

    public enum Type { time, damage, xp, pvShield, ressources };
    public Type type;

    public void load(UIDocument ShopDocument)
    {
        var root = ShopDocument.rootVisualElement;

        boost = root.Q<VisualElement>(name);
        buy = boost.Q<Button>("buy");
        //Lbl_price = boost.Q<Label>("diamand");
        Lbl_time = boost.Q<Label>("time");
        Lbl_coef = boost.Q<Label>("coef");

        if (type != Type.time) loadBonusActive();

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
            Lbl_time.style.display = DisplayStyle.None;
            Lbl_coef.style.display = DisplayStyle.None;
        }

        //Lbl_price.text = price.ToString();
    }

    private bool checkActive()
    {

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
                if(Ship.Current.level < 12)
                {
                    shopUI.Close();
                    MainUi.Instance.offlineUI.showErrorMessage = true;
                    MainUi.Instance.offlineUI.Load();
                    return;
                }
                else
                {
                    shopUI.Close();
                    Stats.Instance.lastConnection = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (time * 3600);
                    MainUi.Instance.offlineUI.Load();
                }
            }
            //if(type == Type.damage)
            //{
            //    Stats.Instance.damageBoostTime = time * 3600;
            //    loadBonusActive();
            //}
            //if(type == Type.xp)
            //{
            //    Stats.Instance.xpBoostTime = time * 3600;
            //    loadBonusActive();
            //}
            //if(type == Type.pvShield)
            //{
            //    Stats.Instance.pvShieldBoostTime = time * 3600;
            //    Ship.Current.life = spaceShip.instance.getMaxLife();
            //    Ship.Current.shield = spaceShip.instance.getMaxShield();
            //    loadBonusActive();
            //}
            //if(type == Type.ressources)
            //{
            //    Stats.Instance.ressourcesBoostTime = time * 3600;
            //    loadBonusActive();
            //}
            //Stats.Instance.AddDiamand(-price);
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
        if (type == Type.ressources)
            Ads.Instance.ShowRewardedAd(Ads.RewardType.BoostRessource);
    }

    public bool CanPay()
    {
        return Stats.Instance.boosts[type].coef != 2f || (Stats.Instance.boosts[type].coef == 2f && Stats.Instance.boosts[type].time <= 0f);

/*        if (type == Type.damage)



        if (type == Type.xp)
            return Stats.Instance.xpBoostTime <= 0;
        if (type == Type.pvShield)
            return Stats.Instance.pvShieldBoostTime <= 0;
        if (type == Type.ressources)
            return Stats.Instance.ressourcesBoostTime <= 0;

        return false;*/
    }   //oui si 1f ( 0f ) so 1.5f( xf ) ou 2f( 0f)
}
