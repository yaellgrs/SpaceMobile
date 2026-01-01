using UnityEngine;

public class DevTest : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            for(int i = 0; i < 5; i++ ) 
                MainUi.Instance.xpUI.LevelUp();

            Stats.Instance.upPrestige(new BigNumber(1, 10), true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Stats.Instance.reset();
        }
    }
}
