using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeableObject : MonoBehaviour
{
	public Collider2D meshCollider;
	protected ColliderLayerComp colliderComp;
	public Renderer objectRenderer;
	public Renderer[] renderersToColor = new Renderer[1];

	private float oldAlpha;

	public float alpha = 0.45f;
    // Start is called before the first frame update
    void Start()
    {
        if(!meshCollider){
			meshCollider = GetComponent<Collider2D>();
		}
		if(!colliderComp){
			colliderComp = meshCollider.GetComponent<ColliderLayerComp>();
		}
		if(!objectRenderer){
			objectRenderer = GetComponentInParent<Renderer>();
		}
		if(objectRenderer){
			oldAlpha = (objectRenderer.material.color).a;
		}
    }

    // Update is called once per frame
    void Update(){
        CheckCollision();
    }

	bool CheckCollision(){
		float delta = Time.deltaTime;
		List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
		int count = meshCollider.Cast(Vector2.zero, new ContactFilter2D(), castCollisions, 0);
		count = 0;

		foreach (var castColl in castCollisions){
			if (!castColl) continue;
			Collider2D collider = castColl.collider;
			var xyz = collider?.GetComponent<ColliderLayerComp>();
			if (colliderComp && colliderComp.HasCollision(xyz)){
				float playerPosY = collider.transform.position.y;
				float objectY = transform.position.y;

				if(playerPosY>objectY) count++;
			}
		}

		if(count > 0){
			FadeRenderers();
		}else{
			UnfadeRenderers();
		}
		return false;
	}

	void FadeRenderers(){
		var renderers = new List<Renderer>(renderersToColor);
		renderers.Add(objectRenderer);

		foreach(var renderer in renderers){
			if(!renderer) continue;
			
			Color _color = renderer.material.color;
			_color.a = alpha;
			renderer.material.color = _color;
		}
	}
	void UnfadeRenderers(){
		var renderers = new List<Renderer>(renderersToColor);
		renderers.Add(objectRenderer);

		foreach(var renderer in renderers){
			if(!renderer) continue;

			Color _color = renderer.material.color;
			_color.a = oldAlpha;
			renderer.material.color = _color;
		}
	}
}
