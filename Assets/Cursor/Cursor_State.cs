using Godot;
using System;
using System.Collections.Generic;

public enum ECursorState
{
	Free,
	Placement,
	Menu_Context,
	Menu_Pause,
}

public abstract partial class Cursor_State : Node
{
    public List<HoverArea> HoverList {get; protected set;} = new();

    public abstract ECursorState GetState();
	public abstract Node2D GetSelectedObject();
	public abstract void OnEnable();
	public abstract void OnDisable();
	public abstract void OnClick();
	public abstract void OnEscape();
	public abstract void OnMove(Vector2I NewMapPosition);
}
