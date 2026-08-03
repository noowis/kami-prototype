using Godot;
using System;
using System.Collections.Generic;

public partial class HitZone : Area2D
{
    private Queue<Note> _currentNotes = new Queue<Note>();
    
    // Use _currentNotes.Peek() to view next note
    public void Enqueue(Note note)
    {
        _currentNotes.Enqueue(note);
    }

    public void OnEarlyZoneEntered(Area2D area)
    {
        GD.Print("OnEarlyZoneEntered: " + area.Name);
    }

    public void OnMissZoneEntered(Area2D area)
    {
        GD.Print("OnMissZoneEntered: " + area.Name);
    }

    public void OnHitZoneEntered(Area2D area)
    {
        GD.Print("OnHitZoneEntered: " + area.Name);
    }
}
