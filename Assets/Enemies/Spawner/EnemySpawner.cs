using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
    public static String Group = "Spawner";
    [Export] public bool Active = true;
    [Export] public float MinSpawnTime = 0.50f;
    [Export] public float MaxSpawnTime = 0.75f;
    public Queue<Enemy> EnemiesToPlace = new();
    double SpawnTimer = 0.5f;

    public override void _Ready()
    {
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (EnemiesToPlace.Count > 0 && SpawnTimer > 0.0f)
        {
            SpawnTimer -= delta * Game.GetSpeed();
            if (SpawnTimer <= 0.0f)
            {
                SpawnEnemy();
                SpawnTimer = MathHelper.GetFloatInRange(MinSpawnTime, MaxSpawnTime);
            }
        }
    }

    void SpawnEnemy()
    {
        if (EnemiesToPlace.Count < 0) return;

        Enemy NewEnemy = EnemiesToPlace.Dequeue();
        if(NewEnemy != null)
        {
            MainMap.Singleton.AddChild(NewEnemy);
            NewEnemy.GlobalPosition = GlobalPosition;
            NewEnemy.Active = true;
        }
    }
}
