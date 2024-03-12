using Godot;
using System;

public partial class Button_Resume : Node2D, IHoverable
{
    public override void _Ready()
    {
        HoverArea Hover = GetNodeOrNull<HoverArea>("HoverArea");
		if (Hover != null)
		{
			Hover.Clicked += OnClick;
		}
    }

    public void OnHovered()
    {

    }

    public void ExitHovered()
    {

    }

    public void OnClick()
    {
        if (GetParent() is PopupMenu ParentMenu)
        {
            ParentMenu.Close();
        }

        if (Cursor.GetCurrentState() == ECursorState.Menu_Pause)
        {
            Cursor.PopState();
        }
    }
}
