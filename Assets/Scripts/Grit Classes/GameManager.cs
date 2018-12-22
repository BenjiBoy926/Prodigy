using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	private static GameManager instance;	// Reference to the only instance of game manager in the scene
	private FadingElement sceneFade;	// UI element that produces a fade-in fade-out effect
	private static bool gameOver;	// True if the game has ended

	// Central dictionary of the stage conveniently stores every node on the board in association
	// with its grid position relative to the starting node
	public static Dictionary <Vector2Int, Node> nodes;

	[SerializeField]
	private int currentLvl;	// Current level of the game
	[SerializeField]
	private float loadTime;	// Time after game over begins that next stage is loaded

	public static GameManager Instance { get { return instance; } }
	public static bool GameOver { get { return gameOver; } }

	void Awake ()
	{
		nodes = new Dictionary<Vector2Int, Node> ();
		nodes.Clear ();
		GameManager.instance = this;
		GameManager.gameOver = false;
		sceneFade = GameObject.FindGameObjectWithTag ("SceneFader").GetComponent <FadingElement> ();
	}

	// Start the process of game over
	public void EndGame (bool movingOn)
	{
		gameOver = true;

		// If we're moving on, load the next stage
		if (movingOn) {
			sceneFade.Fade (true);
			Invoke ("LoadNextScene", loadTime);
		}
		// Otherwise, reload this current scene
		else {
			Invoke ("ReloadScene", loadTime);
		}
	}

	// Reload the current scene
	private void ReloadScene ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	// Load the next level if there is one, or load the main menu
	private void LoadNextScene ()
	{
		int nextLvl;	// Level after this level
		nextLvl = currentLvl + 1;
		DataManager.CompleteLevel (currentLvl);

		// If we are still within the total levels,
		// load the next level
		if (nextLvl <= DataManager.TotalLevels) {
			SceneManager.LoadScene ("Level" + nextLvl);
		}
		// Otherwise, load the main menu
		else {
			// If the player has finished the game before, load the main menu
			if (DataManager.Data.FinishedBefore) {
				SceneManager.LoadScene ("MainMenu");
			}
			// Otherwise, load the epilogue
			else {
				SceneManager.LoadScene ("Epilogue");
			} // END if-else finished before
		} // END if-else this is the last level
	} // END method
}
