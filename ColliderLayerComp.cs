using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using SignalBusNS;

/// <summary>
/// Collision handler that works with a Collider2D to check for collisions only in certain layers and can use depth detection to detect collisions based on the Z-axis too.
/// This can also run live detections for each FixedUpdate() to check for valid collisions and send signals
/// </summary>
public class ColliderLayerComp : MonoBehaviour
{
	// public bool runInEditor = false;

	public static List<string> allLayers = new List<string>();
	public SignalBus signalBus = new SignalBus();

	public static class Signals{
		/// <summary>When collider first enters collision with component</summary>
		public const string CollisionEnter = "collisionEnter";
		/// <summary>When collider is detected for each update check</summary>
		public const string CollisionStep = "collisionStep";
		/// <summary>When collider exits collision with component</summary>
		public const string CollisionExit = "collisionExit";

		/// <summary>When collider first enters trigger with component</summary>
		public const string TriggerEnter = "triggerEnter";
		/// <summary>When collider is detected in trigger for each update check</summary>
		public const string TriggerStep = "triggerStep";
		/// <summary>When collider exits trigger with component</summary>
		public const string TriggerExit = "triggerExit";
		//// <summary>When collider is detected for time DetectCollision is run</summary>
		// public const string DetectedCollision = "detectedCollision";
	}

	public string colliderName = "";

	public string[] collisionLayers = new string[1];
	public Dictionary<string, bool> collLayers = new Dictionary<string, bool>();
	public string[] collisionTargets = new string[1];
	public Dictionary<string, bool> collTargets = new Dictionary<string, bool>();

	public int priority = 0;

	public bool isEnabled = true;
	public bool checkCollisionsOnUpdate = false;
	public bool canDetectNormalCollider = true;

	public bool useDepthDetection = false;
	public float depth = 1f;
	public float depthStart{
		set{
			Vector3 pos = transform.position;
			pos.z = value;
			transform.position = pos;
		}
		get{
			return transform.position.z;
		}
	}
	public float depthEnd{
		set{
			depth = value - depthStart;
		}
		get{
			return (depthStart + depth);
		}
	}

	public Vector2 depthRange{
		set{
			depthStart = value.x;
			depthEnd = value.y;
		}
		get{
			return new Vector2(depthStart, depthEnd);
		}
	}

	/// <summary>
	/// Check if value is in depth with collider's depth start and depth end
	/// </summary>
	/// <param name="val">Value to be checked</param>

	public bool IsInDepth(float val){
		return (val >= depthStart && val < depthEnd);
	}
	public bool IsInDepth(float min, float max){
		return (
			IsInDepth(min) || IsInDepth(max)
		);
	}
	public bool IsInDepth(Vector2 val){
		return (
			IsInDepth(val.x) || IsInDepth(val.y)
		);
	}

	void OnValidate(){
		InitializeLayers();
		ReviseLayers();

		if(_depthRange != depthRange){
			_depthRange = depthRange;
		}
	}

	void Awake(){
		InitializeLayers();
	}

	void InitializeLayers(){
		foreach (string layer in collisionLayers){
			if(layer==null) continue;
			if(!collLayers.ContainsKey(layer)){
				collLayers[layer] = true;
			}

			if (!allLayers.Contains(layer))
			{
				allLayers.Add(layer);
			}
		}
		foreach (string layer in collisionTargets){
			if(layer==null) continue;
			if(!collTargets.ContainsKey(layer)){
				collTargets[layer] = true;
			}

			if (!allLayers.Contains(layer))
			{
				allLayers.Add(layer);
			}
		}
	}

	void ReviseLayers(){
		foreach(string layer in collisionLayers){
			if(layer==null) continue;
			if(!collLayers.ContainsKey(layer)){
				collLayers[layer] = true;
			}
		}
		foreach (string layer in collisionTargets){
			if(layer==null) continue;
			if(!collTargets.ContainsKey(layer)){
				collTargets[layer] = true;
			}
		}

		var _layers = collLayers.Keys.ToArray();
		foreach(string layer in _layers){
			if(layer==null) continue;
			if(!collisionLayers.Contains(layer)){
				collLayers[layer] = false;
			}
		}

		var _targets = collTargets.Keys.ToArray();
		foreach(string layer in _targets){
			if(layer==null) continue;
			if(!collisionTargets.Contains(layer)){
				collTargets[layer] = false;
			}
		}

	}

