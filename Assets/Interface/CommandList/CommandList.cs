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

    void CursorStateChanged()
    {
        Visible = Cursor.GetCurrentState() == RequiredState;
    }
}
