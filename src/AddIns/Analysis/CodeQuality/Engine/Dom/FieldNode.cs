// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class FieldNode : NodeBase
	{
		public IField FieldDefinition { get; private set; }
		
		public FieldNode(IField fieldDefinition)
		{
			this.FieldDefinition = fieldDefinition;
		}
		
		public override string Name {
			get { return FieldDefinition.PrintFullName(); }
		}
		
		public override IList<NodeBase> Children {
			get { return EmptyChildren; }
		}
	}
}
