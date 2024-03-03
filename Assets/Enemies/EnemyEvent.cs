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
    [Signal] public delegate void ShowNextTimedWaveEventHandler(R_SpawnWave NextWave);
    [Signal] public delegate void ShowNextExpandWaveEventHandler(R_SpawnWave NextWave);
    [Signal] public delegate void EnemyDefeatedEventHandler(Enemy DefeatedEnemy);
    [Signal] public delegate void StartFinalWaveEventHandler();

    public EnemyEvent()
    {
        Singleton = this;
    }

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
