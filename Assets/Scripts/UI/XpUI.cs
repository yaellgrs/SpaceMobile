
using System;
using System.IO;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.SmartFormat.Utilities;
using static XpUI;

public class XpUI : MonoBehaviour
{

    public UIDocument xpUI;
    public UIDocument levelUpUI;

    public IronUi ironUI;
    public UraniumUI uraniumUI;


    private Button back;
    private Button exit;
    private Button levelBack;
    private Button levelNext;

    private VisualElement xpBar;
    private VisualElement rewardVE;
    private VisualElement main;


    private Label xpLabel;
    private Label bonusLabel;
    public Label rewardLevelLabel;
    public Label levelLabel;

    public Label damageBonus;
    public Label lifeBonus;
    public Label shieldBonus;

    private LocalizedString localizesReward;

    public enum BonusLevel { Prestige, FerAuto, UraniumAuto, Damage, UnlockPrestige, UnlockUranium, machineBoost, Diamand, UnlockShip, Speed, OfflineProduction, Critical, ShieldRegen,  None };

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    int level = 0;
    void Start()
    {
        xpUI.gameObject.SetActive(false);
        levelUpUI.gameObject.SetActive(false);
        loadBonus();
        
    }

    public void load()
    {

        var root = xpUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");
        levelBack = root.Q<Button>("backLevel");
        levelNext = root.Q<Button>("nextLevel");
        xpBar = root.Q<VisualElement>("xpBar");
        rewardVE = root.Q<VisualElement>("reward");
        xpLabel = root.Q<Label>("xp");
        rewardLevelLabel = root.Q<Label>("levelReward");
        levelLabel = root.Q<Label>("level");
        bonusLabel = root.Q<Label>("bonus");
        damageBonus = root.Q<Label>("damage");
        lifeBonus = root.Q<Label>("life");
        shieldBonus = root.Q<Label>("shield");

        
        level =+ Stats.Instance.level + 1;
        if (level % 2 == 1) level++;

        rewardLevelLabel.text = "Level " +  level.ToString();
        xpBar.style.width = Length.Percent(((float)Stats.Instance.xp / Stats.Instance.xpLevelUp) * 100);
        xpLabel.text = Stats.Instance.xp.ToString("F1") + "/" + Stats.Instance.xpLevelUp.ToString("F1") + "XP" ;
        back.clicked -= Clicked;
        exit.clicked -= Clicked;
        back.clicked += Clicked;
        exit.clicked += Clicked;
        levelLabel.text = Stats.Instance.level.ToString();

        levelBack.clicked += levelBackClicked;
        levelNext.clicked += levelNextClicked;
        loadRewardImage();

        setRewardText(bonusLabel);

        if (level == 2)
        {
            levelBack.enabledSelf = false;
        }

        damageBonus.text = Stats.Instance.damage_Multiplicator_Lvl*100 + "%";
        lifeBonus.text = Stats.Instance.life_Multiplicator_Lvl*100 + "%";
        shieldBonus.text = Stats.Instance.shield_Multiplicator_Lvl*100 + "%";
    }

    // Update is called once per frame

