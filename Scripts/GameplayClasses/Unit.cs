using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Class for units used in combat
/// </summary>
public class Unit
{
    public String name;
    public int ID;
    
    public Action attackCommand;
    public List<Action> possibleActions;

    public int maxHealth;
    public int currentHealth;
    
    public int maxMana;
    public int currentMana;
    
    public int maxStocks;
    public int currentStocks;

    public float multiplier = 0;

    public int attack;
    public int defense;
    public int magic;

    public int experience;
    public int level;

    public bool isPlayable;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="maxHealth">Maximum HP</param>
    /// <param name="maxMana">Maximum MP</param>
    /// <param name="maxStocks">Maximum stats a playable can use</param>
    /// <param name="attack">Attack stat</param>
    /// <param name="defense">Defense stat</param>
    /// <param name="magic">Magic stat</param>
    /// <param name="experience">Playable: current experience, Non-playable: experience given</param>
    /// <param name="level">Current Level</param>
    /// <param name="isPlayable">Whether or not you can control the unit</param>
    public Unit(String name, int ID, int maxHealth, int maxMana, int maxStocks, int attack, int defense, int magic, int experience, int level, bool isPlayable)
    {
        this.name = name;
        this.ID = ID;
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
        this.maxMana = maxMana;
        currentMana = this.maxMana;
        this.maxStocks = maxStocks;
        currentStocks = 0;
        this.attack = attack;
        this.defense = defense;
        this.magic = magic;
        this.experience = experience;
        this.level = level;
    }

    public override string ToString()
    {
        String str = "";
        str += "Name: " + name + " [" + ID + "]" + "\n";
        str += "Level" + level + "\n";
        str += "HP: " + currentHealth + "/" + maxHealth + "\n";
        str += "MP: " + currentMana + "/" + maxMana + "\n";
        str += "STOCK: " + currentStocks + "/" + maxStocks + "\n";
        str += "Attack: " + attack + "\n";
        str += "Defense: " + defense + "\n";
        str += "Magic: " + magic + "\n";
        str += "Experience: " + experience;
        return str;
    }
}
