using Godot;
using System;

public partial class Currency : Node2D
{
    [Export] public String DisplayName;
    [Export] public Texture2D IconLarge;
    [Export] public Texture2D IconSmall;
    [Export] public bool Consumed;
    public int HeldAmount;
    public int MaximumAmount;
}
