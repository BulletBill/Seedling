using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Reflection;

[Tool]
public partial class C_GrassGrowth : Node2D
{
    [Export] public float Radius = 1.0f;
    [Export] public float GrowthInterval = 2.0f;
    [Export] public bool AttractEnemies = true;
    public static int ResourcePerTile = 5;
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

        Array<Vector2I> AllCells = CachedTileMap.GetUsedCells(MainMap.Layer_Ground);
        TilesInRange.Clear();
        foreach(Vector2I i in AllCells)
        {
            // Don't bother with grass tiles
            if (CachedTileMap.GetCellTileData(MainMap.Layer_Ground, i).Terrain == MainMap.Terrain_Grass) continue;

            Vector2 TilePos = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(i));
            float Distance = ParentNode.GlobalPosition.DistanceTo(TilePos);
            if (Distance < (Radius * GlobalScale.X))
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

        List<TileAtDistance> TilesToGrow = new();
        float DistanceToUse = TilesInRange[0].Distance;

        for(int i = TilesInRange.Count - 1; i >= 0; i--)
        {
            if (TilesInRange[i].Distance <= DistanceToUse)
            {
                // Tile might have become grass from another tower in the mean time
                if (CachedTileMap.GetCellTileData(MainMap.Layer_Ground, TilesInRange[i].TilePosition).Terrain != MainMap.Terrain_Grass)
                {
                    TilesToGrow.Add(TilesInRange[i]);
                }
                //TilesInRange.RemoveAt(i);
            }
        }

        if (TilesToGrow.Count > 0)
        {
            int GrowIndex = Game.GetIntInRange(0, TilesToGrow.Count - 1);
            Array<Vector2I> TileToGrow = new() { TilesToGrow[GrowIndex].TilePosition };

            // Add resources for growing grass
            ECurrencyType AddedType = MainMap.GetTileCurrency(TileToGrow[0]);
            Currency AddedCurrency = Player.GetCurrency(AddedType);
            if (AddedCurrency != null)
            {
                AddedCurrency.AddAmount(ResourcePerTile);
                Game.SpawnResourceNumber(CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(TileToGrow[0])), ResourcePerTile, AddedType);
            }

            // Modify Tilemap
            TilesInRange.Remove(TilesToGrow[GrowIndex]);
            CachedTileMap.SetCellsTerrainConnect(MainMap.Layer_Ground, TileToGrow, MainMap.TerrainSet_Default, MainMap.Terrain_Grass);
            if (AttractEnemies)
            {
                Player.Singleton.EmitSignal("GrassGrown", TileToGrow.Count);
            }
        }
    }

    public override void _Draw()
    {
        if (!Engine.IsEditorHint()) return; // Only execute in Editor
        DrawArc(Position, Radius, 0.0f, 360.0f, 36, Colors.LawnGreen);
    }
}
