using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UIElements;

public class BorderUI : MonoBehaviour
{
    public static BorderUI Instance;
    public UIDocument document;

    VisualElement VE_container;
    Button Btn_close;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Close();
    }


    private machineElement machine;

    public void Open(machineElement machine)
    {
        this.machine = machine;
        document.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (machine == null) return;
        var root = document.rootVisualElement;

         VE_container = root.Q<VisualElement>("container");
        Btn_close = root.Q<Button>("close");

        Load();
        Btn_close.clicked += Close;

    }

    public void Load()
    {
        if(machine == null || VE_container == null) return;

        VE_container.Clear();

        foreach (var val in machine.data.borderbuys)
        {

            BuyBorderElement elem = new BuyBorderElement(machine, val.Key, val.Value);

            VE_container.Add(elem);
            if (val.Value == BorderBuyType.unbuyed) break;
        }
    }

    private void OnDisable()
    {
        
    }

    public void Close()
    {
        this.machine = null;
        document.gameObject.SetActive(false);
    }
}
