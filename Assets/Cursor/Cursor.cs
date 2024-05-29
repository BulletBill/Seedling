using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

public partial class Cursor : Node2D
{
	public static Cursor Singleton;
	Stack<Cursor_State> StateStack = new();
	[Export] public Node DefaultCursorState;
	public Vector2I CurrentTile = new();
	int TileSize = 32;
	float MapScale = 1.0f;
	public Sprite2D PlacementGhost;
	public Sprite2D TileSelector;
	public Dictionary<ECursorState, Cursor_State> StateList { get; protected set; } = new();

	// Signals
	[Signal] public delegate void AnyStateChangedEventHandler();
	[Signal] public delegate void SelectableHoveredEventHandler(Data_Hoverable NewHoverable);
	[Signal] public delegate void SelectableExitedEventHandler();
	[Signal] public delegate void SetFixedObjectEventHandler(Data_Hoverable NewFixedObject);
	[Signal] public delegate void ClearFixedObjectEventHandler();
	[Signal] public delegate void AnyStateActionsChangedEventHandler(Godot.Collections.Array<Data_Action> NewActions);

	public Cursor()
	{
		Singleton = this;
	}

    public override void _EnterTree()
    {
		PlayerEvent.Register(PlayerEvent.SignalName.TowerSelected, Callable.From((Tower t) => SetTileHighlight(t)));
		PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => ClearTileHighlight()));
    }

	public override void _Ready()
	{
		MapScale = MainMap.Singleton.GlobalScale.X;
		TileSize = MainMap.GetTileSize();
		PlacementGhost = GetNodeOrNull<Sprite2D>("PlacementGhost");
		TileSelector = GetNodeOrNull<Sprite2D>("TileSelector");

		foreach(Node n in GetChildren())
		{
			if (n is Cursor_State ChildState)
			{
				StateList.Add(ChildState.GetState(), ChildState);
			}
		}

		if (PlacementGhost != null)
		{
			PlacementGhost.GlobalScale = new Vector2(MapScale, MapScale);
		}

		if (DefaultCursorState is Cursor_State cursorState)
		{
			StateStack.Push(cursorState);
			StateStack.Peek().OnEnable();
			Broadcast(SignalName.AnyStateChanged);
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

	Cursor_State PushState_Internal(String StateName)
	{
		Cursor_State NewState = Cursor.Singleton.GetNodeOrNull<Cursor_State>(StateName);
		if (NewState == null) return StateStack.Count > 0 ? StateStack.Peek() : null;
		if (NewState == StateStack.Peek()) return StateStack.Peek();

		StateStack.Peek()?.OnDisable();
		StateStack.Push(NewState);
		NewState?.OnEnable();
		NewState?.OnMove(CurrentTile);

		Broadcast(SignalName.AnyStateChanged);
		return NewState;
	}

	Cursor_State PopState_Internal()
	{
		// Don't pop last state
		if (StateStack.Count <= 1) return StateStack.Count > 0 ? StateStack.Peek() : null;

		StateStack.Peek()?.OnDisable();
		StateStack.Pop();
		StateStack.Peek()?.OnEnable();

		Broadcast(SignalName.AnyStateChanged);
		return StateStack.Peek();
	}

	void SetTileHighlight(Tower TargetTower)
	{
		if (!IsInstanceValid(TileSelector)) return;
		if (!IsInstanceValid(TargetTower))
		{
			TileSelector.Visible = false;
			return;
		}
		TileSelector.Visible = true;
		TileSelector.GlobalPosition = TargetTower.GlobalPosition;
	}

	void ClearTileHighlight()
	{
		if (!IsInstanceValid(TileSelector)) return;
		TileSelector.Visible = false;
	}

	// Static accessors
	public static void AddHoverArea(HoverArea AddedArea, ECursorState state)
	{
		if (Cursor.Singleton == null) return;
		if (!Cursor.Singleton.StateList.ContainsKey(state)) return;

		if (!Cursor.Singleton.StateList[state].HoverList.Contains(AddedArea))
		{
			Cursor.Singleton.StateList[state].HoverList.Add(AddedArea);
		}
	}

	public static void RemoveHoverArea(HoverArea RemovedArea, ECursorState state)
	{
		if (Cursor.Singleton == null) return;
		if (!Cursor.Singleton.StateList.ContainsKey(state)) return;

		Cursor.Singleton.StateList[state].HoverList.Remove(RemovedArea);
	}

	public static Cursor_State PushState(String StateName)
	{
		if (Cursor.Singleton == null) return null;
		return Cursor.Singleton.PushState_Internal(StateName);
	}

	public static Cursor_State PopState()
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

	public static Vector2I GetCurrentTile()
	{
		if (Cursor.Singleton == null) return new(0,0);

		return Cursor.Singleton.CurrentTile;
	}

	public static ECursorState GetCurrentState()
	{
		if (Cursor.Singleton == null) return ECursorState.Free;
		if (Cursor.Singleton.StateStack.Count <= 0) return ECursorState.Free;
		return Cursor.Singleton.StateStack.Peek().GetState();
	}

	public static bool IsOverHoverable()
	{
		if (Cursor.Singleton == null) return false;
		if (Cursor.Singleton.StateStack.Count <= 0) return false;

		return Cursor.Singleton.StateStack.Peek().HoverList.Count > 0;
	}

	public static Node2D GetSelectedObject()
	{
		if (Cursor.Singleton == null) return null;
		if (Cursor.Singleton.StateStack.Count <= 0) return null;
		return Cursor.Singleton.StateStack.Peek().GetSelectedObject();
	}

	// Event bus functions
	public static bool Register(String DelegateName, Callable Receiver)
    {
        if (Singleton == null) return false;
        Error Result = Singleton.Connect(DelegateName, Receiver);
        if (Result == Error.Ok)
        {
            return true;
        }
        return false;
    }

	public static Error Broadcast(String EventName, params Variant[] args)
    {
        if (Singleton == null) return Error.DoesNotExist;
        return Singleton.EmitSignal(EventName, args);
    }
}