using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class machineUraniumElement : machineElement
{
    public machineUraniumElement() : base()
    {
    }

    public machineUraniumElement(string machineName, BigNumber initPrice)
        : base(machineName, initPrice)
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
        return Ship.Current.uranium.isBigger(price);
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

    protected override void LauncherMarker()
    {
        Vector2 panelPos = new Vector2(VE_logo.worldBound.position.x, VE_logo.worldBound.position.y * 0.95f);
        MarkersUI.Instance.ShowMarker(panelPos, "+" + CalculReward(), MarkerType.Uranium, fontFactor: 0.7f);
    }
}
