// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace CSharpBinding.Completion
{
	class TypeCompletionData : CompletionData
	{
		readonly IType type;
		
		public IType Type {
			get { return type; }
		}
		
		public TypeCompletionData(IType type) : base(type.Name)
		{
			this.type = type;
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef != null)
				this.Description = typeDef.Documentation;
			this.Image = ClassBrowserIconService.GetIcon(type);
		}
	}
}
