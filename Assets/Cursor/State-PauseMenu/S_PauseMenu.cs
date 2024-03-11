using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class S_PauseMenu : Cursor_State
{
	Cursor ParentCursor;
	public override void _Ready()
    {
        ParentCursor = GetParentOrNull<Cursor>();
    }

	// Cursor state interface
	public override ECursorState GetState()
	{
		return ECursorState.Menu_Pause;
	}

	public override void OnEnable()
	{
		GD.Print("Cursor State changed to Pause Menu");
		if (HoverList.Count > 0) { HoverList[0].Activate(); }
	}
	public override void OnDisable()
	{
		if (HoverList.Count > 0) { HoverList[0].Deactivate(); }
	}
	public override void OnClick()
	{
		if (ParentCursor == null) return;
        if (HoverList.Count <= 0) return;
		if (IsInstanceValid(HoverList[0]) == false) return;

        HoverList[0].OnClick();
	}
	public override void OnEscape()
	{
		if (Level.IsGameOver()) return;
		Cursor.PopState();
	}
	public override void OnMove(Vector2I NewMapPosition) {}

	public override Node2D GetSelectedObject()
	{
		if (HoverList.Count <= 0) return null;
		return HoverList[0];
	}
}
