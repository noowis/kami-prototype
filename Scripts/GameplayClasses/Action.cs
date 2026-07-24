using Godot;
using System;

/// <summary>
/// Defines the standard action a unit can take on their turn
/// </summary>
public abstract class Action
{
    public abstract int ExecuteAction();
    
    public int mpCost;
    public int hpCost;
    // For when sequences are added:
    // public Sequence HitSequence;
    public int StockCost;
    

}
