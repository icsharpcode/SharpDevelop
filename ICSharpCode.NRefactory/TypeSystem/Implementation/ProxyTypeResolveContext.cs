// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Proxy that forwards calls to another TypeResolveContext.
	/// Useful as base class for decorators.
	/// </summary>
	public class ProxyTypeResolveContext : ITypeResolveContext
	{
		readonly ITypeResolveContext target;
		
		/// <summary>
		/// Creates a new ProxyTypeResolveContext.
		/// </summary>
		public ProxyTypeResolveContext(ITypeResolveContext target)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			this.target = target;
		}
		
		/// <inheritdoc/>
		public virtual ITypeDefinition GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer)
		{
			return target.GetClass(fullTypeName, typeParameterCount, nameComparer);
		}
		
		/// <inheritdoc/>
		public virtual IEnumerable<ITypeDefinition> GetClasses()
		{
			return target.GetClasses();
		}
		
		/// <inheritdoc/>
		public virtual IEnumerable<ITypeDefinition> GetClasses(string nameSpace, StringComparer nameComparer)
		{
			return target.GetClasses(nameSpace, nameComparer);
		}
		
		/// <inheritdoc/>
		public virtual IEnumerable<string> GetNamespaces()
		{
			return target.GetNamespaces();
		}
		
		/// <inheritdoc/>
		public virtual ISynchronizedTypeResolveContext Synchronize()
		{
			return target.Synchronize();
		}
	}
}
