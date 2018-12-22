using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manages the main menu screen
public class MainMenuManager : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		// If the player has never played before, 
		// skip the main menu and go straight to the prologue
		if (!(DataManager.Data.PlayedBefore)) {
			SceneManager.LoadScene ("Prologue");
		}
	}

	// Method called when the play button is pressed
	public void Play ()
	{
		if (!(DataManager.Data.CompletedLevels.Contains (true))) {
			SceneManager.LoadScene ("Level1");
		} else {
			SceneManager.LoadScene ("StageSelect");
		}
	}
}
