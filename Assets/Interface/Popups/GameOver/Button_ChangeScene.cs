using Godot;
using System;

public partial class Button_ChangeScene : Node2D, IHoverable
{
    [Export] public String GotoScene = "TestLevel";
    public override void _Ready()
    {
        HoverArea Hover = GetNodeOrNull<HoverArea>("HoverArea");
		if (Hover != null)
		{
			Hover.Clicked += OnClick;
		}
    }

    public void OnHovered()
    {

    }

    public void ExitHovered()
    {

    }

    public void OnClick()
    {
        AnimationPlayer Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
        Anim?.Play("Click");
        SceneTransition.ChangeScene(GotoScene);
    }
}
