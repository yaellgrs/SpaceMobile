using UnityEngine;
using UnityEngine.UIElements;

public class UpSpeed: MonoBehaviour
{
    public static UpSpeed Instance;
    public enum upSpeeds { one, two, three, four, max };
    public upSpeeds upSpeed = upSpeeds.one;
    public int upModeMultiplicator = 1;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void load(Button upModeButton)
    {
        if (upSpeed == upSpeeds.one)
        {
            upSpeed = upSpeeds.one;
            upModeButton.text = "x1";
            upModeMultiplicator = 1;
        }
        else if (upSpeed == upSpeeds.two)
        {
            upModeButton.text = "x2";
            upModeMultiplicator = 2;
        }
        else if (upSpeed == upSpeeds.three)
        {
            upModeButton.text = "x3";
            upModeMultiplicator = 3;
        }
        else if (upSpeed == upSpeeds.four)
        {
            upModeButton.text = "x4";
            upModeMultiplicator = 4;
        }
    }
    public void UpButton(Button upModeButton)
    {
        upSpeed++;

        if ((int)upSpeed == Stats.Instance.SpeedLevel) upSpeed = upSpeeds.one;
        load(upModeButton);

        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        foreach(spaceObject meteor in meteors)
        {
            meteor.loadSpeed();
        }
    }
}
