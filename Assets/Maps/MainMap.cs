using Godot;
using GodotPlugins.Game;
using System;
using Godot.Collections;

public partial class MainMap : TileMap
{
	[Export] public Color GridColor;
	static readonly uint GridCode = 1;
	uint OutlineActive = 0;
	bool OutlineShown = false;
	public static MainMap Singleton;

	// Tile meta
	Array<Vector2I> HarvestedTiles = new();
	Dictionary<Vector2I, int> GrassTiles = new();
	public int Expansion { get; protected set; } = 0;

	// Terrain Index shortcuts
	public static readonly int Terrain_Void = -1;
	public static readonly int Terrain_Dirt = 0;
	public static readonly int Terrain_Grass = 1;
	public static readonly int Terrain_Stone = 2;
	public static readonly int Terrain_Water = 3;
	public static readonly int Terrain_Chasm = 4;
	public static readonly int Terrain_WetDirt = 5;
	public static readonly int Terrain_WetGrass = 6;

	public static readonly int TerrainSet_Default = 0;

	public static readonly int Layer_Path = 0;
	public static readonly int Layer_Ground = 1;
	public static readonly int Layer_Below = 2;
	public static readonly int Layer_Outline = 3;
	public static readonly int Layer_Water = 4;

	public static readonly String Custom_Spark = "Spark";
	public static readonly String Custom_Grass = "Grass";
	public static readonly String Custom_Water = "Water";

	// Event bus
	[Signal] public delegate void PlayerExpandedEventHandler(int Count);
	[Signal] public delegate void AnyTileChangedEventHandler();
	[Signal] public delegate void GridVisibleChangedEventHandler(bool Shown);

	public MainMap()
	{
		Singleton = this;
	}

	public override void _Ready()
	{
		EmitSignal(MainMap.SignalName.GridVisibleChanged, false);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		SetLayerModulate(Layer_Outline, new Color(0.0f, 0.0f, 0.0f, 0.0f));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!OutlineShown && Input.IsActionJustPressed("ShowGrid"))
		{
			AddOutlineActive_Internal(GridCode);
		}
		
		if (OutlineShown && Input.IsActionJustReleased("ShowGrid"))
		{
			RemoveOutlineActive_Internal(GridCode);
		}

		if (OutlineActive > 0 && !OutlineShown)
		{
			OutlineShown = true;
			SetLayerModulate(Layer_Outline, GridColor);
			EmitSignal(SignalName.GridVisibleChanged, true);
		}

		if (OutlineActive <= 0 && OutlineShown)
		{
			OutlineShown = false;
			SetLayerModulate(Layer_Outline, new Color(0.0f, 0.0f, 0.0f, 0.0f));
			EmitSignal(SignalName.GridVisibleChanged, false);
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

	void AddGrassTile_Internal(Vector2I TileLoc)
	{
		if (GrassTiles.ContainsKey(TileLoc))
		{
			GrassTiles[TileLoc] += 1;
			return;
		}

		GrassTiles.Add(TileLoc, 1);
		Array<Vector2I> TileToGrow = new() { TileLoc };
		if (MainMap.Singleton.GetCellTileData(Layer_Ground, TileToGrow[0]).Terrain != MainMap.Terrain_Water)
		{
			SetCellsTerrainConnect(MainMap.Layer_Ground, TileToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_Grass);
		}
		SetCellsTerrainConnect(MainMap.Layer_Below, TileToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_WetGrass);
        Broadcast(SignalName.AnyTileChanged);

		if (Expansion < GrassTiles.Count)
		{
			Broadcast(SignalName.PlayerExpanded, 1);
			Expansion = GrassTiles.Count;
		}
	}

	void RemoveGrassTile_Internal(Vector2I TileLoc)
	{
		if (!GrassTiles.ContainsKey(TileLoc)) return;

		GrassTiles[TileLoc] -= 1;
		if (GrassTiles[TileLoc] <= 0)
		{
			Array<Vector2I> TileToRemove = new() { TileLoc };
			if (MainMap.Singleton.GetCellTileData(Layer_Ground, TileToRemove[0]).Terrain != MainMap.Terrain_Water)
			{
				SetCellsTerrainConnect(MainMap.Layer_Ground, TileToRemove, MainMap.TerrainSet_Default, MainMap.Terrain_Dirt);
			}
			SetCellsTerrainConnect(MainMap.Layer_Below, TileToRemove, MainMap.TerrainSet_Default, MainMap.Terrain_WetDirt);
			Broadcast(SignalName.AnyTileChanged);
			GrassTiles.Remove(TileLoc);
		}
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

		// Ignore the below layer, it just reflects the ground layer
		//if (MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition) != null)
		//{
			//return MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition).Terrain;
		//}

		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			return MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition).Terrain;
		}

		return Terrain_Void;
	}

	public static bool TileHasFlag(Vector2I GridPosition, String CustomFlag)
	{
		if (MainMap.Singleton == null) return false;
		bool result = false;

		if (MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition) != null)
		{
			TileData BelowData = MainMap.Singleton.GetCellTileData(Layer_Below, GridPosition);
			result |= (bool)BelowData.GetCustomData(CustomFlag);
		}
		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			TileData GroundData = MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition);
			result |= (bool)GroundData.GetCustomData(CustomFlag);
		}
		return result;
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

		if (Singleton.HarvestedTiles.Contains(TileLoc))
		{
			return ECurrencyType.None;
		}

		Singleton.HarvestedTiles.Add(TileLoc);
		return GetTileCurrency(TileLoc);
	}

	public static void UnHarvestTile(Vector2I TileLoc)
	{
		if (Singleton == null) return;
		Singleton.HarvestedTiles.Remove(TileLoc);
	}

	public static void AddGrassTile(Vector2I TileLoc)
	{
		if (Singleton == null) return;
		Singleton.AddGrassTile_Internal(TileLoc);
	}

	public static void RemoveGrassTile(Vector2I TileLoc)
	{
		if (Singleton == null) return;
		Singleton.RemoveGrassTile_Internal(TileLoc);
	}

	public static int GetPlayerExpansionLevel()
	{
		if (Singleton == null) return -1;
		return Singleton.Expansion;
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