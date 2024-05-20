using Godot;
using System;

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
        Game.Log(LogCategory.Cursor, "Cursor State changed to Free");
        Cursor.Broadcast(Cursor.SignalName.AnyStateActionsChanged, ActionList);
        if (HoverList.Count > 0){ HoverList[0].Activate(); }
    }
	public override void OnDisable()
    {
        if (HoverList.Count > 0){ HoverList[0].Deactivate(); }
    }
    public override void OnEscape()
    {
        MenuEvent.Broadcast(MenuEvent.SignalName.OpenPauseMenu);
    }
	public override void OnClick()
    {
        if (ParentCursor == null) return;
        if (HoverList.Count <= 0) return;
        if (IsInstanceValid(HoverList[0]) == false)
        {
            HoverList.RemoveAt(0);
            return;
        }

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
