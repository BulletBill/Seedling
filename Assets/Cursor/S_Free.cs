using Godot;
using System;

public partial class S_Free : Node, ICursorState
{
    [Export] public Godot.Collections.Array<Data_Action> ActionList = new();
    Cursor ParentCursor;
    public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

    // Cursor state interface
    public String GetName()
    {
        return "Free";
    }

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
    }
}
