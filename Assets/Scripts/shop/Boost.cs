using System;
using UnityEngine;
using UnityEngine.UIElements;



public class Boost
{
    public ShopUI shopUI;

    private VisualElement boost;
    private Button buy;
    private Label priceLabel;

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
        priceLabel = boost.Q<Label>("diamand");

        priceLabel.text = price.ToString();
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
            }
            if(type == Type.xp)
            {
                Stats.Instance.xpBoostTime = time * 3600;
            }
            if(type == Type.pvShield)
            {
                Stats.Instance.pvShieldBoostTime = time * 3600;
                Stats.Instance.life = spaceShip.instance.getMaxLife();
                Stats.Instance.shield = spaceShip.instance.getMaxShield();
            }
            if(type == Type.ressources)
            {
                Stats.Instance.ressourcesBoostTime = time * 3600;
            }

            Stats.Instance.upDiamand(price, false);
        }

    }
}
