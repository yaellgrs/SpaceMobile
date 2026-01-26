using UnityEngine;
using UnityEngine.UIElements;

public class UpgradesIronElement : UpgradesElement
{
    #region ----- variables -----
    public enum UpgradeType { Life, Damage, WorldSize, Shield, RegenShield }
    public UpgradeType type;
    #endregion

    #region ----- Constructors -----
    public UpgradesIronElement() : base()
    {

    }

    public UpgradesIronElement(string name, UpgradeType type) : base()
    {
        this.name = name;
        this.type = type;
    }
    #endregion

    #region ----- overrides Methods -----

    protected override void LoadStat()
    {
        switch (type)
        {
            case UpgradeType.Life:
                Lbl_description.text = "Life : " + Stats.Instance.lifeMax;
                break;
            case UpgradeType.Damage:
                Lbl_description.text = "Damage : " + spaceShip.instance.damage;
                break;
            case UpgradeType.WorldSize:
                Lbl_description.text = "WorldSize : " + ((200f / Stats.Instance.scale) - 199f).ToString("F1");
                break;
            case UpgradeType.Shield:
                Lbl_description.text = "Shield : " + Stats.Instance.shieldMax;
                break;
            case UpgradeType.RegenShield:
                Lbl_description.text = "Regen Shield : " + Stats.Instance.regenShield;
                break;
        }

        string logo_path = "Upgrades/Iron/" + type.ToString();
        VE_logo.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(logo_path));
    }

    protected override void GetReward()
    {
        switch (type)
        {
            case UpgradeType.Life:
                BigNumber diff = new BigNumber(spaceShip.instance.getMaxLife());
                diff.Subtract(Stats.Instance.life);

                Stats.Instance.lifeMax = new BigNumber(10, 0);
                Stats.Instance.lifeMax.Multiply(0.5f * Mathf.Pow(level + 1, 1.6f));

                Stats.Instance.life = new BigNumber(spaceShip.instance.getMaxLife());
                Stats.Instance.life.Subtract(diff);
                MainUi.Instance.upHealthBar();
                break;
            case UpgradeType.Damage:
                spaceShip.instance.damage = new BigNumber(1, 0);
                spaceShip.instance.damage.Multiply(Mathf.Pow(1.3f, level));
                spaceShip.instance.damage.Add(0.5f * (level - 1));
                break;
            case UpgradeType.WorldSize:
                Stats.Instance.scale = 1f;
                Stats.Instance.scale = Mathf.Pow(0.992f, level + 1);
                spaceShip.instance.setScale(Stats.Instance.scale);
                gameManager.instance.setMeteorScale();
                break;
            case UpgradeType.Shield:
                diff = new BigNumber(spaceShip.instance.getMaxShield());
                diff.Subtract(Stats.Instance.shield);
                Stats.Instance.shieldMax = new BigNumber(10, 0);
                Stats.Instance.shieldMax.Multiply(0.5f * Mathf.Pow(level + 1, 1.4f));
                Stats.Instance.shield = new BigNumber(spaceShip.instance.getMaxShield());
                Stats.Instance.shield.Subtract(diff);
                break;
            case UpgradeType.RegenShield:
                Stats.Instance.regenShield = new BigNumber(10, 0);
                Stats.Instance.regenShield.Multiply(0.20f * Mathf.Pow(level + 1, 1.30f));
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
        return Stats.Instance.iron.isBigger(CalculLevelUpCost());
    }
    #endregion
}
