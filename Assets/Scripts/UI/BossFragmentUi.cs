using UnityEngine;
using UnityEngine.UIElements;

public class BossFragmentUi : MonoBehaviour
{
    #region constantes
    private static int MAX_FRAGMENT_LEVEL = 20;
    #endregion

    #region variables
    [SerializeField] private UIDocument document;

    private Button Btn_close;
    private Button Btn_fight;
    private Button Btn_nextLevel;
    private Button Btn_lastLevel;

    private Label Lbl_fragmentLevel;
    private Label Lbl_reward;

    private int currentLevel = 1;

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
        gameManager.instance.SetPause(true);

        #region transition
        VisualElement main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);
        #endregion

        Btn_close = root.Q<Button>("exit");
        Btn_fight = root.Q<Button>("fight");
        Btn_nextLevel = root.Q<Button>("nextLevel");
        Btn_lastLevel = root.Q<Button>("lastLevel");
        Lbl_fragmentLevel = root.Q<Label>("fragmentLevel");
        Lbl_reward = root.Q<Label>("reward");

        currentLevel = Ship.Current.fragmentlevel;
        LoadFragmentLevelUI();

        Btn_nextLevel.clicked += () => { upFragmentLevel(1); };
        Btn_lastLevel.clicked += () => { upFragmentLevel(-1); };
        Btn_close.clicked += Close;
        Btn_fight.clicked += FightBoss;
    }

    private void upFragmentLevel(int amount)
    {
        currentLevel = Mathf.Clamp(currentLevel + amount, 1, MAX_FRAGMENT_LEVEL);
        LoadFragmentLevelUI();
    }

    private void LoadFragmentLevelUI()
    {
        Btn_lastLevel.enabledSelf = currentLevel != 1;
        Btn_nextLevel.enabledSelf = currentLevel < Mathf.Min(Ship.Current.fragmentlevel, MAX_FRAGMENT_LEVEL);
        Btn_fight.enabledSelf = currentLevel == Ship.Current.fragmentlevel;
        Debug.Log("current : " + currentLevel + " level : " + Ship.Current.fragmentlevel);

        Lbl_fragmentLevel.text = "level " + currentLevel;
        Lbl_reward.text = getReward().ToString();
    }

    private int getReward()
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
    }

    private void Close()
    {
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
        Btn_close.clicked -= Close;
        Btn_fight.clicked -= FightBoss;
    }
}
