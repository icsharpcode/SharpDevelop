// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.XamlBinding
{
	public class NodeWrapper {
		public string ElementName { get; set; }
		public string Name { get; set; }
		
		public int StartOffset { get; set; }
		public int EndOffset { get; set; }
		
		public IList<NodeWrapper> Children { get; set; }
	}
}
