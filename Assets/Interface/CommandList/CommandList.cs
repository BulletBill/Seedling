using Godot;
using System;

public partial class CommandList : Node2D
{
    [Export] public ECursorState RequiredState = ECursorState.Free;
    public override void _EnterTree()
    {
        Visible = false;
        Cursor.Register(Cursor.SignalName.AnyStateChanged, Callable.From(() => CursorStateChanged()));
    }

    public override void _Ready()
    {
        foreach (Node n in GetChildren())
        if (n is CommandButton button)
        {
            button.SetCursorState(RequiredState);
        }
    }

    public void TakeCommandsFromTower(Tower SelectedTower)
    {
        if (SelectedTower == null) return;
        if (SelectedTower.Actions.Count <= 0) return;

        int ActionIndex = 0;
        foreach (Node n in GetChildren())
        if (n is CommandButton button)
        {
            if (button.ActionParams.ActionType != EActionType.None) continue;
            if (button.Preset) continue;

            if (ActionIndex < SelectedTower.Actions.Count)
            {
                button.AssignActionParams(SelectedTower.Actions[ActionIndex++]);
            }
            else
            {
                button.AssignActionParams(null);
            }
        }
    }

    void CursorStateChanged()
    {
        Visible = Cursor.GetCurrentState() == RequiredState;
    }
}
