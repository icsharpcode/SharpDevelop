// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class AssemblyLoadErrorTreeNode : SharpTreeNode
	{
		public override object Text {
			get {
				return "(Assembly not loadable)";
			}
		}
		
		public override object Icon {
			get {
				return null;
			}
		}
	}
}


