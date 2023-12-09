using Godot;
using System;
using System.Collections.Generic;

public partial class C_TowerBrain : Node
{
    List<C_Attack> AttackerList = new();
    int AttackerIndex = 0;

    public override void _Process(double delta)
    {
        if (AttackerIndex >= AttackerList.Count)
        {
            AttackerIndex = 0;
            PopulateAttackerList();
        }

        if (AttackerList.Count > 0)
        {
            AttackerList[AttackerIndex]?.SeekTarget();
            AttackerIndex++;
        }
    }

    void PopulateAttackerList()
    {
        AttackerList.Clear();
        foreach(Node TowerNode in GetTree().GetNodesInGroup(C_Attack.AttackTowerGroupName))
        {
            AttackerList.Add(TowerNode as C_Attack);
        }
    }
}
