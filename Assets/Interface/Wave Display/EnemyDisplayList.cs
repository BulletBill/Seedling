using Godot;
using System;
using Godot.Collections;
using System.Security;

public partial class EnemyDisplayList : Node
{
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

        EnemyEvent.Register(EnemyEvent.SignalName.ShowNextTimedWave, Callable.From((R_SpawnWave sw) => ApplySpawnWave(sw)));
    }

    void ApplySpawnWave(R_SpawnWave NextWave)
    {
        int i = 0;
        foreach (EnemyDisplay display in EnemyDisplayArray)
        {
            if (NextWave.SpawnCounts.Count > i)
            {
                if (NextWave.SpawnCounts[i].SpawnData != null)
                {
                    display.AssignEnemyParams(NextWave.SpawnCounts[i].SpawnData.Data, NextWave.SpawnCounts[i].Count);
                    i++;
                    continue;
                }
            }
            i++;
            display.AssignEnemyParams(null, 0);
        }
    }
}