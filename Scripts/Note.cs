using Godot;
using System;

/// <summary>
/// Class for an individual note on the highway, checks where it is and moves it
/// </summary>
public partial class Note : Area2D
{
	private float _speed = 200.0f;
	private bool _inZone = false;
	public char Button = ' ';
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveLocalX(_speed * (float) delta, false );
		if (_inZone & (Button == 'a' && Input.IsActionPressed("NoteHitA")) 
		    || (Button == 'b' && Input.IsActionPressed("NoteHitB")))
		{
			GD.Print("NoteHitA");
			QueueFree();
		}
	}
	
	// TODO: SETUP a signal so that it sends it to HighwayHandler when note is pressed.
	
	
	/* TODO: Might wanna change this so that it gives a little leeway?
	 That way if a player hits a note late, it won't count as the next note being hit early */
	private void OnHitZoneEntered(Area2D area)
	{
		if (area.Name == "HitZone")
		{
			_inZone = true;
		}

		if (area.Name == "MissZone")
		{
			QueueFree();
		}
	}

}
