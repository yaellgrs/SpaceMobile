using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
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



    public static void Load()
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

            Instance.LoadData();
        }
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/Datas.json";
        string data = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, data);
    }

    private void LoadData()
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

    public static object SumDataValue(object value, object value2, object? value3 = null, bool max = false)
    {

        if (value == null) return null;

        if (value2 != null && value3 != null) value2 = SumDataValue(value2, value3,max: max);


        if (value is BigNumber bn)
        {
            if (value2 != null && value2 is BigNumber bn2)
                bn += bn2;
            return bn;
        }

        if (value is float f)
        {
            if (value2 != null && value2 is float f2)
                f += f2;
            return f;
        }

        if(value is int i)
        {


            if (value2 != null && value2 is int i2){
                if (max) return Mathf.Max(i, i2);
                i += i2;
            }

            return i;
        }

        return value;
    }

    public void Prestige()
    {
        foreach (FieldInfo field in typeof(Data).GetFields())
        {
            object valueCurrent = field.GetValue(Datas.Instance.current);
            object valueShip = field.GetValue(Datas.Instance.currentShip);

            if (valueCurrent is System.Collections.IDictionary dicoCurrent &&
                valueShip is System.Collections.IDictionary dicoShip)
            {
                var keys = new HashSet<object>();
                foreach (var k in dicoCurrent.Keys) keys.Add(k);
                foreach (var k in dicoShip.Keys) keys.Add(k);

                foreach (var key in keys)
                {
                    if(dicoShip.Contains(key) && dicoCurrent.Contains(key)) dicoCurrent[key] = SumDataValue(dicoCurrent[key], dicoShip[key]); ;
                    //ici
                }
                field.SetValue(Datas.Instance.current, dicoCurrent);
            }
            else
            {
                field.SetValue(Datas.Instance.current, SumDataValue(field.GetValue(Datas.Instance.current), field.GetValue(Datas.Instance.currentShip)));
            }
        }
        Instance.currentShip.prestige++;
        Instance.current = new Data();
        Instance.current.Init();
    }
}
