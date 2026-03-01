using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class IronUi : BaseUI
{
    private Button uraniumButton;
    private Button prestigeButton;
    private Label ironLabel;
    private ScrollView SV_scroll;
    protected override void Start()
    {
        base.Start();
        loadForgeUI();
        var root = forgeUI.rootVisualElement;
        forgeUiVE.AddToClassList("forgeIronTrans");
        forgeUI.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        Rect scrollRect = SV_scroll?.worldBound != null ? SV_scroll.worldBound : new Rect(0f, 0f, 0f, 0f);
        foreach (machineElement machine in Ship.Current.machinesIron)
            machine.Update(scrollRect);
        upIronRaffinedUi();
    }

    public override void IronClicked()
    {
        if (forgeUI.gameObject.activeInHierarchy || upgradeUI.gameObject.activeInHierarchy)
        {
            string className = "";
            if (forgeUI.gameObject.activeInHierarchy)
               className = "forgeIronTrans";
            else
                className = "ironUpTrans";
            forgeUiVE.RemoveFromClassList(className);
            if (!stopAnim)
            {
                forgeUiVE.schedule.Execute(() =>
                {
                    forgeUiVE.AddToClassList(className);
                    black.style.visibility = Visibility.Hidden;
                }).StartingIn(50);
                forgeUiVE.schedule.Execute(() =>
                {
                    forgeUI.gameObject.SetActive(false);
                    upgradeUI.gameObject.SetActive(false);
                    gameManager.instance.SetPause(false);

                }).StartingIn(500);
            }
            else
            {
                forgeUI.gameObject.SetActive(false);
                upgradeUI.gameObject.SetActive(false);
                gameManager.instance.SetPause(false);
            }

            classActived = true;
        }
        else
        {
            gameManager.instance.SetPause(true);
            if (!Stats.Instance.ironTuto)
                Tuto.Instance.LoadForgeTuto(true);
            loadForgeUI();
        }

    }


    public void upIronRaffinedUi()
    {
        if (ironLabel != null)
            ironLabel.text = Ship.Current.iron.ToString();
    }

    protected override void upModeButtonClicked()
    {
        base.upModeButtonClicked();
        if (forgeUI.gameObject.activeInHierarchy)
        {
            foreach (machineElement machine in Ship.Current.machinesIron)
                machine.LoadMachine();
        }
        else
        {
            foreach (UpgradesElement upgrade in Ship.Current.upgradesIron)
                upgrade.Load();
        }
    }

    public override void loadForgeUI()
    {
        base.loadForgeUI();

        var root = forgeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        prestigeButton = root.Q<Button>("prestige");
        ironLabel = root.Query<Label>("iron");
        forgeUiVE = root.Query<VisualElement>("forgeUI");
        SV_scroll = root.Query<ScrollView>("scroll");
        if (!stopAnim)
        {
            if (classActived)
            {
                classActived = false;
                forgeUiVE.AddToClassList("forgeIronTrans");
            }

            forgeUiVE.schedule.Execute(() =>
            {
                forgeUiVE.RemoveFromClassList("forgeIronTrans");
            }).StartingIn(50);
        }


        SV_scroll.Clear();

        bool show = true;

        foreach (machineElement machine in Ship.Current.machinesIron)
        {
            SV_scroll.Add(machine);
            if (show) { 
                machine.LoadMachine();
                machine.style.display = DisplayStyle.Flex;
            }
            else machine.style.display = DisplayStyle.None;

            if (!machine.data.isBuyed) show = false; //on affiche pas le reste des machines
        }

        uraniumButton.clicked += uraniumClicked;
        prestigeButton.clicked += prestigeClicked;
    }

    public override void loadUpdateUI()
    {
        base.loadUpdateUI();
        var root = upgradeUI.rootVisualElement;
        uraniumButton = root.Q<Button>("uranium");
        prestigeButton = root.Q<Button>("prestige");
        ironLabel = root.Query<Label>("iron");
        forgeUiVE = root.Query<VisualElement>("updateUI");
        
        forgeUiVE.RemoveFromClassList("ironUpTrans");

        ScrollView scroll = root.Q<ScrollView>("scroll");
        scroll.Clear();
        foreach (UpgradesElement upgrade in Ship.Current.upgradesIron)
        {
            scroll.Add(upgrade);
            upgrade.Load();
        }

        uraniumButton.clicked += uraniumClicked;
        prestigeButton.clicked += prestigeClicked;
    }

    private void uraniumClicked()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.uraniumUI.gameObject.SetActive(true);
        MainUi.Instance.uraniumUI.loadForgeUI();
    }

    private void prestigeClicked()
    {
        forgeUI.gameObject.SetActive(false);
        upgradeUI.gameObject.SetActive(false);
        MainUi.Instance.prestigeUI.forgeUI.gameObject.SetActive(true);
        MainUi.Instance.prestigeUI.loadForgeUI();
    }
}
