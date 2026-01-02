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

public class QuestUI : MonoBehaviour
{
        
    public UIDocument questUI;
    private VisualElement main;
    private VisualElement questVE;

    private Button back;
    private Button exit;
    private Button claim;

    private Label dialogue;
    private Label quest;
    private Label progress;
    private Label questCount;
    private Label diamandReward;
    private Label xpReward;


    private LocalizedString localizesDialogue;
    private LocalizedString localizesQuest;


    public enum questType { MeteorToKill, ironMeteor, ironUpgrade, starParticule, uraniumMeteor, uraniumUpgrade, upMachines, unlockMachine, Speed, None };

    public questType type;

    public BigNumber objectif = new BigNumber(100);
    private int reward = 5;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questUI.gameObject.SetActive(false);
        initQuest();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
        questUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);

        var root = questUI.rootVisualElement;
        main = root.Q<VisualElement>("main");



        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        questVE = root.Q<VisualElement>("questVE");

        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");
        claim = root.Q<Button>("claim");
        exit.SetEnabled(true);

        dialogue = root.Q<Label>("dialogue");
        quest = root.Q<Label>("quest");
        progress = root.Q<Label>("progress");
        questCount = root.Q<Label>("questCount");
        diamandReward = root.Q<Label>("diamand");
        xpReward = root.Q<Label>("xp");
        initQuest();
        loadQuest();

        diamandReward.text = reward.ToString();
        xpReward.text = CalculXpReward().ToString("F0");

        back.clicked -= Close;
        exit.clicked -= Close;
        back.clicked += Close;
        exit.clicked += Close;

        claim.clicked -= Claim;
        if (isCompleted())
        {
            claim.SetEnabled(true);
            claim.clicked += Claim;
        }
        else
        {
            claim.SetEnabled(false);

        }


    }

    public void Close()
    {
        gameManager.instance.SetPause(false);

        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
            exit.SetEnabled(false);
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            questUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(400);
    }

    public void upQuest()
    {
        if (new[] { questType.MeteorToKill, questType.ironUpgrade, questType.uraniumUpgrade, questType.upMachines, questType.unlockMachine }.Contains(type))
        {
            QuestStats.Instance.progress.Add(1);
        }


        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void upQuest(BigNumber n)
    {
        if (new[] { questType.ironMeteor, questType.uraniumMeteor}.Contains(type))
        {
            QuestStats.Instance.progress.Add(n);
        }

        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void upQuest(float time)
    {
        if(type == questType.Speed)
        {
            QuestStats.Instance.progress.Add(new BigNumber(time, 0));
        }
    }

    private void Claim()
    {
        QuestStats.Instance.questLevel++;
        Stats.Instance.upDiamand(reward, true);
        Stats.Instance.xp += CalculXpReward();
        MainUi.Instance.upLevelUI();

        //tout remettre a 0
        QuestStats.Instance.progress = new BigNumber(0);
        QuestStats.Instance.timeCompleted = 0;
        initQuest();
        loadQuest();

        claim.SetEnabled(false);
        claim.clicked -= Claim;
        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    private void loadQuest()
    {
        if (QuestStats.Instance.questLevel <= QuestStats.Instance.questMaxLevel)
        {
            string key = "Quest_dialogue-" + (QuestStats.Instance.questLevel);
            localizesDialogue = new LocalizedString("UI_Quests", key);
            localizesDialogue.StringChanged += (localizedValue) =>
            {
                dialogue.text = localizedValue;
            };


            key = "Quest_objectif-";
            switch (type)
            {
                case questType.MeteorToKill:
                    key += "killMeteor";
                    break;
                case questType.ironMeteor:
                    key += "IronMeteor";
                    break;
                case questType.ironUpgrade:
                    key += "IronUpgrade";
                    break;
                case questType.starParticule:
                    key += "StarParticule";
                    break;
                case questType.uraniumUpgrade:
                    key += "UraniumUpgrade";
                    break;
                case questType.uraniumMeteor:
                    key += "UraniumMeteor";
                    break;
                case questType.upMachines:
                    key += "upMachines";
                    break;
                case questType.unlockMachine:
                    key += "unlockMachine";
                    break;
                case questType.Speed:
                    key += "speed";
                    break;
            }

            localizesQuest = new LocalizedString("UI_Quests", key);
            if(type == questType.Speed)
            {
                localizesQuest.Arguments = new object[] { new { stage = objectif } };
            }
            localizesQuest.StringChanged += (localizedValue) =>
            {
                quest.text = localizedValue;
            };
            localizesQuest.RefreshString();


            if (type != questType.Speed)
            {
                progress.text = QuestStats.Instance.progress.ToString() + "/" + objectif.ToString();
            }
            else
            {
                if (!isCompleted())
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
        else
        {
            string key = "Quest_dialogue-end";
            localizesDialogue = new LocalizedString("UI_Quests", key);
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

    public void initQuest() {

        switch (QuestStats.Instance.questLevel)
        {
            case 1:
                type = questType.MeteorToKill;
                objectif = new BigNumber(100);
                break;
            case 2:
                type = questType.ironMeteor;
                objectif = new BigNumber(2.5f, 3);
                break;
            case 3:
                type = questType.ironUpgrade;
                objectif = new BigNumber(15);
                break;
            case 4:
                type = questType.MeteorToKill;
                objectif = new BigNumber(250);
                break;
            case 5:
                type = questType.starParticule;
                objectif = new BigNumber(100);
                break;
            case 6:
                type = questType.uraniumMeteor;
                objectif = new BigNumber(2.5f, 3);
                break;
            case 7:
                type = questType.uraniumUpgrade;
                objectif = new BigNumber(15);
                break;
            case 8:
                type = questType.upMachines;
                objectif = new BigNumber(25);
                break;
            case 9:
                type = questType.unlockMachine;
                objectif = new BigNumber(1);
                break;
            case 10:
                type = questType.Speed;
                objectif = new BigNumber(25);   
                break;

        }
        reward = 2;
    }

    public bool isCompleted()
    {
        if (QuestStats.Instance.questLevel > QuestStats.Instance.questMaxLevel) return false;

        if (type != questType.Speed)
        {
            if (QuestStats.Instance.progress.isBigger(objectif))
            {
                return true;
            }
        }
        else
        {
            if(QuestStats.Instance.timeCompleted > 0 && QuestStats.Instance.timeCompleted < 300)
            {
                return true;
            }
        }
            return false;
    }


    private float CalculXpReward()
    {
        return Stats.Instance.xpLevelUp*0.25f;
    }
}
