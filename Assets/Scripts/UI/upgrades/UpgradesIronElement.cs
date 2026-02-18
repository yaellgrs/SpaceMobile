using UnityEngine;
using UnityEngine.UIElements;

public class UpgradesIronElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { Life, Damage, Shield, RegenShield }
    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesIronElement() : base()
    {
    }

    public UpgradesIronElement(string name, UpgradeType type) : base(name)
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    #region ----- overrides Methods -----

    protected override void Init()
    {
        base.Init();
        Stats.Instance.OnIronChanged -= SetLevelUpButton;
        Stats.Instance.OnIronChanged += SetLevelUpButton;
    }

    protected override void LoadStat()
    {
        switch (type)
        {
            case UpgradeType.Life:
                Lbl_description.text = "Life : " + Ship.Current.lifeMax;
                break;
            case UpgradeType.Damage:
                Lbl_description.text = "Damage : " + Ship.Current.damage.initial;
                break;
            case UpgradeType.Shield:
                Lbl_description.text = "Shield : " + Ship.Current.shieldMax;
                break;
            case UpgradeType.RegenShield:
                Lbl_description.text = "Regen Shield : " + Ship.Current.regenShield;
                break;
        }
        string logo_path = "Upgrades/Iron/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));
    }

    public override void GetReward()
    {
        switch (type)
        {
            case UpgradeType.Life:
                BigNumber diff = new BigNumber(spaceShip.instance.getMaxLife());
                diff.Subtract(Ship.Current.life);

                Ship.Current.lifeMax.initial.Set(10);
                Ship.Current.lifeMax.initial *= 0.5f * Mathf.Pow(level + 1, 1.6f);

                Ship.Current.life.Set(spaceShip.instance.getMaxLife());
                Ship.Current.life.Subtract(diff);
                MainUi.Instance.upHealthBar();
                break;
            case UpgradeType.Damage:
                Ship.Current.damage.initial.Set(1);
                Ship.Current.damage.initial *= Mathf.Pow(1.3f, level);
                Ship.Current.damage.initial += (int)( 0.5f * (level - 1));
                break;
            case UpgradeType.Shield:
                diff = new BigNumber(spaceShip.instance.getMaxShield());
                diff.Subtract(Ship.Current.shield);
                Ship.Current.shieldMax.initial.Set(10);
                Ship.Current.shieldMax.initial *= 0.5f * Mathf.Pow(level + 1, 1.4f);

                Ship.Current.shield.Set(spaceShip.instance.getMaxShield());
                Ship.Current.shield.Subtract(diff);
                break;
            case UpgradeType.RegenShield:
                Ship.Current.regenShield = new BigNumber(10, 0);
                Ship.Current.regenShield.Multiply(0.20f * Mathf.Pow(level + 1, 1.30f));
                break;
            default:
                break;
        }

        LoadStat();
    }
    protected override void PayCost()
    {
        Stats.Instance.AddIron(-CalculLevelUpCost());
    }

    protected override bool CanPay()
    {
        return Ship.Current.iron.isBigger(CalculLevelUpCost());
    }

    protected override void SetLogos()
    {
        Texture2D logoTexture = Resources.Load<Texture2D>("logos/iron");
        StyleBackground background = new StyleBackground(logoTexture);
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.AddToClassList("ironColor");
    }

    #endregion
}
