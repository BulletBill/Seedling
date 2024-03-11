using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Level : Node
{
    [Export] public String ExitLevel = "LevelSelect";

    float PausedSpeed = 0.0f;
    float GameSpeed = 1.0f;
    float HighSpeed = 2.0f;
    bool Paused = false;
    List<PopupMenu> OpenMenus = new();
    bool SpedUp = false;
    bool GameIsOver = false;

    public static Level Singleton { get; protected set; }

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
        if (Paused || OpenMenus.Count > 0) GameSpeed = PausedSpeed;
        else if (SpedUp) GameSpeed = HighSpeed;
        else GameSpeed = 1.0f;
    }

    // Static functions
    public static float GetSpeed()
    {
        if (Singleton == null) return 1.0f;
        return Singleton.GameSpeed;
    }

    public static void AddOpenMenu(PopupMenu NewMenu)
    {
        if (Singleton == null) return;
        if (Singleton.OpenMenus.Contains(NewMenu)) return;

        Singleton.OpenMenus.Add(NewMenu);
        Singleton.EvalutateGameSpeed();
    }

    public static void RemoveOpenMenu(PopupMenu RemoveMenu)
    {
        if (Singleton == null) return;
        Singleton.OpenMenus.Remove(RemoveMenu);
        Singleton.EvalutateGameSpeed();
    }

    public static void GameOver(String PopupResult)
    {
        if (Singleton == null) return;
        Singleton.GameIsOver = true;
        MenuEvent.Broadcast(PopupResult);
    }

    public static bool IsGameOver()
    {
        if (Singleton == null) return false;
        return Singleton.GameIsOver;
    }
}
