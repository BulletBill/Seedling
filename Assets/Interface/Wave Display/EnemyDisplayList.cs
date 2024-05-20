using Godot;
using System;
using Godot.Collections;
using System.Security;

public enum EWaveType
{
    None,
    Timed,
    Expansion,
}

public partial class EnemyDisplayList : Node
{
    [Export] public EWaveType WaveType = EWaveType.Timed;
    public Array<EnemyDisplay> EnemyDisplayArray = new();

    public override void _EnterTree()
    {
        foreach (Node n in GetChildren())
        {
            if (n is EnemyDisplay display)
            {
                EnemyDisplayArray.Add(display);
            }
        }

        if (WaveType == EWaveType.Timed)
        {
            EnemyEvent.Register(EnemyEvent.SignalName.ShowNextTimedWave, Callable.From((R_SpawnWave sw) => ApplySpawnWave(sw)));
        }
        else if (WaveType == EWaveType.Expansion)
        {
            EnemyEvent.Register(EnemyEvent.SignalName.ShowNextExpandWave, Callable.From((R_SpawnWave sw) => ApplySpawnWave(sw)));
        }
    }

    void ApplySpawnWave(R_SpawnWave NextWave)
    {
        int i = 0;
        foreach (EnemyDisplay display in EnemyDisplayArray)
        {
            if (NextWave.SpawnCounts.Count > i)
            {
                if (NextWave.SpawnCounts[i].Data != null)
                {
                    display.AssignEnemyParams(NextWave.SpawnCounts[i].Data, NextWave.SpawnCounts[i].Count);
                    i++;
                    continue;
                }
            }
            i++;
            display.AssignEnemyParams(null, 0);
        }
    }
}