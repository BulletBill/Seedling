using Godot;
using System;

public partial class S_Free : Node, ICursorState
{
    Cursor ParentCursor;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

    // Cursor state interface
    public void OnEnable()
    {
        GD.Print("Cursor State changed to Free");
    }
	public void OnDisable()
    {

    }
    public void OnEscape()
    {
        //TODO: Open Game Menu
    }
	public void OnClick()
    {
        if (ParentCursor == null) return;
        if (ParentCursor.HoverList.Count <= 0) return;

        ParentCursor.HoverList[0].OnClick();
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
