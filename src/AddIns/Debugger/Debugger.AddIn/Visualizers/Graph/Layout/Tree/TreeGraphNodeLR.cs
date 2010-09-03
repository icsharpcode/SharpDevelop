// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using Debugger.AddIn.Visualizers.Graph.Drawing;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// TreeNode used in LR layout mode.
	/// </summary>
	public class TreeNodeLR: TreeGraphNode
	{
		public TreeNodeLR(ObjectGraphNode objectNode) : base(objectNode)
		{
		}
		
		public override double MainSize { get { return this.Width; } }
		public override double LateralSize { get { return this.Height ; } }
		
		public override double MainCoord { get { return this.Left; } set { this.Left = value; } }
		public override double LateralCoord { get { return this.Top; } set { this.Top = value; } }
		
		public override double MainMargin { get { return this.HorizontalMargin; } }
		public override double LateralMargin { get { return this.VerticalMargin; } }
	}
}
