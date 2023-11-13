using Godot;
using System;

public partial class S_PlaceTower : Node, ICursorState
{
    [Export] public Color GoodColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    [Export] public Color BadColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    Cursor ParentCursor;
    R_BuildTower TowerToBuild;
    bool PlacementIsValid;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
        PlacementIsValid = false;
    }

    // Cursor state interface
    public void OnEnable()
    {
        GD.Print("Cursor State changed to Placement");
    }
	public void OnDisable()
    {
        TowerToBuild = null;
        PlacementIsValid = false;

        if (ParentCursor == null) return;
        ParentCursor.PlacementGhost.Visible = false;
        ParentCursor.GridHighlight.Visible = false;
    }
	public void OnClick()
    {

    }
    public void OnEscape()
    {

    }
	public void OnMove(Vector2I NewMapPosition)
    {
        if (TowerToBuild == null) return;
    }
    public void SetTowerToBuild(R_BuildTower NewTowerToBuild)
    {
        if (ParentCursor == null) return;
        if (NewTowerToBuild == null) return;
        TowerToBuild = NewTowerToBuild;
        ParentCursor.PlacementGhost.Visible = true;
        ParentCursor.PlacementGhost.Texture = TowerToBuild.PlacementSprite;
        ParentCursor.PlacementGhost.SelfModulate = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        ParentCursor.GridHighlight.Visible = true;
        ParentCursor.GridHighlight.SelfModulate = BadColor;

        GD.Print("Placement target is " + NewTowerToBuild.TowerName);
    }
}
