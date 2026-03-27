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

    public UpgradesIronElement(UpgradeData data, string name, UpgradeType type) : base(data, name)
    {
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
        BigNumber bonus = GetReward(data.level + getMulitplicator());
        bonus.Subtract(GetReward(data.level));

        Lbl_description.text = $"{type.ToString()}: {getStat()}";
        if(data.level < data.levelMax) Lbl_description.text += $" <color=green>(+{bonus.ToString()})</color>";
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

                Ship.Current.lifeMax.initial.Set(GetReward(data.level));
                //Ship.Current.lifeMax.initial *= 0.5f * Mathf.Pow(data.level + 1,  1.64697f); // 5 * 100^1.65

                Ship.Current.life.Set(spaceShip.instance.getMaxLife());
                Ship.Current.life.Subtract(diff);
                if(MainUi.Instance != null) MainUi.Instance.upHealthBar();
                break;
            case UpgradeType.Damage:
                Ship.Current.damage.initial.Set(GetReward(data.level));
                break;
            case UpgradeType.Shield:
                diff = new BigNumber(0);
                diff.Set(spaceShip.instance.getMaxShield());
                diff.Subtract(Ship.Current.shield);

                Ship.Current.shieldMax.initial.Set(GetReward(data.level));

                Ship.Current.shield.Set(spaceShip.instance.getMaxShield());
                Ship.Current.shield.Subtract(diff);
                break;
            case UpgradeType.RegenShield:
                Ship.Current.regenShield = new BigNumber(10, 0);
                Ship.Current.regenShield.Multiply(0.20f * data.level);
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
                reward.Set(2);
                reward *= 0.5f * Mathf.Pow(lvl + 1, 1.64697f);
                break;
            case UpgradeType.Damage:
                reward.Set(Mathf.Pow(lvl, 1.272f));
                break;
            case UpgradeType.Shield:
                reward.Set(1);
                reward *= 0.5f * Mathf.Pow(lvl + 1, 1.14825f);
                break;
            case UpgradeType.RegenShield:
                reward = new BigNumber(10);
                reward.Multiply(0.20f *lvl);
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
        StyleBackground background = Utility.GetMainRessourceLogo();
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.style.color = Utility.GetShipColor();
        Lbl_name.style.color = Utility.GetShipColor();
    }

    #endregion
}
