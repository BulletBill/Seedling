using Godot;
using System;
using System.ComponentModel;

public partial class DetailPanel : Node2D
{
	Sprite2D Icon;
	RichTextLabel Header;
	RichTextLabel Body;

	Texture2D FixedTexture = null;
	String FixedHeader = "";
	String FixedBody = "";

	public override void _EnterTree()
	{
		Cursor.Register(Cursor.SignalName.SelectableHovered, Callable.From((Data_Hoverable T) => ObjectHovered(T)));
		Cursor.Register(Cursor.SignalName.SelectableExited, Callable.From(() => ObjectExitHovered()));
		Cursor.Register(Cursor.SignalName.SetFixedObject, Callable.From((Data_Hoverable T) => SetFixedObject(T)));
		Cursor.Register(Cursor.SignalName.ClearFixedObject, Callable.From(() => ClearFixedObject()));
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
		if (HoveredObject == null || HoveredObject.DisplayName == "")
		{
			if (Icon != null) { Icon.Texture = FixedTexture; }
			if (Header != null) { Header.Text = FixedHeader; }
			if (Body != null) { Body.Text = FixedBody; }
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
			if (Icon != null) { Icon.Texture = FixedTexture; }
			if (Header != null) { Header.Text = FixedHeader; }
			if (Body != null) { Body.Text = FixedBody; }
		}
	}

	public void SetFixedObject(Data_Hoverable FixedObject)
	{
		if (FixedObject == null)
		{
			ClearFixedObject();
			return;
		}

		FixedTexture = FixedObject.Icon;
		FixedHeader = FixedObject.DisplayName;
		FixedBody = FixedObject.Description;
		ObjectHovered(FixedObject);
	}

	public void ClearFixedObject()
	{
		FixedTexture = null;
		FixedHeader = "";
		FixedBody = "";
		ObjectExitHovered();
	}
}
