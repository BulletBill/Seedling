using Godot;
using Microsoft.VisualBasic;
using System;
using System.Linq;

// For buses:
// Establish singleton in Initializer
// Receivers register for events in _EnterTree
// Broadcaster fire initial events in _Ready

public partial class PlayerEvent : Node
{
    public static PlayerEvent Bus { get; protected set; }

    // Lives
    [Signal] public delegate void LivesChangedEventHandler(int NewAmount);
    [Signal] public delegate void LoseLifeEventHandler(int Damage);

    // Resources
    [Signal] public delegate void LifeforceChangedEventHandler(int NewAmount, int NewMax);
    [Signal] public delegate void LifeforceIncomeChangedEventHandler(int NewAmount);
    [Signal] public delegate void AddLifeforceEventHandler(int AddAmount);
    [Signal] public delegate void AddLifeforceMaxEventHandler(int AddMax);
    [Signal] public delegate void AddLifeforceIncomeEventHandler(int AddIncome);

    [Signal] public delegate void SubstanceChangedEventHandler(int NewAmount, int NewMax);
    [Signal] public delegate void SubstanceIncomeChangedEventHandler(int NewAmount);
    [Signal] public delegate void AddSubstanceEventHandler(int AddAmount);
    [Signal] public delegate void AddSubstanceMaxEventHandler(int AddMax);
    [Signal] public delegate void AddSubstanceIncomeEventHandler(int AddIncome);

    [Signal] public delegate void FlowChangedEventHandler(int NewAmount, int NewMax);
    [Signal] public delegate void FlowIncomeChangedEventHandler(int NewAmount);
    [Signal] public delegate void AddFlowEventHandler(int AddAmount);
    [Signal] public delegate void AddFlowMaxEventHandler(int AddMax);
    [Signal] public delegate void AddFlowIncomeEventHandler(int AddIncome);

    [Signal] public delegate void BreathChangedEventHandler(int NewAmount, int NewMax);
    [Signal] public delegate void BreathIncomeChangedEventHandler(int NewAmount);
    [Signal] public delegate void AddBreathEventHandler(int AddAmount);
    [Signal] public delegate void AddBreathMaxEventHandler(int AddMax);
    [Signal] public delegate void AddBreathIncomeEventHandler(int AddIncome);

    [Signal] public delegate void EnergyChangedEventHandler(int NewAmount, int NewMax);
    [Signal] public delegate void EnergyIncomeChangedEventHandler(int NewAmount);
    [Signal] public delegate void AddEnergyEventHandler(int AddAmount);
    [Signal] public delegate void AddEnergyMaxEventHandler(int AddMax);
    [Signal] public delegate void AddEnergyIncomeEventHandler(int AddIncome);

    [Signal] public delegate void AnyResourceChangedEventHandler();
    [Signal] public delegate void SpendResourcesEventHandler(R_Cost Cost);

    // Towers
    [Signal] public delegate void TowerFinishedEventHandler(Tower FinishedTower);
    [Signal] public delegate void TowerHoveredEventHandler(Data_Tower HoveredTower);
    [Signal] public delegate void TowerExitHoveredEventHandler();
    [Signal] public delegate void TowerSelectedEventHandler(Tower SelectedTower);
    [Signal] public delegate void TowerDeselectedEventHandler();

    public PlayerEvent()
    {
        Bus = this;
    }

    public static bool Register(String DelegateName, Callable Receiver)
    {
        if (Bus == null) return false;
        Error Result = Bus.Connect(DelegateName, Receiver);
        if (Result == Error.Ok)
        {
            return true;
        }
        return false;
    }

    public static bool RegisterResourceChanged(ECurrencyType Type, Callable Receiver)
    {
        return PlayerEvent.Register(Type.ToString() + "Changed", Receiver);
    }
    public static bool RegisterResourceIncome(ECurrencyType Type, Callable Receiver)
    {
        return PlayerEvent.Register(Type.ToString() + "IncomeChanged", Receiver);
    }
    public static bool RegisterResourceAdd(ECurrencyType Type, Callable Receiver)
    {
        return PlayerEvent.Register("Add" + Type.ToString(), Receiver);
    }
    public static bool RegisterResourceAddMax(ECurrencyType Type, Callable Receiver)
    {
        return PlayerEvent.Register("Add" + Type.ToString() + "Max", Receiver);
    }
    public static bool RegisterResourceAddIncome(ECurrencyType Type, Callable Receiver)
    {
        return PlayerEvent.Register("Add" + Type.ToString() + "Income", Receiver);
    }

    public static Error Broadcast(String EventName, params Variant[] args)
    {
        if (Bus == null) return Error.DoesNotExist;
        return Bus.EmitSignal(EventName, args);
    }

    public static Error BroadcastAddResource(ECurrencyType Type, params Variant[] args)
    {
        Error ret = Broadcast("Add" + Type.ToString(), args);
        if (args.GetLength(0) > 0 && (int)args[0] > 0)
        {
            Broadcast("AnyResourceChanged");
        }
        return ret;
    }
    public static Error BroadcastAddMaxResource(ECurrencyType Type, params Variant[] args)
    {
        return Broadcast("Add" + Type.ToString() + "Max", args);
    }
    public static Error BroadcastAddIncome(ECurrencyType Type, params Variant[] args)
    {
        return Broadcast("Add" + Type.ToString() + "Income", args);
    }
}
