using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Android.AndroidGame;


public class machineIronElement : machineElement
{
    public machineIronElement() : base()
    {
    }

    public machineIronElement(string machineName, BigNumber initPrice, float time)
        : base(machineName, initPrice, time)
    {
    }
    protected override string getLogoPath()
    {
        return "iron";
    }

    protected override void HandleMoney(BigNumber amount)
    {
        Stats.Instance.AddIron(amount);
    }

    protected override bool canBuy(BigNumber price)
    {
        return Stats.Instance.iron.isBigger(price);
    }

    protected override void reloadUI()
    {
        MainUi.Instance.ironUI.loadForgeUI();
    }

    protected override void SetLogo()
    {
        Texture2D texture = Resources.Load<Texture2D>("logos/iron/" + machineName);
        VE_logo.style.backgroundImage = new StyleBackground(texture);
    }
}
