using Godot;
using System;
using Godot.Collections;

public partial class CommandList : Node2D
{
    public Array<CommandButton> ButtonArray = new();
    public override void _EnterTree()
    {
        Cursor.Register(Cursor.SignalName.AnyStateActionsChanged, Callable.From((Array<Data_Action> a) => ApplyActionArray(a)));

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

            ButtonArray[TryButtonIndex].AssignActionParams(action);
            ButtonArray[TryButtonIndex].SetHotkey(TryButtonIndex);
        }
    }
}