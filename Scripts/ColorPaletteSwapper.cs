using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SignalBusNS;
using VEC2;

public class ColorPaletteSwapper : MonoBehaviour
{
	public SignalBus signalBus = new SignalBus();

	public Color[] colors = new Color[]{ Color.white };

	public int index = 0;

	public Color CurrentColor{
		get{
			if(index>=0 && index<colors.Length){
				return colors[index];
			}
			return default(Color);
		}
	}

	public Renderer rendererComp = null;
	public RendererGroup rendererGroup = null;
	public SpriteRendererGroup sprRendererGroup = null;

	protected Color _rendererCompColor;
	protected Color _rendererGroupColor;
	protected Color _sprRendererGroupColor;

	void Start(){

		if(!rendererComp) rendererComp = GetComponent<Renderer>();
		if(!rendererGroup) rendererGroup = GetComponent<RendererGroup>();
		if(!sprRendererGroup) sprRendererGroup = GetComponent<SpriteRendererGroup>();

		if(rendererComp){
			_rendererCompColor = rendererComp.material.color;
		}
		if(rendererGroup){
			_rendererGroupColor = rendererGroup.settings.color;
		}
		if(sprRendererGroup){
			_sprRendererGroupColor = sprRendererGroup.settings.color;
		}
	}

	void OnEnable(){
		
	}

	void Update(){

	}

	public void SetColors(){
		if(rendererComp) rendererComp.material.color = CurrentColor;
		if(rendererGroup) rendererGroup.settings.color = CurrentColor;
		if(sprRendererGroup) sprRendererGroup.settings.color = CurrentColor;
	}
	public void ResetColors(){
		if(rendererComp) rendererComp.material.color = _rendererCompColor;
		if(rendererGroup) rendererGroup.settings.color = _rendererGroupColor;
		if(sprRendererGroup) sprRendererGroup.settings.color = _sprRendererGroupColor;
	}
}