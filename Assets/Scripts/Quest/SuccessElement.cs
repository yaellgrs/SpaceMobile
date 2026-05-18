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
prestige done - 
omega meteor killed
 
 */

public enum SuccessType
{
    meteorKilled, bossKilled, upgradeBuy, pubWatch, machineClicked, urnaium, iron, startParticle,
    prestige, 
}

[UxmlElement] 
public partial class SuccessElement : VisualElement
{
    private Label Lbl_quest;
    private Label Lbl_progress;
    private Button Btn_claim;

    private Label Lbl_reward;

    private Label Lbl_level;

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
        Init();
        this.type = Type;

    }

    private void Init()
    {
        Lbl_quest = new Label();
        Lbl_progress = new Label();
        Btn_claim = new Button();

        Lbl_reward = new Label();
        Lbl_level = new Label();
        VisualElement diamandLogo = new VisualElement();
        Lbl_reward.text = "1000";


        Lbl_quest.AddToClassList("questLabel");
        Lbl_progress.AddToClassList("progressLabel");
        Btn_claim.AddToClassList("claimButton");
        Btn_claim.AddToClassList("button");
        Lbl_level.AddToClassList("level");

        Lbl_reward.AddToClassList("rewardLabel");
        diamandLogo.AddToClassList("diamandLogo");

        AddToClassList("cadre");
        Add(Lbl_quest);
        Add(Lbl_progress);
        Add(Btn_claim);
        Add(Lbl_level);

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

        Lbl_level.text = isLevelMax() ?  "MAX" : (getObjectiflevel() - 1) + "/" + Consts.SUCCESS_LEVEL_MAX.ToString();

        Lbl_progress.text = progress.ToString() + "/" + objectif.ToString();  

        Lbl_progress.style.visibility = isLevelMax() ? Visibility.Hidden : Visibility.Visible;
        Btn_claim.style.visibility = isLevelMax() ? Visibility.Hidden : Visibility.Visible;

        bool enable = (objectif < progress);
        Btn_claim.SetEnabled(enable);
    }

    private void initButton()
    {
        Btn_claim.text = "claim";
        Btn_claim.clicked -= Claim;
        Btn_claim.clicked += Claim;

    }

    private bool isLevelMax()
    {
        return getObjectiflevel() > Consts.SUCCESS_LEVEL_MAX;
    }

    private void Claim()
    {
        if (Stats.Instance == null) return;

        Stats.Instance.AddDiamand(getReward());
        QuestStats.Instance.succesGoals[(int)type]++;
        initPorgress();
    }

    private BigNumber getProgress()
    {
        if (Datas.Instance == null)
        {
            return new BigNumber(0);
        }

        BigNumber progress = new BigNumber(0);

        var field = typeof(Data).GetField(type.ToString());
        if( field == null)
        {
            if (type == SuccessType.meteorKilled)
            {
                foreach (var type in System.Enum.GetValues(typeof(spaceObject.meteorType)))
                {
                    progress += Datas.Instance.current.meteorKilled[(int)type];
                    progress += Datas.Instance.currentShip.meteorKilled[(int)type];
                    progress += Datas.Instance.total.meteorKilled[(int)type];
                }
            }
            if (type == SuccessType.bossKilled)
            {
                foreach (var type in System.Enum.GetValues(typeof(BossType)))
                {
                    progress += Datas.Instance.current.meteorBossKilled[(int)type];
                    progress += Datas.Instance.currentShip.meteorBossKilled[(int)type];
                    progress += Datas.Instance.total.meteorBossKilled[(int)type];
                }
            }
            else{
                Debug.LogError("no data for : " + type);
                return new BigNumber(0);
            }
        }
        else {
            progress += getStatValue(field.GetValue(Datas.Instance.current));
            progress += getStatValue(field.GetValue(Datas.Instance.currentShip));
            progress += getStatValue(field.GetValue(Datas.Instance.total));
        }

        return progress;
    }

    public BigNumber getStatValue(object value)
    {
        if (value is BigNumber bn)
            return bn;

        if (value is int i)
            return new BigNumber(i);

        return new BigNumber(0);
    }

    private BigNumber getObjectif()
    {
        if (Consts.SUCCESS_OBJECTIF[type].Length <= getObjectiflevel() - 1 || getObjectiflevel() <= 0) return new BigNumber(0);
        return Consts.SUCCESS_OBJECTIF[type][getObjectiflevel()-1];
    }

    private int getObjectiflevel()
    {
        if (QuestStats.Instance == null || QuestStats.Instance.succesGoals == null)  return 0;
        if (QuestStats.Instance.succesGoals?.Length <= (int)type) QuestStats.Instance.initSucces();
        return QuestStats.Instance.succesGoals[(int)type];
    }

    private int getReward()
    {
        return getObjectiflevel() * 5;
    }

}
