using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

    public meteorBoss normalBossPrefab;

    float timeSpawnSpaceObjet = 3f; //2f
    public float timer = 0f;
    float timerSave;
    public int meteorToKill = 10;
    public int meteorKilled = 0;

    private Vector3 initialScale;

    private BigNumber enemyLife = new BigNumber(1, 0);
    public BigNumber BN_ironEarned = new BigNumber(0);
    public BigNumber BN_xpEarned = new BigNumber(0);

    public bool isPaused = false;
    public bool bossStage = false;
    public bool fragmentBoss = false;

    public List<spaceObject> meteors = new List<spaceObject>();

    public Volume V_warning;
    float incWeight = 1f;



    float autoSaveTimer = 0f;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Stats.Initialize();
            Settings.Init();

            Data.Init();

            QuestStats.Init();
            QuestManager.Init();

            SetPause(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is calledonce before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Application.targetFrameRate = 60;
        V_warning.gameObject.SetActive(false);
        InitGame();
        LoadStage();

    }

    private void calculMeteorToKill()
    {
        float n = Stats.Instance.enemyPerStage * 100f;
        meteorToKill = (int)(n / 100f);
        if(n % 100 > UnityEngine.Random.Range(0, 100))
        {
            meteorToKill++;
        }
        MainUi.Instance.upStage();
    }

    public void InitGame()
    {
        SetWorldScale();
    }

    public void SetWorldScale()
    {
        setMeteorScale();
        spaceShip.instance.setAreaScale();
        spaceShip.instance.setScale();
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
            CheckStageBoss();
            if(!bossStage){
                spawnSpaceObject();
                timer = 0f;
            }
        }
        updateStage();
        if(autoSaveTimer > 10f)
        {
            Stats.Instance.save();
            autoSaveTimer = 0f;
        }
        updateWarning();
    }

    public void updateWarning()
    {
        if (V_warning.gameObject.activeSelf)
        {
            V_warning.weight += incWeight * Time.deltaTime;
            if(V_warning.weight <= 0f)
            {
                V_warning.weight = 0f;
                incWeight = Mathf.Abs(incWeight);
            }
            else if (V_warning.weight >= 1f)
            {
                V_warning.weight = 1f;
                incWeight = -Mathf.Abs(incWeight);
            }

        }
    }

    public void updateStage()
    {
        if (meteorKilled >= meteorToKill)
        {
            upStage();

        }
    }

    public void upStage()
    {
        //end stage

        Ship.Current.stage++;
        if (Stats.Instance.stageSkipProb > Random.Range(0, 100))
        {
            Ship.Current.stage++;
            getStageReward(1.70f, 0.75f);
            MainUi.Instance.ShowStageSkip();
        }
        getStageReward(1.95f);
        MainUi.Instance.upStage();
        LoadStage();


        if (QuestManager.Instance.type == QuestType.Speed && !(QuestManager.Instance.isCompleted()))
        {
            if (new BigNumber(Ship.Current.stage).isBigger(QuestManager.Instance.objectif))
            {
                QuestStats.Instance.timeCompleted = Data.Instance.time;
            }
        }
    }

    public void LoadStage()
    {
        meteorKilled = 0;
        calculMeteorToKill();
        if (MainUi.Instance.enemyLabel != null) MainUi.Instance.enemyLabel.text = meteorToKill.ToString() + "/" + meteorToKill.ToString();

        CheckStageBoss();
        MainUi.Instance.updateStage();

        MainUi.Instance.ShowBossLife(bossStage);


    }

    public void CheckStageBoss() {
        if (!bossStage && Ship.Current.stage % Stats.BOSS_STAGE_GAP == 0 && !isPaused)
        {
            SpawnBoss();
        }
        else if(Ship.Current.stage % Stats.BOSS_STAGE_GAP != 0)
        {
            MainUi.Instance.ShowBossLife(false);
        }
    }

    public void SpawnBoss(bool FragmentBoss = false)
    {
        fragmentBoss = FragmentBoss;
        DestroyMeteors();
        bossStage = true;
        int level = FragmentBoss ? Ship.Current.fragmentlevel : Ship.Current.stage;
        SpawnBossMeteor(normalBossPrefab, meteorBoss.BossType.Normal, level);
        if (MainUi.Instance.enemyLabel != null) MainUi.Instance.enemyLabel.text = "BOSS";
        MainUi.Instance.ShowBossLife(true);
        meteorToKill = 1;
    }

    public void getStageReward(float posY, float fontFactor = 1f)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(1.3f * (Screen.width / 2f), posY * (Screen.height / 3f), 10));
        float reward;
        MarkerType type;
        if (Random.Range(0, 2) == 1)
        {//BN_xp
            reward = Ship.Current.stage * 1.25f * 5;
            if (Stats.Instance.xpBoostTime > 0)
                reward *= 2;
            Ship.Current.AddXP(new BigNumber(reward));
            type = MarkerType.Xp;
        }
        else 
        {
            if(Ship.Current.HaveUranium() && Random.Range(0, 2) == 1)
            {//uranium 
                reward = (int)(Ship.Current.stage * 0.5f);
                type = MarkerType.Uranium;
                Stats.Instance.AddUranium(new BigNumber(reward));

            }
            else
            {//iron
                reward  = (int)(Ship.Current.stage * 2.5f);
                type = MarkerType.Iron;
                Stats.Instance.AddIron(new BigNumber(reward));
            }
        }
        MarkersUI.Instance.ShowMarker(worldPos, "stage reward : " + reward.ToString(), type, 0.1f, 0.985f, fontFactor);
    }

    public void SmallVibrate()
    {
        /*
            Called : meteor destroy, upLevel machine/upgrade, on collectible collected
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
        int stage = Ship.Current.stage;

        int BigProb = stage > 25 ? 200 : 0;
        int ScatterProb = stage < 10 ? 0 : 200;
        int uraniumProb = Ship.Current.HaveUranium() && stage > 20 ? 40 : 0;
        int ironProb = stage > 10 ? 40 : 0;
        int normalProb = Mathf.Max(0, 1000 - (ScatterProb + BigProb + ironProb + uraniumProb + Stats.Instance.diamandProb));

        Dictionary<spaceObject, int> probabilites = new Dictionary<spaceObject, int>
            {
                {meteorPredab, normalProb},
                {ScatterMeteorPrefab, ScatterProb},
                {BigMeteorPrefab, BigProb},
                {ironMeteorPrefab, ironProb},
                {uraniumMeteorPrefab, uraniumProb},
                {DiamandMeteorPrefab, Stats.Instance.diamandProb },
            };
        SpawnWithProbability(probabilites);
    }
    
    private void SpawnWithProbability(Dictionary<spaceObject, int> probabilites)
    {
        if (probabilites.Values.Sum() > 1000)
            Debug.LogWarning("Probability sum above 1000");

        int x = UnityEngine.Random.Range(0, 1000);
        int sumProb = 0;

        foreach (var elem in probabilites)
        {
            sumProb += elem.Value;
            if(x < sumProb)
            {
                SpawnMeteor(elem.Key);
                return;
            }
        }
    }

    private void SpawnMeteor(spaceObject meteorPredab)
    {
        spaceObject obj = Instantiate(meteorPredab);
        obj.level = Ship.Current.stage;
        obj.Init();
        meteors.Add(obj);
    }

    public void SpawnMeteor(spaceObject meteorPredab, meteorType type, Vector3 position)
    {
        spaceObject obj = Instantiate(meteorPredab);
        obj.type = type;
        obj.transform.position = position;
        obj.level = Ship.Current.stage;
        obj.Init(false);
        meteors.Add(obj);
    }

    private void SpawnBossMeteor(meteorBoss bossPrefab, meteorBoss.BossType type, int level)
    {
        meteorBoss obj = Instantiate(bossPrefab);
        obj.bossType = type;
        obj.level = level;
        obj.Init();
        meteors.Add(obj);
    }

    public void SetPause(bool isPause)
    {
        isPaused = isPause;
        canon.instance.setPause(isPause);
        if (isPause && Settings.Instance.isPausable)
        { 
            foreach (spaceObject m in meteors)
            {
                m.Pause();
            }
            timerSave = timer;
            timer = -1000f;
        }
        else if(!isPause && !bossStage)
        {
            if(timerSave > 0f)
                timer = timerSave;
            else
                timer = 0f;

            foreach (spaceObject meteor in meteors)
            {
                if (meteor.isPause)
                    meteor.Move();
            }
            canon.instance.setPause(false);
        }
    }

    public void DestroyMeteors()
    {
        foreach (spaceObject obj in meteors)
        {
            Destroy(obj.gameObject);
        }
    }
    public void RestartStage()
    {
        DestroyMeteors();

        meteorKilled = 0;
        Ship.Current.life = new BigNumber(spaceShip.instance.getMaxLife());
        Ship.Current.shield = new BigNumber(spaceShip.instance.getMaxShield());

        MainUi.Instance.enemyLabel.text = meteorToKill.ToString();
        MainUi.Instance.healthBar.style.width = Length.Percent(100);
        MainUi.Instance.updateStage();
    }
    public void launchMiniMeteor(Transform trans)
    {
        spaceObject obj = Instantiate(miniMeteorPrefab);
        spaceObject obj2 = Instantiate(miniMeteorPrefab);
        spaceObject obj3 = Instantiate(miniMeteorPrefab);
        meteors.Add(obj);
        meteors.Add(obj2);
        meteors.Add(obj3);
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

    public void activeWarning(bool active)
    {
        V_warning.gameObject.SetActive(active);
        if(active)
            V_warning.weight = 0f;
    }

    public void setMeteorScale()
    {
        foreach(spaceObject meteor in meteors)
        {
            meteor.transform.localScale = meteor.baseScale;
            meteor.transform.localScale *= Stats.Instance.scale;
        }
    }
}

