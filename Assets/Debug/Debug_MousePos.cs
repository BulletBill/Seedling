using Godot;
using System;

public partial class Debug_MousePos : RichTextLabel
{
    public override void _Process(double delta)
    {
        Vector2I Pos = (Vector2I)CameraController.GetMousePosition();

        Text = TextHelpers.Center(Pos.ToString());
        if (CameraController.MouseIsOverUIPanel())
        {
            Text = TextHelpers.Colorize(Text, Colors.Red);
        }
    }
}
