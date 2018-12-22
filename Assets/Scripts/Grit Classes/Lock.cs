using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS Lock : MonoBehaviour
 * --------------------------
 * Describes an object that locks a node and makes it untraversable
 * until it is successfully unlocked
 * --------------------------
 */ 

public class Lock : MonoBehaviour 
{
	[SerializeField]
	private int locks;	// Number of keys required to unlock this lock
	private Animator anim;	// Animator for the lock
	[SerializeField]
	private AudioClip unlockClip;	// Clip that plays when the lock is successfully unlocked
	[SerializeField]
	private AudioClip jiggleClip;	// Clip that plays when lock is jiggled

	private void Start ()
	{
		anim = GetComponent <Animator> ();
	}

	// Return true if enough keys are used to open the lock.  Otherwise return false
	public bool TryUnlock (int keys, Node myNode)
	{
		bool unlocked;	// True if unlock was successful
		unlocked = keys >= locks;

		// If enough keys are given, make the node plain and destroy this object
		if (unlocked) {
			myNode.DefaultToType (NodeType.Plain);
			Destroy (gameObject);
		}

		UnlockEffect (unlocked);
		return unlocked;
	}

	// Produces an effect depending on whether or not the
	// unlock was successful
	private void UnlockEffect (bool successful)
	{
		if (successful) {
			SoundPlayer.Instance.PlaySoundEffect (unlockClip);
		} else {
			SoundPlayer.Instance.PlaySoundEffect (jiggleClip);
			anim.SetTrigger ("Jiggle");
		}
	}
}
