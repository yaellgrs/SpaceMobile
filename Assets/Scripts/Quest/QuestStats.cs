using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestStats
{
    static public QuestStats Instance;

    public int questMaxLevel = 11;
    public int questLevel = 1;
    public BigNumber progress = new BigNumber(0);

    public float timeCompleted = 0;

    public int[] succesGoals;

    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new QuestStats();
            Instance.Load();
        }
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/statsQuests.json";
        string stat = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, stat);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/statsQuests.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }
        string data = System.IO.File.ReadAllText(path);
        QuestStats loaded = JsonUtility.FromJson<QuestStats>(data);

        if (loaded != null)
        {
            progress = loaded.progress;
            questLevel = loaded.questLevel;
            timeCompleted = loaded.timeCompleted;
            succesGoals = loaded.succesGoals;
        }
    }

    public void initSucces()
    {
        int enumCount = Enum.GetValues(typeof(SuccessType)).Length;

        if (succesGoals == null || succesGoals.Length < enumCount)
        {
            Array.Resize(ref succesGoals, enumCount);
        }
        foreach (SuccessType key in Enum.GetValues(typeof(SuccessType)))
        {
            if (succesGoals[(int)key] <= 0)
                succesGoals[(int)key] = 1;
        }
    }
    public void reset()
    {
        Instance = new QuestStats();
        Save();


    }
}
