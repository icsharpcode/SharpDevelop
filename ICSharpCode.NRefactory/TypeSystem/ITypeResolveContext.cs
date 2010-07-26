// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Context representing the set of assemblies in which a type is being searched.
	/// </summary>
	[ContractClass(typeof(ITypeResolveContextContract))]
	public interface ITypeResolveContext
	{
		ITypeDefinition GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer);
	}
	
	[ContractClassFor(typeof(ITypeResolveContext))]
	abstract class ITypeResolveContextContract : ITypeResolveContext
	{
		ITypeDefinition ITypeResolveContext.GetClass(string fullTypeName, int typeParameterCount, StringComparer nameComparer)
		{
			Contract.Requires(fullTypeName != null);
			Contract.Requires(typeParameterCount >= 0);
			Contract.Requires(nameComparer != null);
			return null;
		}
	}
}
