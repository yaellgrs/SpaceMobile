using UnityEngine;
using UnityEngine.UIElements;

public class BossFragmentUi : MonoBehaviour
{

    #region variables
    [SerializeField] private UIDocument document;

    private Button Btn_close;
    private Button Btn_fight;
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

        VisualElement main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        Btn_close = root.Q<Button>("exit");
        Btn_fight = root.Q<Button>("fight");

        Btn_close.clicked += Close;
        Btn_fight.clicked += FightBoss;
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
    }
}
