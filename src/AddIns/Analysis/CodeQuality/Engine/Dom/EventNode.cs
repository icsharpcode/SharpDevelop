// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Engine.Dom
{
	public class EventNode : NodeBase
	{
		public IEvent EventDefinition { get; private set; }
		
		public EventNode(IEvent eventDefinition)
		{
			this.EventDefinition = eventDefinition;
		}
		
		public override string Name {
			get { return EventDefinition.PrintFullName(); }
		}
		
		public override IList<NodeBase> Children {
			get { return EmptyChildren; }
		}
	}
}
