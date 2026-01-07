using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class SettingUI : MonoBehaviour
{

    public UIDocument menuUI;
    public UIDocument bonusUI;
    public UIDocument statsUI;
    public UIDocument settingUI;
    public UIDocument LangueUI;

    private Button exit;
    private Button back;
    private Button bonus;
    private Button settings;
    private Button stats;
    private Button langue;

    private VisualElement menuVE;

    //bonus
    private Label damageTotal;
    private Label damageBoost;
    private Label damagePrestige;
    private Label damageLevel;
    private Label damageLevelPerm;
    /*    private Label starTotal;
        private Label starLevel;
        private Label starPrestige;*/

    //stats
    private Label time_total;
    private Label meteorKilled_total;
    private Label time_current;
    private Label meteorKilled_current;


    //settings
    private Button pause;
    private Button Btn_toggleVibrate;
    private Button Btn_toggleSound;
    private Button Btn_showBanner;
    private Button damage;
    private Button xp;
    private Slider slider_general;
    private Slider slider_music;
    private Slider slider_soundEffect;

    //Langue
    private Button french;
    private Button english;
    private Button espagna;
    private Button german;

    private void Start()
    {
        SetLanguage(Settings.Instance.currentLangage, false);
        menuUI.gameObject.SetActive(false);
        bonusUI.gameObject.SetActive(false);
        settingUI.gameObject.SetActive(false);
        statsUI.gameObject.SetActive(false);
        LangueUI.gameObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void load()
    {
        menuUI.gameObject.SetActive(true);
        var root = menuUI.rootVisualElement;
        exit = root.Q<Button>("back");
        bonus = root.Q<Button>("bonus");
        settings = root.Q<Button>("settings");
        stats = root.Q<Button>("stats");
        langue = root.Q<Button>("langue");
        menuVE = root.Q<VisualElement>("menu");

        menuVE.AddToClassList("trans");
        menuVE.schedule.Execute(()=>
        {
            menuVE.RemoveFromClassList("trans");
        }).StartingIn(50);

        exit.clicked += backClicked;
        bonus.clicked += loadBonus;
        stats.clicked += laodStats;
        settings.clicked += loadSetting;
        langue.clicked += loadLangue;
    }

    private void backClicked()
    {
        if(menuVE != null)
        {
            menuVE.RemoveFromClassList("trans");
            menuVE.schedule.Execute(() =>
            {
                menuVE.AddToClassList("trans");
            }).StartingIn(50);
            menuVE.schedule.Execute(() =>
            {
                menuUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);
                bonusUI.gameObject.SetActive(false);
                settingUI.gameObject.SetActive(false);
                statsUI.gameObject.SetActive(false);
                LangueUI.gameObject.SetActive(false);
            }).StartingIn(400);
        }
        else
        {
            menuUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
            bonusUI.gameObject.SetActive(false);
            settingUI.gameObject.SetActive(false);
            LangueUI.gameObject.SetActive(false);
            statsUI.gameObject.SetActive(false);
        }
    }

    private void loadBonus()
    {
        bonusUI.gameObject.SetActive(true);
        var root = bonusUI.rootVisualElement;
        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");
        damageTotal = root.Q<Label>("damageTotal");
        damageBoost = root.Q<Label>("damageBoost");
        damagePrestige = root.Q<Label>("damagePrestige");
        damageLevel = root.Q<Label>("damageLevel");
        damageLevelPerm = root.Q<Label>("damageLevelPerm");

        float mult = Stats.Instance.prest_damage_multiplicator * Stats.Instance.damage_Multiplicator_Lvl * Stats.Instance.perm_Damage_Multiplicator_Lvl;
        if (Stats.Instance.damageBoostTime > 0)
        {
            damageBoost.text = "x2.00";
            mult *= 2;
        }
        damageTotal.text = "x" + mult.ToString("F2");
        damagePrestige.text = "x" + Stats.Instance.prest_damage_multiplicator.ToString("F2");
        damageLevel.text = "x" + Stats.Instance.damage_Multiplicator_Lvl.ToString("F2");
        damageLevelPerm.text = "x" + Stats.Instance.perm_Damage_Multiplicator_Lvl.ToString("F2");

        exit.clicked += backClicked;
        back.clicked += backBonusClicked;

    }

    private void backBonusClicked()
    {
        bonusUI.gameObject.SetActive(false);
    }

    /*STATS*/

    private void laodStats()
    {
        statsUI.gameObject.SetActive(true);
        var root = statsUI.rootVisualElement;
        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");

        //total
        time_total = root.Q<Label>("time_total");
        meteorKilled_total = root.Q<Label>("meteorKilled_total");

        time_total.text = BigNumber.floatToTimeHour(Data.Instance.totalTime + Data.Instance.time);
        meteorKilled_total.text = (Data.Instance.totalMeteorKilled + Data.Instance.meteorKilled).ToString();

        //current
        time_current = root.Q<Label>("time_current");
        meteorKilled_current = root.Q<Label>("meteorKilled_current");

        time_current.text = BigNumber.floatToTimeHour(Data.Instance.time);
        meteorKilled_current.text = Data.Instance.meteorKilled.ToString();


        exit.clicked += backClicked;
        back.clicked += backStatClicked;
    }

    private void backStatClicked()
    {
        statsUI.gameObject.SetActive(false);
    }

    /*SETTINGS*/

    private void loadSetting()
    {
        settingUI.gameObject.SetActive(true);
        var root = settingUI.rootVisualElement;
        slider_general = root.Q<Slider>("generalSound");
        slider_music = root.Q<Slider>("music");
        slider_soundEffect = root.Q<Slider>("soundEffect");
        exit = root.Q<Button>("exit");
        Btn_toggleVibrate = root.Q<Button>("vibrate");
        Btn_toggleSound = root.Q<Button>("sound");
        back = root.Q<Button>("back");
        Btn_showBanner = root.Q<Button>("showBanner");
        pause = root.Q<Button>("pause");
        damage = root.Q<Button>("damage");
        xp = root.Q<Button>("xp");

        slider_general.value = Settings.Instance.sound_general_value;
        slider_music.value = Settings.Instance.sound_music_value;
        slider_soundEffect.value = Settings.Instance.sound_effect_value;

        slider_general.RegisterValueChangedCallback(evt =>
        {
            Settings.Instance.sound_general_value = slider_general.value;
            Song.Instance.setMusicVolume();
        });
        slider_music.RegisterValueChangedCallback(evt =>
        {
            Settings.Instance.sound_music_value = slider_music.value;
            Song.Instance.setMusicVolume();
        });
        slider_soundEffect.RegisterValueChangedCallback(evt =>
        {
            Settings.Instance.sound_effect_value = slider_soundEffect.value;
        });


        Btn_toggleVibrate.clicked += VibrateClicked;
        Btn_toggleSound.clicked += soundClicked;
        exit.clicked += backClicked;
        back.clicked += backSettingClicked;
        pause.clicked += pauseClicked;
        damage.clicked += DamageClicked;
        xp.clicked += XpClicked;
        Btn_showBanner.clicked += showBannerClicked;

        SetSettingButton();
    }



    private void soundClicked()
    {
        Settings.Instance.activeSound = !Settings.Instance.activeSound;
        SetSettingButtonColor(Btn_toggleSound, Settings.Instance.activeSound, false);
        Song.Instance.setMusicVolume();
    }

    private void VibrateClicked()
    {
        Settings.Instance.isVibrate = !Settings.Instance.isVibrate;
        SetSettingButtonColor(Btn_toggleVibrate, Settings.Instance.isVibrate, false);
    }

    private void backSettingClicked()
    {
        settingUI.gameObject.SetActive(false);
    }

    private void showBannerClicked()
    {
        Settings.Instance.showBanner = !Settings.Instance.showBanner;
        Ads.Instance.ShowBanner(Settings.Instance.showBanner);
        SetSettingButtonColor(Btn_showBanner, Settings.Instance.showBanner, true);
    }

    private void pauseClicked()
    {
        Settings.Instance.isPausable = !Settings.Instance.isPausable;
        SetSettingButtonColor(pause, Settings.Instance.isPausable, true);
    }

    private void DamageClicked()
    {
        Settings.Instance.displayDamageMarker = !Settings.Instance.displayDamageMarker;
        SetSettingButtonColor(damage, Settings.Instance.displayDamageMarker, true);
    }

    private void XpClicked()
    {
        Settings.Instance.displayXpMarker = !Settings.Instance.displayXpMarker;
        SetSettingButtonColor(xp, Settings.Instance.displayXpMarker, true);
    }

    private void SetSettingButton()
    {
        SetSettingButtonColor(Btn_showBanner, Settings.Instance.showBanner, true);
        SetSettingButtonColor(pause, Settings.Instance.isPausable, true);
        SetSettingButtonColor(damage, Settings.Instance.displayDamageMarker, true);
        SetSettingButtonColor(xp, Settings.Instance.displayXpMarker, true);
        SetSettingButtonColor(Btn_toggleVibrate, Settings.Instance.isVibrate, false);
        SetSettingButtonColor(Btn_toggleSound, Settings.Instance.activeSound, false);
    }

    private void SetSettingButtonColor(Button btn, bool isSelected, bool setText)
    {
        Color color;
        if (isSelected)
        {
            color = Color.green;
            if (setText) btn.text = "active";
        }
        else
        {
            color = Color.red;
            if (setText) btn.text = "inactive";
        }
        Utility.setBorderColor(btn, color);
    }

    //LANGUE
    public void loadLangue()
    {
        if(Stats.Instance.firstConnection) gameManager.instance.SetPause(true);
        LangueUI.gameObject.SetActive(true);
        var root = LangueUI.rootVisualElement;

        french = root.Q<Button>("french");
        english = root.Q<Button>("english");
        german = root.Q<Button>("german");
        espagna = root.Q<Button>("espagna");
        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");

        SetButtonsLanguage();

        french.clicked -= SetLangFr;
        english.clicked -= SetLangEn;
        german.clicked -= SetLangDe;
        espagna.clicked -= SetLangEs;

        french.clicked += SetLangFr;
        english.clicked += SetLangEn;
        german.clicked += SetLangDe;
        espagna.clicked += SetLangEs;

        exit.clicked += backClicked;
        back.clicked += backLangueClicked;
    }

    private void SetLangFr() => SetLanguage("fr-FR", true);
    private void SetLangEn() => SetLanguage("en", true);
    private void SetLangDe() => SetLanguage("de", true);
    private void SetLangEs() => SetLanguage("es", true);

    private void SetLanguage(string languageCode, bool setButton)
    {

        if (LocalizationSettings.SelectedLocale == null)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        Settings.Instance.currentLangage = languageCode;
        var selectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == languageCode);

        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;;
            if (setButton)
            {
                SetButtonsLanguage();
            }
        }
        else
        {
            Debug.LogWarning("Langue non trouvée : " + languageCode);
        }
    }

    private void SetButtonsLanguage()
    {
        string currentLang = LocalizationSettings.SelectedLocale.Identifier.Code;

        SetButtonColor(french, currentLang.StartsWith("fr"));
        SetButtonColor(english, currentLang.StartsWith("en"));
        SetButtonColor(espagna, currentLang.StartsWith("es"));
        SetButtonColor(german, currentLang.StartsWith("de"));
    }

    private void SetButtonColor(Button button, bool isSelected)
    {
        Color color;
        if (isSelected)
        {
            color = Color.green;
        }
        else
        {
            color = Color.white;
        }
        button.style.borderBottomColor = color;
        button.style.borderTopColor = color;
        button.style.borderRightColor = color;
        button.style.borderLeftColor = color;
    }

    private void backLangueClicked()
    {
        if (Stats.Instance.firstConnection) gameManager.instance.SetPause(false);
        LangueUI.gameObject.SetActive(false);
    }
}
