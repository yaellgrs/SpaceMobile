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
            Stats.Instance.upIron(new BigNumber(1, 10), true);
            Stats.Instance.upUranium(new BigNumber(1, 10), true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Stats.Instance.reset();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Stats.Instance.upDiamand(50, true);
            //Stats.Instance.upIron(new BigNumber(1, 100), true);
            MainUi.Instance.xpUI.loadBonus();
        }
    }
}
