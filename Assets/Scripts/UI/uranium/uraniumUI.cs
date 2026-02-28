using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UraniumUI : BaseUI
{
    private Button ironButton;
    private Button prestigeButton;
    private Button unlockButton;
    private VisualElement uraniumUnlockedVE;
    private Label uraniumLabel;
    private ScrollView SV_scroll;

    protected override void Update()
    {
        base.Update();
        upUraniumLabel();

        foreach(machineUraniumElement machine in Ship.Current.machinesUranium)
            machine.Update();
    }

    public void upUraniumLabel()
    {
        if(uraniumLabel != null)
            uraniumLabel.text = Ship.Current.uranium.ToString();
    }

    private void ironClicked()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.ironUI.gameObject.SetActive(true);
        MainUi.Instance.ironUI.loadForgeUI();
    }

    private void prestigeClicked()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.prestigeUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.prestigeUI.loadForgeUI();
    }

    protected override void upModeButtonClicked()
    {
        base.upModeButtonClicked();
        if (forgeUI.gameObject.activeInHierarchy)
        {
            foreach (machineElement machine in Ship.Current.machinesUranium)
                machine.upMachineCostText();
        }
        else
        {
            foreach (UpgradesElement upgrade in Ship.Current.upgradesUranium)
                upgrade.LoadUI();
        }
    }

    public override void IronClicked()
    {
        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            forgeUiVE.RemoveFromClassList("prestigeUITrans");
            forgeUiVE.schedule.Execute(() =>
            {
                forgeUiVE.AddToClassList("prestigeUITrans");
                black.style.visibility = Visibility.Hidden;

            }).StartingIn(50);
            forgeUiVE.schedule.Execute(() =>
            {
                forgeUI.gameObject.SetActive(false);
                upgradeUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);

            }).StartingIn(500);
            classActived = true;
        }
        else
        {
            gameManager.instance.SetPause(true);
            loadForgeUI();
        }
    }

    public override void loadForgeUI()
    {
        base.loadForgeUI();
        var root = forgeUI.rootVisualElement;

        uraniumLabel = root.Q<Label>("uranium");
        uraniumUnlockedVE = root.Q<VisualElement>("unlockLevel");

        prestigeButton = root.Q<Button>("prestige");
        ironButton = root.Q<Button>("iron");
        forgeUiVE = root.Q<VisualElement>("forgeUI");
        SV_scroll = root.Q<ScrollView>("scroll");


        SV_scroll.Clear();

        bool show = true;
        foreach (machineElement machine in Ship.Current.machinesUranium)
        {
            SV_scroll.Add(machine);
            if (show)
            {
                machine.LoadMachine(SV_scroll);
                machine.style.display = DisplayStyle.Flex;
            }
            else machine.style.display = DisplayStyle.None;

            if (!machine.isBuyed) show = false; //on affiche pas le reste des machines
        }

        if (classActived)
        {
            classActived = false;
            forgeUiVE.AddToClassList("prestigeUITrans");
        }
        forgeUiVE.schedule.Execute(() =>
        {
            forgeUiVE.RemoveFromClassList("prestigeUITrans");
        }).StartingIn(50);

        prestigeButton.clicked += prestigeClicked;
        ironButton.clicked += ironClicked;

        if (XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
        {
            uraniumUnlockedVE.style.visibility = Visibility.Hidden;
            uraniumLabel = root.Q<Label>("uranium");
            uraniumLabel = root.Q<Label>("uranium");
            upUraniumLabel();

            foreach (machineUraniumElement machine in Ship.Current.machinesUranium)
                machine.LoadMachine(SV_scroll);
        }
        else
            uraniumUnlockedVE.style.visibility = Visibility.Visible;

        if (!Stats.Instance.uraniumTuto && XpUI.rewardUnlocked(XpUI.BonusLevel.UnlockUranium))
            Tuto.Instance.LoadForgeTuto(false);


    }

    public override void loadUpdateUI()
    {
        base.loadUpdateUI();
        var root = upgradeUI.rootVisualElement;
        prestigeButton = root.Q<Button>("prestige");
        ironButton = root.Q<Button>("iron");
        uraniumLabel = root.Q<Label>("uranium");
        forgeUiVE = root.Q<VisualElement>("forgeUI");
        upUraniumLabel();

        //scroll
        ScrollView scroll = root.Q<ScrollView>("scroll");
        scroll.Clear();
        foreach (UpgradesElement upgrade in Ship.Current.upgradesUranium) 
        {
            scroll.Add(upgrade);
            upgrade.Load();
        }

        prestigeButton.clicked += prestigeClicked;
        ironButton.clicked += ironClicked;
    }
}
