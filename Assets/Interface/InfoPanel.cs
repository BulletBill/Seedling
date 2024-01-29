using Godot;
using System;

public partial class InfoPanel : Node
{
    AnimationPlayer Anim;
    bool IsLeft = true;

    public override void _Ready()
    {
        Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
    }

    public void MoveToLeft()
    {
        if (Anim == null) return;
        if (IsLeft) return;
        IsLeft = true;

        Anim.Play("MoveToLeft");
    }

    public void MoveToRight()
    {
        if (Anim == null) return;
        if (!IsLeft) return;
        IsLeft = false;

        Anim.Play("MoveToRight");
    }
}
