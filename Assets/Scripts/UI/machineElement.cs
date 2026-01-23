using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class machineElement : VisualElement
{
    public Label Lbl_level;
    public VisualElement VE_progressCadre;
    public VisualElement VE_progressBar;
    public Label Lbl_progressTime;
    public Label Lbl_reward;
    public Button Btn_cost;
    public Label Lbl_name;
    public machineElement()
    {

    }
}
