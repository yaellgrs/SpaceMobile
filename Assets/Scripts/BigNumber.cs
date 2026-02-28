using NUnit.Framework.Constraints;
using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BigNumber
{
    //on ne calcule que sur 15 puissance de 10 qu'on connait, au dela le resultat est insignifiant
    //exemple 1e100 + 1e75 = 1e100, inutile de calculer
    //tableau statique pour ne le créer qu'une seule fois, readonly pour ne pas le modifier
    const int MAX_EXP_DIFF = 15;
    private static readonly double[] Pow10 = {1d, 10d, 100d, 1000d, 10000d, 100000d, 1000000d, 10000000d, 100000000d,
    1000000000d, 10000000000d, 100000000000d, 1000000000000d, 10000000000000d, 100000000000000d, 1000000000000000d
    };

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
        if (Mantisse == 0)
        {
            Exp = 0;
            return;
        }
        double abs = Math.Abs(Mantisse);
        int shift = (int)Math.Floor(Math.Log10(abs));

        if (shift > 0)
            Mantisse /= shift < MAX_EXP_DIFF ? Pow10[shift] : Math.Pow(10, shift);
        else 
            Mantisse *= shift > -MAX_EXP_DIFF ? Pow10[-shift] : Math.Pow(10, -shift);

        Exp += shift;

        if (Exp < 0) {
            double realValue = Mantisse * Math.Pow(10, Exp);

            realValue = Math.Round(realValue);

            Mantisse = realValue;
            Exp = 0;
        }
        else if(Exp == 0)
            Mantisse = Math.Round(Mantisse);
        else
            Mantisse = Math.Round(Mantisse * 1000f) / 1000f;

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

    public void Add(BigNumber n, bool normalize = false)
    {
        if (n.Exp > Exp)
        {
            int nexp = n.Exp - Exp;
            if(nexp > MAX_EXP_DIFF)
            {
                Exp = n.Exp;
                Mantisse = n.Mantisse;
                return;
            }
            double x = Mantisse / Pow10[nexp];
            Mantisse =n.Mantisse + x;
            Exp = n.Exp ;
            
        }
        else if (n.Exp < Exp)
        {
            int nexp = Exp - n.Exp;
            if (nexp > MAX_EXP_DIFF) return;

            double x = n.Mantisse / Pow10[nexp];
            Mantisse += x;
        }
        else
        {
            Mantisse += n.Mantisse;
        }
        
        if(normalize) Normalize();
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
            if(nexp > MAX_EXP_DIFF) return;
            double x = n.Mantisse / Pow10[nexp]; 

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

    public void Multiply(double n, bool normalize = true)
    {
        if (n > 0)
        {
            Mantisse *= n;
            if (normalize) Normalize();
        }
        if (n == 0)
        {
            Mantisse = 0;
            Exp = 0;
        }
    }

    public void Multiply(BigNumber n, bool normalize = true)
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

    public void Divide(BigNumber n)
    {
        if (n.Mantisse > 0)
        {
            Mantisse /= n.Mantisse;
            Exp -= n.Exp;
            Normalize();
        }
    }


    public double GetPercentByDivided(BigNumber n)
    {
        if (n.Exp > Exp)
        {
            BigNumber x = new BigNumber(Mantisse, Exp);
            int exp = n.Exp - x.Exp; ///10 / 10000
            if (exp > MAX_EXP_DIFF) return 0f;
            x.Mantisse /=  Pow10[exp];
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
            if (exp > MAX_EXP_DIFF) return 100f;
            x.Mantisse *= Pow10[exp];
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
        if(Mantisse == 0) return "0";

        string prefix;

        string[] Prefixes = { "", "k", "m", "M", "B" };
        prefix = Exp / 3 < Prefixes.Length ? Prefixes[Exp / 3] : "x" + Exp;

        int mod = Exp % 3;
        double nnMantisse = Mantisse * Pow10[mod];

        if(Exp <= 1)
            return nnMantisse.ToString("F0");
        
        switch (mod)
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
