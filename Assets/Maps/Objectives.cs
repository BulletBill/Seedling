using Godot;
using System;

public partial class Objectives : Node
{
    [Export] public int BuildHearts = 1;
    [Export] public String HeartName = "Genesis";
    [Export] public int BuildSunLeaf = 0;
    [Export] public String SunleafName = "Sunleaf";
    [Export] public bool SurviveFinalWave = true;
    int HeartsBuilt = 0;
    int SunLeavesBuilt = 0;
    bool InFinalWave = false;

    public override void _EnterTree()
    {
        PlayerEvent.Register(PlayerEvent.SignalName.TowerFinished, Callable.From((Tower t) => TowerFinishedBuilding(t)));
    }

    void TowerFinishedBuilding(Tower FinishedTower)
    {
        if (IsInstanceValid(FinishedTower) == false) return;

        if (FinishedTower.Name == HeartName)
        {
            HeartsBuilt++;
        }
        else if (FinishedTower.Name == SunleafName)
        {
            SunLeavesBuilt++;
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
        }
    }

    void EnemyDefeated(Enemy DefeatedEnemy)
    {
        if (GetTree().GetNodesInGroup("Enemy").Count <= 0)
        {
            //Victory!
            Level ParentLevel = GetParentOrNull<Level>();
            if (ParentLevel != null)
            {
                SceneTransition.ChangeScene(ParentLevel.ExitLevel);
            }
            else
            {
                GetTree().Quit();
            }
        }
    }
}
