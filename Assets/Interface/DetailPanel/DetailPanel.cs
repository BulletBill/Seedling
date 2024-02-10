using Godot;
using System;
using System.ComponentModel;

public partial class DetailPanel : Node2D
{
	Sprite2D Icon;
	RichTextLabel Header;
	RichTextLabel Body;

	public override void _EnterTree()
	{
		Cursor.Register(Cursor.SignalName.SelectableHovered, Callable.From((Data_Hoverable T) => ObjectHovered(T)));
		Cursor.Register(Cursor.SignalName.SelectableExited, Callable.From(() => ObjectExitHovered()));
	}

	public override void _Ready()
	{
		Icon = GetNodeOrNull<Sprite2D>("Icon");
		Header = GetNodeOrNull<RichTextLabel>("Header");
		Body = GetNodeOrNull<RichTextLabel>("Body");
		ObjectExitHovered();
	}

	public void ObjectHovered(Data_Hoverable HoveredObject)
	{
		if (HoveredObject == null)
		{
			if (Icon != null) { Icon.Texture = null; }
			if (Header != null) { Header.Text = ""; }
			if (Body != null) { Body.Text = ""; }
			return;
		}
		
		if (Icon != null) { Icon.Texture = HoveredObject.Icon; }
		if (Header != null) { Header.Text = HoveredObject.DisplayName; }
		if (Body != null) { Body.Text = HoveredObject.Description; }
	}

	public void ObjectExitHovered()
	{
		if (!Cursor.IsOverHoverable())
		{
			if (Icon != null) { Icon.Texture = null; }
			if (Header != null) { Header.Text = ""; }
			if (Body != null) { Body.Text = ""; }
		}
	}
}
