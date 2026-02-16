using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Utility
{

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

}
