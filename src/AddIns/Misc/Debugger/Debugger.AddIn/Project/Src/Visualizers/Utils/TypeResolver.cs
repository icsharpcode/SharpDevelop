// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections;
using Debugger.MetaData;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Helper for obtaining information about DebugType.
	/// </summary>
	public static class TypeResolverExtension
	{
		public static bool ResolveIListImplementation(this DebugType type, out DebugType iListType, out DebugType itemType)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			iListType = null;
			itemType = null;
			// alternative: search val.Type.Interfaces for IList<T>, from it, get T
			// works when MyClass : IList<int> - MyClass is not generic, yet can be displayed
			iListType = type.GetInterface(typeof(IList).FullName);
			if (iListType != null)
			{
				List<DebugType> genericArguments = type.GenericArguments;
				if (genericArguments.Count == 1)
				{
					itemType = genericArguments[0];
					return true;
				}
			}
			return false;
		}
		
		public static bool ResolveIEnumerableImplementation(this DebugType type, out DebugType iEnumerableType, out DebugType itemType)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			iEnumerableType = null;
			itemType = null;
			// alternative: search val.Type.Interfaces for IList<T>, from it, get T
			// works when MyClass : IEnumerable<int> - MyClass is not generic, yet can be displayed
			iEnumerableType = type.GetInterface(typeof(IEnumerable).FullName);
			if (iEnumerableType != null)
			{
				List<DebugType> genericArguments = type.GenericArguments;
				if (genericArguments.Count == 1)
				{
					itemType = genericArguments[0];
					return true;
				}
			}
			return false;
		}
	}
}
