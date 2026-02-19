using NUnit.Framework.Constraints;
using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BigNumber
{
    public double Mantisse;
    public int Exp;

    public BigNumber(double mantisse, int exp)
    {
        Mantisse = mantisse;
        Exp = exp;
        Normalize();

    }
    public BigNumber(double mantisse)
    {
        Mantisse = mantisse;
        Exp = 0;
        Normalize();
    }

    public BigNumber(BigNumber other) 
    {
        Mantisse = other.Mantisse;
        Exp = other.Exp;
        Normalize();
    }

    public void Set(int n)
    {
        Mantisse = n;
        Exp = 0;
        Normalize();
    }

    public void Set(BigNumber other)
    {
        Mantisse = other.Mantisse;
        Exp = other.Exp;
        Normalize();
    }

    public void Normalize()
    {
        if (Mantisse < 1.0f)
        {
           while(Mantisse < 1.0f && Exp >0)
           {
                Mantisse *= 10;
                Exp--;
           }
        }
        else if (Mantisse >= 10.0f) 
        {
            while (Mantisse >= 10.0f)
            {
                Mantisse /= 10;
                Exp++;
            }
        }
        if(Exp == 0)
        {
            Mantisse = Math.Round(Mantisse);
        }
        else
        {
            Mantisse = Math.Round(Mantisse*1000f)/1000f;
        }

    }

    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a); // ou new BigNumber(a)
        result.Add(b);
        return result;
    }

    public static BigNumber operator +(BigNumber a, int b)
    {
        if (a is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a); // ou new BigNumber(a)
        result.Add(b);
        return result;
    }

    public void Add(BigNumber n)
    {
        if (n.Exp > Exp)
        {
            int nexp = n.Exp - Exp;
            double x = Mantisse / Math.Pow(10, nexp);
            Mantisse =n.Mantisse + x;
            Exp = n.Exp ;
            
        }
        else if (n.Exp < Exp)
        {
            int nexp = Exp - n.Exp;
            double x = n.Mantisse / Math.Pow(10, nexp);
            Mantisse += x;
        }
        else
        {
            Mantisse += n.Mantisse;
        }
        
        Normalize();
    }

    public void Add(float n)
    {
        this.Add(new BigNumber(n));
        Normalize();
    }

    public static BigNumber operator -(BigNumber a, BigNumber b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a); // ou new BigNumber(a)
        result.Subtract(b);
        return result;
    }

    public static BigNumber operator -(BigNumber a)
    {
        if (a is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a);
        result.Mantisse = -result.Mantisse;
        return result;
    }

    public void Subtract(BigNumber n)
    {
        if (n.Exp > Exp)
        {
            Exp = 0;
            Mantisse = 0;

        }
        else if (n.Exp < Exp)
        {
            int nexp = Exp - n.Exp; 
            double x = n.Mantisse / Math.Pow(10, nexp); 

            Mantisse -= x;
        }
        else
        {
            Mantisse -= n.Mantisse;
        }
        Normalize();
    }

    public static BigNumber operator *(BigNumber a, float b)
    {
        if (a is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a); // ou new BigNumber(a)
        result.Multiply(b);
        return result;
    }

    public static BigNumber operator *(BigNumber a, BigNumber b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException();

        BigNumber result = new BigNumber(a); // ou new BigNumber(a)
        result.Multiply(b);
        return result;
    }

    public void Multiply(float n)
    {
        if(n > 0)
        {
            Mantisse *= n;
            Normalize();
        }
        if (n == 0)
        {
            Mantisse = 0;
            Exp = 0;
        }
    }

    public void Multiply(double n)
    {
        if (n > 0)
        {
            Mantisse *= n;
            Normalize();
        }
        if (n == 0)
        {
            Mantisse = 0;
            Exp = 0;
        }
    }

    public void Multiply(BigNumber n)
    {
        Mantisse *= n.Mantisse;
        Exp += n.Exp;
        Normalize();
    }


    public void Divide(float n)
    {
        if (n > 0)
        {
            Mantisse /= n;
            Normalize();
        }
    }


    public double GetPercentByDivided(BigNumber n)
    {
        if (n.Exp > Exp)
        {
            BigNumber x = new BigNumber(Mantisse, Exp);
            int exp = n.Exp - x.Exp;
            x.Mantisse /= Mathf.Pow(10, exp);
            return (x.Mantisse / n.Mantisse) * 100f;

        }
        else if(n.Exp == Exp)
        {
            return (Mantisse / n.Mantisse) * 100f;
        }
        else 
        {
            BigNumber x = new BigNumber(Mantisse, Exp);
            int exp = x.Exp- n.Exp;
            x.Mantisse *= Mathf.Pow(10, exp);
            return (x.Mantisse / n.Mantisse) * 100f;
        }
    }

    public override string ToString()
    {
        if(Settings.Instance != null && Settings.Instance.scientific)
            return getScientificNotation();
        return getNormalNotation();
    }

    private string getNormalNotation()
    {
        string prefix;
        switch (Exp)
        {
            case int n when (n >= 0 && n < 3):
                prefix = "";
                double nMantisse = Mantisse * Mathf.Pow(10, Exp % 3);
                return nMantisse.ToString("F0") + prefix;
            case int n when (n >= 3 && n < 6):
                prefix = "k";
                break;
            case int n when (n >= 6 && n < 9):
                prefix = "m";
                break;
            case int n when (n >= 9 && n < 12):
                prefix = "M";
                break;
            case int n when (n >= 12 && n < 15):
                prefix = "B";
                break;
            default:
                prefix = "x" + Exp;
                break;
        }
        double nnMantisse = Mantisse * Mathf.Pow(10, Exp % 3);
        switch (Exp % 3)
        {
            case 1:
                return nnMantisse.ToString("F1") + prefix;
            case 2:
                return nnMantisse.ToString("F0") + prefix;
            default:
                return nnMantisse.ToString("F2") + prefix;
        }
    }

    private string getScientificNotation()
    {
        if(Exp == 0)
            return Mantisse.ToString("F0");
        else if(Exp == 1)
            return Mantisse.ToString("F1") + "e" + Exp;

        return Mantisse.ToString("F2") + "e" + Exp; 
    }

    public static bool operator >(BigNumber a, BigNumber b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException();

        return a.isBigger(b);
    }
    public static bool operator <(BigNumber a, BigNumber b)
    {
        if (a is null || b is null)
            throw new ArgumentNullException();

        return b.isBigger(a);
    }

    public bool isBigger(int n)
    {
        BigNumber nBig = new BigNumber(n, 1);

        if (nBig.Exp > Exp)
        {
            return false;
        }
        if (nBig.Exp == Exp && nBig.Mantisse > Mantisse)
        {
            return false;
        }
        return true;
    }
    public bool isBigger(BigNumber nBig)
    {
        if (nBig.Exp > Exp)
        {
            return false;
        }
        if (nBig.Exp == Exp && nBig.Mantisse > Mantisse)
        {
            return false;
        }
        return true;
    }

    public bool EqualZero()
    {
        if ((int)Mantisse > 0)
        {
            
            return false;
        }
        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj is not BigNumber other) return false;

        return Exp == other.Exp && Mantisse == other.Mantisse;
    }

    public static bool operator ==(BigNumber a, BigNumber b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(BigNumber a, BigNumber b)
    {
        return !(a == b);
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Exp, Mantisse);
    }



    public static string floatToTimeHour(float time)
    {
        int sec = (int)time % 60;
        time -= sec;
        int min = ((int)time/60) % 3600;
        time -= min;
        int heure = (int)time / 3600;

        string t = heure + "h" + min + "m" + sec + "s";
        return t;
    }

    public static string floatToTimeMinute(float time)
    {
        int sec = (int)time % 60;
        time -= sec;
        int min = ((int)time / 60);

        string t = min + "m" + sec + "s";
        return t;
    }
}
