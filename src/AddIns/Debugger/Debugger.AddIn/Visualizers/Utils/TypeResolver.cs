// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using Debugger.MetaData;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Helper for obtaining information about DebugType.
	/// </summary>
	public static class TypeResolver
	{
		/// <summary>
		/// Resolves implementation of System.Collections.Generic.IList on this type.
		/// </summary>
		/// <param name="iListType">Result found implementation of System.Collections.Generic.IList.</param>
		/// <param name="itemType">The only generic argument of <paramref name="implementation"/></param>
		/// <returns>True if found, false otherwise.</returns>
		public static bool ResolveIListImplementation(this IType type, out ParameterizedType iListType, out IType itemType)
		{
			return type.ResolveKnownBaseType(KnownTypeCode.IListOfT, out iListType, out itemType);
		}
		
		/// <summary>
		/// Resolves implementation of System.Collections.Generic.IEnumerable on this type.
		/// </summary>
		/// <param name="iEnumerableType">Result found implementation of System.Collections.Generic.IEnumerable.</param>
		/// <param name="itemType">The only generic argument of <paramref name="implementation"/></param>
		/// <returns>True if found, false otherwise.</returns>
		public static bool ResolveIEnumerableImplementation(this IType type, out ParameterizedType iEnumerableType, out IType itemType)
		{
			return type.ResolveKnownBaseType(KnownTypeCode.IEnumerableOfT, out iEnumerableType, out itemType);
		}
		
		/// <summary>
		/// Finds IList&lt;T&gt; or IEnumerable&lt;T&gt; base type.
		/// </summary>
		/// <param name="fullNamePrefix">Type code to search for (IList&lt;T&gt; or IEnumerable&lt;T&gt;)</param></param>
		/// <param name="implementation">Found implementation.</param>
		/// <param name="itemType">The only generic argument of <paramref name="implementation"/></param>
		/// <returns>True if found, false otherwise.</returns>
		private static bool ResolveKnownBaseType(this IType type, KnownTypeCode knownTypeCode, out ParameterizedType implementation, out IType itemType)
		{
			if (type == null) throw new ArgumentNullException("type");
			implementation = null;
			itemType = null;
			ParameterizedType impl = 
				type.GetAllBaseTypes().OfType<ParameterizedType>().
				Where(t => t.IsKnownType(knownTypeCode) && t.TypeParameterCount == 1)
				.FirstOrDefault();
			if (impl != null) {
				implementation = impl;
				itemType = impl.GetTypeArgument(0);
				return true;
			}
			return false;
		}
	}
}
