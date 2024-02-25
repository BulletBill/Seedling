using Godot;
using System;

[GlobalClass]
public partial class Data_Enemy : Data_Hoverable
{
    [Export] public int SpawnCost { get; protected set; }
    [Export] public int PlayerDamage { get; protected set; }
    [Export] public float Speed { get; protected set; }
    [Export] public R_Cost Reward { get; protected set; }
    [Export] public Vector2I HealthRange { get; protected set; }

    public override string GetFullDescription()
    {
        String CombinedDescription = "Health: " + HealthRange.X.ToString() + "-" + HealthRange.Y.ToString() + " ";
        CombinedDescription += "Speed: " + Speed.ToString() + "\n";
        CombinedDescription += Description;

        return CombinedDescription;
    }
}