    public void Clicked()
    {

        if (xpUI.gameObject.activeInHierarchy || levelUpUI.gameObject.activeInHierarchy)
        {
            main.RemoveFromClassList("trans");
            main.schedule.Execute(() =>
            {
                main.AddToClassList("trans");
            }).StartingIn(50);
            main.schedule.Execute(() =>
            {
                xpUI.gameObject.SetActive(false);
                levelUpUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);
            }).StartingIn(300);

        }
        else
        {
            xpUI.gameObject.SetActive(true);
            gameManager.instance.SetPause(true);
            load();
        }


    }

    public void LevelUp()
    {

        Stats.Instance.level++;

        if (Stats.Instance.level % 2 == 0) loadLevelUpUI();

        Stats.Instance.xp = 0f;
        Stats.Instance.xpLevelUp *= 1.5f;


        MainUi.Instance.upLevelUI();
        loadBonus();

        foreach(MachineIron m in Stats.Instance.machinesIron)
        {
            m.machineLevelLimite = Stats.Instance.level;
            if(m.machineLevelLimite > m.machineLevelMax)
            {
                m.machineLevelLimite = m.machineLevelMax;
            }
        }
    }

    public void loadLevelUpUI()
    {

        gameManager.instance.SetPause(true);
        gameManager.instance.DestroyMeteors();

        levelUpUI.gameObject.SetActive(true);
        var root = levelUpUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");

        rewardLevelLabel = root.Q<Label>("levelReward");
        bonusLabel = root.Q<Label>("bonus");
        rewardVE = root.Q<VisualElement>("reward");

        damageBonus = root.Q<Label>("damage");
        lifeBonus = root.Q<Label>("life");
        shieldBonus = root.Q<Label>("shield");

        level = Stats.Instance.level;
        rewardLevelLabel.text = "Level " + level;
        setRewardText(bonusLabel);

        damageBonus.text = Stats.Instance.damage_Multiplicator_Lvl * 100 + "%";
        lifeBonus.text = Stats.Instance.life_Multiplicator_Lvl * 100 + "%";
        shieldBonus.text = Stats.Instance.shield_Multiplicator_Lvl * 100 + "%";
        loadRewardImage();

        exit.clicked -= Clicked;
        back.clicked -= Clicked;
        exit.clicked += Clicked;
        back.clicked += Clicked;
    }



    private void levelBackClicked()
    {
        levelNext.enabledSelf = true;
        level -=2;
        rewardLevelLabel.text = "Level " + level.ToString();
        if(level == 2)
        {
            levelBack.enabledSelf = false;
        }

        loadRewardImage();
        setRewardText(bonusLabel);
    }

    private void levelNextClicked()
    {
        levelBack.enabledSelf = true;
        level+=2;
        rewardLevelLabel.text = "Level " + level.ToString();
        if (level == 100)
        {
            levelNext.enabledSelf = false;
        }
        loadRewardImage();
        setRewardText(bonusLabel);

    }

    private void loadRewardImage()
    {
        string path = "xpReward";
        switch (GetEnumReward(level))
        {
            case BonusLevel.FerAuto:
                path += "/ironBonusAuto"; //
                break;
            case BonusLevel.UraniumAuto:
                path += "/uraniumAuto"; //
                break;
            case BonusLevel.machineBoost:
                if (Stats.Instance.uraniumUnlocked) path += "/machineBoost2";
                else path += "/machineBoost1";
                    break;
            case BonusLevel.Prestige:
                path += "/prestige"; //
                break;
            case BonusLevel.Damage:
                path += "/damagePerm"; //
                break;
            case BonusLevel.UnlockPrestige:
                path += "/prestigeUnlock"; //
                break;
            case BonusLevel.UnlockUranium:
                path += "/uraniumUnlock"; //
                break;
            case BonusLevel.Diamand:
                path += "/diamand"; //
                break;
            case BonusLevel.UnlockShip:
                path += "/shipUnlock"; //
                break;
            case BonusLevel.Speed:
                path += "/speed"; 
                break;
            case BonusLevel.OfflineProduction:
                path += "/offlineProduction"; 
                break;
            case BonusLevel.Critical:
                path += "/critical"; 
                break;
            case BonusLevel.ShieldRegen:
                path += "/shieldRegen";  //
                break;
            default:
                path += "/nothing";
                break;
        }
        setRewardText(bonusLabel);
        Texture2D background = Resources.Load<Texture2D>(path);

        if (rewardVE != null)
        {
            rewardVE.style.backgroundImage = background;
        }
    }

    public void loadBonus()
    {
        Stats.Instance.damage_Multiplicator_Lvl = 1f + (Stats.Instance.level - 1) * 0.1f;
        Stats.Instance.life_Multiplicator_Lvl = 1f + (Stats.Instance.level - 1) * 0.1f;
        Stats.Instance.shield_Multiplicator_Lvl = 1f + (Stats.Instance.level - 1) * 0.1f;

        setBonusAutoFer();
        setBonusAutoUranium();
        if (GetEnumRewardCount(BonusLevel.UnlockPrestige) == 1) Stats.Instance.prestigeUnlocked = true;
        if (GetEnumRewardCount(BonusLevel.UnlockUranium) == 1)
        {
            Stats.Instance.uraniumUnlocked = true;
            spaceShip.instance.setAreaScale(1f);
        }
        Stats.Instance.machineBoost_Lvl = 1f + GetEnumRewardCount(BonusLevel.machineBoost) * 0.25f;
        Stats.Instance.star_mutliplicator_level = 1f + GetEnumRewardCount(BonusLevel.Prestige) * 0.25f;
        Stats.Instance.perm_Damage_Multiplicator_Lvl = 1f + GetEnumRewardCount(BonusLevel.Damage) * 0.25f;
        Stats.Instance.diamandProb = 5 + GetEnumRewardCount(BonusLevel.Diamand) * 5;
        Stats.Instance.SpeedLevel = 1 + GetEnumRewardCount(BonusLevel.Speed);
        Stats.Instance.offline_Prod_Part = 0.25f + GetEnumRewardCount(BonusLevel.OfflineProduction) * 0.15f;
        Stats.Instance.critical_Prob = 10 + GetEnumRewardCount(BonusLevel.Critical) * 10;
        Stats.Instance.shield_Regen_Time = 10f - 2*GetEnumRewardCount(BonusLevel.ShieldRegen);
    }

    public BonusLevel GetEnumReward(int lvl)
    {
        return lvl switch
        {
            4 => BonusLevel.UnlockPrestige,
            10 => BonusLevel.UnlockUranium, 
            100 => BonusLevel.Diamand, 
            70 => BonusLevel.UnlockShip,
            2 or 6 or 20 or 32 or 38 or 54 or 56 or 66 or 72 or 78 or 86 or 92 => BonusLevel.machineBoost,
            12 or 26 or 46 or 68 or 88 => BonusLevel.FerAuto, //complet
            22 or 42 or 58 or 80 or 96 => BonusLevel.UraniumAuto, //complet
            18 or 30 or 40 or 60 or 82 or 98 => BonusLevel.Damage, 
            14 or 36 or 48 or 62 or 74 or 90 => BonusLevel.Prestige,
            8 or 16 or 24 => BonusLevel.Speed,
            50 => BonusLevel.OfflineProduction,
            28 or 44 or 64 or 84 or 94 => BonusLevel.Critical,
            34 or 52 or 76=> BonusLevel.ShieldRegen,    
             _ => BonusLevel.None
        };
    }

    private int GetEnumRewardCount(BonusLevel bonus)
    {
        int cpt = 0;
        for(int i = 1; i <= Stats.Instance.level; i++)
        {
            if (GetEnumReward(i) == bonus) cpt++;
        }
        return cpt;
    }

    public void setBonusAutoFer()
    {
        int x = GetEnumRewardCount(BonusLevel.FerAuto);

        for (int i = 0; i < Stats.Instance.machinesIron.Count; i++)
        {
            if (x > i)
            {
                Stats.Instance.machinesIron[i].automatic = true;
            }
            else
            {
                Stats.Instance.machinesIron[i].automatic = false;
            }
        }
    }
    public void setBonusAutoUranium()
    {
        int x = GetEnumRewardCount(BonusLevel.UraniumAuto);

        for (int i = 0; i < Stats.Instance.machinesUranium.Count; i++)
        {
            if (x > i)
            {
                Stats.Instance.machinesUranium[i].automatic = true;
            }
            else
            {
                Stats.Instance.machinesUranium[i].automatic = false;
            }
        }
    }

    private void setRewardText(Label lab)
    {
        string key = "";
        switch (GetEnumReward(level))
        {
            case BonusLevel.FerAuto:
                key = "FerAuto";
                break;
            case BonusLevel.UraniumAuto:
                key = "UraniumAuto";
                break;
            case BonusLevel.machineBoost:
                key = "machineBoost";
                break;
            case BonusLevel.Prestige:
                key = "Prestige";
                break;
            case BonusLevel.Damage:
                key = "Damage";
                break;
            case BonusLevel.UnlockPrestige:
                key = "UnlockPrestige";
                break;
            case BonusLevel.UnlockUranium:
                key = "UnlockUranium";
                break;
            case BonusLevel.Diamand:
                key = "Diamand";
                break;
            case BonusLevel.UnlockShip:
                key = "UnlockShip";
                break;
            case BonusLevel.Speed:
                key = "Speed";
                break;
            case BonusLevel.OfflineProduction:
                key = "OfflineProduction";
                break;
            case BonusLevel.Critical:
                key = "Critical";
                break;
            case BonusLevel.ShieldRegen:
                key = "ShieldRegen";
                break;
        }

        key = "XP_description_" + key;
        localizesReward = new LocalizedString("UI_Rewards", key);
        localizesReward.StringChanged += (localizedValue)=>
        {
            lab.text = localizedValue;
        };

    }

}

