using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.Localization.Tables;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



[System.Serializable]
public class UpgradePrestige : Upgrades
{
    public enum UpgradeType { PrestigeMultiplicator, LessMeteor, LessTimeMachine, LessPriceUpgrades, XpBoost, DamageMultiplicator, StageSkip, OmegaProb, Max };
    /*
     on a : 
        - PrestigeMultiplicator  / LessMeteor / LessTimeMachine / LessPriceUpgrades / StageSkip
     il manque : 
        XpBoost / DamageMultiplicator / OmegaProb / 
     
     */
    public UpgradeType upgradeType;

    LocalizedString localizeUpgrades;

    protected override void loadStat()
    {
        string str = "";
        string key = "Prestige_upgrade_";

        //string logo_path = "prestige/";

        VisualElement logo = upgrade.Q<VisualElement>("logo");
        string logoPath = "Upgrades/prestige/";
        Texture2D logoTexutre = Resources.Load<Texture2D>(logoPath + upgradeType);
        if (logoTexutre == null) logoTexutre = Resources.Load<Texture2D>(logoPath + "CadresBlanc");
        logo.style.backgroundImage = logoTexutre;

        switch (upgradeType)
        {
            case UpgradeType.PrestigeMultiplicator://pas de logo
                key += "PrestigeMultiplicator";
                str = Stats.Instance.star_multiplicator_prestige.ToString("F2");
                name.text = "PrestigeMultiplicator";
                break;
            case UpgradeType.LessMeteor://pas de logo
                key += "LessMeteor";
                str =  Stats.Instance.enemyPerStage.ToString("F2");
                name.text = "LessMeteor";
                break;
            case UpgradeType.LessTimeMachine://pas de logo
                key += "LessTimeMachine";
                str = Stats.Instance.machineTimeReducer.ToString("F2");
                name.text = "LessTimeMachine";
                break;
            case UpgradeType.LessPriceUpgrades://pas de logo
                key += "LessPriceUpgrades";
                str =  Stats.Instance.upgradesPriceReducer.ToString("F2");
                name.text = "LessPriceUpgrades";
                break;
            case UpgradeType.XpBoost://pas de logo
                key += "XpBoost";
                str =Stats.Instance.XpMultiplicator.ToString("F2");
                name.text = "XpBoost";
                break;
            case UpgradeType.DamageMultiplicator:
                key += "DamageMultiplicator";
                str =  Stats.Instance.prest_damage_multiplicator.ToString("F2");
                name.text = "DamageMultiplicator";
                break;
            case UpgradeType.StageSkip://pas de logo
                key += "StageSkip";
                str = Stats.Instance.stageSkipProb.ToString("F0") + "%";
                name.text = "StageSkip";
                break;
            case UpgradeType.OmegaProb:
                key += "OmegaProb";
                str =  Stats.Instance.probabilitéOfOmega.ToString("F0") + "%";
                name.text = "OmegaProb";
                break;
        }

        if (LocalizationSettings.SelectedLocale == null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }

        localizeUpgrades = new LocalizedString("UI_Upgrades", key);

        localizeUpgrades.Arguments = new object[] { str };    
        localizeUpgrades.StringChanged += (localizeValue) =>
        {
            statLabel.text = localizeValue.ToString();
        };
        localizeUpgrades.RefreshString();
    }
    
    protected override void PayCost()
    {
        levelCostMachine1 = CalculUpgradeCost();
        Stats.Instance.upPrestige(levelCostMachine1, false);
    }
    protected override void upMachine1Clicked()
    {
        if (Stats.Instance.starPariticul.isBigger(CalculUpgradeCost()))
        {
            base.upMachine1Clicked();
        }
    }

    public override void update()
    {
        if (Stats.Instance.starPariticul.isBigger(CalculUpgradeCost()) && upButton != null)
        {
            upButton.enabledSelf = true;
        }
        else if (upButton != null)
        {
            upButton.enabledSelf = false;
        }
        upMachineCostText();
    }

    protected override void getReward()
    {
        switch (upgradeType)
        {
            case UpgradeType.PrestigeMultiplicator:
                Stats.Instance.star_multiplicator_prestige = 1f + 0.15f * (machineLevel1 - 1);
                break;
            case UpgradeType.LessMeteor:
                Stats.Instance.enemyPerStage = 10f - 0.16f*(machineLevel1);
                break;
            case UpgradeType.LessTimeMachine:
                Stats.Instance.machineTimeReducer = 1f - 0.229f * Mathf.Log(machineLevel1);
                break;
            case UpgradeType.LessPriceUpgrades:
                Stats.Instance.upgradesPriceReducer = 1f - 0.229f * Mathf.Log(machineLevel1);
                break;
            case UpgradeType.XpBoost:
                Stats.Instance.XpMultiplicator = 1f + 0.25f * (machineLevel1);
                break;
            case UpgradeType.DamageMultiplicator:
                Stats.Instance.prest_damage_multiplicator = 1f + 0.2f * (machineLevel1);
                break;
            case UpgradeType.StageSkip:
                Stats.Instance.prest_damage_multiplicator = machineLevel1;
                break;
            case UpgradeType.OmegaProb:
                Stats.Instance.probabilitéOfOmega = (machineLevel1 + 1 ) * 5;
                break;
        }
        loadStat(); 
    }
}
