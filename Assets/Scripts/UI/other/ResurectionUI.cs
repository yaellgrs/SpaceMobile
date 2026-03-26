using UnityEngine;
using UnityEngine.UIElements;

public class ResurectionUI : MonoBehaviour
{

    public static ResurectionUI Instance;

    public UIDocument resurectionUI;

    private VisualElement main;

    private Button diamand;
    private Button pub;
    private Button Btn_backStage;

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
        resurectionUI.gameObject.SetActive(false);
        if (Ship.Current.isDead) loadResurection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadResurection()
    {
        resurectionUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);

        var root = resurectionUI.rootVisualElement;

        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);


        diamand = root.Q<Button>("diamand");
        pub = root.Q<Button>("pub");
        Btn_backStage = root.Q<Button>("backStage");

        if (Stats.Instance.diamand >= 5)
        {
            diamand.SetEnabled(true);
            diamand.clicked -= diamandClicked;
            diamand.clicked += diamandClicked;
        }
        else diamand.SetEnabled(false);

        if(Stats.Instance.deadPubWatch == 0)
        {
            pub.clicked -= pubClicked;
            pub.clicked += pubClicked;
            pub.style.visibility = Visibility.Visible;
        }
        else pub.style.visibility = Visibility.Hidden;

        Btn_backStage.clicked -= backStage;
        Btn_backStage.clicked += backStage;
    }

    private void diamandClicked()
    {
        Stats.Instance.AddDiamand(-5 * Stats.Instance.deadPubWatch);
        Stats.Instance.ReduceLifeBoss = true;

        Resurection();
    }

    private void pubClicked()
    {
        if (IAPManager.Instance.CheckAds()) Ads.Instance.GetReward(Ads.RewardType.Resurection);
        else Ads.Instance.ShowRewardedAd(Ads.RewardType.Resurection);
    }

    public void Resurection()
    {
        gameManager.instance.bossStage = false;
        Close();
    }

    private void backStage()
    {
        Ship.Current.stage -= 5;
        gameManager.instance.bossStage = false;
        gameManager.instance.RestartStage();
        Close();
    }

    public void Close()
    {
        SoundManager.Instance.lauchTransitionMusic(MusicType.Main);
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            resurectionUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
            Ship.Current.isDead = false;  
        }).StartingIn(400);
    }
}
