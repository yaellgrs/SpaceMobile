using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoaderUI : MonoBehaviour
{
    public UIDocument document;

    private VisualElement VE_loadBar;
    private Label Lbl_load;

    private void OnEnable()
    {
        VE_loadBar = document.rootVisualElement.Q<VisualElement>("LoadingBar");
        Lbl_load = document.rootVisualElement.Q<Label>("loadingLabel");
    }

    public void UpdateLoadBarre(float progress)
    {
        if (VE_loadBar == null || Lbl_load == null ) return;
        VE_loadBar.style.width = new StyleLength(new Length(progress * 100f, LengthUnit.Percent));
        Lbl_load.text = (progress * 100f).ToString("F1") + "%";
    }
}
