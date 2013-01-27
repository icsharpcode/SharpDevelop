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
		public static bool ResolveIListImplementation(this IType type, out IType iListType, out IType itemType)
		{
			return type.ResolveGenericInterfaceImplementation(
				"System.Collections.Generic.IList", out iListType, out itemType);
		}
		
		/// <summary>
		/// Resolves implementation of System.Collections.Generic.IEnumerable on this type.
		/// </summary>
		/// <param name="iEnumerableType">Result found implementation of System.Collections.Generic.IEnumerable.</param>
		/// <param name="itemType">The only generic argument of <paramref name="implementation"/></param>
		/// <returns>True if found, false otherwise.</returns>
		public static bool ResolveIEnumerableImplementation(this IType type, out IType iEnumerableType, out IType itemType)
		{
			return type.ResolveGenericInterfaceImplementation(
				"System.Collections.Generic.IEnumerable", out iEnumerableType, out itemType);
		}
		
		/// <summary>
		/// Resolves implementation of a single-generic-argument interface on this type.
		/// </summary>
		/// <param name="fullNamePrefix">Interface name to search for (eg. "System.Collections.Generic.IList")</param>
		/// <param name="implementation">Result found implementation.</param>
		/// <param name="itemType">The only generic argument of <paramref name="implementation"/></param>
		/// <returns>True if found, false otherwise.</returns>
		public static bool ResolveGenericInterfaceImplementation(this IType type, string fullNamePrefix, out ParameterizedType implementation, out IType itemType)
		{
			if (type == null) throw new ArgumentNullException("type");
			implementation = null;
			itemType = null;
			implementation = 
				type.GetAllBaseTypes().OfType<ParameterizedType>().
				Where(t => t.FullName.StartsWith(fullNamePrefix) && t.TypeParameterCount == 1)
				.FirstOrDefault();
			if (implementation != null) {
				itemType = implementation.GetTypeArgument(0);
				return true;
			}
			return false;
		}
	}
}
