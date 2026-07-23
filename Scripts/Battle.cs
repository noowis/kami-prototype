using Godot;
using System;

public class Battle
{
    public bool currentTurn = false;
	
    public void endTurn()
    {
        GD.Print(currentTurn ? "It's true baybee" : "not so much"); //Man
        currentTurn = !currentTurn;
    }
}