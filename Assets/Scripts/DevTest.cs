using UnityEngine;

public class DevTest : MonoBehaviour
{
    /*
     ASTRIIIIIDDDDDDDDD ( c'est pour être sur que tu lises ) 

    je te laisses ce script ouvert pour que tu puisses savoirs ce que fond les touches
    si jamais tu le veux tu peux modifier a ta guise tant que y'a pas d'erreur ( sinon au mieux tu retourne en arriere ) 
    et au pire tu supprime tout pour que ça marche quand même ( taura juste pas les touches ) 

    je pense que le code est relativement compréhensible sinon tant pis tu pourras pas tester le jeu comme tu le souhaite
     

    regardes whatsapp si jamais
     */


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            for(int i = 0; i < 5; i++ ) //level up 5 fois
                MainUi.Instance.xpUI.LevelUp();

            Stats.Instance.upPrestige(new BigNumber(1, 10), true); // donne 1^10 prestige
            Stats.Instance.upIron(new BigNumber(1, 10), true);// donne 1^10 fer
            Stats.Instance.upUranium(new BigNumber(1, 10), true);// donne 1^10 uranium
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Stats.Instance.reset();// reset ( faut relancer le jeu pour que ça marche a 100% ) 
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Stats.Instance.stage += 10;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Stats.Instance.level += 100;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Stats.Instance.upDiamand(50, true); //donne 50 diamands ) 
            MainUi.Instance.xpUI.loadBonus();
        }


        if (Input.GetKeyDown(KeyCode.Z))
        {
            Ads.Instance.ShowBanner(true);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Ads.Instance.ShowBanner(false);
        }

        testTutos();
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
}
