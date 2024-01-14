using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Game : Node
{
    [Export] public PackedScene DamageNumberPrefab;
    public static Game Singleton { get; protected set; }
    
    float PausedSpeed = 0.0f;
    float GameSpeed = 1.0f;
    float HighSpeed = 2.0f;
    bool Paused = false;
    bool MenuOpen = false;
    bool SpedUp = false;

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            Paused = !Paused;
            EvalutateGameSpeed();
        }
    }

    void EvalutateGameSpeed()
    {
        if (Paused || MenuOpen) GameSpeed = PausedSpeed;
        else if (SpedUp) GameSpeed = HighSpeed;
        else GameSpeed = 1.0f;
    }

    // Static functions
    public static float GetSpeed()
    {
        if (Game.Singleton == null) return 1.0f;
        return Game.Singleton.GameSpeed;
    }

    public static void RestartGame()
    {
        if (Game.Singleton == null) return;
    }
}
