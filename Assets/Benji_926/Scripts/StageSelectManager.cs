using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour 
{
	[SerializeField]
	private Transform stageHolder;	// Transform that holds the stage loaders
	[SerializeField]
	private GameObject stageLoaderPrefab;	// Prefab of the button used to load the stage specified
	[SerializeField]
	private LoreDisplay scroll;	// Reference to the scroll that displays the Sacred Wisdom

	void Start ()
	{
		InitializeStageSelectors ();
	}

	// Instantiate buttons to navigate between stages
	void InitializeStageSelectors ()
	{
		LevelLoader loader;	// Script that describes a level loading button
		bool playable;	// True if the current level loader button is interactable

		// Loop through total levels and instantiate a button for each
		for (int level = 1; level <= DataManager.TotalLevels; level++) {
			loader = Instantiate (stageLoaderPrefab, stageHolder).GetComponent <LevelLoader> ();

			// Make the button interactable if the level before it was completed,
			// or if it represents the first stage
			if (level > 1) {
				playable = DataManager.Data.CompletedLevels [level - 2];
			} else {
				playable = true;
			}

			// Initialize the loader instantiated
			loader.Initialize (level, playable, DataManager.Data.CompletedLevels [level - 1], scroll);
		} // END for loop
	} //  END method
}
