using Godot;
using System;

public partial class R_Cost : Resource
{
    [Export] public int LifeForce;
    [Export] public int Substance;
    [Export] public int Flow;
    [Export] public int Breath;
    [Export] public int Energy;

    public R_Cost()
    {
        LifeForce = 0;
        Substance = 0;
        Flow = 0;
        Breath = 0;
        Energy = 0;
    }
    public R_Cost(int lifeforce, int substance, int flow, int breath, int energy)
    {
        LifeForce = lifeforce;
        Substance = substance;
        Flow = flow;
        Breath = breath;
        Energy = energy;
    }

    public String GetString(ECurrencyType Type)
    {
        if (Type == ECurrencyType.Lifeforce) { return TextHelpers.Icon("Lifeforce Small") + LifeForce.ToString(); }
        if (Type == ECurrencyType.Substance) { return TextHelpers.Icon("Substance Small") + Substance.ToString(); }
        if (Type == ECurrencyType.Flow) { return TextHelpers.Icon("Flow Small") + Flow.ToString(); }
        if (Type == ECurrencyType.Breath) { return TextHelpers.Icon("Breath Small") + Breath.ToString(); }
        if (Type == ECurrencyType.Energy) { return TextHelpers.Icon("Energy Small") + Energy.ToString(); }
        return "";
    }

    public static R_Cost operator* (R_Cost Self, float scalar)
    {
        R_Cost Result = new()
        {
            LifeForce = Self.LifeForce,
            Substance = (int)(Self.Substance * scalar),
            Flow = (int)(Self.Flow * scalar),
            Breath = (int)(Self.Breath * scalar),
            Energy = (int)(Self.Energy * scalar)
        };

        return Result;
    }
}
