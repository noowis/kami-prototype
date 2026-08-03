using Godot;
using System;
using System.Collections.Generic;
using kamiprototype.Scripts.Handlers;
using static Battle;

///<summary>
/// Handles the interactions with the UI done by user.
///</summary>
public partial class InteractionHandler : Node
{
    Stats stats = new Stats();
    UIHandler ui = new UIHandler(); // ui is used to update hud elements.
    HighwayHandler highway = new HighwayHandler();
    private BasicAttack _basicAttack = new BasicAttack();
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ui = GetNode<UIHandler>("UIHandler");
        stats = GetNode<Stats>("UIHandler/Stats");
        highway = GetNode<HighwayHandler>("Highway");
    }

    ///<summary>
    /// This function is called whenever an item in ItemList is clicked.
    ///</summary>
    private void OnItemClicked(int index, Vector2 at_position, int mouse_button)
    {
        if(index == 0)	// End Turn
        {
            ui.AddLogEntry("Turn ended!");
            stats.battle.endTurn();
            ui.Update();
        }
        if (index == 1)	// Attack
        {
            stats.turn++;
            int totalDamage = 0;
            if (stats.battle.currentTurn)
            {
                totalDamage = stats.enemy.attack - stats.player.defense;
                stats.player.currentHealth -= totalDamage;
            }
            else
            {
                totalDamage = stats.player.attack - stats.enemy.defense;
                stats.enemy.currentHealth -= totalDamage;
            }
            ui.AddLogEntry((stats.battle.currentTurn ? "Player" : "Enemy") + " took " + totalDamage + " damage on turn " + stats.turn.ToString());
            ui.Update();
        }
        if (index == 2) // Tech
        {
            _basicAttack.ExecuteAction(highway);
        }
    }
}
