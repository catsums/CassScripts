using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using SignalBusNS;

/// <summary>
/// An audio controller which supports multiple audio sources playing simultaneous with their own clips, without needing to pick an exact source
/// </summary>
public class AudioCtrl : MonoBehaviour
{

	public SignalBus signalBus = new SignalBus();
	public static class Signals{
		public const string 
		AudioStarted = "audioStart",
		AudioPlay = "audioPlay",
		AudioPause = "audioPause",
		AudioStop = "audioStop"
		;
	}

	[Serializable]
	public class AudioSample{
		public string name;
		public AudioClip clip;
		[Range(0.0f, 1.0f)]
		public float volume = 1f;
	}

	public List<AudioSample> audioClips = new List<AudioSample>();

	public AudioMixer audioMixer;

	public List<AudioSource> audioSources = new List<AudioSource>();
	public float volume = 1f;
	public float pitch = 1f;
	public float tempo = 1f;

	protected virtual void Start(){
		
	}

	protected virtual void Update()
	{
		if(GameManager.PlayState.isPaused) return;
		CheckAudioSources();
	}

	/// <summary>
	/// Check if there are any running audio sources to process per update
	/// </summary>
	protected virtual void CheckAudioSources(){
		var sources = audioSources.ToArray();

		foreach(var source in sources){
			if(source.time >= (source.pitch * source.clip.length)){
				if(!source.loop){
					StopAudio(source);
				}
			}
		}
	}

	/// <summary>
	/// Get the audio clip by name (registered on the object with the controller).
	/// </summary>
	/// <param name="name">Registered name for the Audio Sample object</param>
	/// <returns>The AudioClip resource</returns>
	public AudioClip GetAudioClipByName(string name){
		var sample = audioClips.Find((x)=>(x.name == name) );
		if(sample!=null){
			return sample.clip;
		}
		return null;
	}

	/// <summary>
	/// Adds an audio source component to the controller to be usable for multi-stream playing
	/// </summary>
	/// <returns>The new audiosource component</returns>
	protected virtual AudioSource AddAudioSource(){
		var sources = GetComponents<AudioSource>();
		foreach(var src in sources){
			if(!audioSources.Contains(src)){
				audioSources.Add(src);
				return src;
			}
		}
		var source = gameObject.AddComponent<AudioSource>();
		audioSources.Add(source);

		source.loop = false;

		return source;
	}

	/// <summary>
	/// Stops an audio playing on a specific audio source
	/// </summary>
	/// <param name="sourceIndex">Index on the controller list</param>
	/// <returns>The specific audio source on that index</returns>
	public virtual AudioSource StopAudio(int sourceIndex = 0){

		AudioSource source;
		if(sourceIndex < 0 || sourceIndex >= audioSources.Count){
			return null;
		}

		source = audioSources[sourceIndex];
		StopAudio(source);

		return source;

	}
	/// <summary>
	/// Stops an audio playing on a specific audio source
	/// </summary>
	/// <param name="source">The audio source that is playing the audio</param>
	/// <returns>Returns true if the source has been successfully stopped</returns>
	public virtual bool StopAudio(AudioSource source){
		if(!source) return false;

		var exists = audioSources.Remove(source);
		source.Stop();
		source.enabled = false;

		signalBus.Emit(Signals.AudioStop, source);

		return exists;

	}

	/// <summary>
	/// Plays an audio clip in the controller for a source to play
	/// </summary>
	/// <param name="clipname">Registered name for the Audio Sample object</param>
	/// <param name="sourceIndex">Index on the controller list. If < 0, it will use any source that is not currently playing anything.</param>
	/// <returns>Returns the AudioSource that will play the audio</returns>
	public virtual AudioSource PlayAudio(string clipname, int sourceIndex = -1, bool loop = false){
		// var sample = GetAudioClipByName(clipname);
		var sample = audioClips.Find(match: (x)=>(x.name == clipname) );
		var volume = sample != null ? sample.volume : 0;

		return PlayAudio(sample?.clip, sourceIndex, loop, volume);
	}

	/// <summary>
	/// Plays an audio clip in the controller for a source to play
	/// </summary>
	/// <param name="clip">A specific audio clip asset</param>
	/// <param name="sourceIndex">Index on the controller list. If < 0, it will use any source that is not currently playing anything.</param>
	/// <returns>Returns the AudioSource that will play the audio</returns>
	public virtual AudioSource PlayAudio(AudioClip clip, int sourceIndex = -1, bool loop = false, float volume = 0.5f){
		AudioSource source;
		if(sourceIndex < 0){
			source = AddAudioSource();
		}else if(sourceIndex >= audioSources.Count){
			return null;
		}else{
			source = audioSources[sourceIndex];
		}

		PlayAudio(clip, source, loop, volume);

		return source;
	}

