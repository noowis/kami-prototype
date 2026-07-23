using Godot;
using System;
using kamiprototype.Scripts;

public partial class UIHandler : Node
{
    private Log _log = new();

    Stats stats = new Stats();
	
    private RichTextLabel enemyHealth;
    private RichTextLabel playerHealth;
    private RichTextLabel logDisplay;
    private RichTextLabel turnIndicator;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        enemyHealth = GetNode<RichTextLabel>("HealthEnemy");
        playerHealth = GetNode<RichTextLabel>("HealthPlayer");
        logDisplay = GetNode<RichTextLabel>("LogDisplay");
        turnIndicator = GetNode<RichTextLabel>("TurnIndicator");
        stats = GetNode<Stats>("Stats");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Update()
    {
        playerHealth.Text = stats.pHealth.ToString();
        enemyHealth.Text = stats.eHealth.ToString();
        turnIndicator.Text = stats.battle.currentTurn ? "Player Turn" : "Enemy Turn";
        logDisplay.Text = _log.ToString();
    }
    public void AddLogEntry(string entry)
    {
        _log.AddEntry(entry);
    }
}
