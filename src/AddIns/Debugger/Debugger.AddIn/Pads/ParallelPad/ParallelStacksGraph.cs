// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using QuickGraph;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Contextual;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Controls;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public class ParallelStacksEdge : QuickGraph.Edge<ThreadStack>
	{
		public ParallelStacksEdge(ThreadStack source, ThreadStack target) : base(source, target)
		{ }
	}
	
	public class ParallelStacksGraph : BidirectionalGraph<ThreadStack, ParallelStacksEdge>
	{
		public ParallelStacksGraph()
		{ }
	}
	
	public class ParallelStacksGraphLayout : GraphLayout<ThreadStack, ParallelStacksEdge, ParallelStacksGraph> 
	{
		public ParallelStacksGraphLayout()
		{
			// TODO : Replace the current tree layout with EfficientSugiyama layout when Direction is available for this type of layout
			var par = new SimpleTreeLayoutParameters();
			par.LayerGap = 30;
			par.VertexGap = 50;
			par.Direction = LayoutDirection.BottomToTop;
			par.SpanningTreeGeneration = SpanningTreeGeneration.DFS;
			
			this.LayoutParameters = par;
		}
	}
}
