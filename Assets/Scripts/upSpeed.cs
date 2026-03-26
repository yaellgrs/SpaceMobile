using UnityEngine;
using UnityEngine.UIElements;

public class UpSpeed: MonoBehaviour
{
    public static UpSpeed Instance;
    public enum upSpeeds { one, two, three, four, five, max };
    private static float[] speeds = { 1, 1.25f, 1.5f, 2f, 4f, 0f };

    public upSpeeds upSpeed = upSpeeds.one;
    public float upModeMultiplicator = 1;
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
        upModeMultiplicator = speeds[(int)upSpeed];
        upModeButton.text = "x" + upModeMultiplicator.ToString("F2");
    }
    public void UpButton(Button upModeButton)
    {
        upSpeed++;

        if (upSpeed == upSpeeds.max) upSpeed = upSpeeds.one;
        load(upModeButton);

        spaceObject[] meteors = FindObjectsByType<spaceObject>(FindObjectsSortMode.None);
        foreach(spaceObject meteor in meteors)
        {
            meteor.loadSpeed();
        }
    }
}
