using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
    public static String Group = "Spawner";
    [Export] public bool Active = true;
    [Export] public float MinSpawnTime = 0.25f;
    [Export] public float MaxSpawnTime = 2.50f;
    public Queue<Enemy> EnemiesToPlace = new();
    double SpawnTimer = 2.0f;

    public override void _Ready()
    {
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (EnemiesToPlace.Count > 0 && SpawnTimer > 0.0f)
        {
            SpawnTimer -= delta;
            if (SpawnTimer <= 0.0f)
            {
                SpawnEnemy();
                SpawnTimer = Game.GetFloatInRange(MinSpawnTime, MaxSpawnTime);
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
