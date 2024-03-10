using Godot;
using System;

public partial class DamageNumber : RichTextLabel
{
    [Export] float HorizontalVariance = 1.0f;
    [Export] float Speed = 1.0f;
    [Export] float Gravity = 1.0f;
    [Export] float Bounce = 0.6f;
    [Export] bool Centered = true;
    Vector2 Velocity;
    Vector2 StartingPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        Velocity.X = rng.RandfRange(HorizontalVariance * -1.0f, HorizontalVariance);
        Velocity.Y = -1.0f * Speed;
        StartingPosition = GlobalPosition;
    }

    public void AssignResource(Vector2 NewStartingPosition, int Value, ECurrencyType Type)
    {
        StartingPosition = NewStartingPosition + new Vector2(-28.0f, 0.0f);
        GlobalPosition = NewStartingPosition + new Vector2(-28.0f, 0.0f);
        Scale = new Vector2(0.5f, 0.5f);

        Text = "";
        if (Centered) Text += "[center]";
        switch (Type)
        {
            case ECurrencyType.Lifeforce:
            Text += TextHelpers.Icon("Lifeforce Small");
            break;
            case ECurrencyType.Substance:
            Text += TextHelpers.Icon("Substance Small");
            break;
            case ECurrencyType.Flow:
            Text += TextHelpers.Icon("Flow Small");
            break;
            case ECurrencyType.Breath:
            Text += TextHelpers.Icon("Breath Small");
            break;
            case ECurrencyType.Energy:
            Text += TextHelpers.Icon("Energy Small");
            break;
        }
        Text += Value.ToString();
        if (Centered) Text += "[/center]";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (GlobalPosition.Y > StartingPosition.Y)
        {
            Velocity.Y *= (-1.0f * Bounce);
            SetGlobalPosition(new Vector2(GlobalPosition.X, StartingPosition.Y));

            if (Velocity.Y > -10.0f)
            {
                Velocity.X = 0.0f;
                Velocity.Y = 0.0f; 
            }
        }

        Velocity.Y += Gravity * (float)delta * Level.GetSpeed();
        GlobalPosition += Velocity * (float)delta * Level.GetSpeed();
    }
}
