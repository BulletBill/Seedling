using Godot;
using System;
using Godot.Collections;

public partial class CommandList : Node2D
{
    public Array<CommandButton> ButtonArray = new();
    public int SelectedTowerLevel { get; protected set; } = 1;
    Tower SelectedTower = null;

    public override void _EnterTree()
    {
        Cursor.Register(Cursor.SignalName.AnyStateActionsChanged, Callable.From((Array<Data_Action> a) => ApplyActionArray(a)));
        PlayerEvent.Register(PlayerEvent.SignalName.TowerSelected, Callable.From((Tower t) => SelectTower(t)));
        PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => ClearTower()));

        foreach (Node n in GetChildren())
        if (n is CommandButton button)
        {
            ButtonArray.Add(button);
        }
    }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        if (ButtonArray.Count <= 0) return;

        for (int i = 0; i < ButtonArray.Count; i++)
        {
            if (Input.IsActionJustPressed("CommandButton_" + (i+1).ToString()))
            {
                ButtonArray[i].OnClick();
            }
        }
    }
 
    public void ApplyActionArray(Array<Data_Action> InArray)
    {
        // Clear the buttons first
        foreach (CommandButton button in ButtonArray)
        {
            button.AssignActionParams(null);
        }

        if (InArray == null || InArray.Count <= 0) return;

        // Drop each action into an empty slot, starting with its desired position
        foreach (Data_Action action in InArray)
        {
            int TryButtonIndex = Math.Clamp(action.DesiredPosition, 0, ButtonArray.Count -1);
            while (!ButtonArray[TryButtonIndex].Disabled)
            {
                TryButtonIndex++;
                if (TryButtonIndex == action.DesiredPosition) break;
                if (TryButtonIndex >= ButtonArray.Count) { TryButtonIndex = 0; }
            }

            ButtonArray[TryButtonIndex].CostMultiplier = action.ActionType == EActionType.SelfUpgrade ? SelectedTowerLevel : 1;
            ButtonArray[TryButtonIndex].AssignActionParams(action);
            ButtonArray[TryButtonIndex].SetHotkey(TryButtonIndex);
        }
    }

    public void SelectTower(Tower NewTower)
    {
        if (IsInstanceValid(NewTower)) { ClearTower(); return; }
        SelectedTower = NewTower;

        SelectedTowerLevel = SelectedTower.TowerLevel;
        SelectedTower.Connect(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated));
    }

    public void ClearTower()
    {
        SelectedTowerLevel = 1;

        if (IsInstanceValid(SelectedTower))
        {
            SelectedTower.Disconnect(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated));
            SelectedTower = null;
        }
    }

    public void SelectedTowerUpdated()
    {
        SelectedTowerLevel = SelectedTower.TowerLevel;
        foreach (CommandButton button in ButtonArray)
        {
            button.UpdateCosts(true);
        }
    }
}
