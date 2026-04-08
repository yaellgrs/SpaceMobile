
using UnityEngine;
using UnityEngine.UIElements;

public class BorderUI : MonoBehaviour
{
    public static BorderUI Instance;
    public UIDocument document;

    VisualElement VE_main;
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

        VE_main = root.Q<VisualElement>("main");

        VE_main.AddToClassList("trans");
        VE_main.schedule.Execute(() =>
        {
            VE_main.RemoveFromClassList("trans");
        }).StartingIn(50);

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
            if (machine.data.level < machineData.levelColor[(int)val.Key]) break;
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

        if (VE_main != null)
        {
            VE_main.RemoveFromClassList("trans");
            VE_main.schedule.Execute(() =>
            {
                VE_main.AddToClassList("trans");
            }).StartingIn(50);
            VE_main.schedule.Execute(() =>
            {
                this.machine = null;
                document.gameObject.SetActive(false);
            }).StartingIn(500);
        }
        else
        {
            this.machine = null;
            document.gameObject.SetActive(false);
        }
    }
}
