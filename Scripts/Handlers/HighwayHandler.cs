using System.Collections.Generic;
using Godot;
using kamiprototype.Scripts.GameplayClasses;

namespace kamiprototype.Scripts.Handlers;

/// <summary>
/// Highway handler spawns notes and loads sequences of notes to play. Planned to also handle notes being hit properly.
/// Sequences can be strung together by using stocks. The highway handler splits these into seperate sequences within
/// the <code>_sequenceQueue</code>.
/// </summary>
// TODO: Move note hitting to HighwayHandler through signal instead of Note?

public partial class HighwayHandler : Node
{
	/// <summary>
	/// Queue for sequences stocked up by player
	/// </summary>
	private Queue<Sequence> _sequenceQueue = new Queue<Sequence>(); 

	private HitZone _hitZone;
	/// <summary>
	///		Whether or not the sequence is currently being played
	/// </summary>
	public bool InPlay = false;
	private Timer _timer = null;
	
	/// <summary>
	/// Position within current sequence
	/// </summary>
	private int _seqPos = 0;
	private char _button = ' ';
	private Sequence _currSeq = null;
	private PackedScene _noteScene = null;
	
	/// <summary>
	/// Note about to be spawned
	/// </summary>
	private Note _currNote = null;
	
	/// <summary>
	/// Spawn point for note A
	/// </summary>
	private Vector2 _spawnA;
	/// <summary>
	/// Spawn point for note B
	/// </summary>
	private Vector2 _spawnB;
	
	/// <summary>
	/// Loads assets.
	/// </summary>
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimerTimeout;
		_noteScene = GD.Load<PackedScene>("res://Spawnables/note.tscn");
		_spawnA = GetNode<Area2D>("SpawnA").Position;
		_spawnB = GetNode<Area2D>("SpawnB").Position;
		_hitZone = GetChild<HitZone>(1);
		GD.Print(_hitZone.Name);
	}
	
	/// <summary>
	/// Loads up a sequence from an action class
	/// </summary>
	/// <param name="sequence">Sequence that will be added to the queue of seqeunces</param>
	public void LoadSequence(Sequence sequence)
	{
		_sequenceQueue.Enqueue(sequence);
	}

	/// <summary>
	/// When the attack string is initiated Start() should be run. Resets _seqPos, and _button.
	/// Also starts timer, sets _currSequence and sets InPlay to true. First note will be played when timer is
	/// activated.
	/// </summary>
	public void Start()
	{
		InPlay = true;
		_seqPos = 0;
		_button = ' ';
		_currSeq = _sequenceQueue.Dequeue();
		_timer.Start(_currSeq._timing[_seqPos]);

	}
	
	/// <summary>
	/// When timer is activated it will signal this function. Which checks if _seqPos is at the end of the current
	/// sequence. as well as progressing the sequence position.
	/// </summary>
	private void OnTimerTimeout()
	{
		
		_timer.Stop();
		GD.Print("Timer Ping");
		
		if (_seqPos >= _currSeq._timing.Count)
		{
			StartNextSequence();
		}
		if(InPlay)
			SendNote();
		
		_seqPos++;
	}
	
	/// <summary>
	/// Sends a note to the correct position on the highway and starts the next timer
	/// </summary>
	private void SendNote()
	{
		_currNote = (Note) _noteScene.Instantiate();
		_hitZone.Enqueue(_currNote);
		if (_currSeq.Button[_seqPos] == 'a')
		{
			_currNote.Button = 'a';
			_currNote.Position = _spawnA;
		}
		else if (_currSeq.Button[_seqPos] == 'b')
		{
			_currNote.Button = 'b';
			_currNote.Position = _spawnB;
		}
		AddChild(_currNote);
		_timer.Start(_currSeq._timing[_seqPos]);
	}


	/// <summary>
	/// Starts the next sequence and if there are no more it will clear the queue.
	/// </summary>
	private void StartNextSequence()
	{
		if(_sequenceQueue.Count > 1)
		{
			_currSeq = _sequenceQueue.Dequeue();
		}
		else
		{
			InPlay = false;
			_timer.Stop();
			_sequenceQueue.Clear();
		}
	}

	

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}