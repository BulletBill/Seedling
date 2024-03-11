using Godot;
using System;
using System.Security;

public partial class Objectives : Node
{
    [Export] public int BuildHearts = 1;
    [Export] public String HeartName = "HeartBloom";
    [Export] public int BuildSunLeaf = 0;
    [Export] public String SunleafName = "SunLeaf";
    [Export] public bool SurviveFinalWave = true;
    int HeartsBuilt = 0;
    int SunLeavesBuilt = 0;
    bool InFinalWave = false;

    public override void _EnterTree()
    {
        PlayerEvent.Register(PlayerEvent.SignalName.TowerFinished, Callable.From((Tower t) => TowerFinishedBuilding(t)));
    }

    public override void _Ready()
    {
        PlayerEvent.Broadcast(PlayerEvent.SignalName.HeartCountUpdated, BuildHearts);
        PlayerEvent.Broadcast(PlayerEvent.SignalName.SunleafCountUpdated, BuildSunLeaf);
        if (SurviveFinalWave == false)
        {
            PlayerEvent.Broadcast(PlayerEvent.SignalName.FinalWaveSurvived);
        }
    }

    void TowerFinishedBuilding(Tower FinishedTower)
    {
        if (IsInstanceValid(FinishedTower) == false) return;

        if (FinishedTower.TowerData.Ident == HeartName)
        {
            if (HeartsBuilt < BuildHearts)
            {
                HeartsBuilt++;
                GD.Print("Objectives: Finished building a Heart Bloom. " + (BuildHearts - HeartsBuilt).ToString() + " needed.");
            }
            PlayerEvent.Broadcast(PlayerEvent.SignalName.HeartCountUpdated, BuildHearts - HeartsBuilt);
        }
        else if (FinishedTower.TowerData.Ident == SunleafName)
        {
            if (SunLeavesBuilt < BuildSunLeaf)
            {
                SunLeavesBuilt++;
                GD.Print("Objectives: Finished building a Sunleaf. " + (BuildSunLeaf - SunLeavesBuilt).ToString() + " needed.");
            }
            PlayerEvent.Broadcast(PlayerEvent.SignalName.SunleafCountUpdated, BuildSunLeaf - SunLeavesBuilt);
        }

        if (!InFinalWave)
        {
            if (HeartsBuilt >= BuildHearts && SunLeavesBuilt >= BuildSunLeaf)
            {
                StartFinalWave();
            }
        }
    }

    void StartFinalWave()
    {
        if (!InFinalWave)
        {
            InFinalWave = true;
            EnemyEvent.Register(EnemyEvent.SignalName.EnemyDefeated, Callable.From((Enemy e) => EnemyDefeated(e)));
            EnemyEvent.Broadcast(EnemyEvent.SignalName.StartFinalWave);
        }
    }

    void EnemyDefeated(Enemy DefeatedEnemy)
    {
        int RemainingEnemies = GetTree().GetNodesInGroup("Enemy").Count - 1; // The enemy has not yet been removed
        GD.Print("Final wave enemy defeated. " + RemainingEnemies.ToString() + " enemies remain.");
        if (RemainingEnemies <= 0)
        {
            PlayerEvent.Broadcast(PlayerEvent.SignalName.FinalWaveSurvived);

            //Victory!
            Level.GameOver(MenuEvent.SignalName.OpenVictoryMenu);
        }
    }
}
