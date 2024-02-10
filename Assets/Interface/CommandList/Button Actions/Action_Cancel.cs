using Godot;
using System;

public partial class Action_Cancel : Node, IButtonAction
{
    public void Execute()
    {
        Cursor.PopState();
    }
}
