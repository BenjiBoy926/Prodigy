using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Complete an action when the script is destroyed
public class CompleteOnDestroy : MonoBehaviour 
{
	[SerializeField]
	private PlayerFirst action;	// Which action will this be the first time the player does it?

	void OnDestroy ()
	{
		DataManager.CompleteFirst (action);
	}
}
