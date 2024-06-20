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

    public Data_Hoverable() {}

    public Data_Hoverable(Data_Hoverable Other)
    {
        DisplayName = Other.DisplayName;
        Icon = Other.Icon;
        Description = Other.Description;
    }

    public Data_Hoverable(String NewName, Texture2D NewIcon, String NewDescription)
    {
        DisplayName = NewName;
        Icon = NewIcon;
        Description = NewDescription;
    }
}