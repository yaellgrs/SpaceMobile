using System;
using UnityEngine;
using UnityEngine.UIElements;



public class Boost
{
    public ShopUI shopUI;

    private VisualElement boost;
    private Button buy;
    private Label Lbl_time;
    private Label Lbl_price;

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
        Lbl_price = boost.Q<Label>("diamand");
        Lbl_time = boost.Q<Label>("time");

        if (type != Type.time) loadBonusActive();

        buy.clicked += Buy;
        if (Stats.Instance.diamand < price)
        {
            buy.SetEnabled(false);
        }
        else
        {
            buy.SetEnabled(true);
        }
    }

    private void loadBonusActive()
    {
        if (checkActive())
        {
            Utility.setBorderColor(boost, Color.green);
            Lbl_time.style.display = DisplayStyle.Flex;
            Lbl_time.text = getTime();
        }
        else
        {
            Utility.setBorderColor(boost, Color.white);
            Lbl_time.style.display = DisplayStyle.None;
        }

        Lbl_price.text = price.ToString();
    }

    private bool checkActive()
    {
        if (type == Type.damage) return (Stats.Instance.damageBoostTime > 0); // Stats.Instance.damageBoostTime
        if (type == Type.xp) return (Stats.Instance.xpBoostTime > 0); //Stats.Instance.xpBoostTime); 
        if (type == Type.pvShield) return (Stats.Instance.pvShieldBoostTime > 0); //Stats.Instance.pvShieldBoostTime
        if (type == Type.ressources) return (Stats.Instance.ressourcesBoostTime > 0); //Stats.Instance.ressourcesBoostTime
        return false;
    }

    private string getTime()
    {
        if (type == Type.damage) return Utility.TimeToString_hm((long)Stats.Instance.damageBoostTime);
        if (type == Type.xp) return Utility.TimeToString_hm((long)Stats.Instance.xpBoostTime);
        if (type == Type.pvShield) return Utility.TimeToString_hm((long)Stats.Instance.pvShieldBoostTime);
        if (type == Type.ressources) return Utility.TimeToString_hm((long)Stats.Instance.ressourcesBoostTime);
        return "";
    }

    private void Buy()
    {
        if(Stats.Instance.diamand >= price)
        {
            if (type == Type.time)
            {
                if(Stats.Instance.level < 12)
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
            if(type == Type.damage)
            {
                Stats.Instance.damageBoostTime = time * 3600;
                loadBonusActive();
            }
            if(type == Type.xp)
            {
                Stats.Instance.xpBoostTime = time * 3600;
                loadBonusActive();
            }
            if(type == Type.pvShield)
            {
                Stats.Instance.pvShieldBoostTime = time * 3600;
                Stats.Instance.life = spaceShip.instance.getMaxLife();
                Stats.Instance.shield = spaceShip.instance.getMaxShield();
                loadBonusActive();
            }
            if(type == Type.ressources)
            {
                Stats.Instance.ressourcesBoostTime = time * 3600;
                loadBonusActive();
            }

            Stats.Instance.AddDiamand(-price);
        }

    }
}
