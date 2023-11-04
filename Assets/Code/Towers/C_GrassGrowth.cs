using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;

[Tool]
public partial class C_GrassGrowth : Node2D
{
    [Export] public float Radius = 1.0f;
    [Export] public float GrowthInterval = 2.0f;
    float GrowthTimer = 0.1f;
    List<TileAtDistance> TilesInRange = new();

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return; // Don't run in editor
        
        GetTilesInGrowingRange();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint()) return; // Don't run in editor
        
        if (GrowthTimer > 0.0f)
        {
            GrowthTimer -= (float)delta;

            if (TilesInRange.Count > 0 && GrowthTimer <= 0.0f)
            {
                GrowGrass();
                GrowthTimer = GrowthInterval;
            }
        }
    }

    public void GetTilesInGrowingRange()
    {
        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        Node2D ParentNode = GetParentOrNull<Node2D>();
        if (ParentNode == null) return;

        Array<Vector2I> AllCells = CachedTileMap.GetUsedCells(0);
        TilesInRange.Clear();
        foreach(Vector2I i in AllCells)
        {
            // Don't bother with grass tiles
            if (CachedTileMap.GetCellTileData(0, i).Terrain == MainMap.Terrain_Grass) continue;

            Vector2 TilePos = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(i));
            float Distance = ParentNode.GlobalPosition.DistanceTo(TilePos);
            if (Distance < Radius)
            {
                TilesInRange.Add(new TileAtDistance(i, Distance));
            }
        }
        TilesInRange.Sort((tile1, tile2) => tile1.Distance.CompareTo(tile2.Distance));
    }

    public void GrowGrass()
    {
        if (TilesInRange.Count <= 0) return;

        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        Array<Vector2I> TilesToGrow = new();
        float DistanceToUse = TilesInRange[0].Distance;

        for(int i = TilesInRange.Count - 1; i >= 0; i--)
        {
            if (TilesInRange[i].Distance <= DistanceToUse)
            {
                TilesToGrow.Add(TilesInRange[i].TilePosition);
                TilesInRange.RemoveAt(i);
            }
        }
        CachedTileMap.SetCellsTerrainConnect(MainMap.Layer_Ground, TilesToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_Grass);
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return; // Only execute in Editor
        DrawArc(Position, Radius, 0.0f, 360.0f, 36, Colors.LawnGreen);
    }
}
