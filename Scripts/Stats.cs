using Godot;
using System;
///<summary>
/// PLANNED FOR DEPRICATION
/// Stats holds the stats of both player and enemy
/// Mostly uused for testing purposes
///</summary>
public partial class Stats : Node
{
    public int pHealth = 100;

    public int eHealth = 100;

    public int turn = 0;
    
    public Battle battle = new();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