	void Start(){

	}

	public void OnDetectCollisionEnter(Action<Collider2D> handler){
		signalBus.On(Signals.CollisionEnter, handler);
	}
	public void OnDetectCollisionStep(Action<Collider2D> handler){
		signalBus.On(Signals.CollisionStep, handler);
	}
	public void OnDetectCollisionExit(Action<Collider2D> handler){
		signalBus.On(Signals.CollisionExit, handler);
	}
	public void OnDetectTriggerEnter(Action<Collider2D> handler){
		signalBus.On(Signals.TriggerEnter, handler);
	}
	public void OnDetectTriggerStep(Action<Collider2D> handler){
		signalBus.On(Signals.TriggerStep, handler);
	}
	public void OnDetectTriggerExit(Action<Collider2D> handler){
		signalBus.On(Signals.TriggerExit, handler);
	}

	void OnCollisionEnter2D(Collision2D other){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(other?.collider)) return;

		signalBus.Emit(Signals.CollisionEnter, other.collider);
	}
	void OnCollisionStay2D(Collision2D other){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(other?.collider)) return;

		signalBus.Emit(Signals.CollisionStep, other.collider);
	}
	void OnCollisionExit2D(Collision2D other){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(other?.collider)) return;

		signalBus.Emit(Signals.CollisionExit, other.collider);
	}

	void OnTriggerEnter2D(Collider2D collider){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(collider)) return;

		signalBus.Emit(Signals.TriggerEnter, collider);
	}
	void OnTriggerStay2D(Collider2D collider){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(collider)) return;

		signalBus.Emit(Signals.TriggerStep, collider);
	}
	void OnTriggerExit2D(Collider2D collider){
		// if(!checkCollisionsOnUpdate) return;
		if(!DetectCollision(collider)) return;

		signalBus.Emit(Signals.TriggerExit, collider);
	}

	protected Vector2 _depthRange = Vector2.negativeInfinity;
	
	void Update(){
		// if(!Application.isPlaying || !runInEditor) return;
	}

	void FixedUpdate(){
		// if(!Application.isPlaying || !runInEditor) return;

		if(checkCollisionsOnUpdate){
			CheckForCollisions();
			CheckCollisionCache();
		}
	}

	List<Collider2D> colliderCache = new List<Collider2D>();

	void CheckForCollisions(){
		ForEachCurrentCollisions((coll, collComp)=>{
			if(colliderCache.Contains(coll)) return;
			
			colliderCache.Add(coll);

			signalBus.Emit(Signals.CollisionEnter, coll);

		});
	}
	void CheckCollisionCache(){
		Collider2D[] cache = colliderCache.ToArray();
		var currColls = GetCurrentCollisions();
		foreach(var coll in cache){
			if(currColls.Contains(coll)){
				signalBus.Emit(Signals.CollisionStep, coll);
			}else{
				colliderCache.Remove(coll);
				signalBus.Emit(Signals.CollisionExit, coll);
			}
		}
	}

	/// <summary>
	/// Casts the collider in a certain direction and distance but uses the collider comp to filter
	/// </summary>
	/// <param name="selectedCollider">Selected collider to use if any</param>
	/// <param name="direction">Direction to cast</param>
	/// <param name="distance">Distance to reach for cast</param>
	/// <returns>List of raycast hits containing information about colliders </returns>

	public List<RaycastHit2D> Cast(Collider2D selectedCollider, Vector2 direction, float distance){
		List<RaycastHit2D> initColls = new List<RaycastHit2D>();
		List<RaycastHit2D> colls = new List<RaycastHit2D>();
		ContactFilter2D filter = default(ContactFilter2D);

		selectedCollider?.Cast(direction, filter,initColls,distance);
		foreach(var castColl in initColls){
			Collider2D coll = castColl.collider;
			if(this.DetectCollision(coll)){
				colls.Add(castColl);
			}
		}
		return colls;
	}public List<RaycastHit2D> Cast( Vector2 direction, float distance){
		Collider2D thisCollider = GetComponent<Collider2D>();
		
		return Cast(thisCollider, direction, distance);
	}

	/// <summary>
	/// Cast collider in a certain direction and distance and run a function to loop through each detected raycast hit
	/// </summary>
	/// <param name="selectedCollider">Selected collider to use if any</param>
	/// <param name="direction">Direction to cast</param>
	/// <param name="distance">Distance to reach for cast</param>
	/// <param name="handler">Function to run for each detection</param>

	public void ForEachCast(Collider2D selectedCollider, Vector2 direction, float distance, Action<RaycastHit2D> handler){
		List<RaycastHit2D> colls = Cast(selectedCollider, direction, distance);
		foreach(var castColl in colls){
			handler(castColl);
		}
	}public void ForEachCast(Vector2 direction, float distance, Action<RaycastHit2D> handler){
		List<RaycastHit2D> colls = Cast(direction, distance);
		foreach(var castColl in colls){
			handler(castColl);
		}
	}
	public void ForEachCast(Collider2D selectedCollider, Vector2 direction, float distance, Action<RaycastHit2D, ColliderLayerComp> handler){
		List<RaycastHit2D> colls = Cast(selectedCollider, direction, distance);
		foreach(var castColl in colls){
			ColliderLayerComp collComp = null;
			castColl.collider?.TryGetComponent<ColliderLayerComp>(out collComp);
			handler(castColl, collComp);
		}
	}public void ForEachCast(Vector2 direction, float distance, Action<RaycastHit2D, ColliderLayerComp> handler){
		List<RaycastHit2D> colls = Cast(direction, distance);
		foreach(var castColl in colls){
			ColliderLayerComp collComp = null;
			castColl.collider?.TryGetComponent<ColliderLayerComp>(out collComp);
			handler(castColl, collComp);
		}
	}

	/// <summary>
	/// Get collisions in contact with this collider filtered by this component
	/// </summary>
	/// <param name="selectedCollider">Selected collider linked to this component</param>
	/// <returns>List of colliders in contact with this component</returns>
	public List<Collider2D> GetCurrentCollisions(Collider2D selectedCollider){
		List<Collider2D> initColls = new List<Collider2D>();
		List<Collider2D> colls = new List<Collider2D>();
		ContactFilter2D filter = default(ContactFilter2D);

		if(selectedCollider){
			selectedCollider.OverlapCollider(filter,initColls);
		}

		foreach(var coll in initColls){
			if(this.DetectCollision(coll)){
				colls.Add(coll);
			}
		}
		return colls;
	}public List<Collider2D> GetCurrentCollisions(){
		Collider2D thisCollider = GetComponent<Collider2D>();
		
		return GetCurrentCollisions(thisCollider);
	}

	/// <summary>
	/// Get colliders in contact with this collider component and run a function to handler each collider and each collider layer comp
	/// </summary>
	/// <param name="selectedCollider">Selected collider linked to this component</param>
	/// <param name="handler">Function to run for each detection</param>

	public void ForEachCurrentCollisions(Collider2D selectedCollider, Action<Collider2D> handler){
		List<Collider2D> colls = GetCurrentCollisions(selectedCollider);
		foreach(var coll in colls){
			handler(coll);
		}
	}public void ForEachCurrentCollisions(Action<Collider2D> handler){
		List<Collider2D> colls = GetCurrentCollisions();
		foreach(var coll in colls){
			handler(coll);
		}
	}
	public void ForEachCurrentCollisions(Collider2D selectedCollider, Action<Collider2D, ColliderLayerComp> handler){
		List<Collider2D> colls = GetCurrentCollisions(selectedCollider);
		foreach(var coll in colls){
			ColliderLayerComp collComp = null;
			coll?.TryGetComponent<ColliderLayerComp>(out collComp);

			handler(coll, collComp);
		}
	}public void ForEachCurrentCollisions(Action<Collider2D, ColliderLayerComp> handler){
		List<Collider2D> colls = GetCurrentCollisions();
		foreach(var coll in colls){
			ColliderLayerComp collComp = null;
			coll?.TryGetComponent<ColliderLayerComp>(out collComp);

			handler(coll, collComp);
		}
	}

	/// <summary>
	/// Check if Component can detect with that other component
	/// </summary>
	/// <param name="other">Collider Component to check</param>
	public bool CanDetectCollision(ColliderLayerComp other){
		if(!other || other==this) return false;
		if(!isEnabled || !other.isEnabled) return false;

		foreach (var layer in other.collLayers.Keys){
			if(collTargets.ContainsKey(layer) && collTargets[layer]){
				if(useDepthDetection && !IsInDepth(other.depthRange)){
					// continue;
				}else{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Get detected layers from collision between this component and another component
	/// </summary>
	/// <param name="other">The Component to check</param>
	/// <returns>List of layers detected</returns>
	public List<string> GetCollisionsFrom(ColliderLayerComp other)
	{
		List<string> colls = new List<string>();
		if(!other || other==this) return colls;
		if (!isEnabled || !other.isEnabled) return colls;

		foreach (var layer in other.collLayers.Keys){
			if(collTargets.ContainsKey(layer) && collTargets[layer])
			{
				if(other.priority > this.priority) continue;

				colls.Add(layer);
			}
		}
		return colls;
	}
	/// <summary>
	/// Checks if component currently has a collision with other component
	/// </summary>
	/// <param name="other">The component to check</param>
	public bool HasCollision(ColliderLayerComp other){
		if(!other || other==this) return false;
		if(!isEnabled || !other.isEnabled) return false;

		foreach (var layer in other.collLayers.Keys){
			if(collTargets.ContainsKey(layer) && collTargets[layer]){
				if(other.priority > this.priority) continue;

				if(useDepthDetection && !IsInDepth(other.depthRange)){
					// continue;
				}else{
					return true;
				}
			}
		}
		return false;
	}public bool HasCollision(ColliderLayerComp other, string[] layers){
		if(!other || layers==null || layers.Length==0 || other==this) return false;
		if(!isEnabled || !other.isEnabled) return false;

		int count = layers.Length;

		foreach (var layer in other.collLayers.Keys){
			if(!layers.Contains<string>(layer)) continue;
			if(collTargets.ContainsKey(layer) && collTargets[layer]){
				if(other.priority > this.priority) continue;
				if(useDepthDetection && !IsInDepth(other.depthRange)){
					// continue;
				}else{
					count--;
				}
			}
		}
		return (count==0);
	}

	/// <summary>
	/// Detects collision with a Collision2D. Will send a signal if detection is confirmed
	/// </summary>
	/// <param name="collider">The collider to check</param>
	/// <returns>Returns true is the collision was detected</returns>
	public bool DetectCollision(Collider2D collider){
		if(!collider) return false;
		if(!isEnabled) return false;
		
		bool detected = false;
		if (collider == null) return false;
		ColliderLayerComp other = null;
		collider.TryGetComponent<ColliderLayerComp>(out other);

		if (!other){
			detected = canDetectNormalCollider;
		}else{
			detected = HasCollision(other);
		}

		// if (detected){
		// 	signalBus.Emit(Signals.DetectedCollision, other);
		// }

		return detected;

	}public bool DetectCollision(Collider2D collider, string[] layers){
		if(!collider) return false;
		if(!isEnabled) return false;
		
		bool detected = false;
		if (collider == null) return false;
		ColliderLayerComp other = null;
		collider.TryGetComponent<ColliderLayerComp>(out other);

		if (!other) detected = canDetectNormalCollider;
		else detected = HasCollision(other, layers);

		// if (detected){
		// 	signalBus.Emit(Signals.DetectedCollision, other);
		// }

		return detected;

	}

	public void EnableCollisionForLayer(string layer){
		if (!collLayers.ContainsKey(layer)) return;
		collLayers[layer] = true;
	}
	public void DisableCollisionForLayer(string layer){
		if (!collLayers.ContainsKey(layer)) return;
		collLayers[layer] = false;
	}
	public void ToggleCollisionForLayer(string layer){
		if (!collLayers.ContainsKey(layer)) return;
		collLayers[layer] = !collLayers[layer];
	}

	public void EnableCollisionForTarget(string layer){
		if (!collTargets.ContainsKey(layer)) return;
		collTargets[layer] = true;
	}
	public void DisableCollisionForTarget(string layer){
		if (!collTargets.ContainsKey(layer)) return;
		collTargets[layer] = false;
	}
	public void ToggleCollisionForTarget(string layer){
		if (!collTargets.ContainsKey(layer)) return;
		collTargets[layer] = !collTargets[layer];
	}
}