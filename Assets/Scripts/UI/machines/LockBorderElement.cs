
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LockBorderElement : Button
{
    #region --- Elements ---
    VisualElement VE_check;

    private machineElement machine;
    private borderColor borderColor;
    private BorderBuyType buyType;

    #endregion

    public LockBorderElement()
    {
        this.borderColor = borderColor.bronze;
        Init();
    }

    public LockBorderElement(machineElement machine, borderColor borderColor, BorderBuyType buyType)
    {
        this.machine = machine;
        this.borderColor = borderColor;
        this.buyType = buyType;
        Init();
    }

    private void Init()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/LockBorderStyle");
        styleSheets.Add(styleSheet);

        VE_check = new VisualElement();

        VE_check.AddToClassList("LockBorderCheck");
        AddToClassList("LockBorderElement");
        AddToClassList("button");


        style.backgroundColor = Consts.BORDERS_COLORS[(int)borderColor];



        Add(VE_check);

        LoadCheck();
    }

    private void LoadCheck()
    {
        VE_check.style.display = buyType == BorderBuyType.unbuyed ? DisplayStyle.None : DisplayStyle.Flex;
        Color color = buyType == BorderBuyType.permanent ? Color.skyBlue : Color.green;
        VE_check.style.unityBackgroundImageTintColor = color;
    }



}
