using Godot;
using System;
using System.Numerics;

public partial class MathHelper : Node
{
    static RandomNumberGenerator RNG = new();
    
    public static int GetIntInRange(int Min, int Max)
    {
        return RNG.RandiRange(Min, Max);
    }

    public static int GetIntInRange(Vector2I Range)
    {
        return RNG.RandiRange(Range.X, Range.Y);
    }

    public static float GetFloatInRange(float Min, float Max)
    {
        return RNG.RandfRange(Min, Max);
    }
    
    public static float GetFloatInRange(Godot.Vector2 Range)
    {
        return RNG.RandfRange(Range.X, Range.Y);
    }

    public static Godot.Vector2 PositionOffset(Godot.Vector2 Start, float AngleRads, float Distance)
    {
        Godot.Vector2 ret = Start;
        ret.X = Start.X + Distance * Mathf.Cos(AngleRads);
        ret.Y = Start.Y + Distance * Mathf.Sin(AngleRads);
        return ret;
    }

    public static String GetTimeFromSeconds(float InTime)
    {
        int Seconds = Mathf.FloorToInt(InTime % 60.0f);
        int Minutes = Mathf.FloorToInt((InTime/60.0f) % 60.0f);
        int Hours = Mathf.FloorToInt((InTime/60.0f) / 60.0f);

        String ret = "";
        if (Hours > 0) { ret += Hours.ToString() + ":"; }
        if (Minutes > 0) { ret += Minutes.ToString() + ":"; } else { ret += "0:"; }
        if (Seconds < 10) { ret += "0"; }
        ret += Seconds.ToString();

        return ret;
    }

    public static float LeveledDamageFormula(float StartingValue, int TowerLevel)
    {
        return StartingValue * (1.0f + (TowerLevel * 0.5f)) * (1.0f + Mathf.Pow(TowerLevel * 0.3f, 2.0f));
    }
}
