using Godot;
using System;

[GlobalClass]
public partial class Data_Hoverable : Resource
{
    [Export] public String DisplayName { get; protected set; } = "";
    [Export] public Texture2D Icon { get; protected set; } = null;
    [Export(PropertyHint.MultilineText)] public String Description { get; protected set; }

    // Can be inhertied to provide modified descriptions
    public virtual String GetFullDescription()
    {
        return Description;
    }
}