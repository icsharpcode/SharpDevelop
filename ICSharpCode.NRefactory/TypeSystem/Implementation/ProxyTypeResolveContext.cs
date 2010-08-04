// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Proxy that forwards calls to another TypeResolveContext.
	/// Useful as base class for decorators.
	/// </summary>
	public class ProxyTypeResolveContext : ITypeResolveContext
	{
		ITypeResolveContext target;
		
		public ProxyTypeResolveContext(ITypeResolveContext target)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			this.target = target;
		}
		
		public virtual ITypeDefinition GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer)
		{
			return target.GetClass(fullTypeName, typeParameterCount, nameComparer);
		}
		
		public virtual ISynchronizedTypeResolveContext Synchronize()
		{
			return target.Synchronize();
		}
	}
}
