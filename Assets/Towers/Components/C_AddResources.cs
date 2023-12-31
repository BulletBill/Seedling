using Godot;
using System;

public partial class C_AddResources : Node
{
    [Export] public R_Cost AddFlatAmount = new();
    [Export] public R_Cost AddMaximum = new();
    Node2D Parent;

    public override void _Ready()
    {
        Parent = GetParent<Node2D>();
        if (Parent == null) return;

        if (AddFlatAmount.LifeForce > 0) { AddResource(ECurrencyType.Lifeforce, AddFlatAmount.LifeForce, new Vector2(0,0)); }
        if (AddFlatAmount.Substance > 0) { AddResource(ECurrencyType.Substance, AddFlatAmount.Substance, new Vector2(-15,-15)); }
        if (AddFlatAmount.Flow > 0) { AddResource(ECurrencyType.Flow, AddFlatAmount.Flow, new Vector2(+15,-15)); }
        if (AddFlatAmount.Breath > 0) { AddResource(ECurrencyType.Breath, AddFlatAmount.Breath, new Vector2(-15,+15)); }
        if (AddFlatAmount.Energy > 0) { AddResource(ECurrencyType.Energy, AddFlatAmount.Energy, new Vector2(+15,+15)); }

        if (AddMaximum.LifeForce > 0) { AddResourceMax(ECurrencyType.Lifeforce, AddMaximum.LifeForce); }
        if (AddMaximum.Substance > 0) { AddResourceMax(ECurrencyType.Substance, AddMaximum.Substance); }
        if (AddMaximum.Flow > 0) { AddResourceMax(ECurrencyType.Flow, AddMaximum.Flow); }
        if (AddMaximum.Breath > 0) { AddResourceMax(ECurrencyType.Breath, AddMaximum.Breath); }
        if (AddMaximum.Energy > 0) { AddResourceMax(ECurrencyType.Energy, AddMaximum.Energy); }
    }

    void AddResource(ECurrencyType Type, int Amount, Vector2 Offset)
    {
        PlayerEvent.BroadcastAddResource(Type, Amount);
        if (Parent.IsNodeReady())
        {
            Game.SpawnResourceNumber(Parent.GlobalPosition + Offset, Amount, Type);
        }
    }

    static void AddResourceMax(ECurrencyType Type, int Amount)
    {
        PlayerEvent.BroadcastAddMaxResource(Type, Amount);
    }

    public override void _ExitTree()
    {
        // Remove increases to maximum resource
        if (AddMaximum.LifeForce > 0) { AddResourceMax(ECurrencyType.Lifeforce, AddMaximum.LifeForce * -1); }
        if (AddMaximum.Substance > 0) { AddResourceMax(ECurrencyType.Substance, AddMaximum.Substance * -1); }
        if (AddMaximum.Flow > 0) { AddResourceMax(ECurrencyType.Flow, AddMaximum.Flow * -1); }
        if (AddMaximum.Breath > 0) { AddResourceMax(ECurrencyType.Breath, AddMaximum.Breath * -1); }
        if (AddMaximum.Energy > 0) { AddResourceMax(ECurrencyType.Energy, AddMaximum.Energy * -1); }
    }
}
