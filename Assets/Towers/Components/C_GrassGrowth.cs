using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Reflection;

[Tool]
public partial class C_GrassGrowth : Node2D, ITowerComponent
{
    [Export] public float Radius = 1.0f;
    [Export] public float GrowthInterval = 2.0f;
    [Export] public bool AttractEnemies = true;
    public static int ResourcePerTile = 5;
    float GrowthTimer = 0.1f;
    List<TileAtDistance> TilesToGrass = new();
    List<Vector2I> TilesInArea = new();

    public void TowerReady()
    {
        if (Engine.IsEditorHint()) return; // Don't run in editor
        
        GetTilesInGrowingRange();
    }

    public void TowerRemoved()
    {
        foreach(var tile in TilesInArea)
        {
            MainMap.RemoveGrassTile(tile);
        }
    }

    public void TowerUpdated()
    {
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint()) return; // Don't run in editor
        
        if (GrowthTimer > 0.0f)
        {
            GrowthTimer -= (float)delta * Level.GetSpeed();

            if (TilesToGrass.Count > 0 && GrowthTimer <= 0.0f)
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

        Array<Vector2I> AllCells = CachedTileMap.GetUsedCells(MainMap.Layer_Ground);
        TilesInArea.Clear();
        TilesToGrass.Clear();

        // Search all cells to find tiles in range, add non-grass tiles to grow list
        foreach(Vector2I i in AllCells)
        {
            Vector2 TilePos = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(i));
            float Distance = ParentNode.GlobalPosition.DistanceTo(TilePos);
            if (Distance < (Radius * GlobalScale.X))
            {
                TilesToGrass.Add(new TileAtDistance(i, Distance));
            }
        }
        TilesToGrass.Sort((tile1, tile2) => tile1.Distance.CompareTo(tile2.Distance));
    }

    public void GrowGrass()
    {
        if (TilesToGrass.Count <= 0) return;

        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        List<TileAtDistance> TilesToGrow = new();
        float DistanceToUse = TilesToGrass[0].Distance;

        for(int i = TilesToGrass.Count - 1; i >= 0; i--)
        {
            if (TilesToGrass[i].Distance <= DistanceToUse)
            {
                TilesToGrow.Add(TilesToGrass[i]);
            }
        }

        if (TilesToGrow.Count > 0)
        {
            // Modify Tilemap
            foreach (TileAtDistance tile in TilesToGrow)
            {
                MainMap.AddGrassTile(tile.TilePosition);
                TilesInArea.Add(tile.TilePosition);
                TilesToGrass.Remove(tile);
            }
        }
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return; // Only execute in Editor
        DrawArc(Position, Radius, 0.0f, 360.0f, 36, Colors.LawnGreen);
    }
}
