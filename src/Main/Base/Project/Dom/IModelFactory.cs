// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Allows creating models using the default implementation in SharpDevelop.exe.
	/// </summary>
	[SDService]
	public interface IModelFactory
	{
		ITypeDefinitionModel CreateTypeDefinitionModel(IEntityModelContext context, params IUnresolvedTypeDefinition[] parts);
		IMemberModel CreateMemberModel(IEntityModelContext context, IUnresolvedMember member);
		IUpdateableAssemblyModel CreateAssemblyModel(IEntityModelContext context);
	}
}
