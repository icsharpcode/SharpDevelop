// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class ParameterListComparer : IEqualityComparer<IMethod>
	{
		Dictionary<IMethod, int> cachedHashes;
		
		/// <summary>
		/// Gets/Sets whether to cache hashcodes. This can improve performance for
		/// algorithms that repeatedly request the hash of a parameter list.
		/// </summary>
		/// <remarks>
		/// This class is thread-safe only when not using cached hashes.
		/// Also, cached hashes may cause a memory leak when the ParameterListComparer
		/// instance is kept alive.
		/// </remarks>
		public bool UseCachedHashes {
			get {
				return cachedHashes != null;
			}
			set {
				cachedHashes = value ? new Dictionary<IMethod, int>() : null;
			}
		}
		
		public bool Equals(IMethod x, IMethod y)
		{
			if (cachedHashes != null) {
				if (GetHashCode(x) != GetHashCode(y))
					return false;
			}
			var paramsX = x.Parameters;
			var paramsY = y.Parameters;
			if (paramsX.Count != paramsY.Count)
				return false;
			if (x.TypeParameters.Count != y.TypeParameters.Count)
				return false;
			for (int i = 0; i < paramsX.Count; i++) {
				IParameter px = paramsX[i];
				IParameter py = paramsY[i];
				if ((px.IsOut || px.IsRef) != (py.IsOut || py.IsRef))
					return false;
				if (!object.Equals(px.ReturnType, py.ReturnType))
					return false;
			}
			return true;
		}
		
		public int GetHashCode(IMethod obj)
		{
			int hashCode;
			if (cachedHashes != null && cachedHashes.TryGetValue(obj, out hashCode))
				return hashCode;
			hashCode = obj.TypeParameters.Count;
			unchecked {
				foreach (IParameter p in obj.Parameters) {
					hashCode *= 1000000579;
					if (p.IsOut || p.IsRef)
						hashCode += 1;
					if (p.ReturnType != null) {
						hashCode += p.ReturnType.GetHashCode();
					}
				}
			}
			if (cachedHashes != null)
				cachedHashes[obj] = hashCode;
			return hashCode;
		}
	}
}
