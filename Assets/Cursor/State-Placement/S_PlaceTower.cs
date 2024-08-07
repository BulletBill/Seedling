using Godot;
using System;

public partial class S_PlaceTower : Cursor_State
{
    [Export] public Godot.Collections.Array<Data_Action> StateActions;
    [Export] public Color GoodColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    [Export] public Color BadColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    Vector2I CurrentPosition = new();
    Cursor ParentCursor;
    Data_Tower TowerData;
    bool PlacementIsValid;
    bool PlacedOne = false;
    MainMap CachedTileMap;
    static readonly uint GridCode = 2;

    public override void _EnterTree()
    {
        MainMap.Register(MainMap.SignalName.AnyTileChanged, Callable.From(() => UpdateInPlace()));
        PlayerEvent.Register(PlayerEvent.SignalName.AnyResourceChanged, Callable.From(() => UpdateInPlace()));
    }

    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
        PlacementIsValid = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Modify") && PlacedOne)
        {
            PlacedOne = false;
            Cursor.PopState();
        }
    }

    public void UpdateInPlace()
    {
        OnMove(CurrentPosition);
    }
    
    public override ECursorState GetState()
    {
        return ECursorState.Placement;
    }

	public override void OnMove(Vector2I NewMapPosition)
    {
        CurrentPosition = NewMapPosition;

        if (TowerData == null) return;
        if (CachedTileMap == null) { CachedTileMap = MainMap.Singleton; return; }

        bool CanAfford = Player.CanAfford(TowerData.Cost);
        bool CanPlace = CanPlaceTile(NewMapPosition);
        bool HarvestValid = !TowerData.NeedsHarvestFlag || IsTileNearResource(CurrentPosition);
        bool Occupied = false;
        foreach (Node TowerNode in GetTree().GetNodesInGroup(Tower.GroupName))
        {
            Tower Tower = TowerNode as Tower;
            if (Tower == null) continue;
            if (Tower.MapPosition == NewMapPosition)
            {
                Occupied = true;
                break;
            }
        }

        PlacementIsValid = CanAfford && CanPlace && HarvestValid && !Occupied;

        if (ParentCursor != null)
        {
            ParentCursor.PlacementGhost.SelfModulate = PlacementIsValid ? GoodColor : BadColor;
        }
    }
    public void SetTowerToBuild(Data_Tower NewTowerData)
    {
        if (ParentCursor == null) return;
        if (NewTowerData == null) return;
        TowerData = NewTowerData;
        ParentCursor.PlacementGhost.Visible = true;
        ParentCursor.PlacementGhost.Texture = TowerData.SpriteSheet;
        OnMove(Cursor.GetCurrentTile());

        Cursor.Broadcast(Cursor.SignalName.SetFixedObject, TowerData);
        Game.Log(LogCategory.Cursor, "Placement target is " + TowerData.DisplayName);
    }

    bool CanPlaceTile(Vector2I NewMapPosition)
    {
        if (TowerData == null) return false;
        if (CachedTileMap == null) { CachedTileMap = MainMap.Singleton; return false; }

        bool result = true;
        if (TowerData.NeedsSparkFlag)
        {
            result &= MainMap.TileHasFlag(NewMapPosition, MainMap.Custom_Spark);
        }

        if (TowerData.NeedsGrassFlag)
        {
            result &= MainMap.TileHasFlag(NewMapPosition, MainMap.Custom_Grass);
        }

        int TileType = MainMap.GetTileType(NewMapPosition);

        bool MatchesTile = ((TowerData.CanBuildOnGrass && TileType == MainMap.Terrain_Grass) ||
                            (TowerData.CanBuildOnDirt && TileType == MainMap.Terrain_Dirt) ||
                            (TowerData.CanBuildOnStone && TileType == MainMap.Terrain_Stone) ||
                            (TowerData.CanBuildOnWater && (TileType == MainMap.Terrain_WetDirt || TileType == MainMap.Terrain_WetGrass)));
        result &= MatchesTile;

        return result;
    }

    bool IsTileNearResource(Vector2I Position)
    {
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				Vector2I HarvestTile = Position + new Vector2I(x, y);

				ECurrencyType TileCurrency = MainMap.GetTileCurrency(HarvestTile);
				if (TileCurrency != ECurrencyType.None)
				{
					return true;
				}
			}
		}

        return false;
    }

    // Cursor state interface
    public override void OnEnable()
    {
          
        CachedTileMap = MainMap.Singleton;
        MainMap.AddOutlineActive(GridCode);
        Cursor.Broadcast(Cursor.SignalName.AnyStateActionsChanged, StateActions);
        if (HoverList.Count > 0){ HoverList[0].Activate(); }
    }
	public override void OnDisable()
    {
        Cursor.Broadcast(Cursor.SignalName.ClearFixedObject);
        TowerData = null;
        PlacementIsValid = false;
        MainMap.RemoveOutlineActive(GridCode);
        if (HoverList.Count > 0){ HoverList[0].Deactivate(); }

        if (ParentCursor == null) return;
        ParentCursor.PlacementGhost.Visible = false;
    }
	public override void OnClick()
    {
        if (ParentCursor == null) return;
        if (HoverList.Count > 0)
        {
            HoverList[0].OnClick();
            return;
        }

        if (PlacementIsValid == false) return;
        if (TowerData == null) return;
        if (CameraController.MouseIsOverUIPanel()) return;

        Node2D NewTower = TowerData.CreateTower();
        if (NewTower == null) return;
        NewTower.GlobalPosition = Cursor.GetTilePosition();
        MainMap.Singleton.AddChild(NewTower);

        Player.Spend(TowerData.Cost);

        if (Input.IsActionPressed("Modify"))
        {
            PlacedOne = true;
            PlacementIsValid = false;
            ParentCursor.PlacementGhost.SelfModulate = BadColor;
        }
        else
        {
            Cursor.PopState();
        }
    }
    public override void OnEscape()
    {
        Cursor.PopState();
    }

    public override Node2D GetSelectedObject()
    {
        return null;
    }
}
