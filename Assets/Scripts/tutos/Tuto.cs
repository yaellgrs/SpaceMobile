using UnityEngine;
using UnityEngine.UIElements;

public class Tuto: MonoBehaviour
{
    public static Tuto Instance;

    public UIDocument ironUI;
    public UIDocument tutoPopupUI;

    Button exit;
    Button back;
    VisualElement main;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ironUI.gameObject.SetActive(false);
        tutoPopupUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadIronForgeTuto()
    {
        ironUI.gameObject.SetActive(true);
        var root = ironUI.rootVisualElement;

        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");
        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        back.clicked += exitIronTuto;
        exit.clicked += exitIronTuto;

        VisualElement forgeElements = root.Query<VisualElement>("forge");
        forgeElements.style.display = DisplayStyle.None;


        VisualElement upgrades = root.Query<VisualElement>("upgrade");
        upgrades.style.display = DisplayStyle.None;
        
    }

    private void exitIronTuto()
    {
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            main.style.visibility = Visibility.Hidden;
            exit.style.visibility = Visibility.Hidden;
        }).StartingIn(400);

        main.style.visibility = Visibility.Hidden;
        exit.style.visibility = Visibility.Hidden;

        VisualElement forgeElements = ironUI.rootVisualElement.Query<VisualElement>("forge");
        forgeElements.style.display = DisplayStyle.Flex;
    }

    public void loadIronUpgradeTuto() {

        var root = ironUI.rootVisualElement;
        VisualElement forgeElements = root.Query<VisualElement>("forge");

        MainUi.Instance.ironUI.forgeUpgradeClicked();

        Stats.Instance.upIron(new BigNumber(160), true);

        forgeElements.style.display = DisplayStyle.None;


        VisualElement upgrades = root.Query<VisualElement>("upgrade");
        upgrades.style.display = DisplayStyle.Flex;
    }   

    public void ironCloseTuto()
    {
        ironUI.gameObject.SetActive(false);
        Stats.Instance.ironTuto = true;
    }

    public void LoadIronMeteorTuto()
    {
        tutoPopupUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);
        gameManager.instance.DestroyMeteors();

        var root = tutoPopupUI.rootVisualElement;

        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");
        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        Stats.Instance.ironMeteorTuto = true;

        exit.clicked += CloseIronMeteorTuto;
        back.clicked += CloseIronMeteorTuto;
    }

    private void CloseIronMeteorTuto()
    {
        exit.pickingMode = PickingMode.Ignore;
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            tutoPopupUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(400);
    }
}
