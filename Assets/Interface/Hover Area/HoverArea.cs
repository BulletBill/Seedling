using Godot;
using System;
using Godot.Collections;

public partial class HoverArea : Area2D
{
    [Export] public AnimationPlayer ParentAnimator { get; protected set; }
    [Export] public Array<String> ReactStates { get; protected set; } = new();

    [Signal] public delegate void ClickedEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void OnMouseEnter()
    {
        if (ReactStates.Contains(Cursor.GetStateName()) == false) return;
        Cursor.AddHoverArea(this);
        ParentAnimator?.Play("Hover");
        
        IHoverable HoverParent = GetParentOrNull<IHoverable>();
        HoverParent?.OnHovered();
    }

    public void OnMouseExit()
    {
        if (ReactStates.Contains(Cursor.GetStateName()) == false) return;
        ForceMouseExit();
        Cursor.RemoveHoverArea(this);
    }

    public void ForceMouseExit()
    {
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