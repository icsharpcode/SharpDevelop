// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// EventArgs carrying <see cref="PositionedNodeProperty"/>.
	/// </summary>
	public class PositionedPropertyEventArgs : EventArgs
	{
		private PositionedNodeProperty property;
		
		public PositionedPropertyEventArgs(PositionedNodeProperty property)
		{
			this.property = property;
		}
		
		public PositionedNodeProperty Property	{ get { return this.property; } }
	}
}
