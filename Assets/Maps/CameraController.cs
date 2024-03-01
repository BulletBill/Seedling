using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] Vector2 Limits = new Vector2(0.0f, 0.0f);
	[Export] public float MinimumZoom { get; protected set; } = 1.0f;
	[Export] public float MaximumZoom { get; protected set; } = 2.0f;
	[Export] public float PanSpeed { get; protected set; } = 250.0f;
    public Vector2 ViewSize { get; protected set; } = new Vector2(0.0f, 0.0f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//float ViewHeight = ProjectSettings.GetSetting("display/window/size/height").ToString().ToFloat(); //GetViewport().Size.y;
        //float ViewWidth = ProjectSettings.GetSetting("display/window/size/width").ToString().ToFloat(); //GetViewport().Size.x;

        //ViewSize = new Vector2(ViewWidth, ViewHeight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) return;
	}

	public override void _Draw()
	{
		if (!Engine.IsEditorHint()) return;
	}
}
