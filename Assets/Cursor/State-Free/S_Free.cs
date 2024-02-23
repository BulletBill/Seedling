using Godot;
using System;
using System.Reflection;

public partial class S_Free : Cursor_State
{
    [Export] public Godot.Collections.Array<Data_Action> ActionList = new();
    Cursor ParentCursor;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

    // Cursor state interface
    public override ECursorState GetState()
    {
        return ECursorState.Free;
    }

    public override void OnEnable()
    {
        GD.Print("Cursor State changed to Free");
        Cursor.Broadcast(Cursor.SignalName.AnyStateActionsChanged, ActionList);
    }
	public override void OnDisable()
    {

    }
    public override void OnEscape()
    {
        //TODO: Open Game Menu
    }
	public override void OnClick()
    {
        if (ParentCursor == null) return;
        if (HoverList.Count <= 0) return;
        if (IsInstanceValid(HoverList[0]) == false) return;

        HoverList[0].OnClick();
    }
	public override void OnMove(Vector2I NewMapPosition)
    {
    }

    public override Node2D GetSelectedObject()
    {
        return null;
    }
}
