using Godot;
using System;

public partial class S_ContextMenu : Node, ICursorState
{
	Cursor ParentCursor;
	public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

	public void AssignTower(Tower NewTower)
	{
		if (NewTower == null) return;

		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerSelected, NewTower);
	}

	// Cursor state interface
	public String GetName()
	{
		return "ContextMenu";
	}

	public void OnEnable()
	{
		GD.Print("Cursor State changed to Context Menu");
	}
	public void OnDisable() {}
	public void OnClick()
	{
		if (ParentCursor == null) return;
        if (ParentCursor.HoverList.Count <= 0) return;

        ParentCursor.HoverList[0].OnClick();
	}
	public void OnEscape()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
		Cursor.PopState();
	}
	public void OnMove(Vector2I NewMapPosition) {}
}
