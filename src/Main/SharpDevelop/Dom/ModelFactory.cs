// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class ModelFactory : IModelFactory
	{
		public IUpdateableAssemblyModel CreateAssemblyModel(IEntityModelContext context)
		{
			return new AssemblyModel(context);
		}
		
		public ITypeDefinitionModel CreateTypeDefinitionModel(IEntityModelContext context, params IUnresolvedTypeDefinition[] parts)
		{
			var model = new TypeDefinitionModel(context, parts[0]);
			for (int i = 1; i < parts.Length; i++) {
				model.Update(null, parts[i]);
			}
			return model;
		}
		
		public IMemberModel CreateMemberModel(IEntityModelContext context, IUnresolvedMember member)
		{
			return new MemberModel(context, member);
		}
	}
}
