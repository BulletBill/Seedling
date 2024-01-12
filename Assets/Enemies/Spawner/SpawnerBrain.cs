using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Diagnostics.CodeAnalysis;

public partial class SpawnerBrain : Node
{
    [Export] public Array<PackedScene> EnemiesToSpawn = new();
    List<EnemySpawner> Spawners = new();
    public int SpawningPool = 0;
    public float SpawnPressure = 0.0f;

    public float BigWaveTimer { get; protected set; } = 301.0f;
    int BigWaveSpawningPool = 100;

    // DEBUG
    public bool DisableSpawns = false;

    public override void _Ready()
    {
        Player.Singleton.GrassGrown += GrassGrown;
        foreach (Node SpawnerNode in GetTree().GetNodesInGroup(EnemySpawner.Group))
        {
            if (SpawnerNode is EnemySpawner Spawner)
            {
                Spawners.Add(Spawner);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (DisableSpawns) return;
        if (BigWaveTimer > 0)
        {
            BigWaveTimer -= (float)delta;
        }
    }

    void GrassGrown(int Count)
    {
        SpawnPressure += 3.0f * Count;
        SpawningPool += Count;

        //GD.Print("SpawnerBrain.GrassGrown: Current Pressure is " + SpawnPressure.ToString() + " Spawning pool is " + SpawningPool.ToString());

        if (Game.GetFloatInRange(0.0f, 100.0f) < SpawnPressure)
        {
            SpawnEnemies();
            SpawnPressure = 0.0f;
        }
    }

    void SpawnEnemies()
    {
        if (DisableSpawns) return;
        if (EnemiesToSpawn.Count <= 0) return;
        if (Spawners.Count <= 0) return;

        GD.Print("SpawnerBrain.TryToSpawn: Spawn wave!");
        while(SpawningPool > 0)
        {
            int EnemyIndex = Game.GetIntInRange(0, EnemiesToSpawn.Count - 1);
            int SpawnerIndex = Game.GetIntInRange(0, Spawners.Count - 1);
            
            Enemy NewEnemy = EnemiesToSpawn[EnemyIndex].InstantiateOrNull<Enemy>();
            if(NewEnemy != null)
            {
                Spawners[SpawnerIndex].EnemiesToPlace.Enqueue(NewEnemy);
                SpawningPool -= Math.Max(NewEnemy.SpawnCost, 1); // Kind of failsafe
                //GD.Print("SpawnerBrain.TryToSpawn: Spawned enemy " + NewEnemy.Name + " at spawner " + SpawnerIndex.ToString());
            }
            else
            {
                SpawningPool = 0; //Failsafe
            }
        }
    }
}
