using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerProfile : Node
{
    static PlayerProfile Singleton;
    public Dictionary<String, int> CampaignProgress { get; protected set; }
    public Dictionary<String, bool> Unlock { get; protected set; }

    public PlayerProfile()
    {
        Singleton = this;
    }
}


