// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Description of BaseClassNode.
	/// </summary>
	public class BaseClassNode : AbstractNode
	{
		public string TypeName { get; set; }
		public string FullTypeName { get; set; }
		
		public BaseClassNode(string fullTypeName, string typeName)
		{
			this.TypeName = typeName;
			this.FullTypeName = fullTypeName;
		}
	}
}
