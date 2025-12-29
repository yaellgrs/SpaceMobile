using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AdsUI : MonoBehaviour
{

    public UIDocument adsUI;

    private VisualElement main;

    private Button exit;
    private Button back;
    private Button pub;

    private Label title;
    private Label Label_diamand;
    private Label time;
    private Label Label_uranium;
    private Label Label_iron;

    private VisualElement VE_uranium;
    private VisualElement VE_iron;

    public long pubDelay = 15;

    private const long pubDelayClose = 120; //2 minutes
    private const long pubDelayWatch = 900; //15 minutes

    private Ads.RewardType reward;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("next pub in : " + (pubDelay - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastPub)));
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("next pub in : " + (pubDelay - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastPub)));
    }


    public void load()
    {
        adsUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);

        var root = adsUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        exit = root.Q<Button>("exit");
        back = root.Q<Button>("back");
        pub = root.Q<Button>("pub");

        title = root.Q<Label>("title");
        Label_iron = root.Q<Label>("iron");
        Label_uranium = root.Q<Label>("uranium");
        Label_diamand = root.Q<Label>("diamand");
        VE_uranium = root.Q<VisualElement>("uraniumVE");
        VE_iron = root.Q<VisualElement>("ironVE");
        time = root.Q<Label>("time");

        setReward();

        exit.clicked -= Close;
        back.clicked -= Close;
        exit.clicked += Close;
        back.clicked += Close;

        pub.clicked -= WatchPub;
        pub.clicked += WatchPub;

    }

    private void setReward()
    {
        if(UnityEngine.Random.Range(0,2) == 0)
        {
            Label_diamand.style.visibility = Visibility.Hidden;
            VE_iron.style.visibility = Visibility.Visible;
            VE_uranium.style.visibility = Visibility.Visible;
            reward = Ads.RewardType.Ressources;

            Label_uranium.text = Ads.getUraniumAdsReward().ToString();
            Label_iron.text = Ads.getIronAdsReward().ToString();
        }
        else
        {
            Label_diamand.style.visibility = Visibility.Visible;
            VE_iron.style.visibility = Visibility.Hidden;
            VE_uranium.style.visibility = Visibility.Hidden;
            reward = Ads.RewardType.Diamand;
        }
    }

    private void WatchPub()
    {
        Ads.Instance.ShowRewardedAd(reward);
        Stats.Instance.lastPub = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        pubDelay = pubDelayWatch;
        Close();
    }

    private void Close()
    {

        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            adsUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
            Stats.Instance.lastPub = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            pubDelay = pubDelayClose;
        }).StartingIn(400);
    }


    public bool isPubAvable()
    {
        long delta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastPub;
        if (delta > pubDelay) return true;
        return false;
    }
}
