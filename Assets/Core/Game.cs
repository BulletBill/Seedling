using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.ComponentModel;
using Godot.NativeInterop;

public partial class Game : Node
{
    [Export] public Node StartingScene;
    [Export] public Godot.Collections.Array<PackedScene> LevelDefMap = new();
    public System.Collections.Generic.Dictionary<String, PackedScene> LevelMap = new();
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

    public override void _Ready()
    {
        SceneTransition.SetCurrentScene(StartingScene);

        foreach (var L in LevelDefMap)
        {
            LevelMap.Add(((string[])L._Bundled["names"])[0], L);
        }
        LevelDefMap.Clear();
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
        if (Singleton == null) return 1.0f;
        return Singleton.GameSpeed;
    }

    public static void RestartGame()
    {
        if (Singleton == null) return;
    }

    public static PackedScene GetSceneByName(String SceneName)
    {
        if (Singleton == null) return null;
        if (Singleton.LevelMap.ContainsKey(SceneName) == false) return null;
        return Singleton.LevelMap[SceneName];
    }
}