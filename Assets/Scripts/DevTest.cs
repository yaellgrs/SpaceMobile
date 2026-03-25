using System.Linq;
using UnityEngine;

public class DevTest : MonoBehaviour
{
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        ShowTuto(false); // desactive les tutos
    }

    private void ShowTuto(bool show)
    {
        foreach (PopupTuto tuto in Stats.Instance.popupTutos.Keys.ToList())
        {
            Stats.Instance.popupTutos[tuto] = !show;
        }
        Stats.Instance.ironTuto = !show;
        Stats.Instance.uraniumTuto = !show;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuestManager.Instance.Claim();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Ship.Current.type = SpaceShipData.SpaceShipElement.Wood;
            Ship.Current.SetNextType(0);

            Stats.Instance.reset();// reset ( faut relancer le jeu pour que ça marche a 100% ) 
            Init();
            
            BottomUI.Instance.LoadUI();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameManager.instance.upStage();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Ship.Current.level += 100;//level max
        }

        HandleBanner();
        gives();
        testTutos();
        testShips();
    }

    private void testShips()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Type Ship Used : " + Ship.Current.type);
        }
    }

    private void gives()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Stats.Instance.AddDiamand(50); //donne 50 diamands ) 
            MainUi.Instance.xpUI.loadBonus();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //for (int i = 0; i < 50; i++) //level up 5 fois
            //    MainUi.Instance.xpUI.LevelUp();

            Stats.Instance.addPrestige(new BigNumber(1.2, 3)); // donne 1^100 prestige

            Stats.Instance.AddIron(new BigNumber(1.2, 3));// donne 1^100 fer
            Stats.Instance.AddUranium(new BigNumber(1, 100));// donne 1^100 uranium

            Stats.Instance.AddShipMoney(new BigNumber(1, 100), false);
            Stats.Instance.AddShipFragment(100);

            Stats.Instance.AddDiamand(100);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            Stats.Instance.AddShipFragment(100);
            Stats.Instance.prestigeUnlocked = true;
            BottomUI.Instance.LoadUI();

        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            Stats.Instance.AddShipMoney(new BigNumber(30), false);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Stats.Instance.AddPrestigeWainting(new BigNumber(10));
        }
    }



    private void testTutos()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.ironMeteor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.uraniumMeteor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.diamandMeteor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.splitterMeteor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.BigMeteor);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Tuto.Instance.LoadPopupTuto(PopupTuto.rocket);
        }
    }

    private void HandleBanner()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Ads.Instance.ShowBanner(true);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Ads.Instance.ShowBanner(false);
        }
    }
}
