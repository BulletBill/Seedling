using Godot;
using GodotPlugins.Game;
using System;
using System.Buffers;

public partial class MainMap : TileMap
{
	[Export] public Color GridColor;
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
		ShowOutline(OutlineShown);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (OutlineShown != Input.IsActionPressed("ShowGrid"))
		{
			ShowOutline(Input.IsActionPressed("ShowGrid"));
		}
	}

	void ShowOutline(bool Show)
	{
		OutlineShown = Show;
		Color NewGridColor = Show ? GridColor : new Color(0.0f, 0.0f, 0.0f, 0.0f);
		SetLayerModulate(Layer_Outline, NewGridColor);
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