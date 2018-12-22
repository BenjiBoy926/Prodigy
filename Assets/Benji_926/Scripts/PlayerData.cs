using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * CLASS PlayerData
 * ----------------
 * Contains data that needs to be remembered between play sessions
 * Instances of this class are regularly stored on the machine via
 * the DataManager
 * ----------------
 */ 
[Serializable]
public class PlayerData
{
	[SerializeField]
	private List<bool> completedLevels;	// List of bools that reads true if the corresponding level has been completed
	[SerializeField]
	private bool playedBefore;	// True if this is the player has played the game before
	[SerializeField]
	private bool finishedBefore;	// True if the player has finished the game before

	// Properties make variables read-only
	public List<bool> CompletedLevels { get { return completedLevels; } }
	public bool PlayedBefore { get { return playedBefore; } }
	public bool FinishedBefore { get { return finishedBefore; } }

	// Constructor initializes data as if this was the player's first time playing
	public PlayerData (int totalLevels)
	{
		completedLevels = new List<bool> ();
		playedBefore = false;
		finishedBefore = false;

		for (int index = 0; index < totalLevels; index++) {
			completedLevels.Add (false);
		}
	}

	// Complete the level specified
	public void CompleteLevel (int level)
	{
		completedLevels [level - 1] = true;
	}

	// Complete first play or first finish
	public void CompleteFirst (PlayerFirst first)
	{
		if (first == PlayerFirst.Play) {
			playedBefore = true;
		} else {
			finishedBefore = true;
		} // END if first
	} // END method
}

public enum PlayerFirst
{
	Play,
	Finish
}