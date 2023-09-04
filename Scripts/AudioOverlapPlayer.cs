using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOverlapPlayer : MonoBehaviour
{
	public float soundToPlay = -1.0f; //this with designate which sound to play. -1 means don’t play any sound. This is a float because then the animation window can access it.
	public AudioClip[] audioClips; //this holds the sounds

	AudioSource audio;//for holding the audio source

	void Start () {
		//put this in start. This gets the audio source.
		audio = GetComponent<AudioSource>();
	}

	void Update () {
	//put the following in update
		if (soundToPlay > -1.0f) {//if the sound is greater than the value for not playing a sound
			PlaySound((int) soundToPlay, 1);//play the sound, casting the float to an int so that the audio source can use it
			soundToPlay = -1.0f;//set it back to zero to keep this from looping back around and playing the sound again.
		}
	}

	void PlaySound(int clip, float volumeScale){
		audio.PlayOneShot(audioClips[clip], volumeScale);//play the sound with the designated clip and volume scale
	}
}