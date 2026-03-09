using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class machineUraniumElement : machineElement
{
    public machineUraniumElement() : base()
    {
    }

    public machineUraniumElement(machineData data)
        : base(data)
    {
    }
    protected override string getLogoPath()
    {
        return "logos/uranium";
    }

    protected override Color getColor()
    {
        return Utility.Hex("00FF0E");
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
        Texture2D texture = Resources.Load<Texture2D>("logos/uranium/" + data.machineName);
        VE_logo.style.backgroundImage = new StyleBackground(texture);

    }

    protected override void LauncherMarker()
    {
        Vector2 panelPos = new Vector2(VE_logo.worldBound.position.x, VE_logo.worldBound.position.y * 0.95f);
        MarkersUI.Instance.ShowMarker(panelPos, "+" + CalculReward(), MarkerType.Uranium, fontFactor: 0.7f);
    }
}
