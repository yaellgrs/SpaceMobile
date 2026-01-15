using Newtonsoft.Json.Bson;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;
using static QuestUI;

public enum QuestType { KillMeteor, KillIronMeteor, UpgradeIron, GetStarParticle, KillUraniumMeteor, UpgradeUranium, UpgradeMachine, UnlockMachine, Speed, None };

public class QuestManager 
{
    private static QuestManager _Instance;

    public static QuestManager Instance
    {
        get
        {
            if (_Instance == null){
                _Instance = new QuestManager();
                _Instance.LoadQuests();
            }
            return _Instance;
        }
    }




    public QuestType type;
    public BigNumber objectif = new BigNumber(100);
    public int reward = 5;


    public List<Quests> quests;

    private void LoadQuests()
    {
        quests = Resources.LoadAll<Quests>("Data/Quests").OrderBy(q => q.level).ToList();
    }

    #region upQuests
    public void upQuest()
    {
        if (new[] { QuestType.KillMeteor, QuestType.UpgradeIron, QuestType.UpgradeUranium, QuestType.UpgradeMachine, QuestType.UnlockMachine }.Contains(type))
        {
            QuestStats.Instance.progress.Add(1);
        }


        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void upQuest(BigNumber n)
    {
        if (new[] { QuestType.KillIronMeteor, QuestType.KillUraniumMeteor }.Contains(type))
        {
            QuestStats.Instance.progress.Add(n);
        }

        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void upQuest(float time)
    {
        if (type == QuestType.Speed)
        {
            QuestStats.Instance.progress.Add(new BigNumber(time, 0));
        }
    }

    #endregion

    public void Claim()
    {
        QuestStats.Instance.questLevel++;
        Stats.Instance.upDiamand(reward, true);
        Stats.Instance.AddXP(CalculXpReward());

        MainUi.Instance.questUI.refreshQuestUI();
        QuestStats.Instance.progress = new BigNumber(0);
        QuestStats.Instance.timeCompleted = 0;
        initQuest();



        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void initQuest()
    {
        var quest = quests.FirstOrDefault(q => q.level == QuestStats.Instance.questLevel);
        if (quest == null) return;

        type = quest.type;
        objectif = quest.objectif;
        reward = quest.rewardDiamand;
    }

    public bool isCompleted()
    {
        if (QuestStats.Instance.questLevel > QuestStats.Instance.questMaxLevel) return false;

        if (type != QuestType.Speed)
        {
            if (QuestStats.Instance.progress.isBigger(objectif))
            {
                return true;
            }
        }
        else
        {
            if (QuestStats.Instance.timeCompleted > 0 && QuestStats.Instance.timeCompleted < 300)
            {
                return true;
            }
        }
        return false;
    }


    public BigNumber CalculXpReward()
    {
        return new BigNumber(30 * Mathf.Pow(1.5f, QuestStats.Instance.questLevel));
    }
}
