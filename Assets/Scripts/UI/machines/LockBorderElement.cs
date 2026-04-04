using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LockBorderElement : Button
{
    #region --- Elements ---
    VisualElement VE_check;

    private borderColor borderColor;
    private bool isBuy;

    #endregion

    public LockBorderElement()
    {
        this.borderColor = borderColor.bronze;
        Init();
    }

    public LockBorderElement(borderColor borderColor, bool isBuy)
    {
        this.borderColor = borderColor;
        this.isBuy = isBuy;
        Init();
    }

    private void Init()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("styles/LockBorderStyle");
        styleSheets.Add(styleSheet);

        VE_check = new VisualElement();

        VE_check.AddToClassList("LockBorderCheck");


        style.backgroundColor = Consts.BORDERS_COLORS[(int)borderColor];



        Add(VE_check);

        LoadCheck();
    }

    private void LoadCheck()
    {
        VE_check.style.display = isBuy ? DisplayStyle.Flex : DisplayStyle.None;
    }

}
