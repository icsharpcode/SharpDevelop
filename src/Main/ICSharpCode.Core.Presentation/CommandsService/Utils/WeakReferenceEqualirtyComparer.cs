using System;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{ 
	/// <summary>
	/// Compares two instances wrapped inside <see cref="WeakReference" />
	/// </summary>
	public class WeakReferenceTargetEqualirtyComparer : IEqualityComparer<WeakReference>
	{
		/// <summary>
		/// Determines whether two instances found in <see cref="WeakReference.Target" /> are equal
		/// </summary>
		/// <param name="container1">First <see cref="WeakReference" /> container</param>
		/// <param name="container2">Second <see cref="WeakReference" /> container</param>
		/// <returns>Returns <code>true</code> if instances in <see cref="WeakReference.Target" /> are equal; otherwise <code>false</code></returns>
		bool IEqualityComparer<WeakReference>.Equals(WeakReference container1, WeakReference container2) 
		{
			object value1 = null;
			object value2 = null;
			
			if (container1 != null) {
				value1 = container1.Target;
				if (value1 == null) {
					return container1 == container2;
				}
			}
			
			if (container2 != null) {
				value2 = container2.Target;
				if (value2 == null) {
					return container1 == container2;
				}
			}
			
			return value1 == value2;
		}
		
		/// <summary>
		/// Get hash code of instance found in <see cref="WeakReference.Target" />
		/// </summary>
		/// <param name="container"><see cref="WeakReference" /> container</param>
		/// <returns>Returns hash code of the instace found in <see cref="WeakReference.Target" /></returns>
		int IEqualityComparer<WeakReference>.GetHashCode(WeakReference container)
		{
			object target = null;
			
			if (container != null) {
				target = container.Target;
				if (target == null) {
					return container.GetHashCode();
				}
			}
			
			return target == null ? 0 : target.GetHashCode();
		}
	}
}
