using UnityEngine;
using UnityEngine.UIElements;


/*
 success types :

normal meteor
uranium meteor
iron meteor
diamand meteor
splitter meteor
reinforced meteor
prestige done
omega meteor killed
 
 */

public enum SuccessType
{
    basicMeteorKilled, uraniumMeteorKilled, ironMeteorKilled, diamandMeteorKilled, splitterMeteorKilled, reinforcedMeteorKilled, OmegaMeteorKilled,
    PrestigeCount, 
}

[UxmlElement] 
public partial class SuccessElement : VisualElement
{
    private Label Lbl_quest;
    private Label Lbl_progress;
    private Button Btn_claim;

    private Label Lbl_reward;

    private SuccessType _type;

    [UxmlAttribute]
    public SuccessType type {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
            initQuest();
            initPorgress();
        }
    }

    public SuccessElement()
    {
        Init();
    }

    public SuccessElement(SuccessType Type)
    {

        Debug.Log("init with type : " + Type.ToString());
        Init();
        type = Type;
    }

    private void Init()
    {
        Lbl_quest = new Label();
        Lbl_progress = new Label();
        Btn_claim = new Button();

        Lbl_reward = new Label();
        VisualElement diamandLogo = new VisualElement();
        Lbl_reward.text = "1000";


        Lbl_quest.AddToClassList("questLabel");
        Lbl_progress.AddToClassList("progressLabel");
        Btn_claim.AddToClassList("claimButton");
        Btn_claim.AddToClassList("button");

        Lbl_reward.AddToClassList("rewardLabel");
        diamandLogo.AddToClassList("diamandLogo");

        AddToClassList("cadre");
        Add(Lbl_quest);
        Add(Lbl_progress);
        Add(Btn_claim);

        Btn_claim.Add(Lbl_reward);
        Lbl_reward.Add(diamandLogo);

        initQuest();
        initPorgress();

        initButton();
    }

    public void Load()
    {
        initQuest();
        initPorgress();
    }

    public void initQuest()
    {
        Lbl_quest.text = type.ToString();
        Lbl_reward.text = getReward().ToString();
    }

    public void initPorgress()
    {
        BigNumber progress = getProgress();
        BigNumber objectif = getObjectif();

        Lbl_progress.text = progress.ToString() + "/" + objectif.ToString();

        bool enable = (objectif < progress);
        Btn_claim.SetEnabled(enable);
    }

    private void initButton()
    {
        Btn_claim.text = "claim";
        Btn_claim.clicked += Claim;
    }

    private void Claim()
    {
        if (Stats.Instance == null) return;

        Stats.Instance.upDiamand(getReward(), true);
        QuestStats.Instance.successGoals[type]++;
        initPorgress();
    }

    private BigNumber getProgress()
    {
        if (Data.Instance == null) return new BigNumber(0);

        var field = typeof(Data).GetField(type.ToString());

        return field == null ? new BigNumber(0) : (BigNumber)field.GetValue(Data.Instance);
    }

    private BigNumber getObjectif()
    {
        return new BigNumber(getObjectiflevel() * 50);
    }

    private int getObjectiflevel()
    {
        if (QuestStats.Instance == null)  return 0;
        if (!QuestStats.Instance.successGoals.ContainsKey(type)) QuestStats.Instance.initSucces();
        return QuestStats.Instance.successGoals[type];
    }

    private int getReward()
    {
        return getObjectiflevel() * 5;
    }

}
