using Godot;
using System;

public partial class MenuEvent : Node
{
    public static MenuEvent Singleton;

    public MenuEvent()
    {
        Singleton = this;
    }

    [Signal] public delegate void OpenGameOverMenuEventHandler();
    [Signal] public delegate void OpenVictoryMenuEventHandler();
    [Signal] public delegate void OpenPauseMenuEventHandler();
    [Signal] public delegate void CloseMenusEventHandler();

    public override void _Ready()
    {
    }

    // Event bus functions
	public static bool Register(String DelegateName, Callable Receiver)
    {
        if (Singleton == null) return false;
        Error Result = Singleton.Connect(DelegateName, Receiver);
        if (Result == Error.Ok)
        {
            return true;
        }
        return false;
    }

	public static Error Broadcast(String EventName, params Variant[] args)
    {
        if (Singleton == null) return Error.DoesNotExist;
        return Singleton.EmitSignal(EventName, args);
    }
}
