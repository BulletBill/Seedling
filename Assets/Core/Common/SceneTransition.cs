using Godot;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class SceneTransition : CanvasLayer
{
	static SceneTransition Singleton;
	public Node CurrentScene;
	PackedScene PendingScene;
	AnimationPlayer Anim;

	public SceneTransition()
	{
		if (Singleton != null)
		{
			GD.PrintErr("SceneTransition: Singleton already exists!");
			Singleton.QueueFree();
		}
		Singleton = this;
	}

	public override void _Ready()
	{
		Anim = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
	}

	public void ChangeScene_Internal(PackedScene NewScene)
	{
		if (NewScene == null) return;
		PendingScene = NewScene;

		Anim.Play("Dissolve");
	}

	public void FinalizeSceneTransition_Internal()
	{
		if (PendingScene == null) return;

		Node NewScene = PendingScene.Instantiate<Node>();
		AddSibling(NewScene);
		PendingScene = null;
		SetCurrentScene_Internal(NewScene);

		Anim.Play("Reveal");
	}

	public void SetCurrentScene_Internal(Node NewCurrentScene)
	{
		CurrentScene?.QueueFree();
		CurrentScene = NewCurrentScene;
	}

	// Static Functions
	public static void ChangeScene(PackedScene NewScene)
	{
		if (Singleton == null) return;
		Singleton.ChangeScene_Internal(NewScene);
	}

	public static void ChangeScene(String NewSceneName)
	{
		if (Singleton == null) return;
		Singleton.ChangeScene_Internal(Game.GetSceneByName(NewSceneName));
	}

	public static void FinalizeSceneTransition()
	{
		if (Singleton == null) return;
		Singleton.FinalizeSceneTransition_Internal();
	}

	public static void SetCurrentScene(Node NewCurrentScene)
	{
		if (Singleton == null) return;
		Singleton.SetCurrentScene_Internal(NewCurrentScene);
	}
}
