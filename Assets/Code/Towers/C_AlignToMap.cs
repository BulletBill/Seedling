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
        Tower ParentNode = GetParentOrNull<Tower>();
        if (ParentNode == null) return;

        MainMap CachedTileMap = MainMap.Singleton;
        if (CachedTileMap == null) return;

        ParentNode.MapPosition = CachedTileMap.LocalToMap(CachedTileMap.ToLocal(ParentNode.GlobalPosition));
		ParentNode.GlobalPosition = CachedTileMap.ToGlobal(CachedTileMap.MapToLocal(ParentNode.MapPosition));
    }
}
