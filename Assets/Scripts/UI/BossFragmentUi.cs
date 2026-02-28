using System;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UIElements;

public class BossFragmentUi : MonoBehaviour
{
    #region constantes
    private static int MAX_FRAGMENT_LEVEL = 20;
    #endregion

    #region variables
    [SerializeField] private UIDocument document;

    private VisualElement VE_buttons;

    private Button Btn_close;
    private Button Btn_fight;
    private Button Btn_skip;
    private Button Btn_nextLevel;
    private Button Btn_lastLevel;

    private Label Lbl_fragmentLevel;
    private Label Lbl_reward;
    private Label Lbl_timer;

    private int currentLevel = 1;
    public bool canExit = true;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        document.gameObject.SetActive(false); 
    }

    public void Open()
    {
        document.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        var root = document.rootVisualElement;
        if(gameManager.instance != null) gameManager.instance.SetPause(true);

        #region transition
        VisualElement main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);
        #endregion

        VE_buttons = root.Q<VisualElement>("buttons");
        Btn_close = root.Q<Button>("exit");
        Btn_skip = root.Q<Button>("skip");
        Btn_fight = root.Q<Button>("fight");
        Btn_nextLevel = root.Q<Button>("nextLevel");
        Btn_lastLevel = root.Q<Button>("lastLevel");
        Lbl_fragmentLevel = root.Q<Label>("fragmentLevel");
        Lbl_reward = root.Q<Label>("reward");
        Lbl_timer = root.Q<Label>("timer");

        currentLevel = Ship.Current.fragmentlevel;
        LoadFragmentLevelUI();
        LoadDailyReward();

        Btn_nextLevel.clicked += () => { upFragmentLevel(1); };
        Btn_lastLevel.clicked += () => { upFragmentLevel(-1); };
        Btn_close.clicked += Close;
        Btn_fight.clicked += FightBoss;
        Btn_skip.clicked += getReward;
    }

    private void upFragmentLevel(int amount = 1)
    {
        currentLevel = Mathf.Clamp(currentLevel + amount, 1, MAX_FRAGMENT_LEVEL);
        LoadFragmentLevelUI();
    }

    private void LoadFragmentLevelUI()
    {
        Btn_lastLevel.enabledSelf = currentLevel != 1;
        Btn_nextLevel.enabledSelf = currentLevel < Mathf.Min(Ship.Current.fragmentlevel, MAX_FRAGMENT_LEVEL);
        Btn_fight.enabledSelf = currentLevel == Ship.Current.fragmentlevel;

        Lbl_fragmentLevel.text = "level " + currentLevel;
        Lbl_reward.text = calculReward().ToString();
    }

    private void LoadDailyReward()
    {
        bool canFight = CanFight();
        VE_buttons.style.display = canFight ? DisplayStyle.Flex : DisplayStyle.None;
        Lbl_timer.style.display = canFight ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void Update()
    {
        if (!CanFight())
        {
            Lbl_timer.text = "You can fight in " + Utility.TimeToString_hm(Utility.DAY_IN_SECOND - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Ship.Current.lastFragmentFight));
        }
    }

    private bool CanFight()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Ship.Current.lastFragmentFight >= Utility.DAY_IN_SECOND;
    }

    private void getReward()
    {
        Ship.Current.lastFragmentFight = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        LoadDailyReward();

        int reward = calculReward();
        Stats.Instance.AddShipFragment(reward);
        Vector2 panelPos = new Vector2(Lbl_reward.worldBound.position.x *1.25f, Lbl_reward.worldBound.position.y);
        MarkersUI.Instance.ShowMarker(panelPos, "+" + reward.ToString(), MarkerType.Diamand);

        canExit = true;
        //PoolManager.Instance.LaunchPrefab(pos, calculReward().ToString(), MarkerType.Damage);
    }

    private int calculReward()
    {
        return currentLevel;
    }

    private void FightBoss()
    {
        MainUi.Instance.setGameUI(false);
        gameManager.instance.SpawnBoss(true);
        Close();
    }

    public static void EndFragmentBoss(bool win)
    {

        MainUi.Instance.setGameUI(true);
        gameManager.instance.LoadStage();
        gameManager.instance.SetPause(false);

        MainUi.Instance.bossFragmentUi.canExit = false;
        MainUi.Instance.bossFragmentUi.Open();
        MainUi.Instance.bossFragmentUi.StartCoroutine(MainUi.Instance.bossFragmentUi.EndFragmentBossDelay());
    }

    private IEnumerator EndFragmentBossDelay()
    {
        yield return new WaitForSeconds(1f);
        getReward();
        upFragmentLevel();
    }

    private void Close()
    {
        if (!canExit) return;
        var root = document.rootVisualElement;
        VisualElement main = root.Q<VisualElement>("main");

        main.RemoveFromClassList("trans");
        main.schedule.Execute(() => 
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            gameManager.instance.SetPause(false);
            document.gameObject.SetActive(false);

        }).StartingIn(400);

    }

    private void OnDisable()
    {
        Btn_nextLevel.clicked -= () => { upFragmentLevel(1); };
        Btn_lastLevel.clicked -= () => { upFragmentLevel(-1); };
        Btn_close.clicked -= Close;
        Btn_fight.clicked -= FightBoss;
        Btn_skip.clicked -= getReward;
    }
}
