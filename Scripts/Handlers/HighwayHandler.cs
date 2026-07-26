using System.Collections.Generic;
using Godot;
using kamiprototype.Scripts.GameplayClasses;

namespace kamiprototype.Scripts.Handlers;

/// <summary>
/// Highway handler spawns notes and loads sequences of notes to play. Planned to also handle notes being hit properly
/// </summary>
// TODO: Move note hitting to HighwayHandler through signal instead of Note?
public partial class HighwayHandler : Node
{
	
	private Queue<Sequence> _sequenceQueue = new Queue<Sequence>();
	public bool InPlay = false;
	private Timer _timer = null;
	private int _seqPos = 0;
	private char _button = ' ';
	private Sequence _currSeq = null;
	private PackedScene _noteScene = null;
	private Note _currNote = null;
	private Vector2 _spawnA;
	private Vector2 _spawnB;
	
	public void LoadSequence(Sequence sequence)
	{
		_sequenceQueue.Enqueue(sequence);
	}

	public void Start()
	{
		GD.Print("1");
		InPlay = true;
		_seqPos = 0;
		_button = ' ';
		_currSeq = _sequenceQueue.Dequeue();
		_timer.Start(_currSeq._timing[_seqPos]);

	}

	private void SendNote()
	{
		_currNote = (Note) _noteScene.Instantiate();
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

	private void StartNextSequence()
	{
		GD.Print("2");
		GD.Print("seqCount: " + _sequenceQueue.Count);
		if(_sequenceQueue.Count > 1)
		{
			GD.Print("3");
			_currSeq = _sequenceQueue.Dequeue();
			_sequenceQueue.Clear();
		}
		else
		{
			InPlay = false;
			_timer.Stop();
		}
	}

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Ready");
		_timer = GetNode<Timer>("Timer");
		GD.Print("Loaded");
		_timer.Timeout += OnTimerTimeout;
		_noteScene = GD.Load<PackedScene>("res://Spawnables/note.tscn");
		_spawnA = GetNode<Area2D>("SpawnA").Position;
		_spawnB = GetNode<Area2D>("SpawnB").Position;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_timer.TimeLeft != 0)
			GD.Print(_timer.TimeLeft);
	}

}