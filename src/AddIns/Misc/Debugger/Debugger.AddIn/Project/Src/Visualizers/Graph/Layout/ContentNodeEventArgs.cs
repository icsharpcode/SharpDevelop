// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
