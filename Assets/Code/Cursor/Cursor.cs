using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class Cursor : Node2D
{
	public static Cursor Singleton;
	ICursorState CurrentState;
	[Export] public Node DefaultCursorState;
	Vector2I CurrentTile = new();
	int TileSize = 32;
	public Sprite2D GridHighlight;
	public Sprite2D PlacementGhost;
	public List<HoverArea> HoverList {get; protected set;} = new();

    public override void _EnterTree()
    {
        Cursor.Singleton = this;
    }
    
	public override void _Ready()
	{
		TileSize = MainMap.GetTileSize();
		GridHighlight = GetNodeOrNull<Sprite2D>("GridHighlight");
		PlacementGhost = GetNodeOrNull<Sprite2D>("PlacementGhost");

		if (DefaultCursorState is ICursorState cursorState)
		{
			CurrentState = cursorState;
			CurrentState.OnEnable();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2I PrevTile = CurrentTile;
		Position = GetGlobalMousePosition();
		CurrentTile.X = Mathf.FloorToInt(Position.X / TileSize);
		CurrentTile.Y = Mathf.FloorToInt(Position.Y / TileSize);
		
		if (PrevTile != CurrentTile && CurrentState != null)
		{
			CurrentState.OnMove(CurrentTile);
		}

		GridHighlight.GlobalPosition = new Vector2(CurrentTile.X, CurrentTile.Y) * TileSize;

		if (Input.IsActionJustPressed("Click"))
		{
			CurrentState?.OnClick();
		}
	}

	public void SwitchState(ICursorState NewState)
	{
		if (NewState == CurrentState) return;
		
		CurrentState?.OnDisable();
		CurrentState = NewState;
		CurrentState?.OnEnable();
	}

	// Static accessors
	public static void AddHoverArea(HoverArea AddedArea)
	{
		if (Cursor.Singleton == null) return;
		if (Cursor.Singleton.HoverList.Contains(AddedArea)) return;

		Cursor.Singleton.HoverList.Add(AddedArea);
	}

	public static void RemoveHoverArea(HoverArea RemovedArea)
	{
		if (Cursor.Singleton == null) return;
		Cursor.Singleton.HoverList.Remove(RemovedArea);
	}
}

public interface ICursorState
{
	public void OnEnable();
	public void OnDisable();
	public void OnClick();
	public void OnMove(Vector2I NewMapPosition);
}