using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class MainUi : MonoBehaviour
{
    public static MainUi Instance;

    [Header("UI Document principal")]
    public UIDocument mainUI;

    [Header("UI Documents")]
    public IronUi ironUI;
    public UraniumUI uraniumUI;
    public PrestigeUI prestigeUI;
    public XpUI xpUI;
    public SettingUI settingUI;
    public AdsUI adsUI;
    public ShopUI shopUI;
    public OfflineUI offlineUI;
    public QuestUI questUI;


    [Header("Others")]
    public VideoPlayer backgroundAnim;


    //mainUI
    protected Button normalUpButton1;
    protected Button uraniumButton;
    protected Button prestigeButton;
    protected Button xpButton;

    protected Button fire;

    private Label ironLabel;
    private Label uraniumLabel;
    private Label diamandLabel;
    private Label lifeLabel;
    private Label xpLabel;
    private Label shieldLabel;
    private Label shieldTimeLabel;
    private Label shieldRegenLabel;
    private Label stageLabel;
    public Label enemyLabel;
    public Label stageClearLabel;
    public Label meteorWarningLabel;
    public VisualElement healthBar;
    private VisualElement shieldBar;
    public VisualElement xpBar;

    public Button rocketButton;
    public Label rocketLabel;
    public Button settingButton;
    public Button speedButton;
    public Button pubButton;
    public Button shopButton;
    public Button questButton;
    public VisualElement questCompleted;
    public VisualElement rocketCover;
    private float rocketTimer = -1f;

    private Label Label_AutoShoot;
    private VisualElement VE_AutoShoot;
    private VisualElement VE_AutoShootBar;

    private float stageClearTimer = -1f;
    private float meteorWarningTimer = -1f;

    public Label debugLabel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = mainUI.rootVisualElement;

        healthBar = root.Q<VisualElement>("HealthBar"); 
        shieldBar = root.Q<VisualElement>("shieldBar"); 
        xpBar = root.Q<VisualElement>("xpBar");
        rocketCover = root.Q<VisualElement>("rocketCover"); 
        normalUpButton1 = root.Q<Button>("ironUI");
        uraniumButton = root.Q<Button>("uraniumUI");
        prestigeButton = root.Q<Button>("prestigeUI");
        xpButton = root.Q<Button>("xpButton");
        rocketButton = root.Q<Button>("rocket");
        settingButton = root.Q<Button>("setting");
        speedButton = root.Q<Button>("speedButton");
        pubButton = root.Q<Button>("pubButton");
        shopButton = root.Q<Button>("shop");
        fire = root.Q<Button>("fire");
        questButton = root.Q<Button>("quest");
        questCompleted = root.Q<VisualElement>("questCompleted");
        VE_AutoShoot = root.Q<VisualElement>("autoShoot");
        VE_AutoShootBar = root.Q<VisualElement>("autoShootBar");

        ironLabel = root.Q<Label>("iron");
        uraniumLabel = root.Q<Label>("uranium");
        diamandLabel = root.Q<Label>("diamand");
        lifeLabel = root.Q<Label>("life");
        xpLabel = root.Q<Label>("xpLvl");
        shieldLabel = root.Q<Label>("shield");
        shieldTimeLabel = root.Q<Label>("shieldTime");
        shieldRegenLabel = root.Q<Label>("shieldRegen");
        debugLabel = root.Q<Label>("debug");
        stageLabel = root.Q<Label>("stage");
        enemyLabel = root.Q<Label>("enemy");
        rocketLabel = root.Q<Label>("rocketTimer");
        stageClearLabel = root.Q<Label>("stageClear");
        meteorWarningLabel = root.Q<Label>("meteorWarning");
        Label_AutoShoot = root.Q<Label>("autoShootTimer");
        rocketCover.style.height = Length.Percent(0);

        diamandLabel.text = Stats.Instance.diamand.ToString();

        normalUpButton1.clicked += normalUpClicked;
        uraniumButton.clicked += uraniumClicked;
        prestigeButton.clicked += prestigeClicked;
        xpButton.clicked += xpClicked;
        rocketButton.clicked += rocketClicked;
        settingButton.clicked += settingClicked;
        speedButton.clicked += speedButtonClicked;
        pubButton.clicked += pubButtonClicked;
        shopButton.clicked += shopClicked;
        questButton.clicked += questUI.Load;
        enemyLabel.text = gameManager.instance.meteorToKill.ToString() + "/"+ gameManager.instance.meteorToKill.ToString();
        upIronUI();
        upUraniumUI();
        uraniumUI.loadUpdateUI();
        ironUI.loadUpdateUI();
        SetQuestCompleted(questUI.isCompleted());

        upStage();
        upLevelUI();
        upAutoShootUI();
        upAdsUI();
        stageClearLabel.style.visibility = Visibility.Hidden;
        stageClearTimer = -1f;

        uraniumUI.upgradeUI.gameObject.SetActive(false);
        ironUI.upgradeUI.gameObject.SetActive(false);
        adsUI.adsUI.gameObject.SetActive(false);

        if(Stats.Instance.lastConnection == 0)
        {
            Stats.Instance.life = new BigNumber(Stats.Instance.lifeMax);
            Stats.Instance.shield = new BigNumber(Stats.Instance.shieldMax);
        }

        UpSpeed.Instance.load(speedButton);

        fire.clicked += Fire;

        shieldTimeLabel.text = (Stats.Instance.shield_Regen_Time - spaceShip.instance.shieldRegen).ToString("F1") + "s";
        shieldRegenLabel.text = "+ " + Stats.Instance.regenShield;
    }

    private void Fire()
    {
        
        canon.instance.canFire = true;
        canon.instance.moveCanon();
    }

    private void speedButtonClicked()
    {
        UpSpeed.Instance.UpButton(speedButton);
        backgroundAnim.playbackSpeed = 0.5f * UpSpeed.Instance.upModeMultiplicator;
    }

    private void pubButtonClicked()
    {
        adsUI.load();
    }

    private void shopClicked()
    {
        shopUI.loadShop();
    }

    // Update is called once per frame
    void Update()
    {
        upHealthBar();
        upShieldBar();
        upLevelUI();
        upAutoShootUI();
        upAdsUI();
        if (Stats.Instance.xp > Stats.Instance.xpLevelUp)
        {
            xpUI.LevelUp();
        }

        if(rocketTimer > 0 &&  !gameManager.instance.isPaused)
        {
            rocketCover.style.height = Length.Percent((rocketTimer / Stats.Instance.rocketTimerMax) * 100);
            rocketTimer -= Time.deltaTime;
            rocketLabel.text = rocketTimer.ToString("F0");
        }
        else
        {
            rocketLabel.text = "";
            rocketCover.style.height = Length.Percent(0);
        }

        if( stageClearTimer >= 0)
        {
            stageClearTimer += Time.deltaTime;
            stageClearLabel.style.color = new Color(255, 255, 255, stageClearLabel.style.color.value.a * 0.975f);
            if ( stageClearTimer > 2f)
            {
                stageClearLabel.style.visibility = Visibility.Hidden;
                stageClearTimer = -1f;
            }
        }
        if (meteorWarningTimer >= 0)
        {
            meteorWarningTimer += Time.deltaTime;
            meteorWarningLabel.style.color = new Color(meteorWarningLabel.resolvedStyle.color.r, meteorWarningLabel.resolvedStyle.color.g, meteorWarningLabel.resolvedStyle.color.b, stageClearLabel.style.color.value.a * 0.975f);
            Debug.Log(meteorWarningLabel.resolvedStyle.color.r);
            
            if (meteorWarningTimer > 2f)
            {
                meteorWarningLabel.style.visibility = Visibility.Hidden;
                meteorWarningTimer = -1f;
            }
        }
    }

    public void SetQuestCompleted(bool isCompleted)
    {

            if (isCompleted)
            {
                questCompleted.style.visibility = Visibility.Visible;
            }
            else
            {
                questCompleted.style.visibility = Visibility.Hidden;
            }
        
    }

    public void upLevelUI()
    {
        xpLabel.text = Stats.Instance.level.ToString();
        float currentPercent = xpBar.style.width.value.value;
        float targetPercent = Stats.Instance.xp / Stats.Instance.xpLevelUp * 100;

        if (currentPercent < targetPercent - 0.25f)
        {
            float diff = targetPercent - currentPercent;
            xpBar.style.width = Length.Percent(xpBar.style.width.value.value + diff / 25f);
        }
        else
        {
            xpBar.style.width = Length.Percent(targetPercent);
        }
    }

    public void upAdsUI()
    {
        if (adsUI.isPubAvable())
        {
            pubButton.style.visibility = Visibility.Visible;
        }
        else
        {
            pubButton.style.visibility = Visibility.Hidden;
        }
    }

    public void upMeteorUI()
    {
        enemyLabel.text = (gameManager.instance.meteorToKill - gameManager.instance.meteorKilled).ToString() + "/" + gameManager.instance.meteorToKill.ToString();
    }

    private void rocketClicked()
    {
        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        if (rocketTimer <= 0 && meteors.Length > 0)
        {
            rocketTimer = Stats.Instance.rocketTimerMax;
            rocketCover.style.height = Length.Percent(100);
            canon.instance.rocketShoot();
        }
        else if (meteors.Length <= 0)
        {
            meteorWarningTimer = 0f;
            meteorWarningLabel.style.visibility = Visibility.Visible;
        }
    }

    public void upShieldRegenUI()
    {
        shieldTimeLabel.text = (Stats.Instance.shield_Regen_Time - spaceShip.instance.shieldRegen).ToString("F1") + "s";
    }

    private void settingClicked()
    {
        gameManager.instance.SetPause(true);
        settingUI.load();
    }

    public void upDebug(string text)
    {
        debugLabel.text = text;
    }

    private void normalUpClicked()
    {
        ironUI.classActived = true;
        ironUI.IronClicked();
        
    }
    private void uraniumClicked()
    {
        ironUI.classActived = true;
        uraniumUI.IronClicked();
    }

    public void prestigeClicked()
    {
        prestigeUI.IronClicked();
    }
    private void xpClicked()
    {
        xpUI.Clicked();
    }

    private void upAutoShootUI()
    {
        if(VE_AutoShoot != null)
        {
            if (Stats.Instance.uraniumUnlocked)
            {
                VE_AutoShoot.style.visibility = Visibility.Visible;
                float timer = (canon.instance.autoTimer / Stats.Instance.speedAuto) * 100;
                if (timer > 100f) timer = 100f;
                VE_AutoShootBar.style.width = Length.Percent(timer);
                timer = (Stats.Instance.speedAuto - canon.instance.autoTimer);
                if (timer < 0f) timer = 0f;
                Label_AutoShoot.text = timer.ToString("F1") + "s";

            }
            else
            {
                VE_AutoShoot.style.visibility = Visibility.Hidden;
            }
        }

    }

    public void upIronUI()
    {
        if(ironLabel != null)
        {
            string txt = Stats.Instance.iron.ToString();
            ironLabel.text = txt;
            ironLabel.style.fontSize = 50 - (2 * txt.Length);
        } 
    }

    public void upUraniumUI()
    {
        if (uraniumLabel != null)
        {
            string txt = Stats.Instance.uranium.ToString();
            uraniumLabel.text = txt;
            uraniumLabel.style.fontSize = 50 - (2 * txt.Length);
        }
    }

    public void upDiamandUI()
    {
        if (diamandLabel != null)
        {
            diamandLabel.text = Stats.Instance.diamand.ToString();
        }
    }

    public void upHealthBar()
    {
        if(healthBar != null)
        {
            string lifeText = Stats.Instance.life + "/" + spaceShip.instance.getMaxLife();
            lifeLabel.text = lifeText;
            lifeLabel.style.width = Length.Percent(20 + 5 * (lifeText.Length - 5));


            float currentPercent = healthBar.style.width.value.value;
            float targetPercent = Stats.Instance.life.GetPercentByDivided(spaceShip.instance.getMaxLife());


            if (currentPercent > targetPercent + 0.25f)
            {
                float diff = currentPercent - targetPercent;
                healthBar.style.width = Length.Percent(healthBar.style.width.value.value - diff / 15f);
            }
            else if (currentPercent < targetPercent - 0.25f)
            {
                float diff = targetPercent - currentPercent;
                healthBar.style.width = Length.Percent(healthBar.style.width.value.value + diff / 15f);
            }
            else
            {
                healthBar.style.width = Length.Percent(targetPercent);
            }

        }
    }
    public void upShieldBar()
    {
        if (shieldBar != null)
        {
            string shieldText = Stats.Instance.shield + "/" + spaceShip.instance.getMaxShield();
            shieldLabel.text = shieldText;
            shieldLabel.style.width = Length.Percent(20 + 5 * (shieldText.Length - 5));

            float currentPercent = shieldBar.style.width.value.value;
            float targetPercent = Stats.Instance.shield.GetPercentByDivided(spaceShip.instance.getMaxShield());


            if (currentPercent > targetPercent + 0.25f)
            {
                float diff = currentPercent - targetPercent;
                shieldBar.style.width = Length.Percent(shieldBar.style.width.value.value - diff / 15f);
            }
            else if(currentPercent < targetPercent - 0.25f)
            {
                float diff = targetPercent - currentPercent;
                shieldBar.style.width = Length.Percent(shieldBar.style.width.value.value + diff / 15f);
            }
            else
            {
                shieldBar.style.width = Length.Percent(targetPercent);
            }
        }
    }

    public static string ConvertIntToText(int amount)
    {
        float nAmount = (float)amount;
        if(amount < 1000)
        {
            return amount.ToString();
        }
        int cpt = 0 ;
        while (nAmount > 1000)
        {
            nAmount = (float)nAmount / 1000;
            cpt++;
        }
        string prefix;
        switch (cpt)
        {
            case 1: 
                prefix = "k";
                break;
            case 2: 
                prefix = "m";
                break;
            case 3:
                prefix = "M";
                break;
            case 4:
                prefix = "B";
                break;
            default:
                prefix = "";
                break;
        }
        if (nAmount > 100)
        {
            return nAmount.ToString("F1") + prefix;
        }
        else if (nAmount > 10)
        {
            return nAmount.ToString("F2") + prefix;
        }
        else
        {
            return nAmount.ToString("F3") + prefix;
        }
    }

    public void upStage()
    {
        if(stageClearLabel != null)
        {
            stageLabel.text = "Stage : " + Stats.Instance.stage;
            stageClearLabel.style.visibility = Visibility.Visible;
            stageClearLabel.style.color = new Color(255, 255, 255, 1f);
        }

        stageClearTimer = 0f;
        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        foreach (spaceObject obj in meteors)
        {
            Destroy(obj.gameObject);
        }

                
    }

}