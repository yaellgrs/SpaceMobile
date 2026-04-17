using System;
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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBoost(Time.deltaTime);
    }

    public void UpdateBoost(float dt)
    {
        foreach (Boost.Type type in Enum.GetValues(typeof(Boost.Type)))
        {
            if (!Stats.Instance.boosts.ContainsKey(type))
                Stats.Instance.boosts.Add(type, (1f, 0f));
            if (Stats.Instance.boosts[type].time > 0)
            {
                var boost = Stats.Instance.boosts[type];
                boost.time -= dt;
                Stats.Instance.boosts[type] = boost;
            }
        }
    }

    private void initBoost()
    {
        Boost damage = new Boost()
        {
            time = 1,//hour
            price = 50,//diamands
            name = "damage",//name
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
            name = "prestige",
            type = Boost.Type.prestige,
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
            price = 10,
            name = "time1",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time2 = new Boost()
        {
            time = 6,
            price = 25,
            name = "time2",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time3 = new Boost()
        {
            time = 12,
            price = 50,
            name = "time3",
            type = Boost.Type.time,
            shopUI = this
        };
        Boost time4 = new Boost()
        {
            time = 24,
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

        DialogueManager.Instance.TryDialogue("FirstShopOpen");
        

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

        upDiamand();

        foreach (Boost boost in boosts)
        {
            boost.load(shopUI);
        }


        LoadBoost();
    }



    private void ButtonShop()
    {
        Debug.Log("click button shop");
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

        Utility.setBorderColor(timeBtn, Color.white);
        Utility.setBorderColor(boostBtn, Color.black);

        foreach (Boost boost in boostTime)
        {
            boost.load(shopUI);
        }
    }

    public void LoadBoost()
    {
        Debug.Log("load boost");

        timeScroll.style.display = DisplayStyle.None;
        boostScroll.style.display = DisplayStyle.Flex;
        boostBtn.AddToClassList("buttonShopTrans");
        timeBtn.RemoveFromClassList("buttonShopTrans");
        timeBtn.clicked -= ButtonShop;
        timeBtn.clicked += ButtonShop;
        boostBtn.clicked -= ButtonShop;

        Utility.setBorderColor(boostBtn, Color.white);
        Utility.setBorderColor(timeBtn, Color.black);

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
        exit = root.Q<Button>("exit");
        switchButton = root.Q<Button>("switch");
        diamand = root.Q<Label>("diamand");

        Button noAds = root.Q<Button>("noAds");
        if(IAPManager.Instance.CheckAds())
            noAds.SetEnabled(false);
        else
            noAds.clicked += () => {
                IAPManager.Instance.BuyRemoveAds();
            };

        root.Q<Button>("smallPack").clicked += () =>
        {
            IAPManager.Instance.BuyDiamandPack(DiamandPack.SMALL);
        };
        root.Q<Button>("mediumPack").clicked += () =>
        {
            IAPManager.Instance.BuyDiamandPack(DiamandPack.MEDIUM);
        };
        root.Q<Button>("bigPack").clicked += () =>
        {
            IAPManager.Instance.BuyDiamandPack(DiamandPack.BIG); 
        };
        root.Q<Button>("giantPack").clicked += () =>
        {
            IAPManager.Instance.BuyDiamandPack(DiamandPack.GIANT);
        };


        switchButton.clicked += Switch;
        back.clicked += Close;
        exit.clicked += Close;

        upDiamand();



    }




    public void upDiamand()
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
        gameManager.instance.SetPause(false);
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            shopUI.gameObject.SetActive(false);
            buyUI.gameObject.SetActive(false);

            isActive = false;
        }).StartingIn(400);
    }
}
