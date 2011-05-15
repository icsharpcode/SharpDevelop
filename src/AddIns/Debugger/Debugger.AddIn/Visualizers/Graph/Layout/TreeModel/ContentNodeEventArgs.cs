// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// EventArgs carrying <see cref="NestedNodeViewModel"/>.
	/// </summary>
	public class ContentNodeEventArgs : EventArgs
	{
		private ContentNode node;
		
		public ContentNodeEventArgs(ContentNode node)
		{
			this.node = node;
		}
		
		public ContentNode Node	{ get { return this.node; } }
	}
}
