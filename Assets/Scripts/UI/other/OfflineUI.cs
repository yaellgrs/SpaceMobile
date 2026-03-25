
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OfflineUI : MonoBehaviour 
{
    public UIDocument offlineUI;

    private Label timeLabel;
    private Label ironEarned;
    private Label Lbl_message;
    private Label Lbl_win;
    private Button claimBtn;
    VisualElement main;

    public bool showErrorMessage = false;


    public void Start()
    {
        calculOfflineUraniumEarn(30, false);
        if (!Stats.Instance.firstConnection)
        {
            if (Stats.Instance.damageBoostTime > 0)
            {
                long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;
                Stats.Instance.damageBoostTime -= time;
            }
            if (Stats.Instance.xpBoostTime > 0)
            {
                long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;
                Stats.Instance.xpBoostTime -= time;
            }
            Load();
        }
        else
        {
            offlineUI.gameObject.SetActive(false);
        }
    }

    public void Load()
    {
        var root = offlineUI.rootVisualElement;
        offlineUI.gameObject.SetActive(true);

        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        timeLabel = root.Q<Label>("time");
        ironEarned = root.Q<Label>("ironEarned");
        Lbl_message = root.Q<Label>("message");
        Lbl_win = root.Q<Label>("win");
        claimBtn = root.Q<Button>("claim");

        claimBtn.clicked += claimClicked;

        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;

        BigNumber iron = calculOfflineIronEarn(time, true);
        BigNumber uranium = calculOfflineUraniumEarn(time, true);

        ironEarned.text = "+" + iron.ToString();

        timeLabel.text = Utility.TimeToString_dhms(time);

        if(Ship.Current.level < 12)
        {
            timeLabel.text = "";
            Lbl_message.text = "You first need to have the automation for this.";
            Lbl_message.style.color = Color.red;
            ironEarned.style.display = DisplayStyle.None;
            Lbl_win.style.display = DisplayStyle.None;

        }
        else
        {
            Lbl_message.text = "you have been disconnected for";
            Lbl_message.style.color = Color.white;
            ironEarned.style.display = DisplayStyle.Flex;
            Lbl_win.style.display = DisplayStyle.Flex;
        }
       
        if (iron.EqualZero() && !showErrorMessage)
        {
            showErrorMessage = false;
            claimClicked();
        }
    }
    

    private void claimClicked()
    {
        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            offlineUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(300);
    }


    public static BigNumber calculOfflineIronEarn(long time, bool offline)
    {
        BigNumber totaEarn = new BigNumber(0);

        foreach (machineIronElement m in Ship.Current.machinesIron)
        {
            if (m.data.isBuyed)
            {
                if(m.data.production_cps > 0 || !offline) //!offline = booster acheté
                {
                    float mult = Mathf.Min(1, m.data.production_cps + 1);
                    BigNumber earn = m.CalculReward();
                    earn *= time * mult;
                    totaEarn.Add(earn);
                }
            }
        }
        if (offline)
        {
            totaEarn.Multiply(Stats.Instance.offline_Prod_Part);
            Stats.Instance.AddIron(totaEarn);
        }

        return totaEarn;
    }

    public static BigNumber calculOfflineUraniumEarn(long time, bool offline)
    {
        BigNumber totaEarn = new BigNumber(0);

        foreach (machineUraniumElement m in Ship.Current.machinesUranium)
        {
            if (m.data.isBuyed)
            {
                if (m.data.production_cps > 0 || !offline) //!offline = booster acheté
                {
                    float mult = Mathf.Min(1, m.data.production_cps + 1);
                    BigNumber earn = m.CalculReward();
                    earn *= time * mult;
                    totaEarn.Add(earn);
                }
            }
        }

        if (offline)
        {
            totaEarn.Multiply(Stats.Instance.offline_Prod_Part);
            Stats.Instance.AddUranium(totaEarn);
        }
        return totaEarn;
    }
}

