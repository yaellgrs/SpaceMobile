using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;



public class SettingUI : MonoBehaviour
{

    public UIDocument menuUI;
    public UIDocument bonusUI;
    public UIDocument statsUI;
    public UIDocument tutoUI;
    public UIDocument settingUI;
    public UIDocument LangueUI;

    private Button exit;
    private Button back;
    private Button bonus;
    private Button Btn_tuto;
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
    private Button Btn_scientific;
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

    private int foldoutGap = 0;
    private Dictionary<VisualElement, int> gaps = new Dictionary<VisualElement, int>();

    private void Start()
    {

        SetLanguage(Settings.Instance.currentLangage, false);
        menuUI.gameObject.SetActive(false);
        bonusUI.gameObject.SetActive(false);
        settingUI.gameObject.SetActive(false);
        statsUI.gameObject.SetActive(false);
        LangueUI.gameObject.SetActive(false);
        tutoUI.gameObject.SetActive(false);



    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void load()
    {
        menuUI.gameObject.SetActive(true);
        var root = menuUI.rootVisualElement;
        exit = root.Q<Button>("back");
        bonus = root.Q<Button>("bonus");
        Btn_tuto = root.Q<Button>("tuto");
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
        Btn_tuto.clicked += LoadTuto;

        stats.clicked += laodStats;
        settings.clicked += loadSetting;
        langue.clicked += loadLangue;


        Utility.InitClickButtonSound(root);
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
        //damageTotal = root.Q<Label>("damageTotal");
        //damageBoost = root.Q<Label>("damageBoost");
        //damagePrestige = root.Q<Label>("damagePrestige");
        //damageLevel = root.Q<Label>("damageLevel");
        //damageLevelPerm = root.Q<Label>("damageLevelPerm");

        //float mult = Stats.Instance.prest_damage_multiplicator * Stats.Instance.damage_Multiplicator_Lvl * Stats.Instance.perm_Damage_Multiplicator_Lvl;
        //if (Stats.Instance.damageBoostTime > 0)
        //{
        //    damageBoost.text = "x2.00";
        //    mult *= 2;
        //}
        //damageTotal.text = "x" + mult.ToString("F2");
        //damagePrestige.text = "x" + Stats.Instance.prest_damage_multiplicator.ToString("F2");
        //damageLevel.text = "x" + Stats.Instance.damage_Multiplicator_Lvl.ToString("F2");
        //damageLevelPerm.text = "x" + Stats.Instance.perm_Damage_Multiplicator_Lvl.ToString("F2");

        VisualElement Lbl_totalDamage = root.Q<VisualElement>("damage");
        VisualElement Lbl_totalLife = root.Q<VisualElement>("life");
        VisualElement Lbl_totalShield = root.Q<VisualElement>("shield");

        InitFoldout(Lbl_totalDamage, new List<VisualElement>{Lbl_totalLife, Lbl_totalShield}, Ship.Current.damage);

        InitFoldout(Lbl_totalLife, new List<VisualElement>{ Lbl_totalShield}, Ship.Current.lifeMax);
        InitFoldout(Lbl_totalShield, new List<VisualElement>{ }, Ship.Current.shieldMax);


        //exit.clicked += backClicked;
        back.clicked += backBonusClicked;

        Utility.InitClickButtonSound(root);
    }

    private void InitFoldout(VisualElement source, List<VisualElement> elementsToMove, ShipTempStat stat)
    {
        Foldout foldout = source.Q<Foldout>("Foldout");
        Label Lbl_total = source.Q<Label>("total");
        Label Lbl_base = source.Q<Label>("base");
        Label Lbl_totalMult = source.Q<Label>("totalMult");
        Label Lbl_level = source.Q<Label>("level");
        Label Lbl_prestige = source.Q<Label>("prestige");

        foldout.value = false;

        Lbl_total.text = stat.getTotal().ToString();
        Lbl_base.text = stat.initial.ToString();
        Lbl_totalMult.text = "x" + stat.getMultiplier().ToString("F2");
        Lbl_level.text = "x" + Utility.getLevelBonus().ToString("F2");
        Lbl_prestige.text = "x" + stat.prestige_multiplicator.ToString("F2");


        foldout.RegisterValueChangedCallback(e =>
        {

            if (foldout.value)
            {
                float size = foldout.resolvedStyle.height;

                foreach (VisualElement element in elementsToMove)
                {
                    gaps[element] = gaps.ContainsKey(element) ? gaps[element] + 1 : 1;
                    float gap = size * 1.3f * gaps[element];
                    element.style.translate = new Translate(0, gap, 0);
                }
            }
            else
            {
                float size = foldout.resolvedStyle.height;
                foreach (VisualElement element in elementsToMove)
                {
                    gaps[element] = Math.Max(0, gaps.ContainsKey(element) ? gaps[element] - 1 : 0);
                    float gap = size * 1.3f * gaps[element];
                    element.style.translate = new Translate(0, gap, 0);
                }
            }
        });
    }


    private void backBonusClicked()
    {
        bonusUI.gameObject.SetActive(false);
    }

    private void LoadTuto()
    {
        tutoUI.gameObject.SetActive(true);
        var root = tutoUI.rootVisualElement;
        //exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");
        VisualElement tutos = root.Q<VisualElement>("tutos");

        foreach (Button button in tutos.Query<Button>().ToList())
        {
            PopupTuto tuto = Enum.Parse<PopupTuto>(button.name);

            if (!Stats.Instance.popupTutos[tuto])
            {
                button.style.display = DisplayStyle.None;   
            }
            else
            {
                button.style.display = DisplayStyle.Flex;

                button.clicked += () =>
                {
                    Tuto.Instance.LoadPopupTuto(tuto);
                    backTutoClicked();
                    backClicked();
                };
            }
        }

        exit.clicked += backClicked;
        back.clicked += backTutoClicked;

        Utility.InitClickButtonSound(root);
    }

    private void backTutoClicked()
    {
        tutoUI.gameObject.SetActive(false);
    }

    /*STATS*/

    private void laodStats()
    {
        statsUI.gameObject.SetActive(true);
        var root = statsUI.rootVisualElement;
        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");

        //totalssssssssssssssssssssssssssssssssssssss

        ScrollView scrollView = root.Q<ScrollView>("scroll");
        VisualElement titles = scrollView.Q<VisualElement>("titles");
        scrollView.Clear();
        VisualElement parent = new VisualElement();
        parent.style.flexGrow = 1;
        scrollView.Add(parent);
        parent.Add(titles);

        foreach (FieldInfo field in typeof(Data).GetFields())
        {
            object valueCurrent = field.GetValue(Datas.Instance.current);
            object valueShip = field.GetValue(Datas.Instance.currentShip);
            object valueTotal = field.GetValue(Datas.Instance.total);

            string name = field.Name;
            if (valueCurrent is System.Collections.IDictionary dicoCurrent &&
                valueShip is System.Collections.IDictionary dicoShip &&
                valueTotal is System.Collections.IDictionary dicoTotal)
            {
                var keys = new HashSet<object>();
                foreach (var k in dicoCurrent.Keys) keys.Add(k);
                foreach (var k in dicoShip.Keys) keys.Add(k);
                foreach (var k in dicoTotal.Keys) keys.Add(k);

                List<VisualElement> rows = new List<VisualElement>();
                BigNumber currentTotal = new BigNumber(0);
                BigNumber shipTotal = new BigNumber(0);
                BigNumber Total = new BigNumber(0);

                foreach (var key in keys)
                {
                    object obj1 = dicoCurrent.Contains(key) ? dicoCurrent[key] : null;
                    object obj2 = dicoShip.Contains(key) ? dicoShip[key] : null;
                    object obj3 = dicoTotal.Contains(key) ? dicoTotal[key] : null;
                    rows.Add(createStatLine(
                        key.ToString(),
                        FormatValue(obj1),
                        FormatValue(Datas.SumDataValue(obj1, obj2)),
                        FormatValue(Datas.SumDataValue(obj1, obj2, obj3)),
                        true
                    ));
                    currentTotal += dicoCurrent.Contains(key) ?  (BigNumber)dicoCurrent[key] : new BigNumber(0); ;
                    shipTotal += dicoCurrent.Contains(key) ? (BigNumber)dicoShip[key] : new BigNumber(0);
                    Total += dicoCurrent.Contains(key) ? (BigNumber)dicoTotal[key] : new BigNumber(0);

                }
                VisualElement title = createStatLine(
                    name,
                    FormatValue(currentTotal),
                    FormatValue(Datas.SumDataValue(currentTotal, shipTotal)),
                    FormatValue(Datas.SumDataValue(currentTotal, shipTotal, Total, max: (name == "maxStage")))
                );
                title.AddToClassList("totalRow");
                parent.Add(title);
                foreach (var row in rows)
                    parent.Add(row);
            }
            else
            {
                string current = FormatValue(field.GetValue(Datas.Instance.current));
                string total = FormatValue(Datas.SumDataValue(field.GetValue(Datas.Instance.current), field.GetValue(Datas.Instance.currentShip), max: (field.Name == "maxStage")));
                string ship = FormatValue(Datas.SumDataValue(field.GetValue(Datas.Instance.current), field.GetValue(Datas.Instance.currentShip), field.GetValue(Datas.Instance.total), max: (field.Name == "maxStage")));
                // verif name == "maxStage"
                parent.Add(createStatLine(name, current, ship, total));
            }

        }


        exit.clicked += backClicked;
        back.clicked += backStatClicked;

        Utility.InitClickButtonSound(root);
    }

    private VisualElement createStatLine(string name, string current, string ship, string total, bool isDico = false)
    {
        VisualElement line = new VisualElement();
        line.AddToClassList("row");

        Label Lbl_name = new Label(name);
        Label Lbl_current = new Label(current);
        Label Lbl_total = new Label(total);
        Label Lbl_ship = new Label(ship);

        Lbl_name.AddToClassList("stat");
        if (isDico) Lbl_name.AddToClassList("subStatName");
        else Lbl_name.AddToClassList("statName");
        Lbl_current.AddToClassList("stat");
        Lbl_total.AddToClassList("stat");
        Lbl_ship.AddToClassList("stat");

        line.Add(Lbl_name);
        line.Add(Lbl_current);
        line.Add(Lbl_total);
        line.Add(Lbl_ship);

        return line;
    }

    private string FormatValue(object value)
    {
        //if (value2 != null && value.GetType() == value2.GetType()) return FormatValue(value);

        if (value == null) return "null"; 

        if(value is BigNumber bn){
            return bn.ToString();
        }

        if (value is float f) {
            return Utility.TimeToString_dhms((long)f);
        }

        return value.ToString();
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
        Btn_scientific = root.Q<Button>("scientific");
        pause = root.Q<Button>("pause");
        damage = root.Q<Button>("damage");
        xp = root.Q<Button>("xp");

        slider_general.value = Settings.Instance.sound_general_value;
        slider_music.value = Settings.Instance.sound_music_value;
        slider_soundEffect.value = Settings.Instance.sound_effect_value;

        slider_general.RegisterValueChangedCallback(evt =>
        {
            Settings.Instance.sound_general_value = slider_general.value;
            SoundManager.Instance.setMusicVolume();
        });
        slider_music.RegisterValueChangedCallback(evt =>
        {
            Settings.Instance.sound_music_value = slider_music.value;
            SoundManager.Instance.setMusicVolume();
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
        Btn_scientific.clicked += scientificClicked;
        damage.clicked += DamageClicked;
        xp.clicked += XpClicked;
        Btn_showBanner.clicked += showBannerClicked;

        SetSettingButton();

        Utility.InitClickButtonSound(root);
    }



    private void soundClicked()
    {
        Settings.Instance.activeSound = !Settings.Instance.activeSound;
        SetSettingButtonColor(Btn_toggleSound, Settings.Instance.activeSound, false);
        SoundManager.Instance.setMusicVolume();
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

    private void scientificClicked()
    {
        Settings.Instance.scientific = !Settings.Instance.scientific;
        SetSettingButtonColor(Btn_scientific, Settings.Instance.scientific, true);
        MainUi.Instance.upIronUI();
        MainUi.Instance.upUraniumUI();
        MainUi.Instance.upDiamandUI();
        

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
        SetSettingButtonColor(Btn_scientific, Settings.Instance.scientific, true);
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

        Utility.InitClickButtonSound(root);
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
