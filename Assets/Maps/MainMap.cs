using Godot;
using GodotPlugins.Game;
using System;
using Godot.Collections;

public partial class MainMap : TileMap
{
	[Export] public ECurrencyType ResourcePerGrass = ECurrencyType.Lifeforce;
	[Export] public int AmountPerGrass = 1;
	[Export] public bool AddToMaximum = true;
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
	public static readonly int Terrain_WaterEdge = 3;
	public static readonly int Terrain_WetDirt = 4;
	public static readonly int Terrain_WetGrass = 5;
	public static readonly int Terrain_WetRocks = 6;
	public static readonly int Terrain_DirtEdge = 7;
	public static readonly int Terrain_Path = 8;
	public static readonly int Terrain_Sand = 9;

	public static readonly int TerrainSet_Default = 0;

	public static readonly int Layer_Path = 0;
	public static readonly int Layer_BelowGround = 1;
	public static readonly int Layer_WaterShader = 2;
	public static readonly int Layer_Ground = 3;
	public static readonly int Layer_AboveGround = 4;
	public static readonly int Layer_Grid = 5;

	public static readonly String Custom_Spark = "Spark";
	public static readonly String Custom_Grass = "Grass";
	public static readonly String Custom_Water = "Water";
	public static readonly String Custom_CanGrowGrass = "CanGrowGrass";
	public static readonly String Custom_ProduceEarth = "ProduceEarth";
	public static readonly String Custom_ProduceWater = "ProduceWater";
	public static readonly String Custom_ProduceAir = "ProduceAir";
	public static readonly String Custom_ProduceFire = "ProduceFire";

	// Event bus
	[Signal] public delegate void PlayerExpandedEventHandler(int Count);
	[Signal] public delegate void AnyTileChangedEventHandler();
	[Signal] public delegate void GridVisibleChangedEventHandler(bool Shown);
	[Signal] public delegate void OnZoomChangedEventHandler(float NewZoom);

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
		SetLayerEnabled(Layer_Grid, false);
		SetLayerModulate(Layer_Path, new Color(0.0f, 0.0f, 0.0f, 0.0f));
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
			SetLayerEnabled(Layer_Grid, true);
			EmitSignal(SignalName.GridVisibleChanged, true);
		}

		if (OutlineActive <= 0 && OutlineShown)
		{
			OutlineShown = false;
			SetLayerEnabled(Layer_Grid, false);
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
		if ((bool)MainMap.Singleton.GetCellTileData(Layer_Ground, TileToGrow[0]).GetCustomData(Custom_CanGrowGrass))
		{
			SetCellsTerrainConnect(MainMap.Layer_Ground, TileToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_Grass);
		}
		if (Singleton.GetCellTileData(Layer_BelowGround, TileToGrow[0]) != null)
		{
			if ((bool)MainMap.Singleton.GetCellTileData(Layer_BelowGround, TileToGrow[0]).GetCustomData(Custom_CanGrowGrass))
			{
				SetCellsTerrainConnect(MainMap.Layer_BelowGround, TileToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_WetGrass);
			}
		}
        Broadcast(SignalName.AnyTileChanged);
		AddGrassResource(1 * AmountPerGrass);

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
			if ((bool)MainMap.Singleton.GetCellTileData(Layer_Ground, TileToRemove[0]).GetCustomData(Custom_CanGrowGrass))
			{
				SetCellsTerrainConnect(MainMap.Layer_Ground, TileToRemove, MainMap.TerrainSet_Default, MainMap.Terrain_Dirt);
			}
			if ((bool)MainMap.Singleton.GetCellTileData(Layer_BelowGround, TileToRemove[0]).GetCustomData(Custom_CanGrowGrass))
			{
				SetCellsTerrainConnect(MainMap.Layer_BelowGround, TileToRemove, MainMap.TerrainSet_Default, MainMap.Terrain_WetDirt);
			}
			Broadcast(SignalName.AnyTileChanged);
			AddGrassResource(-1 * AmountPerGrass);
			GrassTiles.Remove(TileLoc);
		}
	}

	void AddGrassResource(int Amount)
	{
		if (AddToMaximum)
		{
			PlayerEvent.BroadcastAddMaxResource(ResourcePerGrass, Amount);
		}
		else
		{
			PlayerEvent.BroadcastAddResource(ResourcePerGrass, Amount);
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
		
		if (MainMap.Singleton.GetCellTileData(Layer_AboveGround, GridPosition) != null)
		{
			int Terrain = MainMap.Singleton.GetCellTileData(Layer_AboveGround, GridPosition).Terrain;
			if (Terrain >= 0) { return Terrain; }
		}

		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			int Terrain = MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition).Terrain;
			if (Terrain >= 0 && Terrain != Terrain_DirtEdge && Terrain != Terrain_WaterEdge) { return Terrain; }
		}

		if (MainMap.Singleton.GetCellTileData(Layer_BelowGround, GridPosition) != null)
		{
			int Terrain = MainMap.Singleton.GetCellTileData(Layer_BelowGround, GridPosition).Terrain;
			if (Terrain >= 0) { return Terrain; }
		}

		return Terrain_Void;
	}

	public static bool TileHasFlag(Vector2I GridPosition, String CustomFlag)
	{
		if (MainMap.Singleton == null) return false;

		if (MainMap.Singleton.GetCellTileData(Layer_AboveGround, GridPosition) != null)
		{
			TileData ObjectData = MainMap.Singleton.GetCellTileData(Layer_AboveGround, GridPosition);
			if ((bool)ObjectData.GetCustomData(CustomFlag)) return true;
		}
		if (MainMap.Singleton.GetCellTileData(Layer_BelowGround, GridPosition) != null)
		{
			TileData BelowData = MainMap.Singleton.GetCellTileData(Layer_BelowGround, GridPosition);
			if ((bool)BelowData.GetCustomData(CustomFlag)) return true;
		}
		if (MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition) != null)
		{
			TileData GroundData = MainMap.Singleton.GetCellTileData(Layer_Ground, GridPosition);
			if ((bool)GroundData.GetCustomData(CustomFlag)) return true;
		}
		return false;
	}

	public static ECurrencyType GetTileCurrency(Vector2I GridPosition)
	{
		int Type = GetTileType(GridPosition);
		if (Type == MainMap.Terrain_Void) return ECurrencyType.None;

		if (TileHasFlag(GridPosition, Custom_ProduceEarth))
		{
			return ECurrencyType.Substance;
		}
		if (TileHasFlag(GridPosition, Custom_ProduceWater))
		{
			return ECurrencyType.Flow;
		}
		if (TileHasFlag(GridPosition, Custom_ProduceAir))
		{
			return ECurrencyType.Breath;
		}
		if (TileHasFlag(GridPosition, Custom_ProduceFire))
		{
			return ECurrencyType.Energy;
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