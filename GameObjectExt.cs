using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectExt{
	public static class GameObjectExtensions{
		public static T[] GetComponentsInDirectChildren<T>( this GameObject gameObject ) {
			return GetComponentsInAncestors<T>(gameObject?.transform);
		}public static T[] GetComponentsInDirectChildren<T>( this Component currComponent ) {
			return GetComponentsInDirectChildren<T>(currComponent?.transform);
		}public static T[] GetComponentsInDirectChildren<T>( this Transform transform ) {
			List<T> components = new List<T>();
			for( int i = 0 ; i < transform.childCount ; ++i )
			{
				T[] comps = transform.GetChild( i ).GetComponents<T>();
				foreach(var comp in comps){
					if(comp!=null) components.Add(comp);
				}
			}
			return components.ToArray();
		}
		public static T GetComponentInDirectChildren<T>( this GameObject gameObject ) {
			return GetComponentInDirectChildren<T>(gameObject?.transform);
		}public static T GetComponentInDirectChildren<T>( this Component currComponent ) {
			return GetComponentInDirectChildren<T>(currComponent?.transform);
		}public static T GetComponentInDirectChildren<T>( this Transform transform ) {
			var comps = GetComponentsInAncestors<T>(transform);

			if(comps.Length > 0) return comps[0];
			return default(T);
		}
		

		public static T[] GetComponentsInAncestors<T>( this GameObject gameObject ) {
			return GetComponentsInAncestors<T>(gameObject?.transform);
		}public static T[] GetComponentsInAncestors<T>( this Component currComponent ) {
			return GetComponentsInAncestors<T>(currComponent?.transform);
		}public static T[] GetComponentsInAncestors<T>( this Transform transform ) {
			List<T> components = new List<T>();

			T[] parentComps = transform.GetComponentsInParent<T>();
			foreach(var comp in  parentComps){
				if(comp!=null) components.Add(comp);
			}

			if(transform.parent){
				T[] comps = GetComponentsInAncestors<T>(transform.parent);
				foreach(var comp in comps){
					if(comp!=null) components.Add(comp);
				}
			}
			return components.ToArray();
		}

		public static T GetComponentInAncestors<T>( this GameObject gameObject ) {
			return GetComponentInAncestors<T>(gameObject?.transform);
		}public static T GetComponentInAncestors<T>( this Component currComponent ) {
			return GetComponentInAncestors<T>(currComponent?.transform);
		}public static T GetComponentInAncestors<T>( this Transform transform ) {
			T parentComp = transform.GetComponentInParent<T>();
			if(parentComp!=null){
				return parentComp;
			}

			if(transform.parent){
				return GetComponentInAncestors<T>(transform.parent);
			}
			return default(T);
		}
		
	}
}