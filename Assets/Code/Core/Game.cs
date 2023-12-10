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
}
