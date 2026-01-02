using UnityEngine;
using UnityEngine.UIElements;

public class UpMode : MonoBehaviour
{
    public static UpMode Instance;
    public enum upModes { one, two, three, four, max };
    public upModes upMode = upModes.one;
    public int upModeMultiplicator = 1;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void load(Button upModeButton)
    {
        if (upMode == upModes.max)
        {
            upMode = upModes.one;
            upModeButton.text = "x1";
            upModeMultiplicator = 1;
        }
        else if (upMode == upModes.two)
        {
            upModeButton.text = "x10";
            upModeMultiplicator = 10;
        }
        else if (upMode == upModes.three)
        {
            upModeButton.text = "x25";
            upModeMultiplicator = 25;
        }
        else if (upMode == upModes.four)
        {
            upModeButton.text = "x50";
            upModeMultiplicator = 50;
        }
    }
    public void UpButton(Button upModeButton)
    {
        upMode++;
        if (upMode == upModes.max)
        {
            upMode = upModes.one;
            upModeButton.text = "x1";
            upModeMultiplicator = 1;
        }
        else if (upMode == upModes.two)
        {
            upModeButton.text = "x10";
            upModeMultiplicator = 10;
        }
        else if (upMode == upModes.three)
        {
            upModeButton.text = "x25";
            upModeMultiplicator = 25;
        }
        else if (upMode == upModes.four)
        {
            upModeButton.text = "x50";
            upModeMultiplicator = 50;
        }


    }
}
