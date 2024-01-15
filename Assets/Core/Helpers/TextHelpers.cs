using Godot;
using System;

public partial class TextHelpers
{
    public static T ParseEnum<T>(String value)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
    }

    public static String Icon(String Icon)
    {
        return "[img]res://Assets/Art/Icons/" + Icon + ".png[/img]";
    }

    public static String Colorize(String text, Color color)
    {
        return "[color=" + color.ToHtml() + "]" + text + "[/color]";
    }
}
