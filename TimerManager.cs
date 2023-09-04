using System;
using System.Collections.Generic;
using UnityEngine;

using GameObjectExt;
using MyHelperFunctions;

using SignalBusNS;

public enum TimeProcessMode {
	IDLE, FIXED, UNSCALED
}

/// <summary>
/// Represents the timer that gets processed by its manager until it runs the completed function on timeout or for every repeat.
/// Timers will not run unless they are associated with a certain timer manager.
/// </summary>

public class TimerInstance{

	protected string _id = RandomID(9,"TimerInstance-");

	public string ID{
		get{ return _id; }
	}

	protected static string RandomID(int len, string prefix = "", string suffix = ""){
		return $"{prefix}{RandomString(len)}{suffix}";
	}
	protected static string RandomString(int len, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"){
		Random random = new Random();
		return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
	}

	public TimeProcessMode processMode = TimeProcessMode.IDLE;
	public float timeLeft = 1f;
	public float waitTime = 1f;
	public bool oneshot = true;
	public bool paused = false;
	public Action OnStart = null;
	public Action<float> OnProcess = null;
	public Action OnTimeout = null;

	public TimerManager manager = null;

	public TimerInstance Next(Action OnComplete, float time){
		if(!oneshot) return null;
		TimerInstance inst = new();
		inst.OnTimeout += OnComplete;
		inst.waitTime = time;
		inst.timeLeft = time;

		inst.processMode = processMode;
		inst.oneshot = true;
		inst.manager = manager;

		OnTimeout += () => {
			manager.instances.Add(inst);
		};

		return inst;
	}

	public void StartTimer(){
		StartTimer(waitTime);
	}

	public bool IsOneShot(){
		return (oneshot);
	}

	public void StartTimer(float time){
		if(time > 0){
			waitTime = time;
			ResetTimer();
			ResumeTimer();

			OnStart?.Invoke();
		}
	}

	public void ResumeTimer(){
		paused = false;
	}

	public void PauseTimer(){
		paused = true;
	}

	public void ResetTimer(){
		timeLeft = waitTime;
	}

	public bool ProcessCountDown(float delta){
		if(paused) return false;

		timeLeft -= delta;

		var progress = (1f - timeLeft/waitTime);
		progress = Mathf.Clamp(progress,0,1);

		OnProcess?.Invoke(progress);

		if(timeLeft <= 0){
			timeLeft = 0;

			OnTimeout?.Invoke();

			if(!IsOneShot()){
				StartTimer();
				return false;
			}else{
				PauseTimer();
			}
			return true;
		}
		return false;
	}
}

/// <summary>
/// A Timer Manager that lets you run handlers (functions) after a certain number of time.
/// This works by letting you create and keep track of instances that process on runtime.
/// Each timer instance will have its own processing mode (can run on Update or Fixed time or even Unscaled time)
/// </summary>

public class TimerManager : MonoBehaviour{
	public SignalBus signalBus = new SignalBus();

	public List<TimerInstance> instances = new List<TimerInstance>();

	protected static List<TimerInstance> allInstances = new List<TimerInstance>();

	void Start(){
		
	}
	
	void Update(){
		var delta = Time.deltaTime;

		var insts = instances.ToArray();
		
		foreach(var inst in insts){
			if(inst.processMode != TimeProcessMode.IDLE){
				continue;
			}
			var isComplete = inst.ProcessCountDown(delta);

			if(isComplete){
				ClearInstance(inst);
			}
		}

		delta = Time.unscaledDeltaTime;

		foreach(var inst in insts){
			if(inst.processMode != TimeProcessMode.UNSCALED){
				continue;
			}
			var isComplete = inst.ProcessCountDown(delta);

			if(isComplete){
				ClearInstance(inst);
			}
		}
	}

	void FixedUpdate(){
		var delta = Time.fixedDeltaTime;
		var insts = instances.ToArray();
		
		foreach(var inst in insts){
			if(inst.processMode != TimeProcessMode.FIXED){
				continue;
			}
			var isComplete = inst.ProcessCountDown(delta);

			if(isComplete){
				ClearInstance(inst);
			}
		}
	}

