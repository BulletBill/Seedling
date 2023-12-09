using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node
{
    public static List<Tower> DefendTargets = new();
    static RandomNumberGenerator RNG = new();

    public static int GetIntInRange(int Min, int Max)
    {
        return Game.RNG.RandiRange(Min, Max);
    }
}
