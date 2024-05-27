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
    [Export] public float TimeLostFromExpansion = 2.0f;
    int WaveCount = 1;
    bool FinalWave = false;
    bool PlayerHurt = false;
    float BigWaveTimer = 301.0f;
    int TimerSecondsLeft = 300;
    int ExpansionAmount = 0;
    R_SpawnWave NextTimedWave = new();

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

        BigWaveTimer = SecondsUntilNextWave;

        EnemyEvent.Broadcast(EnemyEvent.SignalName.WaveTimerChanged, SecondsUntilNextWave, SecondsUntilNextWave);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, WaveCount);

        Game.Log(LogCategory.EnemySpawner, "Calculating timed spawn " + WaveCount.ToString() + " with a pool of " + ((int)TimedSpawningPool).ToString());
        CalculateNextWave(NextTimedWave, WaveCount, (int)TimedSpawningPool);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextTimedWave, NextTimedWave);
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
        ExpansionAmount += Count;
        if (ExpansionAmount <= 37) return;
        
        BigWaveTimer -= Count * TimeLostFromExpansion;

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

    void PlayerDamaged(int NewLife)
    {
        PlayerHurt = true;
    }

    void SpawnTimedWave()
    {
        Game.Log(LogCategory.EnemySpawner, "Spawning Wave " + WaveCount.ToString());
        SpawnEnemies(NextTimedWave);

        WaveCount++;
        float Multi = PlayerHurt ? FailurePoolMultiplier : SuccessPoolMultiplier;
        int SpawningPool = Mathf.FloorToInt(TimedSpawningPool * Math.Pow(Multi, WaveCount));
        
        Game.Log(LogCategory.EnemySpawner, "Calculating next wave " + WaveCount.ToString() + " with a pool of " + ((int)TimedSpawningPool).ToString());
        CalculateNextWave(NextTimedWave, WaveCount, SpawningPool);

        EnemyEvent.Broadcast(EnemyEvent.SignalName.TimedWaveCountChanged, WaveCount);
        EnemyEvent.Broadcast(EnemyEvent.SignalName.ShowNextTimedWave, NextTimedWave);

        PlayerHurt = false;
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
                        Game.LogSpam(LogCategory.EnemySpawner, "Adding" + spawnCount.Data.DisplayName + " to next wave. " + SpawningPool.ToString() + " spawn cost remains.");
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

        // Remove 0 count spawns
        for (int i = Wave.SpawnCounts.Count - 1; i >= 0; i--)
        {
            if (Wave.SpawnCounts[i].Count <= 0)
            {
                Wave.SpawnCounts.RemoveAt(i);
            }
        }
    }

    public void SetDisableSpawns(bool Disabled)
    {
        DisableSpawns = Disabled;
    }
}
