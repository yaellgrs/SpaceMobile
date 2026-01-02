using UnityEngine;
using UnityEngine.UIElements;

public class ResurectionUI : MonoBehaviour
{

    public static ResurectionUI Instance;

    public UIDocument resurectionUI;

    private VisualElement main;

    private Button diamand;
    private Button pub;
    private Button prestige;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resurectionUI.gameObject.SetActive(false);
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
        prestige = root.Q<Button>("prestige");

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



        prestige.clicked -= prestigeClicked;
        prestige.clicked += prestigeClicked;
    }

    private void diamandClicked()
    {
        Stats.Instance.upDiamand(5 * Stats.Instance.deadPubWatch, false);
        Close();
    }

    private void pubClicked()
    {
        if (IAPManager.Instance.CheckAds()) Ads.Instance.GetReward(Ads.RewardType.Resurection);
        else Ads.Instance.ShowRewardedAd(Ads.RewardType.Resurection);
        Close();
    }
    private void prestigeClicked()
    {
        MainUi.Instance.prestigeUI.LoadPrestige();
    }

    public void Close()
    {
        Song.Instance.lauchTransitionMusic(Song.Instance.dead_music, Song.Instance.main_music);
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            resurectionUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(400);
    }
}
