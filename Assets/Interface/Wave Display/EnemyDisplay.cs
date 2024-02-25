using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class EnemyDisplay : Node, IHoverable
{
    public Data_Enemy EnemyParams = new();
    bool Initialized = false;
    bool Disabled = true;
    Sprite2D Background;
    Sprite2D Icon;
    AnimationPlayer Anim;
    RichTextLabel Count;
    HoverArea Hover;

    public override void _EnterTree() {}
    public override void _Ready() {}

    public void Initialize()
    {
        Initialized = true;
        Background = GetNodeOrNull<Sprite2D>("Background");
        Icon = GetNodeOrNull<Sprite2D>("Icon");
        Anim = GetNodeOrNull<AnimationPlayer>("HoverAnimator");
        Count = GetNodeOrNull<RichTextLabel>("Count");
        Hover = GetNodeOrNull<HoverArea>("HoverArea");

        Clear();
    }
    
    public void AssignEnemyParams(Data_Enemy NewParams, int NewCount)
    {
        if (!Initialized) { Initialize(); }
        if (NewParams == null)
        {
            Clear();
            return;
        }

        Disabled = false;
        Hover?.SetDisabled(false);
        EnemyParams = NewParams;
        if (IsInstanceValid(Icon))
        {
            Icon.Texture = EnemyParams.Icon;
        }
        if (IsInstanceValid(Count))
        {
            Count.Text = NewCount.ToString();
        }
    }

    void Clear()
    {
        Disabled = true;
        Hover?.SetDisabled(true);
        Anim?.Play("Unhover");
        if (IsInstanceValid(Icon)) { Icon.Texture = null; }
        if (IsInstanceValid(Count)) { Count.Text = ""; }
    }

    void OnClick() {}

	public void OnHovered()
	{
		Cursor.Broadcast(Cursor.SignalName.SelectableHovered, EnemyParams);
	}

	public void ExitHovered()
	{
		Cursor.Broadcast(Cursor.SignalName.SelectableExited);
	}
}
