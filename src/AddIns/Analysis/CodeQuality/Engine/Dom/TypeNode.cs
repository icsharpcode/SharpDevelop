// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class TypeNode : NodeBase
	{
		public ITypeDefinition TypeDefinition { get; private set; }
		
		public TypeNode(ITypeDefinition typeDefinition)
		{
			this.TypeDefinition = typeDefinition;
			children = new List<NodeBase>();
		}
		
		public override string Name {
			get { return TypeDefinition.Name; }
		}
		
		List<NodeBase> children;
		
		public override IList<NodeBase> Children {
			get { return children; }
		}
	}
}
