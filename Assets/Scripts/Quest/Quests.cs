using System.IO;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;

public class QuestStats
{
    static public QuestStats Instance;

    public int questMaxLevel = 10;
    public int questLevel = 1;
    public BigNumber progress = new BigNumber(0);

    public float timeCompleted = 0;

    public static void Init()
    {
        if(Instance == null)
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
        }
    }
    public void reset()
    {
        Instance = new QuestStats();
        Save();
    }

}
