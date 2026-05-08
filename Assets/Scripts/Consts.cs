using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public const int MINIMUM_STAR_PARTICULE_STAGE = 10;

    public const int BASE_STELLAR_METEOR_PROBABILITY = 5; // 0.5%  - 2%
    public const int BASE_STELLAR_BOSS_PROBABILITY = 200; // 20%   -  >100% 
    public const float BANNER_REWARD = 1.25f;

    public static Color[] SHIP_COLOR ={
        Utility.Hex("#B07C61"), //Wood
        Utility.Hex("#FFA300"), //Iron
    };

    public static Color COLOR_URANIUM = Utility.Hex("#00FF0E");

    public static string[] MACHINE_IRON_NAMES = { "Enclume", "forgeuse", "forgeuses" };
    public static string[] MACHINE_WOOD_NAMES = { "Hache", "Scie", "Tronçonneuse" };

    public static Color[] BORDERS_COLORS =
    {    
         new Color(1f, 1f, 1f),
         new Color(208 / 255.0f, 144 / 255.0f, 95 / 255.0f),
         new Color(130 / 255.0f, 130 / 255.0f, 130 / 255.0f),
         new Color(201 / 255.0f, 152 / 255.0f, 44 / 255.0f),
         new Color(2 / 255.0f, 208 / 255.0f, 202 / 255.0f),
         new Color(0f, 0f, 0f),
    };

    public static readonly int[] BORDER_EMPLOYEE = { 0, 1, 2, 4, 6, 10 };



    public static Dictionary<SuccessType, BigNumber[]> SUCCESS_OBJECTIF =
        new Dictionary<SuccessType, BigNumber[]>
    {
    { SuccessType.meteorKilled, new BigNumber[] { new BigNumber(1000), new BigNumber(1000), new BigNumber(5000), new BigNumber(25, 3), new BigNumber(100, 3) } },
    { SuccessType.bossKilled, new BigNumber[] { new BigNumber(10), new BigNumber(25), new BigNumber(50), new BigNumber(250), new BigNumber(1, 3) } },
    { SuccessType.upgradeBuy, new BigNumber[] { new BigNumber(100), new BigNumber(250), new BigNumber(500), new BigNumber(1, 3), new BigNumber(5, 3) } },
    { SuccessType.pubWatch, new BigNumber[] { new BigNumber(1), new BigNumber(5), new BigNumber(10), new BigNumber(25), new BigNumber(50) } },
    { SuccessType.machineClicked,new BigNumber[] { new BigNumber(100), new BigNumber(500), new BigNumber(1000), new BigNumber(5, 3), new BigNumber(25, 3) } },
    { SuccessType.urnaium,new BigNumber[] { new BigNumber(1,3), new BigNumber(1,6), new BigNumber(1,9), new BigNumber(1, 12), new BigNumber(1, 15) } },
    { SuccessType.iron, new BigNumber[] { new BigNumber(1,3), new BigNumber(1,6), new BigNumber(1,9), new BigNumber(1, 12), new BigNumber(1, 15) } },
    { SuccessType.startParticle, new BigNumber[] { new BigNumber(10), new BigNumber(1,3), new BigNumber(50,3), new BigNumber(500, 3), new BigNumber(1, 6) } },
    { SuccessType.prestige, new BigNumber[] { new BigNumber(3), new BigNumber(10), new BigNumber(25), new BigNumber(50), new BigNumber(100) } },
    };

    public static int SUCCESS_LEVEL_MAX = 5;
}