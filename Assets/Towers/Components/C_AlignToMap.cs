using Godot;
using System;

public partial class C_AlignToMap : Node
{
    public override void _Ready()
    {
        AlignToMap();
    }

    public void AlignToMap()
    {
        Node2D ParentNode = GetParentOrNull<Node2D>();
        if (!IsInstanceValid(ParentNode)) return;

        MainMap CachedTileMap = MainMap.Singleton;
        if (!IsInstanceValid(CachedTileMap)) return;

        Vector2I MapPosition = CachedTileMap.LocalToMap(CachedTileMap.ToLocal(ParentNode.GlobalPosition));
		ParentNode.GlobalPosition = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(MapPosition));

        if (ParentNode is Tower ParentTower)
        {
            ParentTower.MapPosition = MapPosition;
        }
    }
}
