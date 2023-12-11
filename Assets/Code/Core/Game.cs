using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node
{
    static RandomNumberGenerator RNG = new();

    public static int GetIntInRange(int Min, int Max)
    {
        return Game.RNG.RandiRange(Min, Max);
    }

    public static float GetFloatInRange(float Min, float Max)
    {
        return Game.RNG.RandfRange(Min, Max);
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
}
