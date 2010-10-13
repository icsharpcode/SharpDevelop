// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents a group of methods.
	/// </summary>
	public class MethodGroupResolveResult : ResolveResult
	{
		readonly ReadOnlyCollection<IMethod> methods;
		
		public MethodGroupResolveResult(IList<IMethod> methods) : base(SharedTypes.UnknownType)
		{
			if (methods == null)
				throw new ArgumentNullException("methods");
			this.methods = new ReadOnlyCollection<IMethod>(methods);
		}
		
		public ReadOnlyCollection<IMethod> Methods {
			get { return methods; }
		}
	}
}
