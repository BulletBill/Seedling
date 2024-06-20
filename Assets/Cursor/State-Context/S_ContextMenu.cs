using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class S_ContextMenu : Cursor_State
{
	[Export] public Godot.Collections.Array<Data_Action> StateActions;
	Tower SelectedTower = null;
	Cursor ParentCursor;

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerRemoved, Callable.From((Tower t) => OnTowerRemoved(t)));
	}

	public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

	public void AssignTower(Tower NewTower)
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
		Cursor.Broadcast(Cursor.SignalName.ClearFixedObject);
		SelectedTower = NewTower;
		SelectedTowerUpdated();

		if (IsInstanceValid(SelectedTower) && !SelectedTower.IsConnected(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated)))
		{
			SelectedTower.Connect(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated));
		}		
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerSelected, NewTower);
	}

	void SelectedTowerUpdated()
	{
		if (SelectedTower == null) return;

		if (SelectedTower.Upgrading)
		{
			StateActions = SelectedTower.UpgradingActions;
		}
		else if (SelectedTower.Building)
		{
			StateActions = SelectedTower.BuildingActions;
		}
		else
		{
			StateActions = SelectedTower.Actions;
		}

		Cursor.Broadcast(Cursor.SignalName.AnyStateActionsChanged, StateActions);
	}

	// Cursor state interface
	public override ECursorState GetState()
	{
		return ECursorState.Menu_Context;
	}

	public override void OnEnable()
	{
		Game.Log(LogCategory.Cursor, "Cursor State changed to Context Menu");
		if (HoverList.Count > 0){ HoverList[0].Activate(); }
	}
	public override void OnDisable()
	{
		PlayerEvent.Broadcast(PlayerEvent.SignalName.TowerDeselected);
		Cursor.Broadcast(Cursor.SignalName.ClearFixedObject);

		if (IsInstanceValid(SelectedTower) && SelectedTower.IsConnected(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated)))
		{
			SelectedTower.Disconnect(Tower.SignalName.TowerUpdated, Callable.From(SelectedTowerUpdated));
		}		
		SelectedTower = null;
	}
	public override void OnClick()
	{
		if (ParentCursor == null) return;
        if (HoverList.Count <= 0) return;
		if (IsInstanceValid(HoverList[0]) == false) return;

        HoverList[0].OnClick();
	}
	public override void OnEscape()
	{
		Cursor.PopState();
	}
	public override void OnMove(Vector2I NewMapPosition) {}

	public override Node2D GetSelectedObject()
	{
		return SelectedTower;
	}

	public void OnTowerRemoved(Tower RemovedTower)
	{
		if (RemovedTower == SelectedTower)
		{
			OnEscape();
		}
	}
}
