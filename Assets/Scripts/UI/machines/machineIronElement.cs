using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Android.AndroidGame;


public class machineIronElement : machineElement
{
    public machineIronElement() : base()
    {
    }

    public machineIronElement(machineData data)
        : base(data)
    {
    }

    protected override void Init()
    {
        base.Init();
        Stats.Instance.OnIronChanged -= SetLevelUpButton;
        Stats.Instance.OnIronChanged += SetLevelUpButton;
    }

    protected override string getLogoPath()
    {
        return Utility.GetMainRessourceLogoPath();
    }

    protected override Color getColor()
    {
        return Utility.GetMainRessourceColor();
    }

    protected override void HandleMoney(BigNumber amount)
    {
        Stats.Instance.AddIron(amount);
    }

    protected override bool canBuy(BigNumber price)
    {
        return Ship.Current.iron.isBigger(price);
    }

    protected override void reloadUI()
    {
        MainUi.Instance.ironUI.loadForgeUI();
    }

    protected override void SetLogo()
    {
        Texture2D texture = Resources.Load<Texture2D>("logos/iron/" + data.machineName);
        VE_logo.style.backgroundImage = new StyleBackground(texture);
    }

    protected override void LauncherMarker()
    {
        Vector2 panelPos = new Vector2(VE_logo.worldBound.position.x, VE_logo.worldBound.position.y *0.95f);
        MarkersUI.Instance.ShowMarker(panelPos, "+" + CalculReward(), MarkerType.Iron, fontFactor : 0.7f);
    }
}
