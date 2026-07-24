using System;
using System.Collections.Generic;

namespace kamiprototype.Scripts;

public class Log
{
	/// <summary>
	/// Log refers to the displayed log for combat turns.
	/// Each time you want to add a new entry, you can use
	/// <code>
	/// AddEntry(Message);
	/// </code>
	/// Please note: You also need to update the UIHandler
	/// </summary>
	
	private LinkedList<String> log = new();
	public int MaxLength = 10;
	
	/// <summary>
	/// AddEntry adds an entry to the end of the log, if log is greater than MaxLength,
	/// the log will be truncated.
	/// </summary>
	/// <param name="msg">Message</param>
	public void AddEntry(string msg)
	{
		if (log.Count < MaxLength)
		{
			log.AddLast(msg);
		}
		else
		{
			log.AddLast(msg);
			log.RemoveFirst();
		}
	}

	public override string ToString()
	{
		string str = "";
		foreach (String s in log)
		{
			str += s + "\n";
		}

		return str;
	}
}
