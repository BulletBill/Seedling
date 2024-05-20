using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Security.Cryptography;

public partial class SpawnerBrain : Node
{
    [Export] public Godot.Collections.Array<R_SpawnCount> PossibleSpawns = new();
    List<EnemySpawner> Spawners = new();

    // Timed waves
    [Export] public int SecondsUntilNextWave { get; protected set; } = 300;
    [Export] public float TimedSpawningPool = 16.0f;
    [Export] public float SuccessPoolMultiplier = 1.5f;
    [Export] public float FailurePoolMultiplier = 1.3f;
    int WaveCount = 1;
    bool FinalWave = false;
    float BigWaveTimer = 301.0f;
    int TimerSecondsLeft = 300;
    R_SpawnWave NextTimedWave = new();

    // Expansion waves
    [Export] Vector2I ExpansionRange = new(18,26);
    [Export] float ExpansionLevelPerSpawn = 0.25f;
    [Export] float ExpansionSpawningPool = 6;
    [Export] float ExpansionPoolMultiplier = 1.2f;
    float ExpansionLevel = 1;
    int ExpansionRemaining = 0;
    R_SpawnWave NextExpansionWave = new();


    // DEBUG
    public bool DisableSpawns = false;

    public override void _EnterTree()
    {
        MainMap.Register(MainMap.SignalName.PlayerExpanded, Callable.From((int n) => GrassGrown(n)));
        EnemyEvent.Register(EnemyEvent.SignalName.SetDisableSpawns, Callable.From((bool b) => SetDisableSpawns(b)));
        EnemyEvent.Register(EnemyEvent.SignalName.StartFinalWave, Callable.From(() => StartFinalWave()));
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

        int NextExpansionRequired = MathHelper.GetIntInRange(ExpansionRange);
        ExpansionRemaining = 37 + NextExpansionRequired;
        BigWaveTimer = SecondsUntilNextWave;

        EnemyEvent.Broadcast(EnemyEvent.SignalName.WaveTimerChanged, SecondsUntilNextWave, SecondsUntilNextWave);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.PlayerExpansionChanged, 0, NextExpansionRequired);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ExpansionWaveCountChanged, ExpansionLevel);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, WaveCount);

        Game.Log(LogCategory.EnemySpawner, "Calculating timed spawn " + WaveCount.ToString() + " with a pool of " + ((int)TimedSpawningPool).ToString());
        CalculateNextWave(NextTimedWave, WaveCount, (int)TimedSpawningPool);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextTimedWave, NextTimedWave);

        Game.Log(LogCategory.EnemySpawner, "Calculating expansion spawn " + ExpansionLevel.ToString() + " with a pool of " + ((int)ExpansionSpawningPool).ToString());
        CalculateNextWave(NextExpansionWave, (int)ExpansionLevel, (int)ExpansionSpawningPool);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextExpandWave, NextExpansionWave);
    }

    public override void _Process(double delta)
    {
        if (DisableSpawns) return;
        if (BigWaveTimer > 0 && !FinalWave)
        {
            BigWaveTimer -= (float)delta * Level.GetSpeed();

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
            ExpansionRemaining += MathHelper.GetIntInRange(ExpansionRange.X, ExpansionRange.Y);
            SpawnExpansionWave();
        }

        EnemyEvent.Broadcast(EnemyEvent.SignalName.PlayerExpansionChanged, ExpansionRemaining, ExpansionRange.Y);
    }

    void SpawnExpansionWave()
    {
        SpawnEnemies(NextExpansionWave);

        ExpansionLevel++;
        int SpawningPool = Mathf.FloorToInt(ExpansionSpawningPool * Math.Pow(ExpansionPoolMultiplier, ExpansionLevel));

        Game.Log(LogCategory.EnemySpawner, "Calculating expansion spawn " + ExpansionLevel.ToString() + " with a pool of " + ((int)ExpansionSpawningPool).ToString());
        CalculateNextWave(NextExpansionWave, (int)ExpansionLevel, SpawningPool);

        EnemyEvent.Broadcast(EnemyEvent.SignalName.ExpansionWaveCountChanged, ExpansionLevel);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextExpandWave, NextExpansionWave);
    }

    void SpawnTimedWave()
    {
        SpawnEnemies(NextTimedWave);

        WaveCount++;
        int SpawningPool = Mathf.FloorToInt(TimedSpawningPool * Math.Pow(SuccessPoolMultiplier, WaveCount));
        
        Game.Log(LogCategory.EnemySpawner, "Calculating timed spawn " + WaveCount.ToString() + " with a pool of " + ((int)TimedSpawningPool).ToString());
        CalculateNextWave(NextTimedWave, WaveCount, SpawningPool);

        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, WaveCount);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextTimedWave, NextTimedWave);
    }

    void StartFinalWave()
    {
        SpawnTimedWave();
        FinalWave = true;
    }

    void SpawnEnemies(R_SpawnWave WaveData)
    {
        if (DisableSpawns) return;
        if (Spawners.Count <= 0) return;
        if (WaveData.SpawnCounts.Count <= 0) return;

        Game.Log(LogCategory.EnemySpawner, "Spawn wave!");

        int SpawnerIndex = MathHelper.GetIntInRange(0, Spawners.Count - 1);
        foreach (R_SpawnCount SpawnCount in WaveData.SpawnCounts)
        {
            for (int i = 0; i < SpawnCount.Count; i++)
            {
                Enemy NewEnemy = SpawnCount.Data.Spawn();
                if (NewEnemy != null)
                {
                    Spawners[SpawnerIndex].EnemiesToPlace.Enqueue(NewEnemy);
                }
            }
        }
    }

    void CalculateNextWave(R_SpawnWave Wave, int Level, int SpawningPool)
    {
        Wave.SpawnCounts = new();
        int TotalWeight = 0;
        // Gather enemies that can possibly spawn this wave
        foreach (R_SpawnCount spawnCount in PossibleSpawns)
        {
            if (spawnCount.WaveRange.Y == 0)
            {
                Game.LogError(LogCategory.EnemySpawner, spawnCount.Data.DisplayName + " has a max wave of 0!");
                continue;
            }
            if (Level >= spawnCount.WaveRange.X && (Level <= spawnCount.WaveRange.Y || spawnCount.WaveRange.Y < 0))
            {
                TotalWeight += spawnCount.Data.SpawnWeight;
                Wave.SpawnCounts.Add(new R_SpawnCount(spawnCount));
            }
        }
        
        // Randomly add enemies to the spawn list by weight
        if (TotalWeight > 0 && SpawningPool > 0)
        {
            while (SpawningPool > 0)
            {
                int Sum = 0;
                int TargetValue = MathHelper.GetIntInRange(1, TotalWeight);
                foreach (R_SpawnCount spawnCount in Wave.SpawnCounts)
                {
                    if (TargetValue <= spawnCount.Data.SpawnWeight + Sum)
                    {
                        spawnCount.Count++;
                        SpawningPool -= spawnCount.Data.SpawnCost;
                        Sum = 0;
                        Game.Log(LogCategory.EnemySpawner, "Adding" + spawnCount.Data.DisplayName + " to next wave. " + SpawningPool.ToString() + " spawn cost remains.");
                        break;
                    }

                    Sum += spawnCount.Data.SpawnWeight;
                }

                if (Sum > 0)
                {
                    Game.LogError(LogCategory.EnemySpawner, "Failed to add enemy to spawn count!");
                    SpawningPool--;
                }
            }
        }
    }

    public void SetDisableSpawns(bool Disabled)
    {
        DisableSpawns = Disabled;
    }
}
