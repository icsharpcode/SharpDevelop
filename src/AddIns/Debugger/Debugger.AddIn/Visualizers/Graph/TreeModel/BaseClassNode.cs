// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
