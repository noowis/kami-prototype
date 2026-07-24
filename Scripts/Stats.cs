using Godot;
using System;
///<summary>
/// PLANNED FOR DEPRICATION
/// Stats holds the stats of both player and enemy
/// Mostly uused for testing purposes
///</summary>
public partial class Stats : Node
{
    public Unit player = new Unit("Kami", 69420,
        50000,
        500, 
        3,
        200,
        10,
        200,
        20000,
        20,
        true);
    
    public Unit enemy = new Unit("FREAK", 67,
        50000,
        500, 
        3,
        200,
        10,
        200,
        20000,
        20,
        false);

    public int turn = 0;
    public Battle battle = new();
}
