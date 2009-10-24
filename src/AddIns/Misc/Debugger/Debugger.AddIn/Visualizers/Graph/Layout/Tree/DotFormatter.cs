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
	public abstract class DotFormatter
	{
		protected PositionedGraph posGraph;
		
		protected NeatoPositionTransform transform;
		
		protected NeatoDoubleFormatter neatoDoubleFormatter = new NeatoDoubleFormatter();
		
		// state (node and edge names) needed for parsing back
		protected Dictionary<PositionedGraphNode, string> nodeNames = new Dictionary<PositionedGraphNode, string>();
		protected Dictionary<PositionedEdge, string> edgeNames = new Dictionary<PositionedEdge, string>();
		
		/// <summary>
		/// Used for generating node and edge names.
		/// </summary>
		protected IdGenerator genId = new IdGenerator();
		
		public DotFormatter(PositionedGraph posGraph)
		{
			if (posGraph.Nodes.Count() == 0)
			{
				throw new ArgumentException("Cannot process empty graphs.");
			}
			this.posGraph = posGraph;
			this.transform = new NeatoPositionTransform(this.posGraph.BoundingRect);
		}
		
		protected abstract void appendPosNode(PositionedGraphNode node, StringBuilder builder);
		
		protected abstract void appendPosEdge(PositionedEdge edge, StringBuilder builder);
		
		protected virtual string getGraphHeader()
		{
			return "digraph G { node [shape = box];";
		}
		
		protected virtual string getGraphFooter()
		{
			return "}";
		}
		
		/// <summary>
		/// Gets Graphviz's dot format string for wrapped positioned graph.
		/// </summary>
		public string OutputGraphInDotFormat()
		{
			StringBuilder dotStringBuilder = new StringBuilder(getGraphHeader());
			
			foreach	(PositionedGraphNode posNode in this.posGraph.Nodes)
			{
				appendPosNode(posNode, dotStringBuilder);
			}
			foreach	(PositionedEdge posEdge in this.posGraph.Edges)
			{
				appendPosEdge(posEdge, dotStringBuilder);
			}
			
			dotStringBuilder.AppendLine(getGraphFooter());
			return dotStringBuilder.ToString();
		}
		
		private bool validateSplinePoints(PositionedEdge edge)
		{
			// must have correct number of points: one start point and 3 points per bezier segment
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
				PositionedGraphNode firstNode = posGraph.Nodes.First();
				Point firstNodePosOur = transform.AsNeato(firstNode.Center);
				// determine how Neato shifted the nodes
				transform.NeatoShiftX = neatoFirstNodePos.X - firstNodePosOur.X;
				transform.NeatoShiftY = neatoFirstNodePos.Y - firstNodePosOur.Y;
				
				// assume that edges on output are in the same order as posGraph.Edges (!)
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
			return double.Parse(readWord(reader), this.neatoDoubleFormatter.DoubleCulture);
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
