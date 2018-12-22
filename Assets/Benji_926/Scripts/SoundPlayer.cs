using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * CLASS SoundPlayer : MonoBehaviour
 * ---------------------------------
 * The central script from which all sound is played.
 * Designed to survive scene loading so that music plays
 * continuously through similarly-themed stages
 * ---------------------------------
 */ 

public class SoundPlayer : MonoBehaviour 
{
	private static SoundPlayer instance;	// Reference to the single sound player active in every scene

	[SerializeField]
	private List<AudioSource> musicChannels;	// Audio sources that play continuous looping music
	[SerializeField]
	private List<AudioSource> sfxChannels;	// List of audio source channels that all play sound effects
	private MusicTheme theme;	// Music theme currently being played by the sound player

	public static SoundPlayer Instance { get { return instance; } }
	public MusicTheme Theme { get { return theme; } }

	// Initialize the player by setting the singleton instance and
	// setting current music theme to unassigned
	public void Initialize ()
	{
		// If no sound object has yet been assigned, make this
		// the sound object and make sure it isn't destroyed on load
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		}
		// Otherwise, if sound already exists but this isn't it, self-destruct
		else if (instance != this) {
			Destroy (gameObject);
		}

		// Initialize music theme to not yet assigned
		theme = MusicTheme.Unassigned;
	}

	// Plays the given music clip of the specified theme
	public void PlayMusicOfTheme (MusicTheme newTheme, List<AudioClip> newMusic)
	{
		theme = newTheme;

		// Go through the list of music channels and play the list of new music clips on each one
		for (int index = 0; index < musicChannels.Count; index++) {
			musicChannels [index].Stop ();

			if (index < newMusic.Count) {
				musicChannels [index].clip = newMusic [index];
				musicChannels [index].Play ();
			}
		}

		// Log a warning if you specified too many new music audio clips
		if (newMusic.Count > musicChannels.Count) {
			Debug.LogWarning ("You specified too many music clips for the sound player to play");
		}
	}

	// Search for an audio source that isn't playing and play the clip on it
	public void PlaySoundEffect (AudioClip effect)
	{
		int index;	// Current index in list of sound effects
		bool clipPlayed;	// True if the clip was successfully played

		// Initialize loop control vars
		index = 0;
		clipPlayed = false;

		// Loop until the effect is successfully played,
		// or all audio sources have been checked
		while (index < sfxChannels.Count && !clipPlayed) {
			// If the current audio source isn't playing,
			// play the effect on it
			if (!(sfxChannels [index].isPlaying)) {
				sfxChannels [index].clip = effect;
				sfxChannels [index].Play ();
				clipPlayed = true;
			}

			// Increment index before moving on
			index++;
		}

		// If the clip still hasn't been played after the loop,
		// play it on the first source
		if (!clipPlayed) {
			sfxChannels [0].clip = effect;
			sfxChannels [0].Play ();
			Debug.LogWarning ("You ran out of sound effects channels to play the effects in the scene");
		}
	}

	// Randomly select an audio clip to play from a list of clips
	public void PlayRandomEffect (List<AudioClip> clips)
	{
		int selection;	// Random selection from the list of clips
		selection = Random.Range (0, clips.Count);
		PlaySoundEffect (clips [selection]);
	}

	// Specify a sound type and specify if it should be enabled or disabled
	public void ToggleSoundType (bool enabled, SoundType type)
	{
		List<AudioSource> modChannels;	// List of channels to be modified

		// Set list of channels to modify based on sound type specified
		if (type == SoundType.Music) {
			modChannels = musicChannels;
		} else {
			modChannels = sfxChannels;
		}

		// Mute/unmute each channel in the list of channels to modify
		foreach (AudioSource channel in modChannels) {
			channel.mute = !enabled;
		}
	}

	// Specify a sound type and specify a new volume for it
	public void AdjustVolumeOnSoundType (float newVolume, SoundType type)
	{
		List<AudioSource> modChannels;	// List of channels to be modified

		// Set list of channels to modify based on sound type specified
		if (type == SoundType.Music) {
			modChannels = musicChannels;
		} else {
			modChannels = sfxChannels;
		}

		// Mute/unmute each channel in the list of channels to modify
		foreach (AudioSource channel in modChannels) {
			channel.volume = newVolume;
		}
	}
}

public enum MusicTheme
{
	Unassigned,
	Level,
	Menu
}

public enum SoundType
{
	Music,
	Effects
}