using UnityEngine;
using UnityEngine.UIElements;

public class LoadAdsUI : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    #region Instance
    public static LoadAdsUI Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);    
    }
    #endregion

    private Label Lbl_time;
    private Label Lbl_error;
    private Button Btn_exit;
    private Button Btn_back;


    private float LoadingTime = 0f;

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        if(document.gameObject.activeInHierarchy) return;
        document.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        var root = document.rootVisualElement;

        Lbl_time = root.Q<Label>("time");
        Lbl_error = root.Q<Label>("error");
        Btn_exit = root.Q<Button>("exit");
        Btn_back = root.Q<Button>("back");

        LoadingTime = 0f;

        Lbl_error.style.display = DisplayStyle.None;

        Btn_exit.clicked += Close;
        Btn_back.clicked += Close;
    }

    public void SetError()
    {
        Lbl_time.style.display = DisplayStyle.None;
        Lbl_error.style.display = DisplayStyle.Flex;
        Btn_exit.style.display = DisplayStyle.None;
    }

    public void Update()
    {
        if (!document.gameObject.activeInHierarchy) return;

        LoadingTime += Time.deltaTime;
        Lbl_time.text = LoadingTime.ToString("F1") + "s";

    }

    private void OnDisable()
    {

        if(Btn_exit != null ) Btn_exit.clicked -= Close;
        if(Btn_back != null ) Btn_back.clicked -= Close;

    }

    public void Close()
    {
        if (!document.gameObject.activeInHierarchy) return;
        document.gameObject.SetActive(false);
    }
}
