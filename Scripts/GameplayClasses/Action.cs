using Godot;
using System;
using kamiprototype.Scripts.GameplayClasses;

/// <summary>
/// Defines the standard action a unit can take on their turn
/// </summary>
public abstract class Action
{
    public abstract int ExecuteAction();
    
    public int mpCost;
    public int hpCost;
    public Sequence HitSequence;
    public int StockCost;
    

}
