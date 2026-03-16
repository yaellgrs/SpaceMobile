using GoogleMobileAds.Api;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradesUraniumElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { SpeedAuto, AreaSlow, AreaWidth, WorldSize,  RocketReload, RocketMultiplier }
    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesUraniumElement() : base()
    {
    }

    public UpgradesUraniumElement(UpgradeData data, string name, UpgradeType type) : base(data, name)
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        BigNumber bonus = GetReward(data.level + getMulitplicator());
        bonus.Subtract(GetReward(data.level));

        Lbl_description.text = $"{type.ToString()}: {getStat()} <color=green>(+{bonus.getNormalNotation(false)})</color>";

        //Lbl_description

        string logo_path = "Upgrades/Uranium/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));
    }

    protected override string getStat()
    {
        switch (type)
        {
            case UpgradeType.SpeedAuto:
                return Stats.Instance.speedAuto.ToString("F2");
            case UpgradeType.AreaSlow:
                return Stats.Instance.areaSpeed.ToString("F2");
            case UpgradeType.AreaWidth:
                return Stats.Instance.areaSize.ToString("F2");
            case UpgradeType.WorldSize:
                return Stats.Instance.scale.ToString("F1");
            case UpgradeType.RocketReload:
                return Stats.Instance.rocketTimerMax.ToString("F2");
            case UpgradeType.RocketMultiplier:
                return Ship.Current.damage.rocket_multiplicator.ToString("F2");
        }
        return "";
    }

    public override void SetReward()
    {
        switch (type)
        {
            case UpgradeType.SpeedAuto:
                Stats.Instance.speedAuto = 1f / (0.09f * (data.level + 1));
                break;
            case UpgradeType.AreaSlow:
                Stats.Instance.areaSpeed = 1f + 0.5f * Mathf.Pow(data.level + 1, 0.6f);
                break;
            case UpgradeType.AreaWidth:
                Stats.Instance.areaSize = 1f + 0.3f * Mathf.Pow(data.level, 0.4f);
                spaceShip.instance.setAreaScale();
                break;
            case UpgradeType.WorldSize:
                Stats.Instance.scale = 1f;
                Stats.Instance.scale = Mathf.Pow(0.992f, data.level + 1);
                gameManager.instance.SetWorldScale();
                break;
            case UpgradeType.RocketReload:
                Stats.Instance.rocketTimerMax = 25f - Mathf.Pow(data.level, 0.4f);
                break;
            case UpgradeType.RocketMultiplier:
                Ship.Current.damage.rocket_multiplicator = 5f + 0.25f * Mathf.Pow(data.level - 1, 1.15f);
                break;
        }

        LoadStat();
    }

    public override BigNumber GetReward(int lvl)
    {
        BigNumber reward = new BigNumber(0);
        switch (type)
        {
            case UpgradeType.SpeedAuto:
                reward.Set(1f / (0.09f * (lvl + 1)));
                break;
            case UpgradeType.AreaSlow:
                reward.Set(1f + 0.5f * Mathf.Pow(lvl + 1, 0.6f));
                break;
            case UpgradeType.AreaWidth:
                reward.Set(1f + 0.3f * Mathf.Pow(lvl, 0.4f));
                break;
            case UpgradeType.WorldSize:
                reward.Set(Mathf.Pow(0.992f, lvl + 1));
                break;
            case UpgradeType.RocketReload:
                reward.Set(25f - Mathf.Pow(lvl, 0.4f));
                break;
            case UpgradeType.RocketMultiplier:
                reward.Set(5f + 0.25f * Mathf.Pow(lvl - 1, 1.15f));
                break;
        }
        return reward;
    }

    protected override void PayCost()
    {
        Stats.Instance.AddUranium(-CalculLevelUpCost());
    }

    protected override bool CanPay()
    {
        return Ship.Current.uranium.isBigger(CalculLevelUpCost());
    }

    protected override void SetLogos()
    {
        Texture2D logoTexture = Resources.Load<Texture2D>("logos/uranium");
        StyleBackground background = new StyleBackground(logoTexture);
        VE_levelUpCostLogo.style.backgroundImage = background;
        Lbl_levelUpCost.style.color = Consts.COLOR_URANIUM;
        Lbl_name.style.color = Consts.COLOR_URANIUM; 

    }
    #endregion
}
