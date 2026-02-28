using UnityEngine;
using UnityEngine.UIElements;

public class UpMode : MonoBehaviour
{
    public static UpMode Instance;

    static readonly string[] text = { "x1", "x5", "x10", "x25", "x50" };
    static readonly int[] mults = { 1, 5, 10, 25, 50 };

    int index = 0;
    public int upModeMultiplicator = 1;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public void load(Button upModeButton)
    {

        upModeButton.text = text[index];
        upModeMultiplicator = mults[index];
    }
    public void UpButton(Button upModeButton)
    {
        index++;
        if (index >= text.Length) index = 0;
        load(upModeButton);
    }
}
