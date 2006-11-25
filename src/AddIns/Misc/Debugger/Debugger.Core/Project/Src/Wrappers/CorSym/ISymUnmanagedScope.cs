// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public partial class ISymUnmanagedScope
	{
		public ISymUnmanagedScope[] Children {
			get {
				uint count;
				GetChildren(0, out count, new ISymUnmanagedScope[0]);
				ISymUnmanagedScope[] children = new ISymUnmanagedScope[count];
				GetChildren(count, out count, children);
				return children;
			}
		}
		
		public ISymUnmanagedVariable[] Locals {
			get {
				uint count;
				GetLocals(0, out count, new ISymUnmanagedVariable[0]);
				ISymUnmanagedVariable[] locals = new ISymUnmanagedVariable[count];
				GetLocals(count, out count, locals);
				return locals;
			}
		}
		
		public ISymUnmanagedNamespace[] Namespaces {
			get {
				uint count;
				GetNamespaces(0, out count, new ISymUnmanagedNamespace[0]);
				ISymUnmanagedNamespace[] namespaces = new ISymUnmanagedNamespace[count];
				GetNamespaces(0, out count, namespaces);
				return namespaces;
			}
		}
	}
}

#pragma warning restore 1591
