using UnityEngine;

[System.Serializable]
public class Settings
{
    static public Settings Instance;

    //settings
    public bool isPausable = true;
    public bool showBanner = true;
    public bool isVibrate = true;
    public bool activeSound = true;
    public bool displayDamageMarker = true;
    public bool displayXpMarker = true;
    public bool scientific = false;

    public float sound_general_value = 100f;
    public float sound_music_value = 100f;
    public float sound_effect_value = 100f;

    public string currentLangage = "en";



    public static void Init()
    {
        if (Instance == null)
        {
            Instance = new Settings();
            Instance.Load();
        }
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/Settings.json";
        string data = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, data);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + "/Settings.json";

        if (!System.IO.File.Exists(path))
        {
            return;
        }
        string data = System.IO.File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(data, this);
    }
    public void reset()
    {
        Instance = new Settings();
        Save();
    }
}
