// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class PropertyNode : NodeBase
	{
		public IProperty PropertyDefinition { get; private set; }
		
		public PropertyNode(IProperty propertyDefinition)
		{
			this.PropertyDefinition = propertyDefinition;
		}
		
		public override string Name {
			get { return PropertyDefinition.PrintFullName(); }
		}
		
		public override IList<NodeBase> Children {
			get { return EmptyChildren; }
		}
	}
}
