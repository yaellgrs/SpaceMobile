using UnityEngine;
using UnityEngine.UIElements;

public class BossFragmentUi : MonoBehaviour
{

    #region variables
    [SerializeField] private UIDocument document;

    private Button Btn_close;
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

        Btn_close = root.Q<Button>("exit");

        Btn_close.clicked += Close;
    }

    private void Close()
    {
        document.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        gameManager.instance.SetPause(false);
        Btn_close.clicked -= Close;
    }
}
