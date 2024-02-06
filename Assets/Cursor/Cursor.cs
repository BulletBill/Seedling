using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class Cursor : Node2D
{
	public static Cursor Singleton;
	Stack<ICursorState> StateStack = new();
	[Export] public Node DefaultCursorState;
	Vector2I CurrentTile = new();
	int TileSize = 32;
	float MapScale = 1.0f;
	public Sprite2D PlacementGhost;
	public List<HoverArea> HoverList {get; protected set;} = new();

    public override void _EnterTree()
    {
        Cursor.Singleton = this;
    }
    
	public override void _Ready()
	{
		MapScale = MainMap.Singleton.GlobalScale.X;
		TileSize = MainMap.GetTileSize();
		PlacementGhost = GetNodeOrNull<Sprite2D>("PlacementGhost");

		if (PlacementGhost != null)
		{
			PlacementGhost.GlobalScale = new Vector2(MapScale, MapScale);
		}

		if (DefaultCursorState is ICursorState cursorState)
		{
			StateStack.Push(cursorState);
			StateStack.Peek().OnEnable();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2I PrevTile = CurrentTile;
		Position = GetGlobalMousePosition();
		CurrentTile.X = Mathf.FloorToInt(Position.X / (TileSize * MapScale));
		CurrentTile.Y = Mathf.FloorToInt(Position.Y / (TileSize * MapScale));
		
		if (PrevTile != CurrentTile && StateStack.Peek() != null)
		{
			StateStack.Peek().OnMove(CurrentTile);
		}

		Vector2 TileCenter = new Vector2(CurrentTile.X, CurrentTile.Y) * (TileSize * MapScale);
		PlacementGhost.GlobalPosition = TileCenter;

		if (Input.IsActionJustPressed("Click"))
		{
			StateStack.Peek()?.OnClick();
		}
		if (Input.IsActionJustPressed("UI_Back"))
		{
			StateStack.Peek()?.OnEscape();
		}
	}

	ICursorState PushState_Internal(String StateName)
	{
		ICursorState NewState = Cursor.Singleton.GetNodeOrNull<ICursorState>(StateName);
		if (NewState == null) return StateStack.Count > 0 ? StateStack.Peek() : null;

		StateStack.Peek()?.OnDisable();
		StateStack.Push(NewState);
		NewState?.OnEnable();

		FlushHoverList();

		return NewState;
	}

	ICursorState PopState_Internal()
	{
		// Don't pop last state
		if (StateStack.Count <= 1) return StateStack.Count > 0 ? StateStack.Peek() : null;

		StateStack.Peek()?.OnDisable();
		StateStack.Pop();
		StateStack.Peek()?.OnEnable();

		FlushHoverList();

		return StateStack.Peek();
	}

	void FlushHoverList()
	{
		foreach (HoverArea hoverArea in HoverList)
		{
			hoverArea.ForceMouseExit();
		}
		HoverList.Clear();
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

	public static ICursorState PushState(String StateName)
	{
		if (Cursor.Singleton == null) return null;
		return Cursor.Singleton.PushState_Internal(StateName);
	}

	public static ICursorState PopState()
	{
		if (Cursor.Singleton == null) return null;
		return Cursor.Singleton.PopState_Internal();
	}

	public static Vector2 GetTilePosition()
	{
		if (Cursor.Singleton == null) return new Vector2(0, 0);

		return new Vector2(Cursor.Singleton.CurrentTile.X * Cursor.Singleton.TileSize,
						   Cursor.Singleton.CurrentTile.Y * Cursor.Singleton.TileSize);
	}

	public static String GetStateName()
	{
		if (Cursor.Singleton == null) return "None";
		if (Cursor.Singleton.StateStack.Count <= 0) return "None";
		return Cursor.Singleton.StateStack.Peek().GetName();
	}

	public static bool IsOverHoverable()
	{
		if (Cursor.Singleton == null) return false;
		return Cursor.Singleton.HoverList.Count > 0;
	}
}

public interface ICursorState
{
	public String GetName();
	public void OnEnable();
	public void OnDisable();
	public void OnClick();
	public void OnEscape();
	public void OnMove(Vector2I NewMapPosition);
}