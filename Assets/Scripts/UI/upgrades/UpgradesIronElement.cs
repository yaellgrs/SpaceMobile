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
        //pas propre mais bon
        BigNumber bonus = GetReward(level + getMulitplicator());
        bonus.Subtract(GetReward(level));

        Lbl_description.text = $"{type.ToString()}: {getStat()} <color=green>(+{bonus.ToString()})</color>";
        string logo_path = "Upgrades/Iron/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));
    }

    protected override string getStat()
    {
        switch (type)
        {
            case UpgradeType.Life:
                return Ship.Current.lifeMax.initial.ToString();
            case UpgradeType.Damage:
                return Ship.Current.damage.initial.ToString();
            case UpgradeType.Shield:
                return Ship.Current.shieldMax.initial.ToString();
            case UpgradeType.RegenShield:
                return Ship.Current.regenShield.ToString();
        }
        return "";
    }

    public override void SetReward()
    {
        switch (type)
        {
            case UpgradeType.Life:
                BigNumber diff = new BigNumber(0);
                diff.Set(spaceShip.instance.getMaxLife());
                diff.Subtract(Ship.Current.life);

                Ship.Current.lifeMax.initial.Set(10);
                Ship.Current.lifeMax.initial *= 0.5f * Mathf.Pow(level + 1, 1.6f);

                Ship.Current.life.Set(spaceShip.instance.getMaxLife());
                Ship.Current.life.Subtract(diff);
                if(MainUi.Instance != null) MainUi.Instance.upHealthBar();
                break;
            case UpgradeType.Damage:
                Ship.Current.damage.initial.Set(1);
                Ship.Current.damage.initial *= Mathf.Pow(1.3f, level);
                Ship.Current.damage.initial += (int)( 0.5f * (level - 1));
                break;
            case UpgradeType.Shield:
                diff = new BigNumber(0);
                diff.Set(spaceShip.instance.getMaxShield());
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

    public override BigNumber GetReward(int lvl)
    {
        BigNumber reward = new BigNumber(0);
        switch (type)
        {
            case UpgradeType.Life:
                reward.Set(10);
                reward *= 0.5f * Mathf.Pow(lvl + 1, 1.6f);
                break;
            case UpgradeType.Damage:
                reward.Set(1);
                reward *= Mathf.Pow(1.3f, lvl);
                reward += (int)(0.5f * (lvl - 1));
                break;
            case UpgradeType.Shield:
                reward.Set(10);
                reward *= 0.5f * Mathf.Pow(lvl + 1, 1.4f);
                break;
            case UpgradeType.RegenShield:
                reward = new BigNumber(10);
                reward.Multiply(0.20f * Mathf.Pow(lvl + 1, 1.30f));
                break;
        }
        return reward;
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
