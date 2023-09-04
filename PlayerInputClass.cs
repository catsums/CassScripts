using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInputsNS{
	public class PlayerInputs{
		public static void ProcessInputActions(MyInputAction[] inputActions, float delta){
			foreach(MyInputAction inputAction in inputActions){
				ProcessInputAction(inputAction, delta);
			}
		}
		public static void ProcessInputActions(List<MyInputAction> inputActions, float delta){
			ProcessInputActions(inputActions.ToArray(), delta);
			return;
		}
		public static void ProcessInputAction(MyInputAction inputAction, float delta){

			switch(inputAction.pressType){
				case InputPress.RELEASE:
					inputAction.pressType = InputPress.UP;
					if(inputAction.IsOnHoldTime()){
						break;
					}
					//Just do Unpress
					inputAction.UnPress();
					if(inputAction.IsOnPressTime() && !inputAction.IsOnHoldTime()){
						inputAction.tapCount ++;
						inputAction.currTapTime = 0;
					}else{
						inputAction.tapCount = 0;
					}
					inputAction.currPressTime = 0;
					break;
				case InputPress.UP:
					inputAction.UnPress();
					if(inputAction.IsOnPressTime() && !inputAction.IsOnHoldTime()){
						inputAction.tapCount ++;
						inputAction.currTapTime = 0;
					}else{
						inputAction.tapCount = 0;
					}
					inputAction.currPressTime = 0;
					break;
				case InputPress.DOWN:
					inputAction.currPressTime += delta;
					break;
				case InputPress.PRE_PRESS:
					inputAction.currPressTime += delta;
					if(inputAction.IsOnPressTime()){
						inputAction.pressType = InputPress.PRESS;
					}
					break;
				case InputPress.PRESS:
					inputAction.currPressTime += delta;
					inputAction.pressType = InputPress.POST_PRESS;
					break;
				case InputPress.POST_PRESS:
					inputAction.currPressTime += delta;
					if(inputAction.IsOnHoldTime()){
						inputAction.Hold();
					}
					break;
				default:
					inputAction.currPressTime = 0;
					break;
			}

			if(inputAction.IsTapped()){
				inputAction.currTapTime += delta;
				if(inputAction.IsOnTapTime()){
					inputAction.tapCount = 0;
					inputAction.currTapTime = 0;
				}
			}

			// if(inputAction.IsUp()){
			// 	inputAction.pressType = InputPress.UNPRESSED;
			// 	if(inputAction.IsOnPressTime() && !inputAction.IsOnHoldTime()){
			// 		inputAction.tapCount ++;
			// 		inputAction.currTapTime = 0;
			// 	}else{
			// 		inputAction.tapCount = 0;
			// 	}
			// 	inputAction.currPressTime = 0;
			// 	inputAction.currHoldTime = 0;
			// }else if(inputAction.IsDown()){
			// 	inputAction.currHoldTime += delta;
			// }else if(inputAction.IsJustPressed()){
			// 	inputAction.currPressTime += delta;
			// 	inputAction.currHoldTime += delta;
			// 	if(inputAction.IsOnHoldTime()){
			// 		inputAction.Hold();
			// 	}
			// }else{
			// 	inputAction.currPressTime = 0;
			// 	inputAction.currHoldTime = 0;
			// }

		}
	}
	public enum InputPress{
		UNPRESSED,
		PRE_PRESS, PRESS, POST_PRESS,
		DOWN, UP,
		RELEASE
	}

	[Serializable]
	public class MyInputAction{
		public string name = "";
		public object value;

		public InputPress pressType = InputPress.UNPRESSED;
		public InputPress justPressType = InputPress.UNPRESSED;

		public float pressTime = 0.2f;
		public float holdTime = 0.5f;
		public float tapTime = 0.1f;
		// [HideInInspector]
		public float currPressTime = 0;
		// [HideInInspector]
		public float currTapTime = 0;
		// [HideInInspector]
		public int tapCount = 0;

		public MyInputAction(){

		}

		public T Get<T>(){
			T val = default(T);
			if(value is T){
				return (T) value;
			}
			// try{
			// 	val = (T) value;
			// }catch (System.Exception){}
			return val;
		}
		public void Set(object val){
			value = val;
		}
		public bool IsPressed(){
			return (
				(pressType != InputPress.UNPRESSED) &&
				(pressType != InputPress.UP) &&
				(pressType != InputPress.RELEASE)
			);
			// return (pressType == InputPress.PRESS);
		}
		public bool IsReleased(){
			return (pressType == InputPress.UNPRESSED) || (pressType == InputPress.UP);
		}
		public bool IsJustPressed(){ return (pressType == InputPress.PRESS);}
		public bool IsTapped(){ return (tapCount > 0);}
		public bool IsDown(){ return (pressType == InputPress.DOWN);}
		public bool IsUp(){ return (pressType == InputPress.UP);}

		public bool IsOnPressTime(){
			return (currPressTime >= pressTime);
		}
		public bool IsOnHoldTime(){
			return (currPressTime >= (holdTime+pressTime));
		}
		public bool IsOnTapTime(){
			return (currTapTime >= tapTime);
		}

		public void Press(){
			pressType = InputPress.PRE_PRESS;
		}
		public void Hold(){
			if(IsPressed()){
				pressType = InputPress.DOWN;
			}
		}
		public void Release(){
			if(IsPressed()){
				pressType = InputPress.RELEASE;
			}
		}
		public void UnPress(){
			pressType = InputPress.UNPRESSED;
		}

		public void ResetPress(){
			currPressTime = 0f;
			currTapTime = 0f;
		}

		public void ResetAll(){
			currPressTime = 0f;
			currTapTime = 0f;
			tapCount = 0;
		}

	}

}
