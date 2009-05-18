// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Converts <see cref="PositionedGraph/> to Graphviz's string (dot format) and back (from plain format).
	/// </summary>
	public class DotGraph
	{
		private PositionedGraph posGraph;
		
		NeatoPositionTransform transform;
		
		// state (node and edge names) needed for converting back
		private Dictionary<PositionedNode, string> nodeNames = new Dictionary<PositionedNode, string>();
		private Dictionary<PositionedEdge, string> edgeNames = new Dictionary<PositionedEdge, string>();
		
		private CultureInfo formatCulture = CultureInfo.GetCultureInfo("en-US");
		
		/// <summary>
		/// Used for generating node and edge names.
		/// </summary>
		private IdGenerator genId = new IdGenerator();
		
		public DotGraph(PositionedGraph posGraph)
		{
			if (posGraph.Nodes.Count() == 0)
			{
				throw new ArgumentException("Cannot process empty graphs.");
			}
			this.posGraph = posGraph;
			this.transform = new NeatoPositionTransform(this.posGraph.BoundingRect);
		}
		
		/// <summary>
		/// Gets Graphviz's dot format string for the positioned graph.
		/// </summary>
		public string DotGraphString
		{
			get
			{
				StringBuilder dotStringBuilder = new StringBuilder("digraph G { node [shape = box];");
				
				foreach	(PositionedNode posNode in this.posGraph.Nodes)
				{
					appendPosNode(posNode, dotStringBuilder);
				}
				foreach	(PositionedEdge posEdge in this.posGraph.Edges)
				{
					appendPosEdge(posEdge, dotStringBuilder);
				}
				
				dotStringBuilder.AppendLine("}");
				return dotStringBuilder.ToString();
			}
		}
		
		private void appendPosNode(PositionedNode node, StringBuilder builder)
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
		
		private void appendPosEdge(PositionedEdge edge, StringBuilder builder)
		{
			string sourceNodeName = nodeNames[edge.SourceNode];
			string targetNodeName = nodeNames[edge.TargetNode];
			
			builder.AppendLine(string.Format("{0} -> {1}", sourceNodeName, targetNodeName));
		}
		
		private bool validateSplinePoints(PositionedEdge edge)
		{
			// must have correct number of points: one start point and 3 points for every bezier segment
			return ((edge.SplinePoints.Count - 1) % 3) == 0;
		}
		
		/// <summary>
		/// Parses edge positions (from Graphviz's plain format) and sets these positions to underlying positioned graph.
		/// </summary>
		/// <param name="dotGraphString">Graph with positions in Graphviz's plain format</param>
		/// <returns><see cref="PositionedGraph"/> with edge positions filled.</returns>
		public PositionedGraph ParseEdgePositions(string dotGraphString)
		{
			using (StringReader reader = new System.IO.StringReader(dotGraphString))
			{
				skipAfterPattern(reader, "node " + nodeNames[posGraph.Nodes.First()] + " ");
				Point neatoFirstNodePos = readPoint(reader);
				PositionedNode firstNode = posGraph.Nodes.First();
				Point firstNodePosOur = transform.AsNeato(firstNode.Center);
				// determine how Neato shifted the nodes
				transform.NeatoShiftX = neatoFirstNodePos.X - firstNodePosOur.X;
				transform.NeatoShiftY = neatoFirstNodePos.Y - firstNodePosOur.Y;
				
				foreach (PositionedEdge posEdge in posGraph.Edges)
				{
					skipAfterPattern(reader, "edge ");

					readWord(reader);		// source node name
					readWord(reader);		// target node name
					
					int splinePointCount = readInt(reader);
					for (int i = 0; i < splinePointCount; i++)
					{
						Point edgePoint = readPoint(reader);
						edgePoint = transform.FromNeatoOutput(edgePoint);
						posEdge.SplinePoints.Add(edgePoint);
					}

					bool edgeOk = validateSplinePoints(posEdge);
					if (!edgeOk)
						throw new DebuggerVisualizerException("Parsed edge invalid");
				}
			}
			// return original graph with filled edge positions
			return this.posGraph;
		}
		
		private Point readPoint(TextReader reader)
		{
			double x = readDouble(reader);
			double y = readDouble(reader);
			
			return new Point(x, y);
		}
		
		private double readDouble(TextReader reader)
		{
			return double.Parse(readWord(reader), formatCulture);
		}
		
		private int readInt(TextReader reader)
		{
			return int.Parse(readWord(reader));
		}
		
		private string readWord(TextReader reader)
		{
			StringBuilder word = new StringBuilder();
			int ch = ' ';
			while ((ch = reader.Read()) != ' ')
			{
				if (ch == -1 || ch == '\n' || ch == '\t')
					break;
				
				word.Append((char)ch);
			}
			return word.ToString();
		}
		
		private bool skipAfterPattern(StringReader reader, string pattern)
		{
			int ch = -1;
			int pIndex = 0;
			int pTarget = pattern.Length;
			while ((ch = reader.Read()) != -1)
			{
				if (ch == pattern[pIndex])
				{
					pIndex++;
					if (pIndex == pTarget)
						return true;
				}
				else
				{
					pIndex = 0;
				}
			}
			return false;
		}
	}
}
