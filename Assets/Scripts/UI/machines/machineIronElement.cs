using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Machine;
using static UnityEngine.Android.AndroidGame;


public class machineIronElement : machineElement
{
    public machineIronElement() : base()
    {
        // logique spécifique à SuperMachine (optionnel)
    }

    public machineIronElement(string machineName, BigNumber initPrice, float time)
        : base(machineName, initPrice, time)
    {
        // logique spécifique à SuperMachine
    }
    protected override string getLogoPath()
    {
        return "logos/iron";
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
}
