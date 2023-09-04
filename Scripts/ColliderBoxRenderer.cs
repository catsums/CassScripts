using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using VEC2;

[ExecuteInEditMode]
public class ColliderBoxRenderer : MonoBehaviour{
	
	Collider2D myCollider;
	SpriteShapeRenderer sprRenderer;
	SpriteShapeController sprController;

    GameObject colliderRendererObj;

    public Color myColor = new Color(0.5f,0f,0f,0.3f);

	public SpriteShape shapeProfile;
	public int sortingOrder = 9100;

	public float lineThickness = 0f;
	public float fillAmount = 0f;

	protected List<Vector2> myPoints = new List<Vector2>();

	// [ExecuteInEditMode]
	void Awake()
	{
		if(!shapeProfile){
			shapeProfile = Resources.Load<SpriteShape>("ScriptResources/Sprite Shape Profile");
			shapeProfile = Instantiate(shapeProfile);
		}
		
		// if(shapeProfile){
		// 	shapeProfile = Instantiate(shapeProfile);
		// }
		
	}

	void OnEnable(){
		InstantiateObjects();
		colliderRendererObj.SetActive(true);
	}
	void OnDisable(){
		colliderRendererObj.SetActive(false);
	}

	void OnDestroy(){	
		// DestroyImmediate(colliderRendererObj);
	}

    // Start is called before the first frame update
    void Start(){
        InstantiateObjects();
    }

	void InstantiateObjects(){

		if(!myCollider){
			myCollider = GetComponent<Collider2D>();
		}

		colliderRendererObj = transform.Find("ColliderRenderer")?.gameObject;
		if(!colliderRendererObj){
        	colliderRendererObj = new GameObject("ColliderRenderer");
			colliderRendererObj.transform.SetParent(transform);

			colliderRendererObj.transform.localPosition = Vector2.zero;
			colliderRendererObj.transform.rotation = colliderRendererObj.transform.parent.rotation;
			colliderRendererObj.transform.localScale = Vector2.one;

		}

		colliderRendererObj?.TryGetComponent<SpriteShapeRenderer>(out sprRenderer);
		if(!sprRenderer){
			sprRenderer = colliderRendererObj.AddComponent<SpriteShapeRenderer>();
		}

		colliderRendererObj?.TryGetComponent<SpriteShapeController>(out sprController);
		if(!sprController){
        	sprController = colliderRendererObj.AddComponent<SpriteShapeController>();
		}
		
	}

    // Update is called once per frame
    void Update(){
        Draww();
    }

    void Draww(){
		if(!shapeProfile) return;

		if(!myCollider || !sprRenderer || !sprController || !colliderRendererObj){
			InstantiateObjects();
			return;
		}

		List<Vector2> pts = new List<Vector2>();

		if(myCollider is PolygonCollider2D){
			var polyColl = (PolygonCollider2D) myCollider;
			pts = new List<Vector2>(polyColl.points);

			for(int i=0;i<pts.Count;i++){
				pts[i] += myCollider.offset;
			}
		}else{
			var shapesGroup = new PhysicsShapeGroup2D();
			myCollider.GetShapes(shapesGroup);

			var _shapes = new List<PhysicsShape2D>();
			var _verts = new List<Vector2>();
			shapesGroup.GetShapeData(_shapes, pts);

			Transform2D t0 = new Transform2D();
			t0.position = transform.localPosition;
			t0.scale = transform.localScale;
			// t0.rotation = transform.rotation.z;
			
			for(int i=0; i<pts.Count;i++){
				Vector2 pt = pts[i];

				pts[i] = t0.ApplyInverseTransform(pt);
			}
		}

		pts.Reverse();

		bool same = true;

		if(pts.Count != myPoints.Count){
			same = false;
		}else{
			for(int i=0; i<pts.Count; i++){
				if(pts[i] != myPoints[i]){
					same = false;
					break;
				}
			}
		}

		if(same) return;

		myPoints = pts;

		var spline = sprController.spline;

		spline.Clear();

		sprController.spriteShape = shapeProfile;
		sprController.spriteShape.fillOffset = fillAmount;

		foreach(var pt in myPoints){
			try{
				spline.InsertPointAt(spline.GetPointCount(), pt);
				spline.SetTangentMode(spline.GetPointCount()-1, ShapeTangentMode.Linear);
				spline.SetHeight(spline.GetPointCount()-1, lineThickness);
			}catch(Exception e){
				Debug.LogError(e.StackTrace);
			}
		}
		sprController.splineDetail = (int) QualityDetail.High;

		sprRenderer.color = myColor;
		sprRenderer.sortingOrder = sortingOrder;
		sprController.BakeMesh();

		// Vector2 bl = Vector2.negativeInfinity;
		// Vector2 tr = Vector2.positiveInfinity;
		// foreach(var pt in myPoints){
		// 	if(pt.x > bl.x) bl.x = pt.x;
		// 	if(pt.x < tr.x) tr.x = pt.x;
		// 	if(pt.y > bl.y) bl.y = pt.y;
		// 	if(pt.y < tr.y) tr.y = pt.y;
		// }
		// Vector2 wh = new Vector2(tr.x-bl.x, tr.y-bl.y);

		// Vector2 center = new Vector2(bl.x+(wh.x/2), bl.y+(wh.y/2));

		// colliderRendererObj.transform.position = (Vector2)transform.InverseTransformPoint(transform.position) + center; // return to world space

        colliderRendererObj.transform.SetParent(transform);

    }
}