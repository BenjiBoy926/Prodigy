using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Triggers the specified monologue at the start of the scene,
// and triggers an event when it finishes
public class MonologueManager : MonoBehaviour 
{
	[SerializeField]
	private Monologue monologue;	// The monologue to be trigggered at the start of the scene
	[SerializeField]
	private float waitTime;	// Time after starting to wait before starting the monologue
	[SerializeField]
	private string sceneName;	// Name of the scene to load
	[SerializeField]
	private PlayerFirst action;	// Action completed by the player when the monologue finishes

	void Start ()
	{
		StartCoroutine (WaitInvokeMonologue ());
		monologue.SubscribeOnFinished (FinishMonologueAndAction);
	}

	// Start monologue after waiting for wait time seconds
	IEnumerator WaitInvokeMonologue ()
	{
		yield return new WaitForSeconds (waitTime);
		monologue.StartMonologue ();
	}

	// Load the scene specified and complete the specified action
	public void FinishMonologueAndAction ()
	{
		SceneManager.LoadScene (sceneName);
		DataManager.CompleteFirst (action);
	}
}
