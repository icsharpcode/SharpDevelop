// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class MethodNode : NodeBase
	{
		public IMethod MethodDefinition { get; private set; }
		
		public int CyclomaticComplexity { get; set; }
		
		public MethodNode(IMethod methodDefinition)
		{
			this.MethodDefinition = methodDefinition;
		}
		
		public override string Name {
			get { return MethodDefinition.PrintFullName(); }
		}
		
		public override IList<NodeBase> Children {
			get { return EmptyChildren; }
		}
	}
}
