using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;


public enum PopupTuto
{
    ironMeteor, uraniumMeteor, diamandMeteor, splitterMeteor, BigMeteor, rocket
    //uranium tuto ? 
    //prestige ? 
};

public class Tuto: MonoBehaviour
{
    public static Tuto Instance;

    public UIDocument ironUI;
    public UIDocument tutoPopupUI;

    Label Lbl_title;
    Label Lbl_description;
    VisualElement VE_image;
    Button Btn_exit;
    Button Btn_back;
    VisualElement VE_main;

    private int machineClicked = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ironUI.gameObject.SetActive(false);
        tutoPopupUI.gameObject.SetActive(false);
        loadPopupTuto();
    }

    public void loadPopupTuto()
    {
        foreach (PopupTuto tuto in Enum.GetValues(typeof(PopupTuto)))
            if (!Stats.Instance.popupTutos.ContainsKey(tuto)) Stats.Instance.popupTutos[tuto] = false;
    }

    public void AddMachineClicked()
    {
        machineClicked++;
        if(machineClicked == 3) loadIronUpgradeTuto(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadForgeTuto(bool isIron)
    {
        VisualElement root;
        ironUI.gameObject.SetActive(true);


        root = ironUI.rootVisualElement;

        Btn_exit = root.Q<Button>("exit");
        Btn_back = root.Q<Button>("back");
        VE_main = root.Q<VisualElement>("main");

        VE_main.AddToClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.RemoveFromClassList("trans");
        }).StartingIn(50);

        Btn_back.clicked += ()=> { exitIronTuto(isIron); };
        Btn_exit.clicked += () => { exitIronTuto(isIron); };

        VisualElement forgeElements = root.Query<VisualElement>("forge");
        forgeElements.style.display = DisplayStyle.None;


        VisualElement upgrades = root.Query<VisualElement>("upgrade");
        upgrades.style.display = DisplayStyle.None;

    }

    private void exitIronTuto(bool isIron)
    {
        VE_main.RemoveFromClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.AddToClassList("trans");
        }).StartingIn(50);
        VE_main.schedule.Execute(() =>
        {
            VE_main.style.visibility = Visibility.Hidden;
            Btn_exit.style.visibility = Visibility.Hidden;
        }).StartingIn(400);

        VE_main.style.visibility = Visibility.Hidden;
        Btn_exit.style.visibility = Visibility.Hidden;

            var forgeElement = ironUI.rootVisualElement.Q<VisualElement>("forge");
            forgeElement.style.display = DisplayStyle.Flex;
    }

    public void loadIronUpgradeTuto(bool isIron) {

        VisualElement root;
        
        root = ironUI.rootVisualElement;

        VisualElement forgeElements = root.Query<VisualElement>("forge");

/*        if(isIron)
            MainUi.Instance.ironUI.forgeUpgradeClicked();
        else
            MainUi.Instance.uraniumUI.forgeUpgradeClicked();*/

        forgeElements.style.display = DisplayStyle.None;

        VisualElement upgrades = root.Query<VisualElement>("upgrade");
        upgrades.style.display = DisplayStyle.Flex;
    }   

    public void ironCloseTuto(bool isIron)
    {
        ironUI.gameObject.SetActive(false);
        if (isIron){

            Stats.Instance.ironTuto = true;
        }
        else{
            Stats.Instance.uraniumTuto = true;
        }
    }

    public void LoadPopupTuto(PopupTuto type)
    {
        tutoPopupUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);
        //gameManager.instance.DestroyMeteors();

        var root = tutoPopupUI.rootVisualElement;

        Btn_exit = root.Q<Button>("exit");
        Btn_back = root.Q<Button>("back");
        Lbl_description = root.Q<Label>("description");
        Lbl_title = root.Q<Label>("title");
        VE_main = root.Q<VisualElement>("main");
        VE_image = root.Q<VisualElement>("image");

        VE_main.AddToClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.RemoveFromClassList("trans");
        }).StartingIn(50);

        SetPopup(type);

        Btn_exit.clicked += CloseIronMeteorTuto;
        Btn_back.clicked += CloseIronMeteorTuto;
    }
    private void SetPopup(PopupTuto type)
    {
        Stats.Instance.popupTutos[type] = true;
        
        Color color = Color.gray;
        string titlekey = "";
        string descKey = "";
        string path = "tuto/";
        switch (type)
        {
            case PopupTuto.ironMeteor:
                titlekey = descKey = "ironMeteor";
                path += "ironMeteor";
                ColorUtility.TryParseHtmlString("#610000", out color);
                break;
            case PopupTuto.uraniumMeteor:
                titlekey = descKey = "uraniumMeteor";
                path += "uraniumMeteor";
                ColorUtility.TryParseHtmlString("#006123", out color);
                break;
            case PopupTuto.diamandMeteor:
                titlekey = descKey = "diamandMeteor";
                path += "diamandMeteor";
                ColorUtility.TryParseHtmlString("#004B4B", out color);
                break;
            case PopupTuto.splitterMeteor:
                titlekey = descKey = "splitterMeteor";
                path += "splitterMeteor";
                ColorUtility.TryParseHtmlString("#300049", out color);
                break;
            case PopupTuto.BigMeteor:
                titlekey = descKey = "BigMeteor";
                path += "BigMeteor";
                ColorUtility.TryParseHtmlString("#292627", out color);
                break;
            case PopupTuto.rocket:
                titlekey = descKey = "rocket";
                path += "rocket";
                ColorUtility.TryParseHtmlString("#292627", out color);
                break;
        }

        //image
        VE_image.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>(path));

        //title
        titlekey = "Tuto_title_" + titlekey;
        var titleLoc = new LocalizedString("UI_tutos", titlekey);
        titleLoc.StringChanged += (localizedValue) =>
        {
            Lbl_title.text = localizedValue;
        };

        //description
        descKey = "Tuto_description_" + descKey;

        var descLoc = new LocalizedString("UI_tutos", descKey);
        descLoc.StringChanged += (localizedValue) =>
        {
            Lbl_description.text = localizedValue;
        };

        //color
        VE_main.style.backgroundColor = color;
    }


    private void CloseIronMeteorTuto()
    {
        Btn_exit.pickingMode = PickingMode.Ignore;
        VE_main.RemoveFromClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.AddToClassList("trans");
        }).StartingIn(50);
        VE_main.schedule.Execute(() =>
        {
            tutoPopupUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(400);
    }
}
