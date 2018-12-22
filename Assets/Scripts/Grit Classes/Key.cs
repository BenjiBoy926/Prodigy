using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CLASS Key : MonoBehaviour
// Defaults the current node to a plain node when it leaves
// It's up to the player to know and increase the amount of keys when this component is recovered
public class Key : MonoBehaviour 
{
	[SerializeField]
	private AudioClip keyCollectClip;	// clip played when the key is collected

	// Leave the node this key is in by making it a plain node and destroying its child object
	public void LeaveNode (Node myNode)
	{
		myNode.DefaultToType (NodeType.Plain);
		SoundPlayer.Instance.PlaySoundEffect (keyCollectClip);
		Destroy (gameObject);
	}
}
