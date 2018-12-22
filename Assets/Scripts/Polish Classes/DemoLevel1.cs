using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple class that manages UI objects that tell the player how to play
public class DemoLevel1 : MonoBehaviour 
{
	[SerializeField]
	private Enemy distractedEnemy;	// Changed controls displayed based on if this enemy is distracted
	[SerializeField]
	private GameObject promptDistract;	// Game object prompts player to distract the enemy
	[SerializeField]
	private GameObject onDistract;	// Game object gives a different prompt when enemy is distracted

	// Activate distract prompt when enemy is not distracted
	// Activate on distract prompt when they are
	void Update ()
	{
		promptDistract.SetActive (!distractedEnemy.Distracted);
		onDistract.SetActive (distractedEnemy.Distracted);
	}
}
