using Godot;
using System;

public partial class HoverArea : Area2D
{
    [Export] public AnimationPlayer ParentAnimator { get; protected set; }

    [Signal] public delegate void ClickedEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void OnMouseEnter()
    {
        Cursor.AddHoverArea(this);
        ParentAnimator?.Play("Hover");
        
        IHoverable HoverParent = GetParentOrNull<IHoverable>();
        HoverParent?.OnHovered();
    }

    public void OnMouseExit()
    {
        Cursor.RemoveHoverArea(this);
        ParentAnimator?.Play("Unhover");

        IHoverable HoverParent = GetParentOrNull<IHoverable>();
        HoverParent?.ExitHovered();
    }

    public void OnClick()
    {
        EmitSignal(SignalName.Clicked);
    }
}

public interface IHoverable
{
    public void OnHovered();
    public void ExitHovered();
}