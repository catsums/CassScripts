using System;
using System.Collections.Generic;
using UnityEngine;

using GameObjectExt;

using SignalBusNS;

/// <summary>
/// TimerComp lets you add a Timer to any GameObject to run a certain time on the inspector or control a timer visually within code.
/// This is different from coroutines, and this makes use of a signalbus to run a function after a certain time.
/// This also relies on the game process than C# process time.
/// </summary>


public class TimerComp : MonoBehaviour{
	public SignalBus signalBus = new SignalBus();

	public enum TimeProcessMode {
		IDLE, FIXED, UNSCALED
	}

	public static class Signals{
		/// <summary>When Timer reaches timeout or when it repeats/resets if oneshot is disabled</summary>
		public const string Timeout = "timeout";
		/// <summary>When Timer updates per step depending on the Process Mode</summary>
		public const string Process = "process";
	}

	///<summary> Check if timer is paused </summary>
	public bool paused = false; 
	///<summary> Check if timer is only running once and not repeating</summary>
	public bool oneshot = true;
	///<summary> Check if timer is to start when enabled</summary>
	public bool autostart = false;

	/// <summary>Total time for timer overall</summary>
	public float waitTime = 1f;
	/// <summary>Amount of time left for timer to end</summary>
	public float timeLeft = 0f;

	/// <summary> Process mode for timer that checks which update mode it is using at runtime </summary>
	public TimeProcessMode processMode = TimeProcessMode.IDLE; 

	/// </summary>
	
	/// On Start, timer will check to autostart timer
	/// </summary>

	void Start(){
		if(autostart){
			StartTimer();
		}else{
			PauseTimer();
		}
	}

	///</summary>
	/// On Update, if on IDLE mode, the timer will countdown its timer based on the deltatime
	/// </summary>
	
	void Update(){
		if(processMode != TimeProcessMode.IDLE && processMode != TimeProcessMode.UNSCALED) return;

		var delta = Time.deltaTime;
		if(processMode == TimeProcessMode.UNSCALED){
			delta = Time.unscaledDeltaTime;
		}
		ProcessCountDown(delta);
	}
	/// <summary>
	/// On Update, if on FIXED mode, the timer will countdown based on the game FPS deltatime
	/// </summary>
	void FixedUpdate(){
		if(processMode != TimeProcessMode.FIXED) return;

		var delta = Time.fixedDeltaTime;
		ProcessCountDown(delta);
	}

	/// <summary>
	/// This starts the timer based on the waittime, which will be the duration of the timer
	/// </summary>

	public void StartTimer(){
		StartTimer(waitTime);
	}

	/// <summary>
	/// This starts timer with a newly set duration
	/// </summary>
	/// <param name="time">New total wait time duration</param>
	public void StartTimer(float time){
		if(time > 0){
			waitTime = time;
			ResetTimer();
			ResumeTimer();
		}
	}

	/// <summary>
	/// Resumes the timer if it was paused
	/// </summary>

	public void ResumeTimer(){
		paused = false;
	}

	/// <summary>
	/// Pauses timer if it was not paused
	/// </summary>
	public void PauseTimer(){
		paused = true;
	}

	/// <summary>
	/// Resets the timer, thus making the timeleft set back to the waittime
	/// </summary>

	public void ResetTimer(){
		timeLeft = waitTime;
	}

	///<summary>
	/// Processes timer by counting down based on a certain value and will not process it if its paused or stopped
	/// </summary>

	void ProcessCountDown(float delta){
		if(paused) return;

		timeLeft -= delta;

		signalBus.Emit(Signals.Process, timeLeft);

		if(timeLeft <= 0){
			timeLeft = 0;

			signalBus.Emit(Signals.Timeout);

			if(!oneshot){
				StartTimer();
			}else{
				PauseTimer();
			}
		}
	}
}