using Godot;
using System;

public partial class S_PlaceTower : Node, ICursorState
{
    [Export] public Color GoodColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    [Export] public Color BadColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    Vector2I CurrentPosition = new();
    Cursor ParentCursor;
    Data_Tower TowerData;
    PackedScene TowerToBuild;
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
    
    public ECursorState GetState()
    {
        return ECursorState.Placement;
    }

	public void OnMove(Vector2I NewMapPosition)
    {
        if (TowerData == null) return;
        if (CachedTileMap == null) { CachedTileMap = MainMap.Singleton; return; }
        CurrentPosition = NewMapPosition;

        bool CanAfford = Player.CanAfford(TowerData.Cost);
        bool CanPlace = CanPlaceTile(NewMapPosition);
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

        PlacementIsValid = CanAfford && CanPlace && !Occupied;

        if (ParentCursor != null)
        {
            ParentCursor.PlacementGhost.SelfModulate = PlacementIsValid ? GoodColor : BadColor;
        }
    }
    public void SetTowerToBuild(Data_Tower NewTowerData, PackedScene NewTowerToBuild)
    {
        if (ParentCursor == null) return;
        if (NewTowerData == null) return;
        if (NewTowerToBuild == null) return;
        TowerData = NewTowerData;
        TowerToBuild = NewTowerToBuild;
        ParentCursor.PlacementGhost.Visible = true;
        ParentCursor.PlacementGhost.Texture = TowerData.Icon;
        ParentCursor.PlacementGhost.SelfModulate = PlacementIsValid ? GoodColor : BadColor;

        GD.Print("Placement target is " + TowerData.DisplayName);
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
                            (TowerData.CanBuildOnWater && TileType == MainMap.Terrain_Water) ||
                            (TowerData.CanBuildOnChasm && TileType == MainMap.Terrain_Chasm));
        result &= MatchesTile;

        return result;
    }

    // Cursor state interface
    public void OnEnable()
    {
        GD.Print("Cursor State changed to Placement");
        CachedTileMap = MainMap.Singleton;
        MainMap.AddOutlineActive(GridCode);
    }
	public void OnDisable()
    {
        TowerData = null;
        PlacementIsValid = false;
        MainMap.RemoveOutlineActive(GridCode);

        if (ParentCursor == null) return;
        ParentCursor.PlacementGhost.Visible = false;
    }
	public void OnClick()
    {
        if (ParentCursor == null) return;
        if (ParentCursor.HoverList.Count > 0)
        {
            ParentCursor.HoverList[0].OnClick();
            return;
        }

        if (PlacementIsValid == false) return;
        if (TowerData == null) return;
        if (TowerToBuild == null) return;

        Node2D NewTower = TowerToBuild.Instantiate<Node2D>();
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
    public void OnEscape()
    {
        Cursor.PopState();
    }
}
