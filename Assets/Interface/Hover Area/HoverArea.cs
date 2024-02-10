using Godot;
using System;
using Godot.Collections;

public partial class HoverArea : Area2D
{
    [Export] public AnimationPlayer ParentAnimator { get; protected set; }
    [Export] public Array<ECursorState> ReactStates { get; protected set; } = new();
    IHoverable HoverParent;
    public bool Disabled { get; protected set; } = false;
    bool Hovered = false;

    [Signal] public delegate void ClickedEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HoverParent = GetParentOrNull<IHoverable>();
    }

    public void OnMouseEnter()
    {
        Hovered = true;
        if (ReactStates.Contains(Cursor.GetCurrentState()) == false) return;
        Cursor.AddHoverArea(this);
        if (!Disabled)
        {
            ParentAnimator?.Play("Hover");
        }

        HoverParent?.OnHovered();
    }

    public void OnMouseExit()
    {
        Hovered = false;
        if (ReactStates.Contains(Cursor.GetCurrentState()) == false) return;
        Cursor.RemoveHoverArea(this);
        if (!Disabled)
        {
            ParentAnimator?.Play("Unhover");
        }

        HoverParent?.ExitHovered();
    }

    public void ForceMouseExit()
    {
        if (!Disabled)
        {
            ParentAnimator?.Play("Unhover");
        }

        IHoverable HoverParent = GetParentOrNull<IHoverable>();
        HoverParent?.ExitHovered();
    }

    public void OnClick()
    {
        if (Disabled) return;
        EmitSignal(SignalName.Clicked);
    }

    public void SetDisabled(bool NewDisabled)
    {
        Disabled = NewDisabled;
        if (Disabled)
        {
            ForceMouseExit();
        }
        else if (Hovered)
        {
            OnMouseEnter();
        }
    }

    public void AddReactState(ECursorState NewState)
    {
        if (ReactStates.Contains(NewState) == false)
        {
            ReactStates.Add(NewState);
        }
    }

    public void RemoveReactState(ECursorState RemoveState)
    {
        ReactStates.Remove(RemoveState);
    }
}

public interface IHoverable
{
    public void OnHovered();
    public void ExitHovered();
}