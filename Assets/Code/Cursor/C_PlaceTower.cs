using Godot;
using System;

public partial class C_PlaceTower : Node, ICursorState
{
    Cursor ParentCursor;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

    // Cursor state interface
    public void OnEnable()
    {

    }
	public void OnDisable()
    {

    }
	public void OnClick()
    {

    }
	public void OnMove(Vector2I NewMapPosition)
    {
        
    }
}
