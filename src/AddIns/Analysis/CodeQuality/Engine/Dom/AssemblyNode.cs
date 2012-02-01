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
	public class AssemblyNode : NodeBase
	{
		public AssemblyNode(IAssembly assembly)
		{
			this.AssemblyInfo = assembly;
			namespaces = new List<NodeBase>();
		}
		
		public IAssembly AssemblyInfo { get; private set; }
		
		public override string Name {
			get { return AssemblyInfo.AssemblyName; }
		}
		
		internal List<NodeBase> namespaces;
		
		public override IList<NodeBase> Children {
			get { return namespaces; }
		}
	}
}
