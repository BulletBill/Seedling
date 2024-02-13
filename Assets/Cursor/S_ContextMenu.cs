using Godot;
using System;

public partial class S_ContextMenu : Node, ICursorState
{
	[Export] public Godot.Collections.Array<Data_Action> StateActions;
	Tower SelectedTower = null;
	Cursor ParentCursor;
	public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

	public void AssignTower(Tower NewTower)
	{
		SelectedTower = NewTower;
		if (NewTower == null) return;

		StateActions = NewTower.Actions;
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerSelected, NewTower);
		Cursor.Broadcast(Cursor.SignalName.AnyStateActionsChanged, StateActions);
	}

	// Cursor state interface
	public ECursorState GetState()
	{
		return ECursorState.Menu_Context;
	}

	public void OnEnable()
	{
		GD.Print("Cursor State changed to Context Menu");
	}
	public void OnDisable()
	{
		SelectedTower = null;
	}
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

	public Node2D GetSelectedObject()
	{
		return SelectedTower;
	}
}
