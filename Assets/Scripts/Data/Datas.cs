using System;
using System.Collections.Generic;
using UnityEngine;


public class Data
{
    public float time = 0;
    public int maxStage = 1;
    public int stagePassed, prestige, dead, pubWatch, rocket, missMeteor, stageSkipped = 0;
    public BigNumber iron = new BigNumber(0);
    public BigNumber uranium = new BigNumber(0);
    public BigNumber startParticle = new BigNumber(0);

    public Dictionary<spaceObject.meteorType, BigNumber> meteorKilled = new();
    public Dictionary<BossType, BigNumber> meteorBossKilled = new();

    public void Init()
    {
        foreach (spaceObject.meteorType type in Enum.GetValues(typeof(spaceObject.meteorType))) meteorKilled[type] = new BigNumber(0);
        foreach (BossType type in Enum.GetValues(typeof(BossType))) meteorBossKilled[type] = new BigNumber(0);
    }
}
public class Datas
{
    static public Datas Instance;

    public Data current, currentShip, total;



    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new Datas();
            Instance.current = new Data();
            Instance.currentShip = new Data();
            Instance.total = new Data();

            Instance.current.Init();
            Instance.currentShip.Init();
            Instance.total.Init();

            Instance.Load();
        }
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/Datas.json";
        string data = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, data);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/Datas.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }
        string data = System.IO.File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(data, this);
    }
    public void reset()
    {
        current = new Data();
        currentShip = new Data();
        total = new Data();
        Save();
    }
    
    public void Prestige()
    {
        //totalTime += time;
        //time = 0;
        //totalMeteorKilled += meteorKilled;
        //meteorKilled = 0;
    }
}
