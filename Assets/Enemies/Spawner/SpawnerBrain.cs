using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Diagnostics.CodeAnalysis;

public partial class SpawnerBrain : Node
{
    [Export] public int ExpansionPerWave { get; protected set; } = 10;
    [Export] public int SecondsUntilNextWave { get; protected set; } = 300;
    List<EnemySpawner> Spawners = new();
    
    [Export] public Godot.Collections.Array<R_SpawnWave> ExpansionWaves = new();
    int ExpansionIndex = 0;
    int ExpansionRemaining = 0;

    [Export] public Godot.Collections.Array<R_SpawnWave> TimedWaves = new();
    int TimedIndex = 0;
    float BigWaveTimer = 301.0f;
    int TimerSecondsLeft = 300;

    // DEBUG
    public bool DisableSpawns = false;

    public override void _EnterTree()
    {
        MainMap.Register(MainMap.SignalName.PlayerExpanded, Callable.From((int n) => GrassGrown(n)));
        EnemyEvent.Register(EnemyEvent.SignalName.SetDisableSpawns, Callable.From((bool b) => SetDisableSpawns(b)));
    }

    public override void _Ready()
    {
        foreach (Node SpawnerNode in GetTree().GetNodesInGroup(EnemySpawner.Group))
        {
            if (SpawnerNode is EnemySpawner Spawner)
            {
                Spawners.Add(Spawner);
            }
        }

        ExpansionRemaining = 37 + ExpansionPerWave;
        BigWaveTimer = SecondsUntilNextWave;

        EnemyEvent.Broadcast(EnemyEvent.SignalName.WaveTimerChanged, SecondsUntilNextWave, SecondsUntilNextWave);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.PlayerExpansionChanged, ExpansionPerWave, ExpansionPerWave);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ExpansionWaveCountChanged, ExpansionIndex + 1);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, TimedIndex + 1);
        if (TimedWaves.Count > 0)
        {
            EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextTimedWave, TimedWaves[0]);
        }
    }

    public override void _Process(double delta)
    {
        if (DisableSpawns) return;
        if (BigWaveTimer > 0)
        {
            BigWaveTimer -= (float)delta * Game.GetSpeed();

            // Broadcast when seconds left changes
            int LastSecond = TimerSecondsLeft;
            TimerSecondsLeft = Mathf.FloorToInt(BigWaveTimer);
            if (LastSecond != TimerSecondsLeft)
            {
                EnemyEvent.Broadcast(EnemyEvent.SignalName.WaveTimerChanged, TimerSecondsLeft, SecondsUntilNextWave);
            }

            if (BigWaveTimer <= 0)
            {
                BigWaveTimer += SecondsUntilNextWave;
                SpawnTimedWave();
            }
        }
    }

    void GrassGrown(int Count)
    {
        ExpansionRemaining -= Count;

        if (ExpansionRemaining <= 0)
        {
            ExpansionRemaining += ExpansionPerWave;
            SpawnExpansionWave();
        }

        EnemyEvent.Broadcast(EnemyEvent.SignalName.PlayerExpansionChanged, ExpansionRemaining, ExpansionPerWave);
    }

    void SpawnExpansionWave()
    {
        int WaveIndex = Math.Min(ExpansionIndex++, ExpansionWaves.Count - 1);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ExpansionWaveCountChanged, ExpansionIndex + 1);
        if (ExpansionWaves.Count > 0)
        {
            SpawnEnemies(ExpansionWaves[WaveIndex]);
        }
    }

    void SpawnTimedWave()
    {
        int WaveIndex = Math.Min(TimedIndex++, TimedWaves.Count - 1);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, TimedIndex + 1);
        if (TimedWaves.Count > 0)
        {
            SpawnEnemies(TimedWaves[WaveIndex]);
        }
    }

    void SpawnEnemies(R_SpawnWave WaveData)
    {
        if (DisableSpawns) return;
        if (Spawners.Count <= 0) return;
        if (WaveData.SpawnCounts.Count <= 0) return;

        GD.Print("SpawnerBrain.TryToSpawn: Spawn wave!");
        foreach (R_SpawnCount SpawnCount in WaveData.SpawnCounts)
        {
            for (int i = 0; i < SpawnCount.Count; i++)
            {
                Enemy NewEnemy = SpawnCount.SpawnData.EnemyScene.InstantiateOrNull<Enemy>();
                if (NewEnemy != null)
                {
                    int SpawnerIndex = MathHelper.GetIntInRange(0, Spawners.Count - 1);
                    Spawners[SpawnerIndex].EnemiesToPlace.Enqueue(NewEnemy);
                }
            }
        }
    }

    public void SetDisableSpawns(bool Disabled)
    {
        DisableSpawns = Disabled;
    }
}
