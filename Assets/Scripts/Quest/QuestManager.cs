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

using static QuestUI;

public enum QuestType { MeteorToKill, ironMeteor, ironUpgrade, starParticule, uraniumMeteor, uraniumUpgrade, upMachines, unlockMachine, Speed, None };

public class QuestManager : MonoBehaviour
{
    private static QuestManager _Instance;

    public static QuestManager Instance
    {
        get
        {
            if (_Instance == null) _Instance = new QuestManager();
            return _Instance;
        }
    }


    public QuestType type;
    public BigNumber objectif = new BigNumber(100);
    private int reward = 5;


    #region upQuests
    public void upQuest()
    {
        if (new[] { QuestType.MeteorToKill, QuestType.ironUpgrade, QuestType.uraniumUpgrade, QuestType.upMachines, QuestType.unlockMachine }.Contains(type))
        {
            QuestStats.Instance.progress.Add(1);
        }


        MainUi.Instance.SetQuestCompleted(isCompleted());
    }

    public void upQuest(BigNumber n)
    {
        if (new[] { QuestType.ironMeteor, QuestType.uraniumMeteor }.Contains(type))
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

        switch (QuestStats.Instance.questLevel)
        {
            case 1:
                type = QuestType.MeteorToKill;
                objectif = new BigNumber(100);
                break;
            case 2:
                type = QuestType.ironMeteor;
                objectif = new BigNumber(2.5f, 3);
                break;
            case 3:
                type = QuestType.ironUpgrade;
                objectif = new BigNumber(15);
                break;
            case 4:
                type = QuestType.MeteorToKill;
                objectif = new BigNumber(250);
                break;
            case 5:
                type = QuestType.starParticule;
                objectif = new BigNumber(100);
                break;
            case 6:
                type = QuestType.uraniumMeteor;
                objectif = new BigNumber(2.5f, 3);
                break;
            case 7:
                type = QuestType.uraniumUpgrade;
                objectif = new BigNumber(15);
                break;
            case 8:
                type = QuestType.upMachines;
                objectif = new BigNumber(25);
                break;
            case 9:
                type = QuestType.unlockMachine;
                objectif = new BigNumber(1);
                break;
            case 10:
                type = QuestType.Speed;
                objectif = new BigNumber(25);
                break;

        }
        reward = 2;
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
