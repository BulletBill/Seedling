using Godot;
using System;

public partial class EnemyEvent : Node
{
    public static EnemyEvent Singleton;

    [Signal] public delegate void PlayerExpansionChangedEventHandler(int ExpansionUntilNextWave, int ExpansionBetweenWaves);
    [Signal] public delegate void ExpansionWaveCountChangedEventHandler(int WaveCount);
    [Signal] public delegate void WaveTimerChangedEventHandler(int SecondsUntilNextWave, int SecondsBetweenWaves);
    [Signal] public delegate void TimedWaveCountChangedEventHandler(int WaveCount);
    [Signal] public delegate void SetDisableSpawnsEventHandler(bool Disabled);

    public EnemyEvent()
    {
        Singleton = this;
    }

    public override void _Ready()
    {
        
    }

    /*
    public static SpawnerBrain GetSpawnerBrain()
    {
        if (Singleton == null) return null;

        return Singleton.GetNodeOrNull<SpawnerBrain>("SpawnerBrain");
    }
    */

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
