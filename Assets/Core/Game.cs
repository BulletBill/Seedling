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
    [Export] public Godot.Collections.Array<LogCategory> ShowLogsFor = new();
    [Export] public Godot.Collections.Array<LogCategory> HideErrorsFor = new();
    public System.Collections.Generic.Dictionary<String, PackedScene> LevelMap = new();
    public static Game Singleton { get; protected set; }

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

    // Static functions
    public static PackedScene GetSceneByName(String SceneName)
    {
        if (Singleton == null) return null;
        if (Singleton.LevelMap.ContainsKey(SceneName) == false) return null;
        return Singleton.LevelMap[SceneName];
    }

    public static void Log(LogCategory Category, params object[] what)
    {
        if (Game.Singleton == null) { GD.Print(what[0]); return; }
        if (Game.Singleton.ShowLogsFor.Contains(Category) == false) return;
        GD.Print(Category.ToString() + ": " + what[0]);
    }

    public static void LogError(LogCategory Category, params object[] what)
    {
        if (Game.Singleton == null) { GD.PrintErr(what[0]); return; }
        if (Game.Singleton.HideErrorsFor.Contains(Category)) return;
        GD.PrintErr(Category.ToString() + ": " + what[0]);
    }
}

public enum LogCategory
{
    Core,
    Player,
    Tower,
    Objectives,
    Map,
    UI,
    Cursor,
    Enemy,
    EnemySpawner,
}