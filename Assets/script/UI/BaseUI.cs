using UnityEngine;
using UnityEngine.UIElements;

public class BaseUI : MonoBehaviour
{

    public UIDocument forgeUI;
    public UIDocument upgradeUI;


    //ui général
    private Button backButton;
    private Button backButton2;
    private Button upgradeButton;
    private Button forgeButton;
    private Button upModeButton;

    protected VisualElement black;
    public VisualElement forgeUiVE;

    public int upModeMultiplicator = 1;

    //scroll
    private ScrollView scrollView;

    private bool isDragging = false;
    private Vector3 lastMousePosition;
    float back = 0;

    public bool classActived = true;

    public bool stopAnim = false;

    //machine 1
    /* Faire une class pour chaque machine ? */
    Machine machine1;
    Machine machine2;
    Machine machine3;

    Upgrades upgrade1;
    Upgrades upgrade2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        initializeMachine();
        initializeUpgrade();

    }

    public virtual void initializeMachine()
    {

    }

    public virtual void initializeUpgrade()
    {
        

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                scrolling();
            }

        }
    }

    public virtual void IronClicked()
    {
        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            forgeUI.gameObject.SetActive(false);
            upgradeUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }
        else
        {

            gameManager.instance.SetPause(true);

            loadForgeUI();
        }
    }

    public virtual void forgeUpgradeClicked()
    {
        if (forgeUI.gameObject.activeInHierarchy)
        {
            forgeUI.gameObject.SetActive(false);
            loadUpdateUI();
        }
        else
        {
            upgradeUI.gameObject.SetActive(false);
            loadForgeUI();


        }


    }
        
    private void scrolling()
    {
        if(scrollView != null)
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (!isDragging)
                {
                    lastMousePosition = Input.mousePosition;
                    back = 0f;
                    isDragging = true;
                }



            }
            else if (isDragging)
            {

                Vector3 delta = Input.mousePosition - lastMousePosition;

                if (scrollView.contentContainer.transform.position.y - delta.y * 2 < 25f)
                {
                    if (scrollView.contentContainer.transform.position.y - delta.y > -(Screen.height * 0.125f))
                    {
                        scrollView.contentContainer.transform.position -= new Vector3(0, delta.y) * 2;
                    }
                }
                lastMousePosition = Input.mousePosition;
                scrollView.MarkDirtyRepaint();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        if (!isDragging && scrollView != null)
        {
            if (scrollView.contentContainer.transform.position.y < -(Screen.height * 0.125f) * 3)
            {
                scrollView.contentContainer.transform.position += new Vector3(0, back / 15);
            }

        }
    }

    void upModeButtonClicked()
    {
        UpMode.Instance.UpButton(upModeButton);
    }


    /*
                FORGE
     */
    #region 
    public virtual void loadForgeUI()
    {
        forgeUI.gameObject.SetActive(true);

        var root = forgeUI.rootVisualElement;
        scrollView = root.Query<ScrollView>("scrollView");
        backButton = root.Q<Button>("back");
        backButton2 = root.Q<Button>("back2");
        upgradeButton = root.Q<Button>("upgrade");
        upModeButton = root.Q<Button>("upMode");
        black = root.Q<VisualElement>("black");
        UpMode.Instance.load(upModeButton);

        if(black != null)
        {
            black.style.visibility = Visibility.Visible;
        }


        backButton.clicked += IronClicked;
        backButton2.clicked += IronClicked;
        upgradeButton.clicked += forgeUpgradeClicked;
        upModeButton.clicked += upModeButtonClicked;

        isDragging = false;
    }

    #endregion
    /*
                FORGE
     */


    /*
              UPDATE
     */
    #region
    public virtual void loadUpdateUI()
    {
        upgradeUI.gameObject.SetActive(true);

        var root = upgradeUI.rootVisualElement;
        scrollView = root.Query<ScrollView>("scrollView");
        backButton = root.Q<Button>("back");
        backButton2 = root.Q<Button>("back2");
        forgeButton = root.Q<Button>("forge");
        upModeButton = root.Q<Button>("upMode");
        black = root.Q<VisualElement>("black");
        UpMode.Instance.load(upModeButton);

        if (black != null)
        {
            black.style.visibility = Visibility.Visible;
        }

        backButton.clicked += IronClicked;
        backButton2.clicked += IronClicked;
        forgeButton.clicked += forgeUpgradeClicked;
        if (upModeButton != null)
        {

            upModeButton.clicked += upModeButtonClicked;
        }


        isDragging = false;

    }
    #endregion
    /*
                UPDATE
     */

}