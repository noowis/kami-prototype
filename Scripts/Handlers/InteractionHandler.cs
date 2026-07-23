using Godot;
using System;
using System.Collections.Generic;
using static Battle;

public partial class InteractionHandler : Node
{
    Stats stats = new Stats();
    UIHandler ui = new UIHandler();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ui = GetNode<UIHandler>("UIHandler");
        stats = GetNode<Stats>("UIHandler/Stats");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void OnItemClicked(int index, Vector2 at_position, int mouse_button)
    {
        GD.Print(("bwuh???"));
        if(index == 0)
        {
            ui.AddLogEntry("Turn ended!");
            stats.battle.endTurn();
            ui.Update();
        }
        if (index == 1)
        {
            stats.turn++;
            if (stats.battle.currentTurn)
                stats.pHealth--;
            else
                stats.eHealth--;
            ui.AddLogEntry((stats.battle.currentTurn ? "Player" : "Enemy") + " took 1 damage on turn " + stats.turn.ToString());
            ui.Update();
        }
    }
}