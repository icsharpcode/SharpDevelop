using System;
using System.Collections.Generic;

namespace ICSharpCode.Core.Presentation
{ 
	public class WeakReferenceEqualirtyComparer : IEqualityComparer<WeakReference>
	{
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
