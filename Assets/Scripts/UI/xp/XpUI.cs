using System;
using System.IO;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;
using static XpUI;

public class XpUI : MonoBehaviour
{
    public UIDocument xpUI;
    public UIDocument levelUpUI;

    private Button back;
    private Button exit;

    private VisualElement xpBar;
    private VisualElement main;

    private Label xpLabel;

    public Label levelLabel;

    public Label damageBonus;
    public Label lifeBonus;
    public Label shieldBonus;


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
        xpBar = root.Q<VisualElement>("xpBar");
        xpLabel = root.Q<Label>("xp");
        levelLabel = root.Q<Label>("level");
        damageBonus = root.Q<Label>("damage");
        lifeBonus = root.Q<Label>("life");
        shieldBonus = root.Q<Label>("shield");

       
        if (Ship.Current.level == 100) {
            xpBar.style.width = Length.Percent(100);
            xpLabel.text = "MAX";
        }
        else if (xpLabel ==null)
        {
            Debug.LogError("Ship.Current est NULL dans XpUI.Load()");
        }
        else
        {

            xpBar.style.width = (float)Ship.Current.BN_xp.GetPercentByDivided(Ship.Current.BN_xpMax);
            xpLabel.text = Ship.Current.BN_xp.ToString() + "/" + Ship.Current.BN_xpMax.ToString() + "XP";
        }

        back.clicked -= Clicked;
        exit.clicked -= Clicked;
        back.clicked += Clicked;
        exit.clicked += Clicked;
        levelLabel.text = Ship.Current.level.ToString();


        damageBonus.text = Stats.Instance.damage_Multiplicator_Lvl*100 + "%";
        lifeBonus.text = Stats.Instance.life_Multiplicator_Lvl *100 + "%";
        shieldBonus.text = Stats.Instance.shield_Multiplicator_Lvl *100 + "%";
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
        Ship.Current.level = Math.Clamp(Ship.Current.level + 1, 0, 100);


        //if (Ship.Current.level % 2 == 0) loadLevelUpUI();

        Ship.Current.BN_xp.Set(0);
        Ship.Current.BN_xpMax = new BigNumber(50 * Mathf.Pow(1.15f, Ship.Current.level));

        loadBonus();

    }

    //public void loadLevelUpUI()
    //{
    //    gameManager.instance.SetPause(true);
    //    gameManager.instance.DestroyMeteors();

    //    levelUpUI.gameObject.SetActive(true);
    //    var root = levelUpUI.rootVisualElement;

    //    main = root.Q<VisualElement>("main");

    //    main.AddToClassList("trans");
    //    main.schedule.Execute(() =>
    //    {
    //        main.RemoveFromClassList("trans");
    //    }).StartingIn(50);

    //    back = root.Q<Button>("back");
    //    exit = root.Q<Button>("exit");

    //    damageBonus = root.Q<Label>("damage");
    //    lifeBonus = root.Q<Label>("life");
    //    shieldBonus = root.Q<Label>("shield");

    //    damageBonus.text = Stats.Instance.damage_Multiplicator_Lvl * 100 + "%";
    //    lifeBonus.text = Stats.Instance.life_Multiplicator_Lvl * 100 + "%";
    //    shieldBonus.text = Stats.Instance.shield_Multiplicator_Lvl * 100 + "%";

    //    exit.clicked -= Clicked;
    //    back.clicked -= Clicked;
    //    exit.clicked += Clicked;
    //    back.clicked += Clicked;
    //}

    public void loadBonus()
    {
        int lvl = Ship.Current.level;
        Stats.Instance.damage_Multiplicator_Lvl = 1f + lvl * 0.01f;
		Stats.Instance.life_Multiplicator_Lvl = 1f + lvl * 0.01f;
		Stats.Instance.shield_Multiplicator_Lvl = 1f + lvl * 0.01f;
    }
}

