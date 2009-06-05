// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// <see cref="DotFormatter"/> that treats node as a atomic "box". Edges go from box to box.
	/// </summary>
	public class BoxDotFormatter : DotFormatter
	{
		public BoxDotFormatter(PositionedGraph posGraph) : base(posGraph)
		{
		}
		
		protected override void appendPosNode(PositionedNode node, StringBuilder builder)
		{
			string nodeName = genId.GetNextId().ToString();
			nodeNames[node] = nodeName;
			
			Rect neatoInput = transform.NodeToNeatoInput(node);
			
			string dotFormatNode =
				string.Format(formatCulture,
				              "{0} [pos=\"{1},{2}!\" width=\"{3}\" height=\"{4}\"];",
				              nodeName, neatoInput.Location.X, neatoInput.Location.Y, neatoInput.Width, neatoInput.Height);
			builder.AppendLine(dotFormatNode);
		}
		
		protected override void appendPosEdge(PositionedEdge edge, StringBuilder builder)
		{
			string sourceNodeName = nodeNames[edge.Source.ContainingNode];
			string targetNodeName = nodeNames[edge.Target];
			
			builder.AppendLine(string.Format("{0} -> {1}", sourceNodeName, targetNodeName));
		}
	}
}
