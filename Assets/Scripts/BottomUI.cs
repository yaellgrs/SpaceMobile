using UnityEngine;
using UnityEngine.UIElements;

public enum SelectedMenu { None, SecondForge, MainForge, Prestige }

public class BottomUI : MonoBehaviour
{
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static BottomUI Instance;

    [SerializeField] private UIDocument document;

    private VisualElement VE_SecondForge;
    private VisualElement VE_MainForge;
    private VisualElement VE_Prestige;

    private VisualElement VE_hide;

    SelectedMenu menu = SelectedMenu.None;

    private void OnEnable()
    {
        var root = document.rootVisualElement;
        VE_SecondForge = root.Q<VisualElement>("secondForge");
        VE_MainForge = root.Q<VisualElement>("mainForge");
        VE_Prestige = root.Q<VisualElement>("prestige");
        VE_hide = root.Q<VisualElement>("hide");

        Ship.Current.OnTypeChanged += LoadUI;
    }

    public void LoadUI()
    {
        string firstForgePath = Stats.Instance.ironUnlocked ? "UI/Bottom/" + Ship.Current.type + "/FirstForge" 
                                                            : "UI/Bottom/Wood/firstForgeLocked";

        string secondForgePath = Ship.Current.type == SpaceShipData.SpaceShipElement.Wood ?
            "UI/Bottom/Wood/SecondForge" : "UI/Bottom/SecondForge";
        string prestigePath = Stats.Instance.prestigeUnlocked ? "UI/Bottom/prestige" : "UI/Bottom/prestigeLocked";
        VE_MainForge.style.backgroundImage = Resources.Load<Texture2D>(firstForgePath);
        VE_SecondForge.style.backgroundImage = Resources.Load<Texture2D>(secondForgePath);
        VE_Prestige.style.backgroundImage = Resources.Load<Texture2D>(prestigePath);

    }

    public void OpenMenu(SelectedMenu menuToOpen)
    {
        if (menu == SelectedMenu.SecondForge) VE_SecondForge.RemoveFromClassList("transition");
        else if (menu == SelectedMenu.MainForge) VE_MainForge.RemoveFromClassList("transition");
        else if (menu == SelectedMenu.Prestige) VE_Prestige.RemoveFromClassList("transition");

        if (menuToOpen == SelectedMenu.SecondForge) VE_SecondForge.AddToClassList("transition");
        else if (menuToOpen == SelectedMenu.MainForge) VE_MainForge.AddToClassList("transition");
        else if (menuToOpen == SelectedMenu.Prestige) VE_Prestige.AddToClassList("transition");

        menu = menuToOpen;
    }

    public void Show(bool show)
    {
        document.rootVisualElement.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Hide(bool hide)
    {
        VE_hide.style.visibility = hide ? Visibility.Visible : Visibility.Hidden;
    }


    private void OnDisable()
    {
        
    }
}
