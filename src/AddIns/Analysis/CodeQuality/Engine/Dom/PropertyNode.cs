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
		public override string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override IList<NodeBase> Children {
			get { return null; }
		}
	}
}
