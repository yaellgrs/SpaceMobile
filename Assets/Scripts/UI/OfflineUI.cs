
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OfflineUI : MonoBehaviour 
{



    public UIDocument offlineUI;

    private Label timeLabel;
    private Label ironEarned;
    private Button claimBtn;


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
        offlineUI.gameObject.SetActive(true);
        var root = offlineUI.rootVisualElement;

        timeLabel = root.Q<Label>("time");
        ironEarned = root.Q<Label>("ironEarned");
        claimBtn = root.Q<Button>("claim");

        claimBtn.clicked += claimClicked;

        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;

        BigNumber iron = calculOfflineIronEarn(time, true);
        BigNumber uranium = calculOfflineUraniumEarn(time, true);

        ironEarned.text = "+" + iron.ToString();

        timeLabel.text = TimeToString(time);

        if (iron.EqualZero())
        {
            claimClicked();
        }
    }
    

    private void claimClicked()
    {
        offlineUI.gameObject.SetActive(false);
        gameManager.instance.SetPause(false);
    }

    private string TimeToString(long time)
    {
        int minute = (int)time / 60;
        time %= 60;
        int heure = (int)minute / 60;
        minute %= 60;

        int jours = (int)heure / 24;
        heure %= 24;

        return jours + "d " + heure + "h " + minute + "m " + time + "s";
        
    }

    public static BigNumber calculOfflineIronEarn(long time, bool offline)
    {
        BigNumber totaEarn = new BigNumber(0);

        foreach (MachineIron m in Stats.Instance.machinesIron)
        {
            if (m.isActive)
            {
                if(m.automatic || !offline)
                {
                    BigNumber earn = new BigNumber(m.machineEarn1);
                    earn.Multiply(time);
                    earn.Divide(m.machineTimeMaxReel);

                    totaEarn.Add(earn);
                }
            }
        }
        if (offline)
        {
            totaEarn.Multiply(Stats.Instance.offline_Prod_Part);
            Stats.Instance.upIron(totaEarn, true);
        }

        return totaEarn;
    }

    public static BigNumber calculOfflineUraniumEarn(long time, bool offline)
    {
        BigNumber totaEarn = new BigNumber(0);

        foreach (machineUranium m in Stats.Instance.machinesUranium)
        {
            if (m.isActive)
            {
                if (m.automatic || !offline)
                {
                    BigNumber earn = new BigNumber(m.machineEarn1.Mantisse, m.machineEarn1.Exp);
                    earn.Multiply(time);
                    earn.Divide(m.machineTimeMaxReel);

                    totaEarn.Add(earn);
                }
            }
        }

        if (offline)
        {
            totaEarn.Multiply(Stats.Instance.offline_Prod_Part);
            Stats.Instance.upUranium(totaEarn, true);
        }
        return totaEarn;
    }
}

