using Godot;
using System;

public partial class S_ContextMenu : Node, ICursorState
{

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
	public void OnClick() {}
	public void OnEscape()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
		Cursor.PopState();
	}
	public void OnMove(Vector2I NewMapPosition) {}
}
