using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MarkersUI : MonoBehaviour
{
    #region Instance
    public static MarkersUI Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    public UIDocument document;

    public List<markerElement> markers = new List<markerElement>();

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T)) ShowMarker();
    }

    public void ShowMarker(Vector3 pos, string txt, MarkerType type, float speed = 1f, float alpha_decrease = 0.975f, float fontFactor = 1f)
    {
/*        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(document.rootVisualElement.panel, screenPos);*/
        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(
            document.rootVisualElement.panel,
            pos,
            Camera.main
        );
        ShowMarker(panelPos, txt, type, speed, alpha_decrease, fontFactor);
    }

    public void ShowMarker(Vector2 panelPos, string txt, MarkerType type, float speed, float alpha_decrease, float fontFactor)
    {
        markerElement m;
        if(markers.Count > 0)
        {
            m = markers[0];
            markers.RemoveAt(0);
        }
        else
            m = new markerElement();

        m.Load(panelPos, txt, type);
        document.rootVisualElement.Add(m);
    }

}
