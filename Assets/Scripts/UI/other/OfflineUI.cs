
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class OfflineUI : MonoBehaviour 
{
    public UIDocument offlineUI;

    private Label timeLabel;

    private Label Lbl_iron;
    private VisualElement VE_iron;
    private VisualElement VE_reward;
    private Label Lbl_uranium;

    private Label Lbl_message;
    private Label Lbl_win;
    private Button claimBtn;
    VisualElement main;

    public bool showErrorMessage = false;

    BigNumber iron;
    BigNumber uranium;


    public void Start()
    {
        calculOfflineUraniumEarn(30, false);
        if (!Stats.Instance.firstConnection)
        {
            long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;
            MainUi.Instance.shopUI.UpdateBoost(time);
            Load();
        }
        else
        {
            offlineUI.gameObject.SetActive(false);
        }
    }

    public void Load(bool offline = true)
    {
        offlineUI.gameObject.SetActive(true);
        gameManager.instance.SetPause(true);
        var root = offlineUI.rootVisualElement;

        if (offlineUI == null) return;
        main = root.Q<VisualElement>("main");

        main.AddToClassList("trans");
        main.schedule.Execute(() =>
        {
            main.RemoveFromClassList("trans");
        }).StartingIn(50);

        timeLabel = root.Q<Label>("time");
        Lbl_iron = root.Q<Label>("ironEarned");
        Lbl_uranium = root.Q<Label>("uraniumEarned");
        VE_iron = root.Q<VisualElement>("mainRessourceLogo");
        VE_reward = root.Q<VisualElement>("reward");
        Lbl_message = root.Q<Label>("message");
        Lbl_win = root.Q<Label>("win");
        claimBtn = root.Q<Button>("claim");

        claimBtn.clicked += claimClicked;

        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Stats.Instance.lastConnection;

        iron = calculOfflineIronEarn(time, offline);
        uranium = calculOfflineUraniumEarn(time, offline);

        Lbl_iron.text = "+" + iron.ToString();
        Lbl_iron.style.color = Utility.GetShipColor();
        VE_iron.style.backgroundImage= Utility.GetMainRessourceLogo();

        Lbl_uranium.style.color = Consts.COLOR_URANIUM;
        Lbl_uranium.text = "+" + uranium.ToString();
        Lbl_uranium.style.display = Ship.Current.HaveUranium() ? DisplayStyle.Flex : DisplayStyle.None;

        timeLabel.text = Utility.TimeToString_dhms(time);

        if (!haveAutomation() && offline)
        {
            timeLabel.text = "";
            Lbl_message.text = "You first need to have the automation for this.";
            Lbl_message.style.color = Color.red;
            VE_reward.style.display = DisplayStyle.None;
            Lbl_win.style.display = DisplayStyle.None;
        }
        else
        {
            Lbl_message.text = "you have been disconnected for";
            Lbl_message.style.color = Color.white;
            VE_reward.style.display = DisplayStyle.Flex;
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
        if(iron != null)
            Stats.Instance.AddIron(iron);

        if (uranium != null && Ship.Current != null && Ship.Current.HaveUranium())
            Stats.Instance.AddIron(uranium);

        main.RemoveFromClassList("trans");
        main.schedule.Execute(() =>
        {
            main.AddToClassList("trans");
        }).StartingIn(50);
        main.schedule.Execute(() =>
        {
            offlineUI.gameObject.SetActive(false);
            gameManager.instance.SetPause(false);
        }).StartingIn(500);
    }

    public bool haveAutomation()
    {
        foreach (machineIronElement m in Ship.Current.machinesIron)
        {
            if (m.data.production_cps != 0) //!offline = booster acheté
                return true;
        }
        foreach (machineUraniumElement m in Ship.Current.machinesUranium)
        {
            if (m.data.production_cps != 0) //!offline = booster acheté
                return true;
        }
        return false;
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
        totaEarn *= Stats.Instance.offline_Prod_Part;
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
        totaEarn *= Stats.Instance.offline_Prod_Part;
        return totaEarn;
    }
}

