using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{

    public UIDocument shopUI;
    public UIDocument buyUI;

    private VisualElement main;

    private Button switchButton;
    private Button exit;
    private Button back;
    private bool isActive = false;

    private Label diamand;

    private ScrollView boostScroll;
    private ScrollView timeScroll;
    private Button boostBtn;
    private Button timeBtn;

    private List<Boost> boostTime=  new List<Boost>();
    private List<Boost> boosts=  new List<Boost>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initBoost();
        initBoostTime();
        shopUI.gameObject.SetActive(false);
        buyUI.gameObject.SetActive(false);
        if(Stats.Instance.xpBoostTime > 0)
        {
            Debug.Log("boost xp :  " + Stats.Instance.xpBoostTime);
        }
        if (Stats.Instance.damageBoostTime > 0)
        {
            Debug.Log("boost damage :  " + Stats.Instance.damageBoostTime);
        }
        if (Stats.Instance.pvShieldBoostTime > 0)
        {
            Debug.Log("boost pv and shield :  " + Stats.Instance.pvShieldBoostTime);
        }
        if (Stats.Instance.ressourcesBoostTime > 0)
        {
            Debug.Log("boost ressources :  " + Stats.Instance.ressourcesBoostTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("+ 50 diamand grom ShopUI");
            Stats.Instance.upDiamand(50, true);
            Stats.Instance.upIron(new BigNumber(1, 100), true);
            MainUi.Instance.xpUI.loadBonus();
        }
        if(Stats.Instance.damageBoostTime > 0)
        {
            Stats.Instance.damageBoostTime-= Time.deltaTime;
        }
        if (Stats.Instance.xpBoostTime > 0)
        {
            Stats.Instance.xpBoostTime -= Time.deltaTime;
        }
        if (Stats.Instance.pvShieldBoostTime > 0)
        {
            Stats.Instance.pvShieldBoostTime -= Time.deltaTime;
        }
        if (Stats.Instance.ressourcesBoostTime > 0)
        {
            Stats.Instance.ressourcesBoostTime -= Time.deltaTime;
        }
    }

    private void initBoost()
    {
        Boost damage = new Boost()
        {
            time = 1,
            price = 50,
            name = "damage",
            type = Boost.Type.damage,
            shopUI = this
        };
        Boost xp = new Boost()
        {
            time = 1,
            price = 50,
            name = "xp",
            type = Boost.Type.xp,
            shopUI = this
        };
        Boost pvShield = new Boost()
        {
            time = 1,
            price = 50,
            name = "pvShield",
            type = Boost.Type.pvShield,
            shopUI = this
        };
        Boost ressources = new Boost()
        {
            time = 1,
            price = 50,
            name = "ressources",
            type = Boost.Type.ressources,
            shopUI = this
        };

        boosts.Add(damage);
        boosts.Add(xp);
        boosts.Add(pvShield);
        boosts.Add(ressources);
    }

    private void initBoostTime()
    {
        Boost time1 = new Boost()
        {
            time = 1,
            price = 25,
            name = "time1",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time2 = new Boost()
        {
            time = 6,
            price = 50,
            name = "time2",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time3 = new Boost()
        {
            time = 1,
            price = 75,
            name = "time3",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time4 = new Boost()
        {
            time = 1,
            price = 100,
            name = "time4",
            type = Boost.Type.time,
            shopUI = this
        };
        boostTime.Add(time1);
        boostTime.Add(time2);
        boostTime.Add(time3);
        boostTime.Add(time4);
    }

    public void loadShop()
    {
        shopUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);

        var root = shopUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        if (!isActive)
        {
            main.AddToClassList("trans");
            main.schedule.Execute(() =>
            {
                main.RemoveFromClassList("trans");
            }).StartingIn(50);
            isActive = true;
        }

        back = root.Q<Button>("back");
        exit = root.Q<Button>("exit");
        switchButton = root.Q<Button>("switch");
        diamand = root.Q<Label>("diamand");

        boostScroll = root.Q<ScrollView>("boostScroll");
        timeScroll = root.Q<ScrollView>("timeScroll");
        boostBtn = root.Q<Button>("boostBtn");
        timeBtn = root.Q<Button>("timeBtn");

        timeScroll.style.display = DisplayStyle.None;
        boostScroll.style.display = DisplayStyle.Flex;
        boostBtn.AddToClassList("buttonShopTrans");
        timeBtn.clicked += ButtonShop;

        switchButton.clicked += Switch;
        back.clicked += Close;
        exit.clicked += Close;

        foreach (Boost boost in boosts)
        {
            boost.load(shopUI);
        }

        upDiamand();
        LoadBoost();
    }

    private void setBorderColor(Button btn, Color color)
    {
        btn.style.borderLeftColor = color;
        btn.style.borderRightColor = color;
        btn.style.borderTopColor = color;
        btn.style.borderBottomColor = color;
    }

    private void ButtonShop()
    {
        Debug.Log("click");
        if (timeScroll.style.display == DisplayStyle.None)
        {
            LoadTime();
        }
        else
        {
            LoadBoost();
        }
    }

    private void LoadTime()
    {
        Debug.Log("load time");

        timeScroll.style.display = DisplayStyle.Flex;
        boostScroll.style.display = DisplayStyle.None;
        boostBtn.RemoveFromClassList("buttonShopTrans");
        timeBtn.AddToClassList("buttonShopTrans");
        timeBtn.clicked -= ButtonShop;
        boostBtn.clicked -= ButtonShop;
        boostBtn.clicked += ButtonShop;

        setBorderColor(timeBtn, Color.white);
        setBorderColor(boostBtn, Color.black);

        foreach (Boost boost in boostTime)
        {
            boost.load(shopUI);
        }
    }

    private void LoadBoost()
    {
        Debug.Log("load boost");

        timeScroll.style.display = DisplayStyle.None;
        boostScroll.style.display = DisplayStyle.Flex;
        boostBtn.AddToClassList("buttonShopTrans");
        timeBtn.RemoveFromClassList("buttonShopTrans");
        timeBtn.clicked -= ButtonShop;
        timeBtn.clicked += ButtonShop;
        boostBtn.clicked -= ButtonShop;

        setBorderColor(boostBtn, Color.white);
        setBorderColor(timeBtn, Color.black);

        foreach (Boost boost in boosts)
        {
            boost.load(shopUI);
        }
    }

    public void loadBuy()
    {
        buyUI.gameObject.SetActive(true);

        var root = buyUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        back = root.Q<Button>("back");
        switchButton = root.Q<Button>("switch");
        diamand = root.Q<Label>("diamand");

        switchButton.clicked += Switch;
        back.clicked += Close;
        upDiamand();

    }




    private void upDiamand()
    {
        string dmd = Stats.Instance.diamand.ToString();
        diamand.style.width = new Length(7.5f*dmd.Length, LengthUnit.Percent);
        diamand.text = dmd;
    }

    private void Switch()
    {
        if (shopUI.gameObject.activeSelf) {
            shopUI.gameObject.SetActive(false);
            loadBuy();
        }
        else
        {
            buyUI.gameObject.SetActive(false);
            loadShop();
        }
    }

    public void Close()
    {

        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            shopUI.gameObject.SetActive(false);
            buyUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
            isActive = false;
        }).StartingIn(400);
    }
}
