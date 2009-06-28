// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
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
		private Dictionary<PositionedNodeProperty, string> propertyIds = new Dictionary<PositionedNodeProperty, string>();
		
		public RecordDotFormatter(PositionedGraph posGraph) : base(posGraph)
		{
		}
		
		protected override string getGraphHeader()
		{
			return "digraph G { rankdir=LR; node [shape = record];";
		}
		
		protected override void appendPosNode(PositionedGraphNode node, StringBuilder builder)
		{
			string nodeName = genId.GetNextId().ToString();
			nodeNames[node] = nodeName;
			
			Rect neatoInput = transform.NodeToNeatoInput(node);
			
			StringBuilder recordLabel = new StringBuilder();
			for (int i = 0; i < node.Properties.Count; i++)
			{
				string propertyId = "f" + genId.GetNextId().ToString();
				propertyIds[node.Properties[i]] = propertyId;
				recordLabel.Append(string.Format("<{0}> l", propertyId));
				if (i < node.Properties.Count - 1)
				{
					recordLabel.Append("|");
				}
			}
			
			string dotFormatNode =
				string.Format(this.neatoDoubleFormatter,
				              "{0} [pos=\"{1},{2}!\" width=\"{3}\" height=\"{4}\" label=\"{5}\"];",
				              nodeName,
				              neatoInput.Location.X, neatoInput.Location.Y, neatoInput.Width, neatoInput.Height,
				              recordLabel.ToString());
			builder.AppendLine(dotFormatNode);
		}
		
		protected override void appendPosEdge(PositionedEdge edge, StringBuilder builder)
		{
			string sourceNodeName = nodeNames[edge.Source.ContainingNode];
			string sourcePropertyName = propertyIds[edge.Source];
			string targetNodeName = nodeNames[edge.Target];
			
			builder.AppendLine(string.Format("{0}:{1} -> {2}", sourceNodeName, sourcePropertyName, targetNodeName));
		}
	}
}
