using Godot;
using System;

///<summary>
/// Planned for deprecation and or major reworking
/// Holds data for what turn it is
/// Mostly for testing purposes
///</summary>
public class Battle
{
	public bool currentTurn = false;
	
	public void endTurn()
	{
		currentTurn = !currentTurn;
	}
}
