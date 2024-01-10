using Godot;
using GodotPlugins.Game;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;

public partial class MainMap : TileMap
{
	[Export] public Color GridColor;
	static readonly uint GridCode = 1;
	uint OutlineActive = 0;
	bool OutlineShown = false;
	public static MainMap Singleton;

	// Harvested Tiles
	Dictionary<Vector2I, int> HarvestedTileCount = new();

	// Terrain Index shortcuts
	public static readonly int Terrain_Void = -1;
	public static readonly int Terrain_Dirt = 0;
	public static readonly int Terrain_Grass = 1;
	public static readonly int Terrain_Stone = 2;
	public static readonly int Terrain_Water = 3;
	public static readonly int Terrain_Chasm = 4;

	public static readonly int TerrainSet_Default = 0;

	public static readonly int Layer_Path = 0;
	public static readonly int Layer_Ground = 1;
	public static readonly int Layer_Below = 2;
	public static readonly int Layer_Outline = 3;

	public static readonly String Custom_Spark = "Spark";
	public static readonly String Custom_Grass = "Grass";


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
		
		if (MainMap.Singleton.GetCellTileData(Layer_Path, GridPosition) != null)
		{
			return MainMap.Singleton.GetCellTileData(Layer_Path, GridPosition).Terrain;
		}

		if (MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition) != null)
		{
			return MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition).Terrain;
		}

		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			return MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition).Terrain;
		}

		return Terrain_Void;
	}

	public static bool TileHasFlag(Vector2I GridPosition, String CustomFlag)
	{
		if (MainMap.Singleton == null) return false;

		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			return (bool)MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition).GetCustomData(CustomFlag);
		}
		return false;
	}

	public static ECurrencyType GetTileCurrency(Vector2I GridPosition)
	{
		int Type = GetTileType(GridPosition);
		if (Type == MainMap.Terrain_Void) return ECurrencyType.None;

		if (Type == MainMap.Terrain_Dirt || Type == MainMap.Terrain_Stone || Type == MainMap.Terrain_Grass)
		{
			return ECurrencyType.Substance;
		}
		if (Type == MainMap.Terrain_Water)
		{
			return ECurrencyType.Flow;
		}
		if (Type == MainMap.Terrain_Chasm)
		{
			return ECurrencyType.Breath;
		}

		return ECurrencyType.None;
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

	public static bool IsOutlineActive()
	{
		if (MainMap.Singleton == null) return false;

		return MainMap.Singleton.OutlineActive > 0;
	}

	public static ECurrencyType HarvestTile(Vector2I TileLoc)
	{
		if (Singleton == null) return ECurrencyType.None;

		if (Singleton.HarvestedTileCount.ContainsKey(TileLoc))
		{
			Singleton.HarvestedTileCount[TileLoc] += 1;
			return ECurrencyType.None;
		}

		Singleton.HarvestedTileCount.Add(TileLoc, 1);
		return GetTileCurrency(TileLoc);
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