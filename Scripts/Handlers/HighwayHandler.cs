using System.Collections.Generic;
using Godot;
using kamiprototype.Scripts.GameplayClasses;

namespace kamiprototype.Scripts.Handlers;

public partial class HighwayHandler : Node
{
	private Queue<Sequence> _sequenceQueue = new Queue<Sequence>();
	public bool InPlay = false;
	//private Queue<Note> _noteSequence = new Queue<Note>();
	private Timer _timer = null;
	private int _seqPos = 0;
	private char _button = ' ';
	private Sequence _currSeq = null;
	private PackedScene _noteScene = null;
	private Note _currNote = null;
	
	public void LoadSequence(Queue<Sequence> sequence)
	{
		_sequenceQueue = sequence;
	}

	public void Start()
	{
		InPlay = true;
		_seqPos = 0;
		_button = ' ';
		StartNextSequence();

	}

	private void SendNote()
	{
		_timer.WaitTime = _currSeq._timing[_seqPos];
		_currNote = (Note) _noteScene.Instantiate();
		AddChild(_currNote);
		_seqPos++;
	}

	private void OnTimerTimeout()
	{
		_seqPos++;
		if (_seqPos >= _currSeq._timing.Count)
		{
			StartNextSequence();
		}
		SendNote();
	}

	private void StartNextSequence()
	{
		if(_sequenceQueue.Count >= 1)
		{
			_currSeq = _sequenceQueue.Dequeue();
			_sequenceQueue.Clear();
		}
		else
		{
			InPlay = false;
		}
	}

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimerTimeout;
		_noteScene = GD.Load<PackedScene>("res://Spawnables/note.tscn");
	}
	
	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

}