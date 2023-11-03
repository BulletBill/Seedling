using Godot;
using System;

public partial class C_AlignToMap : Node
{
    public Vector2I MapPosition = new Vector2I();

    public override void _Ready()
    {
        AlignToMap();
    }

    public void AlignToMap()
    {
        Node2D ParentNode = GetParentOrNull<Node2D>();
        if (ParentNode == null) return;

        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        MapPosition = CachedTileMap.LocalToMap(CachedTileMap.ToLocal(ParentNode.GlobalPosition));
		ParentNode.GlobalPosition = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(MapPosition));
    }
}
