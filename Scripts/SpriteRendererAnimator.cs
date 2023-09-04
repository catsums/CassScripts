using System;
using System.Collections.Generic;
using UnityEngine;

using GameObjectExt;

using SignalBusNS;
using MyHelperFunctions;
using VEC2;

[ExecuteInEditMode]

public class SpriteRendererAnimator : MonoBehaviour{
	public SignalBus signalBus = new SignalBus();

	public static class Signals{
		/// <summary>When Animation is played or resumed</summary>
		public const string AnimationPlay = "animationPlay";
		/// <summary>When Animation is processed per update based on processing mode</summary>
		public const string AnimationStep = "animationStep";
		/// <summary>When Animation reaches last frame, whether it is ending or looping</summary>
		public const string AnimationEnd = "animationEnd";
		/// <summary>When Animation is paused</summary>
		public const string AnimationPause = "animationPause";
		/// <summary>When Animation is reset</summary>
		public const string AnimationReset = "animationReset";
		/// <summary>When Animation frame is changed</summary>
		public const string FrameChange = "frameChange";
	}

	public enum ProcessMode {
		IDLE, FIXED, UNSCALED
	}

	public bool paused = false;
	public bool loop = false;
	public bool autostart = false;

	public bool playInEditor = false;

	[Range(0f, 360)]
	public float FPS = 24;

	public float speed = 1f;

	protected float currentTime = 0;

	public int index = 0;

	public ProcessMode processMode = ProcessMode.IDLE;
	public SpriteRenderer spriteRenderer;

	public Sprite[] frames = new Sprite[0];

	public Sprite CurrentFrame{
		get{
			if(index>=0 && index<frames.Length){
				return frames[index];
			}
			return null;
		}
	}

	void Start(){
		if(!spriteRenderer){
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		if(autostart){
			PlayAnimation();
		}else{
			PauseAnimation();
		}
	}
	
	void Update(){
		if(!Application.isPlaying && !playInEditor){
			return;
		}

		if(processMode != ProcessMode.IDLE && processMode != ProcessMode.UNSCALED){
			return;
		}

		var delta = Time.deltaTime;
		if(processMode == ProcessMode.UNSCALED){
			delta = Time.unscaledDeltaTime;
		}
		ProcessAnimation(delta);
	}
	void OnValidate(){
		SetCurrentFrame(index);
	}
	void FixedUpdate(){
		if(!Application.isPlaying && !playInEditor){
			return;
		}
		if(processMode != ProcessMode.FIXED) return;

		var delta = Time.fixedDeltaTime;
		ProcessAnimation(delta);
	}

	public void PlayAnimation(){
		if(paused){
			paused = false;
			signalBus.Emit(Signals.AnimationPlay, this);
		}
	}
	public void PauseAnimation(){
		if(!paused){
			paused = true;
			signalBus.Emit(Signals.AnimationPause, this);
		}
	}
	public void ResetAnimation(){
		currentTime = 0;
		signalBus.Emit(Signals.AnimationReset, this);
	}

	public void SetCurrentFrame(int newIndex){
		int prevIndex = index;
		Sprite currSprite = frames[newIndex];
		if(spriteRenderer){
			spriteRenderer.sprite = currSprite;
		}
		index = newIndex;
		if(prevIndex != newIndex){
			signalBus.Emit(Signals.FrameChange, currSprite);
		}
	}

	void ProcessAnimation(float delta){
		if(paused) return;

		float timePerFrame = (1f/FPS);
		currentTime += (delta * speed);

		int numOfFrames = frames.Length;

		if(numOfFrames<=0) return;

		int prevFrame = index;

		int currFrame = (int) MOD(currentTime/timePerFrame, numOfFrames+1);

		// if(currFrame>=0 && currFrame<numOfFrames){
		// 	index = currFrame;
		// }

		signalBus.Emit(Signals.AnimationStep, this);
		
		if(currFrame >= numOfFrames){
			currentTime = 0;
			currFrame = numOfFrames-1;
			
			signalBus.Emit(Signals.AnimationEnd, this);

			if(loop){
				currFrame = 0;
				PlayAnimation();
			}else{
				PauseAnimation();
			}
		}
		SetCurrentFrame(index);

	}

	static protected int MOD(int a, int b){
		return ((a % b) + b) % b;
	}
	static protected long MOD(long a, long b){
		return ((a % b) + b) % b;
	}static protected decimal MOD(decimal a, decimal b){
		return ((a % b) + b) % b;
	}static protected float MOD(float a, float b){
		return ((a % b) + b) % b;
	}static protected double MOD(double a, double b){
		return ((a % b) + b) % b;
	}

}