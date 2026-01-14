
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static spaceObject;
using static UnityEngine.Rendering.DebugUI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public spaceObject meteorPredab;
    public spaceObject BigMeteorPrefab;
    public spaceObject DiamandMeteorPrefab;
    public spaceObject ScatterMeteorPrefab;
    public spaceObject miniMeteorPrefab;
    public spaceObject ironMeteorPrefab;
    public spaceObject uraniumMeteorPrefab;

    float timeSpawnSpaceObjet = 3f; //2f
    public float timer = 0f;
    float timerSave;
    public int meteorToKill = 10;
    public int meteorKilled = 0;

    public float meteorScale = 1f;
    private Vector3 initialScale;

    private BigNumber enemyLife = new BigNumber(1, 0);
    public BigNumber BN_ironEarned = new BigNumber(0);
    public BigNumber BN_xpEarned = new BigNumber(0);

    public bool isPaused = false;

    float autoSaveTimer = 0f;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Settings.Init();
            QuestStats.Init();
            Data.Init();
            Stats.Initialize();

            SetPause(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is calledonce before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        calculMeteorToKill();
        Application.targetFrameRate = 60;
    }

    private void calculMeteorToKill()
    {
        float n = Stats.Instance.enemyPerStage * 100f;
        meteorToKill = (int)(n / 100f);
        if(n % 100 > UnityEngine.Random.Range(0, 100))
        {
            meteorToKill++;
        }
        MainUi.Instance.updateStage();
    }

    private void OnApplicationQuit()
    {
        Stats.Instance.save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Stats.Instance.save();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!isPaused) timer += Time.deltaTime;
        autoSaveTimer += Time.deltaTime;
        Data.Instance.time += Time.deltaTime;

        if(timer >= ( timeSpawnSpaceObjet / UpSpeed.Instance.upModeMultiplicator))
        {
            spawnSpaceObject();
            timer = 0f;
        }
        upStage();
        if(autoSaveTimer > 10f)
        {
            Stats.Instance.save();
            autoSaveTimer = 0f;
        }
    }

    public void upStage()
    {
        if (meteorKilled >= meteorToKill)
        {
            meteorKilled = 0;
            MainUi.Instance.enemyLabel.text = meteorToKill.ToString() + "/" + meteorToKill.ToString();
            Stats.Instance.stage++;
            if (Stats.Instance.stageSkipProb > Random.Range(0, 100))
            {
                Stats.Instance.stage++;
                gameManager.instance.getStageReward(1.70f, 0.75f);
                MainUi.Instance.ShowStageSkip();
            }
            gameManager.instance.getStageReward(1.95f);
            MainUi.Instance.updateStage();
            calculMeteorToKill();

            if (QuestManager.Instance.type == QuestType.Speed && !(QuestManager.Instance.isCompleted()))
            {
                if(new BigNumber(Stats.Instance.stage).isBigger(QuestManager.Instance.objectif))
                {
                    QuestStats.Instance.timeCompleted = Data.Instance.time;
                }
            }
        }
    }

    public void getStageReward(float posY, float fontFactor = 1f)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(1.3f * (Screen.width / 2f), posY * (Screen.height / 3f), 10));
        float reward;
        MarkerType type;
        if (Random.Range(0, 2) == 1)
        {//xp
            reward = Stats.Instance.stage * 1.25f * 5;
            if (Stats.Instance.xpBoostTime > 0)
                reward *= 2;
            Stats.Instance.AddXP(new BigNumber(reward));
            type = MarkerType.Xp;
        }
        else 
        {
            if(Stats.Instance.uraniumUnlocked && Random.Range(0, 2) == 1)
            {//uranium 
                reward = (int)(Stats.Instance.stage * 0.5f);
                type = MarkerType.Uranium;
                Stats.Instance.upUranium(new BigNumber(reward), true);

            }
            else
            {//iron
                reward  = (int)(Stats.Instance.stage * 2.5f);
                type = MarkerType.Iron;
                Stats.Instance.upIron(new BigNumber(reward), true);
            }
        }
        PoolManager.Instance.LaunchPrefab(worldPos, "stage reward : " + reward.ToString(), type, 0.1f, 0.985f, fontFactor);
    }

    public void SmallVibrate()
    {
        /*
            Called : meteor destroy, upLevel machie/upgrade, collect collectible
         */

        if (!Settings.Instance.isVibrate) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        long[] pattern = {0, 50}; // 50ms vibration
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
        vibrator.Call("vibrate", pattern, -1); // -1 = no repeat
#endif
    }

    private void spawnSpaceObject()
    {
        int x = UnityEngine.Random.Range(0, 1000);
        int BigProb;
        int ScatterProb;

        int stage = Stats.Instance.stage;

        BigProb = stage < 25 ? 0 : 650; // 45 - 65 = 25%

        ScatterProb = stage < 10 ? 0 :
                      stage < 25 ? 450 : 450;// diamondLimit + 7.5 - 45 = 30-35( environ ) 

        int diamondLimit = Stats.Instance.diamandProb;
        int rareLimit = diamondLimit + 75; //uranium ou fer

        if (x < diamondLimit)
            SpawnMeteor(DiamandMeteorPrefab, meteorType.Diamand);
        else if(x < rareLimit)
        {
            if (Stats.Instance.uraniumUnlocked && UnityEngine.Random.Range(0, 2) == 0)
                SpawnMeteor(uraniumMeteorPrefab, meteorType.Uranium);
            else
                SpawnMeteor(ironMeteorPrefab, meteorType.Iron);
        }
        else if (x < ScatterProb)
            SpawnMeteor(ScatterMeteorPrefab, meteorType.Scatter);
        else if (x < BigProb)
            SpawnMeteor(BigMeteorPrefab, meteorType.Big);
        else 
            SpawnMeteor(meteorPredab, meteorType.Normal);
    }

    private void SpawnMeteor(spaceObject meteorPredab, meteorType type)
    {
        spaceObject obj = Instantiate(meteorPredab);
        obj.type = type;
        obj.Init();
    }

    public void SetPause(bool isPause)
    {
        isPaused = isPause;
        if (isPause && Settings.Instance.isPausable)
        { 
            spaceObject[] enemies = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
            foreach (spaceObject obj in enemies)
            {
                obj.Pause();

            }
            canon.instance.setPause(true);
            timerSave = timer;
            timer = -1000f;
        }
        else if(!isPause)
        {
            if(timerSave > 0f)
            {
                timer = timerSave;
            }
            else
            {
                timer = 0f;
            }

                spaceObject[] enemies = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
            foreach (spaceObject obj in enemies)
            {
                if (obj.isPause)
                {
                    obj.Move();
                }


            }
            canon.instance.setPause(false);
        }


    }

    public void DestroyMeteors()
    {
        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        foreach (spaceObject obj in meteors)
        {

            Destroy(obj.gameObject);

        }
    }
    public void RestartStage()
    {
        DestroyMeteors();

        meteorKilled = 0;
        Stats.Instance.life = new BigNumber(spaceShip.instance.getMaxLife());
        Stats.Instance.shield = new BigNumber(spaceShip.instance.getMaxShield());

        MainUi.Instance.enemyLabel.text = meteorToKill.ToString();
        MainUi.Instance.healthBar.style.width = Length.Percent(100);
    }
    public void launchMiniMeteor(Transform trans)
    {
        spaceObject obj = Instantiate(miniMeteorPrefab);
        spaceObject obj2 = Instantiate(miniMeteorPrefab);
        spaceObject obj3 = Instantiate(miniMeteorPrefab);
        obj.transform.position = trans.position;
        Vector3 n= trans.position;
        if (Mathf.Abs(n.y - spaceShip.instance.transform.position.y) < 2)
        {
            n.y += 1f;
            obj2.transform.position = n;
            n.y -= 2f;
        }
        else
        {
            n.x += 1f;
            obj2.transform.position = n;
            n.x -= 2f;
        }
        obj3.transform.position = n;
        obj.Init();
        obj2.Init();
        obj3.Init();

    }

    public void setMeteorScale()
    {
        meteorScale = Stats.Instance.scale;
        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        foreach(spaceObject obj in meteors)
        {
            obj.transform.localScale = obj.baseScale;
            obj.transform.localScale *= Stats.Instance.scale;
        }
    }
}

