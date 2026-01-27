using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Machine;
using static UnityEngine.Android.AndroidGame;


public class machineUraniumElement : machineElement
{
    public machineUraniumElement() : base()
    {
    }

    public machineUraniumElement(string machineName, BigNumber initPrice, float time)
        : base(machineName, initPrice, time)
    {
    }
    protected override string getLogoPath()
    {
        return "uranium";
    }

    protected override void HandleMoney(BigNumber amount)
    {
        Stats.Instance.AddUranium(amount);
    }

    protected override bool canBuy(BigNumber price)
    {
        return Stats.Instance.uranium.isBigger(price);
    }

    protected override void reloadUI()
    {
        MainUi.Instance.uraniumUI.loadForgeUI();
    }

    protected override void SetLogo()
    {
        Texture2D texture = Resources.Load<Texture2D>("logos/uranium/" + machineName);
        VE_logo.style.backgroundImage = new StyleBackground(texture);

    }
}