	/// <summary>
	/// Plays an audio clip in the controller for a source to play
	/// </summary>
	/// <param name="clipname">Registered name for the Audio Sample object</param>
	/// <param name="source">The audio source that is playing the audio</param>
	/// <returns>Returns true if the source has been successfully played</returns>
	public virtual bool PlayAudio(string clipname, AudioSource source, bool loop = false){
		// var sample = GetAudioClipByName(clipname);
		var sample = audioClips.Find(match: (x)=>(x.name == clipname) );
		var volume = sample != null ? sample.volume : 0;

		return PlayAudio(sample?.clip, source, loop, volume);
	}

	/// <summary>
	/// Plays an audio clip in the controller for a source to play
	/// </summary>
	/// <param name="clip">A specific audio clip asset</param>
	/// <param name="source">The audio source that is playing the audio</param>
	/// <returns>Returns true if the source has been successfully played</returns>
	public virtual bool PlayAudio(AudioClip clip, AudioSource source, bool loop = false, float volume = 0.5f){
		if(!clip || !source) return false;
		
		source.enabled = true;
		source.Stop();
		source.clip = clip;
		source.Play();
		source.loop = loop;
		source.volume = volume * this.volume;

		signalBus.Emit(Signals.AudioStarted, source);

		return true;
	}
	
	/// <summary>
	/// Resume an audio source in the controller to play an audio
	/// </summary>
	/// <param name="sourceIndex">Index on the controller list.</param>
	/// <returns>The audio source which is meant to be resumed</returns>
	public virtual AudioSource Play(int sourceIndex){
		if(sourceIndex < 0 || sourceIndex >= audioSources.Count){
			return null;
		}

		var source = audioSources[sourceIndex];

		Play(source);

		return source;
	}
	/// <summary>
	/// Resume an audio source in the controller to play an audio
	/// </summary>
	/// <param name="source">The audio source that is playing the audio</param>
	/// <returns>Returns true if it got successfully resumed</returns>
	public virtual bool Play(AudioSource source){
		if(!source) return false;
		
		if(source.isPlaying){
			return false;
		}
		source.Play();
		signalBus.Emit(Signals.AudioPlay, source);

		return true;
	}

	/// <summary>
	/// Resume all audio sources linked to this controller
	/// </summary>
	/// <returns>Returns true if they can be successfully played</returns>
	public virtual bool PlayAll(){
		var check = false;
		foreach(var src in audioSources){
			if(Play(src)){
				check = true;
			}
		}
		return check;
	}

	/// <summary>
	/// Pause an audio source in the controller to play an audio
	/// </summary>
	/// <param name="sourceIndex">Index on the controller list.</param>
	/// <returns>The audio source which is meant to be paused</returns>
	public virtual AudioSource Pause(int sourceIndex){
		if(sourceIndex < 0 || sourceIndex >= audioSources.Count){
			return null;
		}

		var source = audioSources[sourceIndex];

		Pause(source);

		return source;
	}
	/// <summary>
	/// Paused an audio source in the controller to play an audio
	/// </summary>
	/// <param name="source">The audio source that is playing the audio</param>
	/// <returns>Returns true if it got successfully paused</returns>
	public virtual bool Pause(AudioSource source){
		if(!source) return false;
		
		if(!source.isPlaying){
			return false;
		}
		source.Pause();
		signalBus.Emit(Signals.AudioPause, source);

		return true;
	}

	/// <summary>
	/// Pause all audio sources linked to this controller
	/// </summary>
	/// <returns>Returns true if they can be successfully paused</returns>
	public virtual bool PauseAll(){
		var check = false;
		foreach(var src in audioSources){
			if(Pause(src)){
				check = true;
			}
		}
		return check;
	}

	/// <summary>
	/// Check if AudioSource in the controller is playing
	/// </summary>
	/// <param name="sourceIndex">Index on the controller list.</param>
	public virtual bool IsPlaying(int sourceIndex = 0){
		if(sourceIndex < 0 || sourceIndex >= audioSources.Count){
			return false;
		}

		var source = audioSources[sourceIndex];

		return source.isPlaying;
	}
	public virtual bool IsPlaying(AudioSource source){
		if(!source) return false;

		return source.isPlaying;
	}

}