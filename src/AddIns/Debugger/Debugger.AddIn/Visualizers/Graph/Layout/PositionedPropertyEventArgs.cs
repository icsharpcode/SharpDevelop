// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
