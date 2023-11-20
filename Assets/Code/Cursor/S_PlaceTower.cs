using Godot;
using System;

public partial class S_PlaceTower : Node, ICursorState
{
    [Export] public Color GoodColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    [Export] public Color BadColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    Cursor ParentCursor;
    R_BuildTower TowerToBuild;
    bool PlacementIsValid;
    MainMap CachedTileMap;
    static readonly uint GridCode = 2;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
        PlacementIsValid = false;
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
        TowerToBuild = null;
        PlacementIsValid = false;
        MainMap.RemoveOutlineActive(GridCode);

        if (ParentCursor == null) return;
        ParentCursor.PlacementGhost.Visible = false;
    }
	public void OnClick()
    {
        if (PlacementIsValid == false) return;
        if (TowerToBuild.TowerToBuild == null) return;

        Tower NewTower = TowerToBuild.TowerToBuild.Instantiate<Tower>();
        if (NewTower == null) return;
        NewTower.GlobalPosition = Cursor.GetTilePosition();
        MainMap.Singleton.AddChild(NewTower);

        Player.Spend(TowerToBuild.Cost);

        Cursor.PopState();
    }
    public void OnEscape()
    {

    }
	public void OnMove(Vector2I NewMapPosition)
    {
        if (TowerToBuild == null) return;
        if (CachedTileMap == null) { CachedTileMap = MainMap.Singleton; return; }

        bool CanAfford = Player.CanAfford(TowerToBuild.Cost);
        bool CanPlace = (CachedTileMap.GetCellTileData(MainMap.Layer_Ground, NewMapPosition).Terrain == MainMap.Terrain_Grass)
                            || !TowerToBuild.NeedsGrass;

        PlacementIsValid = CanAfford && CanPlace;

        if (ParentCursor != null)
        {
            ParentCursor.PlacementGhost.SelfModulate = PlacementIsValid ? GoodColor : BadColor;
        }
    }
    public void SetTowerToBuild(R_BuildTower NewTowerToBuild)
    {
        if (ParentCursor == null) return;
        if (NewTowerToBuild == null) return;
        TowerToBuild = NewTowerToBuild;
        ParentCursor.PlacementGhost.Visible = true;
        ParentCursor.PlacementGhost.Texture = TowerToBuild.PlacementSprite;
        ParentCursor.PlacementGhost.SelfModulate = PlacementIsValid ? GoodColor : BadColor;

        GD.Print("Placement target is " + NewTowerToBuild.TowerName);
    }
}
