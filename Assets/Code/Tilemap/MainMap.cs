using Godot;
using GodotPlugins.Game;
using System;
using System.Buffers;
using System.Collections.Generic;

public partial class MainMap : TileMap
{
	[Export] public Color GridColor;
	static readonly uint GridCode = 1;
	uint OutlineActive = 0;
	bool OutlineShown = false;
	public static MainMap Singleton;

	// Terrain Index shortcuts
	public static readonly int Terrain_Void = -1;
	public static readonly int Terrain_Dirt = 0;
	public static readonly int Terrain_Grass = 1;
	public static readonly int Terrain_Stone = 2;
	public static readonly int Terrain_Water = 3;
	public static readonly int Terrain_Chasm = 4;

	public static readonly int TerrainSet_Default = 0;

	public static readonly int Layer_Ground = 0;
	public static readonly int Layer_Below = 1;
	public static readonly int Layer_Above = 2;
	public static readonly int Layer_Outline = 3;


	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		MainMap.Singleton = this;
		SetLayerModulate(Layer_Outline, new Color(0.0f, 0.0f, 0.0f, 0.0f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!OutlineShown && Input.IsActionJustPressed("ShowGrid"))
		{
			AddOutlineActive_Internal(GridCode);
			GD.Print("MainMap._Process: ShowGrid action just pressed.");
		}
		
		if (OutlineShown && Input.IsActionJustReleased("ShowGrid"))
		{
			RemoveOutlineActive_Internal(GridCode);
			GD.Print("MainMap._Process: ShowGrid action just released.");
		}

		if (OutlineActive > 0 && !OutlineShown)
		{
			OutlineShown = true;
			SetLayerModulate(Layer_Outline, GridColor);
		}

		if (OutlineActive <= 0 && OutlineShown)
		{
			OutlineShown = false;
			SetLayerModulate(Layer_Outline, new Color(0.0f, 0.0f, 0.0f, 0.0f));
		}
	}

	void AddOutlineActive_Internal(uint OutlineActivate)
	{
		OutlineActive |= OutlineActivate;
	}

	void RemoveOutlineActive_Internal(uint OutlineActivate)
	{
		OutlineActive &= ~OutlineActivate;
	}

	// Static accessors
	public static int GetTileSize()
	{
		if (MainMap.Singleton == null) return 32;
		return MainMap.Singleton.TileSet.TileSize.X;
	}

	public static int GetTileType(Vector2I GridPosition)
	{
		if (MainMap.Singleton == null) return MainMap.Terrain_Void;
		int Above = MainMap.Singleton.GetCellTileData(Layer_Above, GridPosition).Terrain;
		int Below = MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition).Terrain;
		int Ground = MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition).Terrain;

		return Ground;
	}

	public static void AddOutlineActive(uint ActiveFlag)
	{
		if (MainMap.Singleton == null) return;
		MainMap.Singleton.AddOutlineActive_Internal(ActiveFlag);
	}

	public static void RemoveOutlineActive(uint ActiveFlag)
	{
		if (MainMap.Singleton == null) return;
		MainMap.Singleton.RemoveOutlineActive_Internal(ActiveFlag);
	}
}

public class TileAtDistance
{
    public Vector2I TilePosition;
    public float Distance;

	public TileAtDistance(Vector2I tilePosition, float distance)
	{
		TilePosition = tilePosition;
		Distance = distance;
	}
}