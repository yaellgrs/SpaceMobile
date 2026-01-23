using UnityEngine;

public class Data
{
    static public Data Instance;

    //total
    public float totalTime = 0;
    public int totalMeteorKilled = 0;

    //curent
    public float time = 0;
    public int meteorKilled = 0;

    //a mettre dans stats
    public BigNumber uraniumMeteorKilled = new BigNumber(0);
    public BigNumber basicMeteorKilled = new BigNumber(0);
    public BigNumber ironMeteorKilled = new BigNumber(0);
    public BigNumber diamandMeteorKilled = new BigNumber(0);
    public BigNumber splitterMeteorKilled = new BigNumber(0);
    public BigNumber reinforcedMeteorKilled = new BigNumber(0);
    public BigNumber OmegaMeteorKilled = new BigNumber(0);


    public BigNumber PrestigeCount = new BigNumber(0);

    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new Data();
            Instance.Load();
        }
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/Data.json";
        string data = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, data);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/Data.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }
        string data = System.IO.File.ReadAllText(path);
        Instance = JsonUtility.FromJson<Data>(data);
    }
    public void reset()
    {
        Instance = new Data();
        Save();
    }
    
    public void Prestige()
    {
        totalTime += time;
        time = 0;
        totalMeteorKilled += meteorKilled;
        meteorKilled = 0;
    }
}
