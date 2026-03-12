using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utility
{
    public const long DAY_IN_SECOND = 86400;

    public static void setBorderColor(Button btn, Color color)
    {
        btn.style.borderLeftColor = color;
        btn.style.borderRightColor = color;
        btn.style.borderTopColor = color;
        btn.style.borderBottomColor = color;
    }

    public static void setBorderColor(VisualElement VE, Color color)
    {
        VE.style.borderLeftColor = color;
        VE.style.borderRightColor = color;
        VE.style.borderTopColor = color;
        VE.style.borderBottomColor = color;
    }

    public static string TimeToString_dhms(long time)
    {
        int minute = (int)time / 60;
        time %= 60;
        int heure = (int)minute / 60;
        minute %= 60;
        int jours = (int)heure / 24;
        heure %= 24;

        return jours + "d " + heure + "h " + minute + "m " + time + "s";
    }

    public static string TimeToString_hm(long time)
    {
        int minute = (int)time / 60;
        time %= 60;
        int heure = (int)minute / 60;
        minute %= 60;
        int jours = (int)heure / 24;
        heure %= 24;

        return heure + "h" + minute + "m ";
    }

    public static void SetAlphaColor(Label label, float alpha)
    {
        Color color = label.resolvedStyle.color;
        color.a = alpha;
        label.style.color = color;
    }


    public static void AddMachineToData<T>(List<T> init, List<T> data)
    {
        foreach (T m in init)
        {
            if (!data.Contains(m))
                data.Add(m);
        }
    }

    public static bool HaveTheShipUpgrade(UpgradesShipElement.UpgradeType type)
    {
        switch (type)
        {
            case UpgradesShipElement.UpgradeType.AdditionalLevel:
                return (int)Ship.Current.type >= (int)SpaceShipData.SpaceShipElement.Iron;
            case UpgradesShipElement.UpgradeType.Magnectic:
                return (int)Ship.Current.type >= (int)SpaceShipData.SpaceShipElement.Magnetic;
            case UpgradesShipElement.UpgradeType.DamageOverTime:
                return (int)Ship.Current.type >= (int)SpaceShipData.SpaceShipElement.Fire;
            case UpgradesShipElement.UpgradeType.ZoneDamage:
                return (int)Ship.Current.type >= (int)SpaceShipData.SpaceShipElement.Poison;
        }
        return false;

    }

    public static spaceObject FindMeteor(bool biggest = false)
    {
        if (gameManager.instance.meteors.Count == 0) return null;
        List<spaceObject> meteors = gameManager.instance.meteors;

        BigNumber maxPv = new BigNumber(0);
        int n = -1;
        for (int i = 0; i < meteors.Count; i++)
        {
            if (meteors[i].type != spaceObject.meteorType.Diamand)
            {
                if (!biggest)
                    return meteors[i];
                else if (meteors[i].lifeMax.isBigger(maxPv))
                {
                    n = i;
                    maxPv = new BigNumber(meteors[i].lifeMax);
                }
            }
        }
        if (biggest && n >= 0)
            return meteors[n];

        return null;
    }

    public static spaceObject FindNearestMeteor(Vector3 position)
    {
        float minDist = float.MaxValue;
        List<spaceObject> meteors = gameManager.instance.meteors;

        int n = -1;
        for (int i = 0; i < meteors.Count; i++)
        {
            if (meteors[i].type != spaceObject.meteorType.Diamand)
            {
                float distance = Vector3.Distance(meteors[i].transform.position, position);
                if (distance < minDist)
                {
                    minDist = distance;
                    n = i;
                }
            }
        }
        if (n >= 0)
            return meteors[n];

        return null;
    }

    public static bool isInScreen(Vector3 position, float gap = 0f)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(position);
        return (viewportPos.x > gap && viewportPos.x < (1f - gap) 
            && viewportPos.y > gap && viewportPos.y < (1f - gap));
    }

    public static string GetMainRessourceLogoPath()
    {
        return  "logos/mainRessource/" + Ship.Current.type.ToString() + "/mainRessource";
    }

    public static Texture2D GetMainRessourceLogo()
    {
        string path = GetMainRessourceLogoPath();
        Texture2D texture2D = Resources.Load<Texture2D>(path);
        if (texture2D == null) {
            Debug.LogWarning("Can't load Texture2D at : " + path);
            return null;
        }
        return texture2D;
    }

    public static Color GetShipColor()
    {
        int colorNumber = Mathf.Clamp((int)Ship.Current.type , 0, Consts.SHIP_COLOR.Length - 1);
        return Consts.SHIP_COLOR[colorNumber];
    }

    public static Texture2D GetShipFragmentLogo()
    {
        string path = "logos/ShipFragments/" + Ship.Current.type.ToString();
        Texture2D texture2D = Resources.Load<Texture2D>(path);
        if (texture2D == null)
        {
            Debug.LogWarning("Can't load Texture2D at : " + path);
            return null;
        }
        return texture2D;
    }

    public static Color Hex(string hex)
    {
        //retourn une couleur a partir d'un hexa décimal
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public static int GetStellarMeteorProbability()
    {
        return (int)(Consts.BASE_STELLAR_METEOR_PROBABILITY * Stats.Instance.probabilitéOfOmega);
    }
    public static int GetStellarBossProbability()
    {
        return (int)(Consts.BASE_STELLAR_BOSS_PROBABILITY * Stats.Instance.probabilitéOfOmega);
    }
}
