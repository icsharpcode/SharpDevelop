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
	public class NestedNodeViewModelEventArgs : EventArgs
	{
		private NestedNodeViewModel node;
		
		public NestedNodeViewModelEventArgs(NestedNodeViewModel node)
		{
			this.node = node;
		}
		
		public NestedNodeViewModel Node	{ get { return this.node; } }
	}
}
