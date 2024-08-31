using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Reflection;

public partial class C_GrassGrowth : TowerComponent
{
    [Export] public float Radius = 1.0f;
    [Export] public float GrowthInterval = 2.0f;
    [Export] public bool AttractEnemies = true;
    [Export] public bool GrassSource = false;
    float GrowthTimer = 0.1f;
    List<TileAtDistance> TilesToGrass = new();
    List<Vector2I> TilesInArea = new();
    C_GrassGrowth ParentConnection = null;
    List<C_GrassGrowth> ChildConnections = new List<C_GrassGrowth>();
    public int ConnectionDepth { get; protected set;} = 0;
    public bool Connected { get; protected set; } = false;
    public static readonly String GrassNodeGroup = "GrassNode";

    public override void TowerReady()
    {
        ParentTower = GetParentOrNull<Tower>();
        
        GetTilesInGrowingRange();
        AddToGroup(GrassNodeGroup);

        if (GrassSource)
        {
            Connected = true;
            ConnectionDepth = 0;
        }
        else
        {
            ConnectToParent();
        }
    }

    public override void TowerRemoved()
    {
        foreach(var tile in TilesInArea)
        {
            MainMap.RemoveGrassTile(tile);
        }
        RemoveFromGroup(GrassNodeGroup);

        foreach (C_GrassGrowth OtherGrower in ChildConnections)
        {
            OtherGrower.ConnectToParent();
        }
    }

    public override void TowerDisabled()
    {
    }

    public override void TowerEnabled()
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
            if (Distance < Radius)
            {
                TilesInArea.Add(i);
                if (MainMap.TileHasFlag(i, MainMap.Custom_Grass))
                {
                    MainMap.AddGrassTile(i);
                }
                else
                {
                    TilesToGrass.Add(new TileAtDistance(i, Distance));
                }
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
                if (MainMap.TileHasFlag(TilesToGrass[i].TilePosition, MainMap.Custom_Grass))
                {
                    TilesToGrass.RemoveAt(i);
                }
                else
                {
                    TilesToGrow.Add(TilesToGrass[i]);
                }
            }
        }

        if (TilesToGrow.Count > 0)
        {
            // Modify Tilemap
            int ChosenTile = MathHelper.GetIntInRange(0, TilesToGrow.Count - 1);
            MainMap.AddGrassTile(TilesToGrow[ChosenTile].TilePosition);
            TilesToGrass.Remove(TilesToGrow[ChosenTile]);
            
            /* Add all close tiles
            foreach (TileAtDistance tile in TilesToGrow)
            {
                MainMap.AddGrassTile(tile.TilePosition);
                TilesToGrass.Remove(tile);
            }
            */
        }
    }

    public void ConnectToParent()
    {
        Connected = false;
        ParentConnection = null;

        int BestConnection = 10000;
        float BestDistance = 10000.0f;
        foreach (Node GrassNode in GetTree().GetNodesInGroup(GrassNodeGroup))
        {
            if (GrassNode is C_GrassGrowth OtherGrower)
            {
                if (GrassNode == this) continue;
                if (!OtherGrower.Connected) continue;
                float Distance = ParentTower.GlobalPosition.DistanceTo(OtherGrower.ParentTower.GlobalPosition);
                if (Distance > Radius) continue;
                if (OtherGrower.ConnectionDepth < BestConnection || (OtherGrower.ConnectionDepth == BestConnection && Distance < BestDistance))
                {
                    BestConnection = OtherGrower.ConnectionDepth;
                    BestDistance = Distance;
                    ParentConnection = OtherGrower;
                    OtherGrower.AddChildConnection(this);
                    Connected = true;

                    Sprite2D ConnectionArrow = GetNode<Sprite2D>("ConnectionArrow");
                    if (IsInstanceValid(ConnectionArrow))
                    {
                        ConnectionArrow.Visible = true;
                        ConnectionArrow.LookAt(OtherGrower.ParentTower.GlobalPosition);
                        ConnectionArrow.Rotate(Mathf.Pi);
                        ConnectionArrow.Scale = new Vector2(Distance / 182.0f, ConnectionArrow.Scale.Y);
                    }
                }
            }
        }

        if (Connected)
        {
            ConnectionDepth = ParentConnection.ConnectionDepth + 1;
        }
        else
        {
            Sprite2D ConnectionArrow = GetNode<Sprite2D>("ConnectionArrow");
            if (IsInstanceValid(ConnectionArrow))
            {
                ConnectionArrow.Visible = false;
            }

            foreach (C_GrassGrowth OtherGrower in ChildConnections)
            {
                OtherGrower.ConnectToParent();
            }
        }
    }

    void ConnectToChildren()
    {
        
    }

    public void AddChildConnection(C_GrassGrowth NewChild)
    {
        if (NewChild == null) return;
        if (ChildConnections.Contains(NewChild)) return;

        ChildConnections.Add(NewChild);
    }
}
