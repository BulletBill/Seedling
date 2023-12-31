using Godot;
using System;

public partial class EnemyController : Node
{
    public static EnemyController Singleton;

    public override void _EnterTree()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        
    }

    public static SpawnerBrain GetSpawnerBrain()
    {
        if (EnemyController.Singleton == null) return null;

        return EnemyController.Singleton.GetNodeOrNull<SpawnerBrain>("SpawnerBrain");
    }
}
