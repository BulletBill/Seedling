using Godot;
using System;
using System.Net.Http.Headers;

public partial class CameraController : Camera2D
{
	[Export] public float MinimumZoom { get; protected set; } = 0.75f;
	[Export] public float MaximumZoom { get; protected set; } = 1.0f;
	[Export] public float ZoomSpeed { get; protected set; } = 3f;
	[Export] public float PanSpeed { get; protected set; } = 600.0f;
	[Export] public int UIPanelHeight { get; protected set; } = 192;
    public Vector2 HalfSize { get; protected set; } = new Vector2(0.0f, 0.0f);
	int DefaultLimitBottom;
	public float ScaledUIPanelHeight;
	float CurrentZoom = 1.0f;
	protected static CameraController Singleton;

	bool MousePanning = false;
	Vector2 MousePanStart;
	Vector2 CameraPanStart;

	public CameraController()
	{
		Singleton = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DefaultLimitBottom = LimitBottom;
		UpdateSizes();
	}

	void UpdateSizes()
	{
		float HalfHeight = GetViewportRect().Size.Y / 2 / Zoom.X; //ProjectSettings.GetSetting("display/window/size/height").ToString().ToFloat(); //GetViewport().Size.y;
        float HalfWidth = GetViewportRect().Size.X / 2 / Zoom.Y; //ProjectSettings.GetSetting("display/window/size/width").ToString().ToFloat(); //GetViewport().Size.x;

        HalfSize = new Vector2(HalfWidth, HalfHeight);

		ScaledUIPanelHeight = UIPanelHeight / Zoom.X;
		LimitBottom = DefaultLimitBottom + Mathf.FloorToInt(ScaledUIPanelHeight);

		float NewX = HalfSize.X;
		float NewY = HalfSize.Y;
		if (LimitLeft + HalfSize.X < LimitRight - HalfSize.X)
		{
			NewX = Mathf.Clamp(GlobalPosition.X, LimitLeft + HalfSize.X, LimitRight - HalfSize.X);
		}
		if (LimitTop + HalfSize.Y < LimitBottom - HalfSize.Y)
		{
			NewY = Mathf.Clamp(GlobalPosition.Y, LimitTop + HalfSize.Y, LimitBottom - HalfSize.Y);
		}
		
		GlobalPosition = new Vector2(NewX, NewY);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 Dir = new(0,0);
		if (Input.IsActionPressed("CameraPan_Left")){ Dir.X -= 1.0f; }
		if (Input.IsActionPressed("CameraPan_Right")){ Dir.X += 1.0f; }
		if (Input.IsActionPressed("CameraPan_Up")){ Dir.Y -= 1.0f; }
		if (Input.IsActionPressed("CameraPan_Down")){ Dir.Y += 1.0f; }

		if (Input.IsActionJustPressed("CameraZoom_In")) { AdjustZoom(ZoomSpeed * (float)delta); }
		if (Input.IsActionJustPressed("CameraZoom_Out")) { AdjustZoom(-ZoomSpeed * (float)delta); }

		if (Dir.X != 0 || Dir.Y != 0)
		{
			Pan(Dir, delta);
		}

		if (Input.IsActionJustPressed("CameraPan_Mouse"))
		{
			MousePanning = true;
			//PositionSmoothingEnabled = false;
			MousePanStart = GetMousePosition();
			CameraPanStart = GlobalPosition;
		}
		if (Input.IsActionPressed("CameraPan_Mouse") == false && MousePanning)
		{
			MousePanning = false;
			//PositionSmoothingEnabled = true;
		}

		if (MousePanning)
		{
			Vector2 PosDiff = MousePanStart - GetMousePosition();
			
			float NewX = Mathf.Clamp(CameraPanStart.X + PosDiff.X, LimitLeft + HalfSize.X, LimitRight - HalfSize.X);
			float NewY = Mathf.Clamp(CameraPanStart.Y + PosDiff.Y, LimitTop + HalfSize.Y, LimitBottom - HalfSize.Y);
	
			GlobalPosition = new Vector2(NewX, NewY);
		}
	}

	public void Pan(Vector2 Direction, double delta)
	{
		float NewX = GlobalPosition.X;
		float NewY = GlobalPosition.Y;

		if (LimitLeft + HalfSize.X < LimitRight - HalfSize.X)
		{
			NewX = Mathf.Clamp(GlobalPosition.X + ((Direction.X * PanSpeed) * (float)delta), LimitLeft + HalfSize.X, LimitRight - HalfSize.X);
		}

		if (LimitTop + HalfSize.Y < LimitBottom - HalfSize.Y)
		{
			NewY = Mathf.Clamp(GlobalPosition.Y + ((Direction.Y * PanSpeed) * (float)delta), LimitTop + HalfSize.Y, LimitBottom - HalfSize.Y);
		}
		
		GlobalPosition = new Vector2(NewX, NewY);
	}

	public void AdjustZoom(float Delta)
	{
		CurrentZoom = Mathf.Clamp(CurrentZoom + Delta, MinimumZoom, MaximumZoom);
		Zoom = new Vector2(CurrentZoom, CurrentZoom);
		UpdateSizes();
	}

	// Static accessors
	public static Vector2 GetPosition()
	{
		if (Singleton == null) return new Vector2(0,0);

		return Singleton.GlobalPosition;
	}

	public static Vector2 GetMousePosition()
	{
		if (Singleton == null) return new Vector2(0,0);
		Vector2 SmoothingDiff = Singleton.GetScreenCenterPosition() - Singleton.GlobalPosition;

		return Singleton.GetLocalMousePosition() - SmoothingDiff;
	}

	public static bool MouseIsOverUIPanel()
	{
		if (Singleton == null) return false;

		return GetMousePosition().Y > Singleton.HalfSize.Y - Singleton.ScaledUIPanelHeight;
	}
}
