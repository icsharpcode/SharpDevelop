// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Represents a namespace within a single project.
	/// </summary>
	public interface INamespaceModel : ISymbolModel
	{
		string FullName { get; }
		
		INamespaceModel ParentNamespace { get; }
		IModelCollection<INamespaceModel> ChildNamespaces { get; }
		IModelCollection<ITypeDefinitionModel> Types { get; }
	}
}
