using System.Linq;
using UnityEngine;

namespace UnityExtensions
{
	public static class ComponentUtility
	{
		/// <summary>
		/// Finds a component living on either the gameobject, it's parent, then it's children.
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <param name="go">GameObject to check</param>
		/// <returns>Component of type `T` or null if not found.</returns>
		public static T FindComponent<T>(this GameObject go) where T : Component
		{
			var comp = go.GetComponent<T>();
			if (comp != null) return comp;
			comp = go.GetComponentInParent<T>();
			if (comp != null) return comp;
			comp = go.GetComponentInChildren<T>();
			if (comp != null) return comp;
			return null;
        }

		/// <summary>
		/// Finds all instances of components type `T` on GameObject and it's children.
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <param name="go">GameObject to check</param>
		/// <param name="includeInactive">Include components from inactive children.</param>
		/// <returns>Array of components type `T`.</returns>
		public static T[] GetAllComponents<T>(this GameObject go, bool includeInactive = true)
		{
			return go.GetComponents<T>().Concat(go.GetComponentsInChildren<T>(includeInactive)).ToArray();
		}
	}
}
