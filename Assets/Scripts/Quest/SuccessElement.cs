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




[UxmlElement] 
public partial class SuccessElement : VisualElement
{
    private Label Lbl_quest;
    private Label Lbl_progress;
    private Button Btn_claim;


    public SuccessElement()
    {
        Lbl_quest = new Label();
        Lbl_progress = new Label();
        Btn_claim = new Button();

        Lbl_quest.AddToClassList("questLabel");
        Lbl_progress.AddToClassList("progressLabel");
        Btn_claim.AddToClassList("claimButton");

        AddToClassList("cadre");
        Add(Lbl_quest);
        Add(Lbl_progress);
        Add(Btn_claim);

        initQuest();
        initPorgress();
        initButton();
    }

    public void initQuest()
    {
        Lbl_quest.text = "quest init";
    }

    public void initPorgress()
    {
        Lbl_progress.text = "0/0";
    }

    private void initButton()
    {
        Btn_claim.text = "claim";
    }

}
