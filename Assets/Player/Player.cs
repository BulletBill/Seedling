using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Player : Node2D
{
    public static Player Singleton;
    [Export] public int StartingLives = 12;
    public int Lives { get; protected set;}
    public Dictionary<ECurrencyType, Currency> Currencies = new();
    public static List<Tower> DefendTargets = new();
    public static readonly float IncomeTime = 5.0f;

    // DEBUG
    public bool FreeTowers = false;

    public Player()
    {
        Singleton = this;
    }

    public override void _EnterTree()
    {
        PlayerEvent.Register(PlayerEvent.SignalName.LoseLife, Callable.From((int n) => TakeDamage(n)));
    }

    public override void _Ready()
    {
        Lives = StartingLives;
        PlayerEvent.Broadcast(PlayerEvent.SignalName.LivesChanged, Lives);
    }

    void TakeDamage(int Damage)
    {
        Lives -= Damage;
        PlayerEvent.Broadcast(PlayerEvent.SignalName.LivesChanged, Lives);
        if (Lives <= 0)
        {
            Level ParentLevel = GetParentOrNull<Level>();
            if (ParentLevel != null)
            {
                SceneTransition.ChangeScene(ParentLevel.ExitLevel);
            }
            else
            {
                GetTree().Quit();
            }
        }
    }

    // Static functions

    public static int GetCurrentAmount(ECurrencyType Type)
    {
        if (Singleton == null) return 0;
        return Singleton.Currencies[Type].Amount;
    }
    public static int GetCurrentMax(ECurrencyType Type)
    {
        if (Singleton == null) return 0;
        return Singleton.Currencies[Type].MaximumAmount;
    }
    public static int GetCurrentIncome(ECurrencyType Type)
    {
        if (Singleton == null) return 0;
        return Singleton.Currencies[Type].Income;
    }

    public static bool CanAfford(R_Cost Cost)
    {
        if (Singleton == null) return false;

        if (Singleton.FreeTowers) return true;

        if (Cost.LifeForce > 0 && Singleton.Currencies[ECurrencyType.Lifeforce].Amount < Cost.LifeForce) return false;
        if (Cost.Substance > 0 && Singleton.Currencies[ECurrencyType.Substance].Amount < Cost.Substance) return false;
        if (Cost.Flow > 0 && Singleton.Currencies[ECurrencyType.Flow].Amount < Cost.Flow) return false;
        if (Cost.Breath > 0 && Singleton.Currencies[ECurrencyType.Breath].Amount < Cost.Breath) return false;
        if (Cost.Energy > 0 && Singleton.Currencies[ECurrencyType.Energy].Amount < Cost.Energy) return false;

        return true;
    }

    public static bool Spend(R_Cost Cost)
    {
        if (Singleton == null) return false;
        if (CanAfford(Cost) == false) return false;

        Singleton.Currencies[ECurrencyType.Lifeforce].AddAmount(-1 * Cost.LifeForce);
        Singleton.Currencies[ECurrencyType.Substance].AddAmount(-1 * Cost.Substance);
        Singleton.Currencies[ECurrencyType.Flow].AddAmount(-1 * Cost.Flow);
        Singleton.Currencies[ECurrencyType.Breath].AddAmount(-1 * Cost.Breath);
        Singleton.Currencies[ECurrencyType.Energy].AddAmount(-1 * Cost.Energy);

        return true;
    }
}
