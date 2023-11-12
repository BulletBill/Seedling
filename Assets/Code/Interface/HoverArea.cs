using Godot;
using System;

public partial class HoverArea : Area2D
{
    Sprite2D Background;

    [Signal] public delegate void ClickedEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Background = GetNodeOrNull<Sprite2D>("Background");
        MatchSpriteToCollision();
    }

    void MatchSpriteToCollision()
    {
        if (Background == null) { GD.Print("HoverArea: no background"); return; }
        CollisionShape2D CachedShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (CachedShape == null) { GD.PrintErr("HoverArea: CollisionShape2D is null!"); return; }
        RectangleShape2D Rect = CachedShape.Shape as RectangleShape2D;
        if (Rect == null) { GD.PrintErr("HoverArea: Area2D shape is not rectangle!"); return; }

        Background.Transform = CachedShape.Transform;
        float ScaleX = Rect.Size.X * (1.0f / Background.Texture.GetWidth());
        float ScaleY = Rect.Size.Y * (1.0f / Background.Texture.GetHeight());
        Background.Scale = new Vector2(ScaleX, ScaleY);
    }

    public void OnMouseEnter()
    {
        Cursor.AddHoverArea(this);
        SetHovering(true);
    }

    public void OnMouseExit()
    {
        Cursor.RemoveHoverArea(this);
        SetHovering(false);
    }

    public void OnClick()
    {
        EmitSignal("Clicked");
    }

    public void SetHovering(bool Hovering)
    {
        if (Background == null) return;

        if (Hovering) MatchSpriteToCollision();
        Background.Visible = Hovering;
    }
}
