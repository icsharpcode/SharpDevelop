// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorSym
{
	using System;
	
	
	public static partial class CorSymExtensionMethods
	{
		public static ISymUnmanagedScope[] GetChildren(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetChildren(0, out count, new ISymUnmanagedScope[0]);
			ISymUnmanagedScope[] children = new ISymUnmanagedScope[count];
			symScope.GetChildren(count, out count, children);
			return children;
		}
		
		public static ISymUnmanagedVariable[] GetLocals(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetLocals(0, out count, new ISymUnmanagedVariable[0]);
			ISymUnmanagedVariable[] locals = new ISymUnmanagedVariable[count];
			symScope.GetLocals(count, out count, locals);
			return locals;
		}
		
		public static ISymUnmanagedNamespace[] GetNamespaces(this ISymUnmanagedScope symScope)
		{
			uint count;
			symScope.GetNamespaces(0, out count, new ISymUnmanagedNamespace[0]);
			ISymUnmanagedNamespace[] namespaces = new ISymUnmanagedNamespace[count];
			symScope.GetNamespaces(0, out count, namespaces);
			return namespaces;
		}
	}
}

#pragma warning restore 1591
