using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Contracts;

public partial class ContextMenu : Node2D
{
	protected Dictionary<int, ActionButton> ButtonMap = new();
	public Tower SelectedTower { get; protected set; }
	AnimationPlayer Anim;

	public override void _EnterTree()
	{
		PlayerEvent.Register(PlayerEvent.SignalName.TowerSelected, Callable.From((Tower T) => TowerSelected(T)));
		PlayerEvent.Register(PlayerEvent.SignalName.TowerDeselected, Callable.From(() => TowerDeselected()));
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonMap.Clear();
		foreach(Node n in GetChildren())
		{
            if (n is ActionButton ActionButtonChild)
            {
                ButtonMap.Add(ActionButtonChild.ActionIndex, ActionButtonChild);
            }
        }
		Anim = GetNodeOrNull<AnimationPlayer>("ButtonAnimator");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TowerSelected(Tower NewTower)
	{
		SelectedTower = NewTower;

		if (SelectedTower == null)
		{
			CloseMenu();
			return;
		}

		GlobalPosition = SelectedTower.GlobalPosition;

		for (int i = 0; i < ButtonMap.Count; i++)
		{
			if (ButtonMap.ContainsKey(i) == false) continue;
			if (SelectedTower.Actions.Count <= i)
			{
				ButtonMap[i].AssignActionParams(null);
			}
			else
			{
				ButtonMap[i].AssignActionParams(NewTower.Actions[i]);
			}
		}

		OpenMenu();
	}

	public void TowerDeselected()
	{
		CloseMenu();
	}

	public void ItemClicked(int ItemIndex)
	{
		if (SelectedTower == null) return;

	}

	public void OpenMenu()
	{
		Anim.Play("Show");
	}

	public void CloseMenu()
	{
		Anim.PlayBackwards("Show");
	}
}
