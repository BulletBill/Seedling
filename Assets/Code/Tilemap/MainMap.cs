using Godot;
using GodotPlugins.Game;
using System;
using System.Buffers;

public partial class MainMap : TileMap
{
	public static MainMap Singleton;

	// Terrain Index shortcuts
	public static readonly int Terrain_Dirt = 0;
	public static readonly int Terrain_Grass = 1;
	public static readonly int Terrain_Stone = 2;
	public static readonly int Terrain_Water = 3;
	public static readonly int Terrain_Chasm = 4;

	public static readonly int TerrainSet_Default = 0;

	public static readonly int Layer_Ground = 0;
	public static readonly int Layer_Below = 1;
	public static readonly int Layer_Above = 2;


	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		MainMap.Singleton = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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