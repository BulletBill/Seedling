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
}