using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.SmartFormat.Utilities;
using Unity.VisualScripting;
using System.Linq;
using Newtonsoft.Json.Bson;
using UnityEngine.Localization.Components;
using Unity.Loading;
using System;

public class QuestUI : MonoBehaviour
{
        
    public UIDocument questUI;
    public UIDocument successUI;

    private VisualElement VE_main;
    private VisualElement questVE;

    private Button Btn_switch;
    private Button back;
    private Button exit;
    private Button claim;

    private Label dialogue;
    private Label quest;
    private Label progress;
    private Label questCount;
    private Label diamandReward;
    private Label xpReward;


    private bool lauchTransition = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questUI.gameObject.SetActive(false);
        successUI.gameObject.SetActive(false);
        QuestManager.Instance.initQuest();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadQuestUI()
    {
        questUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);

        var root = questUI.rootVisualElement;
        VE_main = root.Q<VisualElement>("main");

        if (lauchTransition)
        {
            VE_main.AddToClassList("trans");
            VE_main.schedule.Execute(() =>
            {
                VE_main.RemoveFromClassList("trans");
            }).StartingIn(50);

            lauchTransition = false;
        }

        questVE = root.Q<VisualElement>("questVE");

        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");
        claim = root.Q<Button>("claim");
        Btn_switch = root.Q<Button>("switch");
        exit.SetEnabled(true);

        dialogue = root.Q<Label>("dialogue");
        quest = root.Q<Label>("quest");
        progress = root.Q<Label>("progress");
        questCount = root.Q<Label>("questCount");
        diamandReward = root.Q<Label>("diamand");
        xpReward = root.Q<Label>("xp");


        refreshQuestUI();

        back.clicked -= Close;
        exit.clicked -= Close;
        back.clicked += Close;
        exit.clicked += Close;
        Btn_switch.clicked += Switch;

        claim.clicked -= ClaimClicked;


    }

    public void refreshQuestUI()//refaire le nom
    {
        QuestManager.Instance.initQuest();
        loadQuest();

        diamandReward.text = QuestManager.Instance.reward.ToString();
        xpReward.text = QuestManager.Instance.CalculXpReward().ToString();

        if (QuestManager.Instance.isCompleted())
        {
            claim.SetEnabled(true);
            claim.clicked += ClaimClicked;
        }
        else
            claim.SetEnabled(false);
    }

    public void Close()
    {
        lauchTransition = true;
        gameManager.instance.SetPause(false);

        VE_main.RemoveFromClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.AddToClassList("trans");
            exit.SetEnabled(false);
        }).StartingIn(50);
        VE_main.schedule.Execute(() =>
        {
            questUI.gameObject.SetActive(false);
            successUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(400);
    }

    private void Switch()
    {
        bool active = questUI.gameObject.activeSelf;
        questUI.gameObject.SetActive(!active);
        successUI.gameObject.SetActive(active);
        if (active)
            LoadSuccessUI();
        else 
            LoadQuestUI();    
    }


    public void ClaimClicked()
    {
        QuestManager.Instance.Claim();
        loadQuest();
        claim.SetEnabled(false);
        claim.clicked -= ClaimClicked;
    }

    public void loadQuest()
    {
        if (QuestStats.Instance.questLevel <= QuestStats.Instance.questMaxLevel)
        {
            //dialogue
            string key = "Quest_dialogue-" + (QuestStats.Instance.questLevel);
            LocalizedString localizesDialogue = new LocalizedString("UI_Quests", key);
            localizesDialogue.StringChanged += (localizedValue) =>
            {
                dialogue.text = localizedValue;
            };

            //objectif
            key = "Quest_objectif-" + QuestManager.Instance.type.ToString();
            LocalizedString localizesQuest = new LocalizedString("UI_Quests", key);
            if(QuestManager.Instance.type == QuestType.Speed)
            {
                localizesQuest.Arguments = new object[] { new { stage = QuestManager.Instance.objectif } };
            }
            localizesQuest.StringChanged += (localizedValue) =>
            {
                quest.text = localizedValue;
            };
            localizesQuest.RefreshString();
            
            //avancement
            if (QuestManager.Instance.type != QuestType.Speed)
            {
                progress.text = QuestStats.Instance.progress.ToString() + "/" + QuestManager.Instance.objectif.ToString();
            }
            else
            { //speed
                if (!QuestManager.Instance.isCompleted())
                {
                    progress.text = BigNumber.floatToTimeMinute(Data.Instance.time);
                }
                else
                {
                    progress.text = BigNumber.floatToTimeMinute(QuestStats.Instance.timeCompleted);
                }
            }
            questCount.text = QuestStats.Instance.questLevel + "/" + QuestStats.Instance.questMaxLevel;


        }
        else //level Max
        {
            string key = "Quest_dialogue-end";
            LocalizedString localizesDialogue = new LocalizedString("UI_Quests", key);
            localizesDialogue.StringChanged += (localizedValue) =>
            {
                dialogue.text = localizedValue;
            };
            diamandReward.style.visibility = Visibility.Hidden;
            xpReward.style.visibility = Visibility.Hidden;
            questVE.style.visibility = Visibility.Hidden;
            questCount.text = QuestStats.Instance.questMaxLevel + "/" + QuestStats.Instance.questMaxLevel;
        }
    }


    public void LoadSuccessUI()
    {
        var root = successUI.rootVisualElement;
        Btn_switch = root.Q<Button>("switch");
        VE_main = root.Q<VisualElement>("main");
        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");

        Btn_switch.clicked -= Switch;
        back.clicked -= Close;
        exit.clicked -= Close;
        back.clicked += Close;
        exit.clicked += Close;
        Btn_switch.clicked += Switch;

        ScrollView scroll = root.Q<ScrollView>("scroll");

        scroll.Clear();

        foreach(SuccessType key in Enum.GetValues(typeof(SuccessType)))
        {
            scroll.Add(new SuccessElement(key));
        }

        refreshSuccessUi();

    }

    public void refreshSuccessUi()
    {

    }
}
