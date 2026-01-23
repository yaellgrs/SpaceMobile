using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class machineElement : VisualElement
{
    //progress Barre
    public VisualElement VE_progressCadre;
    public VisualElement VE_rewardLogo;
    public VisualElement VE_progressBar;
    public Label Lbl_progressTime;
    public Label Lbl_reward;
    public Label Lbl_time;

    //Button
    public Button Btn_up;
    public VisualElement VE_lockedCover;
    public VisualElement VE_upCostLogo;
    public Label Lbl_upName;
    public Label Lbl_upCost;
    public Label Lbl_lockedLevel;

    //other
    public Label Lbl_level;
    public Label Lbl_name;

    public machineElement()
    {
        AddToClassList("machineCadre");

        Lbl_level = new Label();
        Lbl_name = new Label();

        Lbl_level.text = "1/5";
        Lbl_name.text = "Anvil";
        Lbl_level.AddToClassList("machineLevel");
        Lbl_name.AddToClassList("machineName");

        Add(Lbl_level);
        Add(Lbl_name);

        InitProgressBar();
        InitUpButton();
    }

    private void InitProgressBar()
    {
        VE_progressCadre = new VisualElement();
        VE_progressBar = new VisualElement();
        VE_rewardLogo = new VisualElement();

        Lbl_time = new Label();
        Lbl_progressTime = new Label();
        Lbl_reward = new Label();

        VE_progressCadre.AddToClassList("machineProgressCadre");
        VE_progressBar.AddToClassList("machineProgressBar");
        VE_rewardLogo.AddToClassList("machineRewardLogo");

        Lbl_time.text = "3s";
        Lbl_reward.text = "x10";
        Lbl_time.AddToClassList("machineTime");
        Lbl_reward.AddToClassList("machineReward");

        Add(VE_progressCadre);
        VE_progressCadre.Add(VE_progressBar);
        VE_progressCadre.Add(Lbl_time);
        VE_progressCadre.Add(Lbl_reward);
        Lbl_reward.Add(VE_rewardLogo);
    }

    private void InitUpButton()
    {
        Btn_up = new Button();
        VE_lockedCover = new VisualElement();
        VE_upCostLogo = new VisualElement();
        Lbl_upName = new Label();
        Lbl_upCost = new Label();
        Lbl_lockedLevel = new Label();

        Btn_up.AddToClassList("machineUpButton");
        Btn_up.AddToClassList("button");

        VE_lockedCover.AddToClassList("machineLockedCover");
        VE_upCostLogo.AddToClassList("machineUpCostLogo");

        Lbl_upName.text = "UPGRADE";
        Lbl_upCost.text = "10";
        Lbl_lockedLevel.text = "1";
        Lbl_upName.AddToClassList("machineUpName");
        Lbl_upCost.AddToClassList("machineUpCost");
        Lbl_lockedLevel.AddToClassList("machineLockedLevel");

        Add(Btn_up);
        Btn_up.Add(Lbl_upName);
        Btn_up.Add(Lbl_upCost);
        Btn_up.Add(VE_lockedCover);

        Lbl_upCost.Add(VE_upCostLogo);
        VE_lockedCover.Add(Lbl_lockedLevel);
    }
}
