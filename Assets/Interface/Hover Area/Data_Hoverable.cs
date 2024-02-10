using Godot;
using System;

[GlobalClass]
public partial class Data_Hoverable : Resource
{
    [Export] public String DisplayName = "";
    [Export] public Texture2D Icon = null;
    [Export(PropertyHint.MultilineText)] public String Description { get; protected set; }
}
