using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using SignalBusNS;
using GameObjectExt;

[ExecuteInEditMode]
public class RendererGroup : MonoBehaviour
{
	Component[] Children {
		get {
			List<Component> list = new List<Component>();

			var renderers = this.GetComponentsInDirectChildren<Renderer>();
			foreach(var comp in renderers){
				list.Add(comp);
				if(!cache.ContainsKey(comp)){
					var sett = new CompSettings();

					sett.color = comp.material.color;

					cache[comp] = sett;
				}
			}
			var rendererGroups = this.GetComponentsInDirectChildren<RendererGroup>();
			foreach(var comp in rendererGroups){
				list.Add(comp);
				if(!cache.ContainsKey(comp)){
					var sett = new CompSettings();

					sett.color = comp.settings.color;

					cache[comp] = sett;
				}
			}

			return list.ToArray();
		}
	}

	public enum BlendMode{
		Default, Multiply, Add, Average, Blender
	}

	public class BlendAlgo{
		public static Color Blend(Color colA, Color colB, float val){
			Color col = colB;
			return Color.Lerp(colA, col, val);
		}
	}public class BlendAlgoMultiply{
		public static Color Blend(Color colA, Color colB, float val){
			Color col = (colA*colB);
			col.a = Mathf.Clamp(colA.a+colB.b,0f,1f);
			return Color.Lerp(colA, col, val);
		}
	}public class BlendAlgoAverage{
		public static Color Blend(Color colA, Color colB, float val){
			Color col = (colA+colB)/2;
			col.a = Mathf.Clamp(colA.a+colB.b,0f,1f);
			return Color.Lerp(colA, col, val);
		}
	}public class BlendAlgoAdd{
		public static Color Blend(Color colA, Color colB, float val){
			Color col = (colA+colB);
			col.a = Mathf.Clamp(colA.a+colB.b,0f,1f);
			return Color.Lerp(colA, col, val);
		}
	}public class BlendAlgoBlender{
		public static Color Blend(Color colA, Color colB, float val){
			// var t = Mathf.Clamp((colB.a-colA.a),0f,1f);
			float t;
			if(colB.a >= colA.a){
				t = Mathf.Clamp((2*colB.a-colA.a),0f,1f);
			}else{
				t = Mathf.Clamp((2*colA.a-colB.a),0f,1f);
			}

			Color col = Color.Lerp(colA,colB,t);
			col.a = Mathf.Clamp(colA.a+colB.b,0f,1f);

			return Color.Lerp(colA, col, val);
		}
	}


	public virtual Color ColorBlend(Color colA, Color colB, BlendMode mode, float intensity = 1f){
		switch(mode){
			case BlendMode.Multiply: return BlendAlgoMultiply.Blend(colA,colB, intensity);
			case BlendMode.Add: return BlendAlgoAdd.Blend(colA,colB, intensity);
			case BlendMode.Average: return BlendAlgoAverage.Blend(colA,colB, intensity);
			case BlendMode.Blender: return BlendAlgoBlender.Blend(colA,colB, intensity);
			default: return BlendAlgo.Blend(colA,colB, intensity);
		}
	}

	public BlendMode blendMode = BlendMode.Multiply;
	protected BlendMode oldBlendMode;
	[Range(0f,1f)]
	public float blendIntensity = 1f;
	protected float oldBlendIntensity;

	[Serializable]
	public class CompSettings{
		public Color color;
		[Range(0f,1f)]
		public float alpha;

		public Dictionary<string,object> data = new Dictionary<string,object>();
	}

	Dictionary<Component, CompSettings> cache = new Dictionary<Component, CompSettings>();

	public CompSettings settings = new CompSettings();
	protected CompSettings oldSettings = new CompSettings();
	void Start() {
		settings.color = Color.white;
		settings.alpha = 1f;
	}

	void OnEnable(){
		UpdateGroup();
	}

	void OnDisable(){
		var renderers = this.GetComponentsInDirectChildren<Renderer>();
		foreach(var comp in renderers){
			if(cache.ContainsKey(comp)){
				var sett = cache[comp];

				comp.material.color = sett.color;

				cache.Remove(comp);
			}
		}
		var rendererGroups = this.GetComponentsInDirectChildren<RendererGroup>();
		foreach(var comp in rendererGroups){
			if(cache.ContainsKey(comp)){
				var sett = cache[comp];

				comp.settings.color = sett.color;

				cache.Remove(comp);
			}
		}
	}
	
	
	void Update()
	{
		if(HasChanges()){
			UpdateGroup();
			UpdateSettings();
		}
	}
	void OnValidate() {
		UpdateGroup();
		UpdateSettings();
	}

	protected CompSettings[] oldChilds = new CompSettings[0];

	protected virtual bool HasChanges(){

		var childs = Children;
		if(childs.Length != oldChilds.Length) return true;
		// foreach(var compSett in oldChilds){
			
		// }

		if(oldSettings.color != settings.color) return true;
		if(oldSettings.alpha != settings.alpha) return true;

		if(oldBlendMode != blendMode) return true;
		if(oldBlendIntensity != blendIntensity) return true;

		return false;
	}

	protected virtual void UpdateSettings(){
		oldSettings = (CompSettings) settings;
		oldChilds = cache.Values.ToArray<CompSettings>();
		
		oldBlendMode = blendMode;
		oldBlendIntensity = blendIntensity;
	}

	protected virtual void UpdateGroup(){
		if(!enabled) return;

		var renderers = this.GetComponentsInDirectChildren<Renderer>();
		foreach(var comp in renderers){
			CompSettings sett;
			if(!cache.ContainsKey(comp)){
				sett = new CompSettings();

				sett.color = comp.material.color;

				cache[comp] = sett;
			}

			sett = cache[comp];

			Color col = ColorBlend(sett.color, settings.color, blendMode, blendIntensity);
			col.a *= settings.alpha;
			comp.material.color = col;

		}
		var rendererGroups = this.GetComponentsInDirectChildren<RendererGroup>();
		foreach(var comp in rendererGroups){
			CompSettings sett;
			if(!cache.ContainsKey(comp)){
				sett = new CompSettings();

				sett.color = comp.settings.color;

				cache[comp] = sett;
			}

			sett = cache[comp];

			Color col = ColorBlend(sett.color, settings.color, blendMode, blendIntensity);
			col.a *= settings.alpha;
			comp.settings.color = col;
		}
	}

}