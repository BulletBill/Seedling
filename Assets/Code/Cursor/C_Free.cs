using Godot;
using System;

public partial class C_Free : Node, ICursorState
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
        bool ShowHighlight = false;
        foreach(Tower t in GetTree().GetNodesInGroup(Tower.GroupName))
        {
            if (t.MapPosition == NewMapPosition)
            {
                ShowHighlight = true;
            }
        }

        ParentCursor.GridHighlight.Visible = ShowHighlight;
    }
}
