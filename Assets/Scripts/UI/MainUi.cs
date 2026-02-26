using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static Utility;

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
    public BossFragmentUi bossFragmentUi;


    [Header("Others")]
    public VideoPlayer backgroundAnim;


    //mainUI
    private VisualElement VE_main;

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
    public Label Lbl_stageClear;
    public Label Lbl_stageSkip;
    public Label Lbl_meteorWarning;
    public VisualElement healthBar;
    private VisualElement shieldBar;
    public VisualElement xpBar;
    private VisualElement VE_gameUI;

    public Button rocketButton;
    public Label rocketLabel;
    public Button speedButton;
    public Button pubButton;
    public VisualElement questCompleted;
    public VisualElement rocketCover;
    private float rocketTimer = -1f;
    public Button Btn_bossFragment;

    private Label Label_AutoShoot;
    private VisualElement VE_AutoShoot;
    private VisualElement VE_AutoShootBar;

    private float stageClearTimer = -1f;
    private float stageSkipTimer = -1f;
    private float meteorWarningTimer = -1f;

    public Label debugLabel;

    //Debug
    Label Lbl_bossLife;
    VisualElement VE_bossLife;
    VisualElement VE_bossLifeLerp;
    float currentBossPercent;
    float LerpBossPercent;

    private void Awake()
    {
        if (Instance == null)
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

        VE_main = root.Q<VisualElement>("main");
        healthBar = root.Q<VisualElement>("HealthBar");
        shieldBar = root.Q<VisualElement>("shieldBar");
        xpBar = root.Q<VisualElement>("xpBar");
        rocketCover = root.Q<VisualElement>("rocketCover");
        Button normalUpButton1 = root.Q<Button>("ironUI");
        Button uraniumButton = root.Q<Button>("uraniumUI");
        Button prestigeButton = root.Q<Button>("prestigeUI");
        Button xpButton = root.Q<Button>("xpButton");
        rocketButton = root.Q<Button>("rocket");
        Button settingButton = root.Q<Button>("setting");
        speedButton = root.Q<Button>("speedButton");
        pubButton = root.Q<Button>("pubButton");
        Button shopButton = root.Q<Button>("shop");
        fire = root.Q<Button>("fire");
        Button questButton = root.Q<Button>("quest");
        questCompleted = root.Q<VisualElement>("questCompleted");
        VE_AutoShoot = root.Q<VisualElement>("autoShoot");
        VE_AutoShootBar = root.Q<VisualElement>("autoShootBar");
        Btn_bossFragment = root.Q<Button>("bossFragment");
        VE_gameUI = root.Q<VisualElement>("gameUI");

        ironLabel = root.Q<Label>("iron");
        uraniumLabel = root.Q<Label>("uranium");
        diamandLabel = root.Q<Label>("diamand");
        lifeLabel = root.Q<Label>("life");
        xpLabel = root.Q<Label>("xpLvl");
        shieldLabel = root.Q<Label>("shield");
        shieldTimeLabel = root.Q<Label>("shieldTime");
        shieldRegenLabel = root.Q<Label>("shieldRegen");
        debugLabel = root.Q<Label>("debug");
        enemyLabel = root.Q<Label>("enemy");
        rocketLabel = root.Q<Label>("rocketTimer");
        Lbl_stageClear = root.Q<Label>("stageClear");
        Lbl_stageSkip = root.Q<Label>("stageSkip");
        Lbl_meteorWarning = root.Q<Label>("meteorWarning");
        Label_AutoShoot = root.Q<Label>("autoShootTimer");
        rocketCover.style.height = Length.Percent(0);

        VE_bossLife = mainUI.rootVisualElement.Q<VisualElement>("BossLifeBarre");
        VE_bossLifeLerp = mainUI.rootVisualElement.Q<VisualElement>("BossLifeBarreLerp");
        Lbl_bossLife = mainUI.rootVisualElement.Q<Label>("bossLife");

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
        questButton.clicked += questUI.LoadQuestUI;
        Btn_bossFragment.clicked += bossFragmentUi.Open;
        enemyLabel.text = gameManager.instance.meteorToKill.ToString() + "/" + gameManager.instance.meteorToKill.ToString();
        upIronUI();
        upUraniumUI();
        uraniumUI.loadUpdateUI();
        ironUI.loadUpdateUI();
        SetQuestCompleted(QuestManager.Instance.isCompleted());

        //updateStage();
        upLevelUI();
        upAutoShootUI();
        upAdsUI();
        Lbl_stageClear.style.visibility = Visibility.Hidden;
        Lbl_stageSkip.style.visibility = Visibility.Hidden;
        stageClearTimer = -1f;
        stageSkipTimer = -1f;

        uraniumUI.upgradeUI.gameObject.SetActive(false);
        ironUI.upgradeUI.gameObject.SetActive(false);
        adsUI.adsUI.gameObject.SetActive(false);

        if (Stats.Instance.lastConnection == 0)
        {
            Ship.Current.life = new BigNumber(Ship.Current.lifeMax.getTotal());
            Ship.Current.shield = new BigNumber(Ship.Current.lifeMax.getTotal());
        }

        UpSpeed.Instance.load(speedButton);

        fire.clicked += Fire;
        loadRocketButton();

        shieldTimeLabel.text = (Stats.Instance.shield_Regen_Time - spaceShip.instance.shieldRegen).ToString("F1") + "s";
        shieldRegenLabel.text = "+ " + Ship.Current.regenShield;

        Stats.Instance.OnIronChanged += upIronUI;
    }

    public void setGameUI(bool active)
    {
        VE_gameUI.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void adaptBanner(bool adapt)
    {
        Debug.Log("adapt banner : " + adapt);
        if (adapt)
        {
            VE_main.style.height = Length.Percent(93.5f);
        }
        else
        {
            VE_main.style.height = Length.Percent(100);
        }
    }

    public void ShowBossLife(bool show)
    {
        if (show)
            currentBossPercent = LerpBossPercent = 100f;

        mainUI.rootVisualElement.Q<VisualElement>("boss").style.visibility = show ? Visibility.Visible : Visibility.Hidden;
    }

    public void loadRocketButton()
    {
        if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockRocket))
        {
            rocketCover.style.display = DisplayStyle.Flex;
            rocketButton.style.display = DisplayStyle.Flex;
            rocketLabel.style.display = DisplayStyle.Flex;
        }
        else
        {
            rocketCover.style.display = DisplayStyle.None;
            rocketButton.style.display = DisplayStyle.None;
            rocketLabel.style.display = DisplayStyle.None;
        }
    }

    private void Fire()
    {
        Debug.Log("fire");
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

        if (gameManager.instance.bossStage)
        {
            if (gameManager.instance.meteors.Count > 0)
            {
                Lbl_bossLife.text = gameManager.instance.meteors[0].life.ToString() + "/" + gameManager.instance.meteors[0].lifeMax.ToString();
                float targetPercent = (float)gameManager.instance.meteors[0].life.GetPercentByDivided(gameManager.instance.meteors[0].lifeMax);
                currentBossPercent = Mathf.Lerp(currentBossPercent, targetPercent, Time.deltaTime * 30f);
                LerpBossPercent = Mathf.Lerp(LerpBossPercent, targetPercent, Time.deltaTime * 2.5f);

            }
            else
            {
                currentBossPercent = 0f;
                LerpBossPercent = 0f;
            }
            VE_bossLife.style.width = Length.Percent(currentBossPercent);
            VE_bossLifeLerp.style.width = Length.Percent(LerpBossPercent);

        }


        if (rocketTimer > 0 && !gameManager.instance.isPaused)
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
        meteorWarningTimer = UpdatePopup(meteorWarningTimer, Lbl_meteorWarning);
        stageSkipTimer = UpdatePopup(stageSkipTimer, Lbl_stageSkip);
        stageClearTimer = UpdatePopup(stageClearTimer, Lbl_stageClear);
    }

    private float UpdatePopup(float timer, Label label)
    {
        if (timer >= 0)
        {
            timer += Time.deltaTime;
            float normalizedAlpha = Mathf.Pow(0.985f, timer * Time.deltaTime * 60f);
            SetAlphaColor(label, label.resolvedStyle.color.a * normalizedAlpha);
            if (timer > 2f)
            {
                label.style.visibility = Visibility.Hidden;
                timer = -1f;
            }
        }

        return timer;
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
        if (Ship.Current.level > 100) Ship.Current.level = 100;
        if (Ship.Current.level == 100)
        {
            xpBar.style.width = Length.Percent(100f);
            xpLabel.text = "100";
            return;
        }
        xpLabel.text = Ship.Current.level.ToString();
        float currentPercent = xpBar.style.width.value.value;
        xpBar.style.height = Length.Percent(100f);
        float targetPercent = (float)Ship.Current.BN_xp.GetPercentByDivided(Ship.Current.BN_xpMax);

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
        float lenght = gameManager.instance.meteors.Count;
        if (rocketTimer <= 0 && lenght > 0)
        {
            rocketTimer = Stats.Instance.rocketTimerMax;
            rocketCover.style.height = Length.Percent(100);
            canon.instance.rocketShoot();
        }
        else if (lenght <= 0)
        {
            meteorWarningTimer = 0f;
            Lbl_meteorWarning.style.visibility = Visibility.Visible;
            SetAlphaColor(Lbl_meteorWarning, 1f);
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
        if (VE_AutoShoot != null)
        {
            if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
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
        if (ironLabel != null)
        {
            string txt = Ship.Current.iron.ToString();
            ironLabel.text = txt;
            ironLabel.style.fontSize = 50 - (2 * txt.Length);
        }
    }

    public void upUraniumUI()
    {
        if (uraniumLabel != null)
        {
            string txt = Ship.Current.uranium.ToString();
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
        if (healthBar != null)
        {
            string lifeText = Ship.Current.life + "/" + spaceShip.instance.getMaxLife();
            lifeLabel.text = lifeText;
            lifeLabel.style.width = Length.Percent(40 + 20 * (lifeText.Length - 9.5f));


            float currentPercent = healthBar.style.width.value.value;
            float targetPercent = (float)Ship.Current.life.GetPercentByDivided(spaceShip.instance.getMaxLife());


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
            string shieldText = Ship.Current.shield + "/" + spaceShip.instance.getMaxShield();
            shieldLabel.text = shieldText;
            shieldLabel.style.width = Length.Percent(20 + 5 * (shieldText.Length - 5));

            float currentPercent = shieldBar.style.width.value.value;
            float targetPercent = (float)Ship.Current.shield.GetPercentByDivided(spaceShip.instance.getMaxShield());


            if (currentPercent > targetPercent + 0.25f)
            {
                float diff = currentPercent - targetPercent;
                shieldBar.style.width = Length.Percent(shieldBar.style.width.value.value - diff / 15f);
            }
            else if (currentPercent < targetPercent - 0.25f)
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
        if (amount < 1000)
        {
            return amount.ToString();
        }
        int cpt = 0;
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
        if (Lbl_stageClear != null)
        {

            Lbl_stageClear.style.visibility = Visibility.Visible;
            SetAlphaColor(Lbl_stageClear, 1f);
        }
        updateStage();

        stageClearTimer = 0f;
        foreach (spaceObject obj in gameManager.instance.meteors)
            Destroy(obj.gameObject);
    }

    public void updateStage()
    {
        if (stageLabel == null)
            stageLabel = mainUI.rootVisualElement.Q<Label>("stage");
        if(Ship.Current != null) stageLabel.text = "Stage : " + Ship.Current.stage;
    }

    public void ShowStageSkip()
    {
        if (Lbl_stageSkip == null) return;

        stageSkipTimer = 0f;
        Lbl_stageSkip.style.visibility = Visibility.Visible;
        SetAlphaColor(Lbl_stageSkip, 1f);
    }

}