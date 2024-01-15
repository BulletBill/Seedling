using Godot;
using System;
using Godot.Collections;

public partial class LevelSelectMenu : Node
{
    [Export] public Array<String> Levels = new();

    public void LoadLevel(int Index)
    {
        if (Index < 0 || Index > Levels.Count -1) return;

        SceneTransition.ChangeScene(Levels[Index]);
    }
}
