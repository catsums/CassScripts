using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SignalBusNS;
using Serializables;
using VEC2;

[ExecuteInEditMode]

public class ColorPaletteAnimator : MonoBehaviour
{
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
		/// <summary>When Color palette is changed</summary>
		public const string PaletteChange = "paletteChange";
	}

	public enum ProcessMode {
		IDLE, FIXED, UNSCALED
	}

	public SerialList<SerialList<Color>> palettes = new SerialList<SerialList<Color>>(){
		new SerialList<Color>(){Color.white}
	};

	public Color[] currentPalette = new Color[0];

	[SerializeField] 
	protected float currentTime = 0;
	[SerializeField] 
	protected float timeBetweenEachFrame = 0f;

	public int index = 0;

	public bool paused = false;
	public bool loop = false;
	public bool autostart = false;
	public bool useLerp = false;

	public bool playInEditor = false;
	[Range(0f, 360)]
	public float FPS = 24;

	public float speed = 1f;

	public ProcessMode processMode = ProcessMode.IDLE;

	public Color[] SelectedPalette{
		get{
			return palettes[index]?.ToArray();
		}
	}
	public int PaletteSize{
		get{
			int s = 0;
			foreach(var p in palettes){
				if(p==null) continue;
				s = Mathf.Max(s, p.Count);
			}
			return s;
		}
	}

	void Start(){
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
		SetCurrentPalette(index);
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

	public static List<Color> LerpColors(IEnumerable<Color> arrA, IEnumerable<Color> arrB, float t){
		List<Color> listA = new List<Color>(arrA);
		List<Color> listB = new List<Color>(arrB);
		List<Color> newList = new List<Color>();

		if(listA==null || listB == null) return null;
		
		for(int i=0; i<listA.Count; i++){
			var colA = listA[i];
			var colB = listA[i];
			if(i < listB.Count){
				colB = listB[i];
			}

			var newCol = Color.Lerp(colA, colB, t);
			newList.Add(newCol);
		}
		for(int i=0; i<listB.Count; i++){
			if(i < newList.Count) continue;
			var colB = listB[i];
			newList.Add(colB);
		}
		return newList;
	}

	public List<Color> GetFullPalette(int _index){
		int currIndex = MOD((_index), palettes.Count);
		int prevIndex = MOD((currIndex-1), palettes.Count);

		if(!loop && currIndex==0){
			prevIndex = currIndex;
		}

		var prevPalette = new List<Color>(palettes[prevIndex]?.ToArray());
		var nextPalette = new List<Color>(palettes[currIndex]?.ToArray());
		var newPalette = new List<Color>();


		for(int i=0; i<nextPalette.Count; i++){
			Color nextCol = nextPalette[i];
			Color newCol = nextCol;
			newPalette.Add(newCol);
		}
		for(int i=0; i<prevPalette.Count; i++){
			if(i < newPalette.Count) continue;
			Color prevCol = prevPalette[i];
			newPalette.Add(prevCol);
		}

		int len = PaletteSize;
		for(int i=0; i<len; i++){
			if(i < newPalette.Count) continue;

			Color newCol = default(Color);
			for(int x=1; x<palettes.Count; x++){
				var p = palettes[(currIndex + x) % palettes.Count];

				if(p==null) continue;

				if(p.Count > i){
					newCol = p[i];
					break;
				}
			}

			newPalette.Add(newCol);
		}

		return newPalette;
	}

	public void SetCurrentPalette(int newIndex){
		// print($"<---");
		int prevIndex = index;

		var newPalette = GetFullPalette(newIndex);

		if(useLerp){
			int nextIndex = MOD((newIndex+1), palettes.Count);
			if(!loop && (newIndex+1)>=palettes.Count){
				nextIndex = newIndex;
			}
			var currPalette = newPalette;
			var nextPalette = GetFullPalette(nextIndex);

			newPalette = LerpColors(currentPalette, nextPalette, timeBetweenEachFrame);

			// newPalette = new List<Color>();

			// for(int i=0; i<currPalette.Count; i++){
			// 	var currCol = currPalette[i];
			// 	var nextCol = nextPalette[i];

			// 	var newCol = Color.Lerp(currCol, nextCol, timeBetweenEachFrame);
			// 	newPalette.Add(newCol);
			// }
		}

		currentPalette = newPalette.ToArray();
	
		index = newIndex;
		if(prevIndex != newIndex){
			timeBetweenEachFrame = 0;
			signalBus.Emit(Signals.PaletteChange, currentPalette);
		}
		// print($"--->");
	}

	void ProcessAnimation(float delta){
		if(paused) return;

		float timePerFrame = (1/FPS);
		currentTime += (delta * speed);

		timeBetweenEachFrame += (delta * speed);

		int numOfFrames = palettes.Count;

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
				SetCurrentPalette(currFrame);
			}else{
				PauseAnimation();
			}
		}else{
			SetCurrentPalette(currFrame);
		}

	}

	//HELPER

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