using Godot;
using System;
using Godot.Collections;

[Tool]
public partial class C_GrassGrowth : Node2D
{
    [Export] public float Radius = 1.0f;

    public void GetTilesInGrowingRange()
    {
        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        Array<Vector2I> AllCells = CachedTileMap.GetUsedCells(0);
        Array<Vector2I> AllCells_Masked = new Array<Vector2I>();
        foreach(Vector2I i in AllCells)
        {
            
        }
    }

    public void GrowGrass()
    {
        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;
    }

    public override void _Draw()
    {
        DrawArc(Position, Radius, 0.0f, 360.0f, 18, Colors.Red);
    }
}
