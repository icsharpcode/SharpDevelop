// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class NamespaceNode : NodeBase
	{
		public NamespaceNode(string name)
		{
			this.name = name;
			types = new List<NodeBase>();
		}
		
		string name;
		
		public override string Name {
			get { return name; }
		}
		
		List<NodeBase> types;
	
		public override IList<NodeBase> Children {
			get { return types; }
		}
	}
}
