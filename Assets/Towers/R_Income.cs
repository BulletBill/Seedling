using Godot;
using System;

[GlobalClass]
public partial class R_Income : Resource
{
    [Export] public float LifeForce;
    [Export] public float Substance;
    [Export] public float Flow;
    [Export] public float Breath;
    [Export] public float Energy;

    public R_Income() {}

    public R_Income (float NewLifeforce, float NewSubstance, float NewFlow, float NewBreath, float NewEnergy)
    {
        LifeForce = NewLifeforce;
        Substance = NewSubstance;
        Flow = NewFlow;
        Breath = NewBreath;
        Energy = NewEnergy;
    }
}
