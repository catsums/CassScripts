using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SignalBusNS;

public class VisibilityHandler : MonoBehaviour
{

	public SignalBus signalBus = new SignalBus();

	public static class Signals{
		public const string
		IsVisible = "isVisible",
		IsInvisible = "isInvisible";
	}

	public List<MonoBehaviour> componentsToHandle = new List<MonoBehaviour>();
	new Renderer renderer;

	protected void Start()
	{
		renderer = GetComponentInChildren<Renderer>();
		DisableComponents();

		signalBus.On(Signals.IsVisible, EnableComponents);
		signalBus.On(Signals.IsInvisible, DisableComponents);
	}

	protected void OnBecameVisible() {
		signalBus.Emit(Signals.IsVisible);
	}
	protected void OnBecameInvisible() {
		signalBus.Emit(Signals.IsInvisible);
	}

	public void EnableComponents(){
		foreach(var comp in componentsToHandle){
			if(!comp) continue;

			comp.enabled = true;
		}
	}
	public void DisableComponents(){
		foreach(var comp in componentsToHandle){
			if(!comp) continue;

			comp.enabled = false;
		}
	}
}