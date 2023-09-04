using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
	public GameObject cam; // Game camera to be added for the parallax to work
	public Vector2 parallax = Vector2.zero; //parallax position and amount of parallax

	Vector3 prevCameraPos;

	public enum ProcessMode {
		IDLE, FIXED, LATE
	}

	public ProcessMode processMode = ProcessMode.IDLE;

    void Start(){

    }

    void LateUpdate(){
		if(processMode == ProcessMode.LATE){
			ProcessParallax();
		}
	}
    void Update(){
		if(processMode == ProcessMode.IDLE){
			ProcessParallax();
		}
	}
    void FixedUpdate(){
		if(processMode == ProcessMode.FIXED){
			ProcessParallax();
		}
	}

	void ProcessParallax(){
		if(!cam){
			return;
		}

		Vector3 currCameraPos = cam.transform.position;
		Vector3 mov = currCameraPos - prevCameraPos;

		if(mov == Vector3.zero){
			return;
		}

        var dist = new Vector2((mov.x * parallax.x), (mov.y * parallax.y));

		transform.position = new Vector3(transform.position.x + dist.x, transform.position.y + dist.y, transform.position.z);

		prevCameraPos = currCameraPos;
	}
}
