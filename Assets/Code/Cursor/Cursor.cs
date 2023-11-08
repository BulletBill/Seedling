using Godot;
using System;

public partial class Cursor : Node2D
{
	ICursorState CurrentState;
	[Export] public Node DefaultCursorState;
	Vector2I CurrentTile = new();
	int TileSize = 32;
	public Sprite2D GridHighlight;
	public Sprite2D PlacementGhost;

	// Called when the node enters the scene tree for the first time.
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
	}

	public void SwitchState(ICursorState NewState)
	{
		if (NewState == CurrentState) return;
		
		CurrentState?.OnDisable();
		CurrentState = NewState;
		CurrentState?.OnEnable();
	}
}

public interface ICursorState
{
	public void OnEnable();
	public void OnDisable();
	public void OnClick();
	public void OnMove(Vector2I NewMapPosition);
}