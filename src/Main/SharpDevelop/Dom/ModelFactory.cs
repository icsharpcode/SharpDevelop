// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class ModelFactory : IModelFactory
	{
		public IMutableTypeDefinitionModelCollection CreateTopLevelTypeDefinitionCollection(IEntityModelContext context)
		{
			return new TopLevelTypeDefinitionModelCollection(context);
		}
		
		public ITypeDefinitionModel CreateTypeDefinitionModel(IEntityModelContext context, params IUnresolvedTypeDefinition[] parts)
		{
			return new TypeDefinitionModel(context, parts);
		}
		
		public IMemberModel CreateMemberModel(IEntityModelContext context, IUnresolvedMember member)
		{
			return new MemberModel(context, member);
		}
	}
}
