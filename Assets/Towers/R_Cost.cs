using Godot;
using System;

public partial class R_Cost : Resource
{
    [Export] public int LifeForce;
    [Export] public int Substance;
    [Export] public int Flow;
    [Export] public int Breath;
    [Export] public int Energy;

    public String GetString(ECurrencyType Type)
    {
        if (Type == ECurrencyType.Lifeforce) { return TextHelpers.Icon("Lifeforce Small") + LifeForce.ToString(); }
        if (Type == ECurrencyType.Substance) { return TextHelpers.Icon("Substance Small") + Substance.ToString(); }
        if (Type == ECurrencyType.Flow) { return TextHelpers.Icon("Flow Small") + Flow.ToString(); }
        if (Type == ECurrencyType.Breath) { return TextHelpers.Icon("Breath Small") + Breath.ToString(); }
        if (Type == ECurrencyType.Energy) { return TextHelpers.Icon("Energy Small") + Energy.ToString(); }
        return "";
    }
}