	/// <summary>
	/// Create a timer that will run a function on completion after a certain number of seconds
	/// </summary>
	/// <param name="OnComplete">Function to run when timer is complete</param>
	/// <param name="waitTime">Duration of timer in seconds</param>
	/// <param name="processMode">Process mode may be Idle, Fixed or Unscaled</param>
	/// <returns>A timer instance that can be used to keep track of timer or be used to cancel timer.</returns>
	public TimerInstance SetTimeout(Action OnComplete, float waitTime, TimeProcessMode processMode = TimeProcessMode.IDLE){
		var inst = new TimerInstance();
		inst.OnTimeout = OnComplete;
		inst.waitTime = waitTime;
		inst.processMode = processMode;
		inst.oneshot = true;
		inst.manager = this;

		instances.Add(inst);
		allInstances.Add(inst);
		inst.StartTimer();

		return inst;
	}
	public TimerInstance SetTimeout(Action OnComplete, Action<float> OnProcess, float waitTime, TimeProcessMode processMode = TimeProcessMode.IDLE){
		var inst = SetTimeout(OnComplete, waitTime, processMode);
		inst.OnProcess += OnProcess;

		return inst;
	}
	/// <summary>
	/// Create a timer that will run a function for every certain number of seconds on repeat
	/// </summary>
	/// <param name="OnRepeat">Function to run when timer is repeats</param>
	/// <param name="waitTime">Duration of timer in seconds</param>
	/// <param name="processMode">Process mode may be Idle, Fixed or Unscaled</param>
	/// <returns>A timer instance that can be used to keep track of timer or be used to cancel timer.</returns>
	public TimerInstance SetInterval(Action OnRepeat, float waitTime, TimeProcessMode processMode = TimeProcessMode.IDLE){
		var inst = new TimerInstance();
		inst.OnTimeout = OnRepeat;
		inst.waitTime = waitTime;
		inst.processMode = processMode;
		inst.oneshot = false;
		inst.manager = this;

		instances.Add(inst);
		allInstances.Add(inst);
		inst.StartTimer();

		return inst;
	}
	public TimerInstance SetInterval(Action OnRepeat, Action<float> OnProcess, float waitTime, TimeProcessMode processMode = TimeProcessMode.IDLE){
		var inst = SetInterval(OnRepeat, waitTime, processMode);
		inst.OnProcess += OnProcess;

		return inst;
	}

	/// <summary>
	/// Clears a timer to make it stop running
	/// </summary>
	/// <param name="inst">The timer instance to clear</param>
	/// <returns>Returns True if clearing was successful</returns>

	public static bool GlobalClearInstance(TimerInstance inst){
		TimerManager manager = inst?.manager;
		if(manager!=null){
			return manager.ClearInstance(inst);
		}
		return allInstances.Remove(inst);
	}
	/// <summary>
	/// Forces a certain timer to complete. This will run that function of that timer before removing it.
	/// </summary>
	/// <param name="inst">The timer instance to force complete</param>
	/// <returns>Returns True if completion was successful</returns>

	public bool ForceCompleteInstance(TimerInstance inst){
		if(instances.Contains(inst)){
			inst.ProcessCountDown(inst.waitTime);
			instances.Remove(inst);
			allInstances.Remove(inst);
			return true;
		}
		return false;
	}
	/// <summary>
	/// Clears a timer owned by this manager to make it stop running
	/// </summary>
	/// <param name="inst">The timer instance to clear</param>
	/// <returns>Will return true if clearing was successful</returns>
	public bool ClearInstance(TimerInstance inst){
		if(instances.Contains(inst)){
			instances.Remove(inst);
			allInstances.Remove(inst);
			inst.manager = null;
			return true;
		}
		return false;
	}
	/// <summary>
	/// Clears all timers owned by this manager
	/// </summary>

	public void ClearAllInstances(){
		var insts = instances.ToArray();
		foreach(var inst in insts){
			ClearInstance(inst);
		}
	}

	
}