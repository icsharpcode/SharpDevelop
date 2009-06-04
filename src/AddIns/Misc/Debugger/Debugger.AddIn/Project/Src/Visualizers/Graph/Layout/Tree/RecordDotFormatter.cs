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
	/// <see cref="DotFormatter"/> that treats nodes as records of properties. 
	/// Edges start at property, end at node.
	/// </summary>
	public class RecordDotFormatter : DotFormatter
	{
		public RecordDotFormatter(PositionedGraph posGraph) : base(posGraph)
		{
		}
		
		protected override void appendPosNode(PositionedNode node, StringBuilder builder)
		{
			string nodeName = genId.GetNextId().ToString();
			nodeNames[node] = nodeName;
			
			Rect neatoInput = transform.NodeToNeatoInput(node);
			
			/*LEFT [
	pos="0,0!"
	width="1", height="1"
	label = "<f0> a| <f1> b| <f2> c|<f3>d"];*/

			StringBuilder recordLabel = new StringBuilder();
			
			
			string dotFormatNode =
				string.Format(formatCulture,
				              "{0} [pos=\"{1},{2}!\" width=\"{3}\" height=\"{4}\" label=\"{5}\"];",
				              nodeName, 
				              neatoInput.Location.X, neatoInput.Location.Y, neatoInput.Width, neatoInput.Height,
				              recordLabel.ToString());
			builder.AppendLine(dotFormatNode);
		}
		
		protected override void appendPosEdge(PositionedEdge edge, StringBuilder builder)
		{
			string sourceNodeName = nodeNames[edge.SourceNode];
			string targetNodeName = nodeNames[edge.TargetNode];
			
			builder.AppendLine(string.Format("{0} -> {1}", sourceNodeName, targetNodeName));
		}
	}
}
