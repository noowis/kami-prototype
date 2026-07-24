using Godot;
using System;
using kamiprototype.Scripts;


/// <summary>
/// Handles the UI so that it updates and holds the log
/// </summary>
public partial class UIHandler : Node
{
    private Log _log = new();
    Stats stats = new Stats();
	
    private RichTextLabel _enemyHealth;
    private RichTextLabel _playerHealth;
    private RichTextLabel _logDisplay;
    private RichTextLabel _turnIndicator;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _enemyHealth = GetNode<RichTextLabel>("HealthEnemy");
        _playerHealth = GetNode<RichTextLabel>("HealthPlayer");
        _logDisplay = GetNode<RichTextLabel>("LogDisplay");
        _turnIndicator = GetNode<RichTextLabel>("TurnIndicator");
        stats = GetNode<Stats>("Stats");
    }
    
    /// <summary>
    /// Updates ui elements with current stats
    /// </summary>
    public void Update()
    {
        _playerHealth.Text = stats.pHealth.ToString();
        _enemyHealth.Text = stats.eHealth.ToString();
        _turnIndicator.Text = stats.battle.currentTurn ? "Player Turn" : "Enemy Turn";
        _logDisplay.Text = _log.ToString();
    }
    /// <summary>
    /// Adds an entry to the log
    /// </summary>
    /// <param name="entry">Entry dialogue</param>
    public void AddLogEntry(string entry)
    {
        _log.AddEntry(entry);
    }
}
