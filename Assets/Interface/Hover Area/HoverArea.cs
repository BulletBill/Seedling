using Godot;
using System;
using Godot.Collections;

public partial class HoverArea : Area2D
{
    [Export] public AnimationPlayer ParentAnimator { get; protected set; }
    [Export] public Array<ECursorState> ReactStates { get; protected set; } = new();
    IHoverable HoverParent;
    public bool Disabled { get; protected set; } = false;
    public bool Hovered { get; protected set; } = false;

    [Signal] public delegate void ClickedEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HoverParent = GetParentOrNull<IHoverable>();
    }

    public override void _ExitTree()
    {
        foreach (ECursorState state in ReactStates)
        {
            Cursor.RemoveHoverArea(this, state);
        }
    }

    public void OnMouseEnter()
    {
        Hovered = true;
        foreach (ECursorState state in ReactStates)
        {
            Cursor.AddHoverArea(this, state);
        }

        Activate();
    }

    public void Activate()
    {
        if (ReactStates.Contains(Cursor.GetCurrentState()))
        {
            HoverParent?.OnHovered();

            if (!Disabled)
            {
                ParentAnimator?.Play("Hover");
            }
        }
    }

    public void OnMouseExit()
    {
        Hovered = false;
        foreach (ECursorState state in ReactStates)
        {
            Cursor.RemoveHoverArea(this, state);
        }

        Deactivate();
    }

    public void Deactivate()
    {
        if (ReactStates.Contains(Cursor.GetCurrentState()))
        {
            if (!Disabled)
            {
                ParentAnimator?.Play("Unhover");
            }

            HoverParent?.ExitHovered();
        }
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
        if (!Visible) return;
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

    public bool HasReactState(ECursorState State)
    {
        return ReactStates.Contains(State);
    }
}

public interface IHoverable
{
    public void OnHovered();
    public void ExitHovered();
}