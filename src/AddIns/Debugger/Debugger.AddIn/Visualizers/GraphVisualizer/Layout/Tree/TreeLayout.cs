// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Windows;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Calculates layout of <see cref="ObjectGraph" />, producing <see cref="PositionedGraph" />.
	/// </summary>
	public class TreeLayout
	{
		static readonly double NodeMarginH = 30;
		static readonly double NodeMarginV = 30;
		static readonly double MarginTop = 0;
		static readonly double MarginBottom = 0;
		
		
		GraphEdgeRouter edgeRouter = new GraphEdgeRouter();
		/// <summary>
		/// The produced layout is either a horizontal or vertical tree.
		/// </summary>
		public LayoutDirection LayoutDirection { get; private set; }
		
		public TreeLayout(LayoutDirection layoutDirection)
		{
			this.LayoutDirection = layoutDirection;
		}
		
		/// <summary>
		/// Calculates layout for given <see cref="ObjectGraph" />.
		/// </summary>
		/// <param name="objectGraph"></param>
		/// <returns></returns>
		public PositionedGraph CalculateLayout(ObjectGraph objectGraph, Expanded expanded)
		{
			var positionedGraph = BuildPositionedGraph(objectGraph, expanded);
			CalculateLayout(positionedGraph);
			this.edgeRouter.RouteEdges(positionedGraph);
			
			return positionedGraph;
		}
		
		// Expanded is passed so that the correct ContentNodes are expanded in the PositionedNode
		PositionedGraph BuildPositionedGraph(ObjectGraph objectGraph, Expanded expanded)
		{
			var positionedNodeFor = new Dictionary<ObjectGraphNode, PositionedNode>();
			var positionedGraph = new PositionedGraph();
			
			// create empty PositionedNodes
			foreach (ObjectGraphNode objectNode in objectGraph.ReachableNodes) {
				var posNode = new PositionedNode(objectNode, expanded);
				posNode.MeasureVisualControl();
				positionedGraph.AddNode(posNode);
				positionedNodeFor[objectNode] = posNode;
			}
			
			// create edges
			foreach (PositionedNode posNode in positionedGraph.Nodes)
			{
				foreach (PositionedNodeProperty property in posNode.Properties)	{
					if (property.ObjectGraphProperty.TargetNode != null) {
						ObjectGraphNode targetObjectNode = property.ObjectGraphProperty.TargetNode;
						PositionedNode edgeTarget = positionedNodeFor[targetObjectNode];
						property.Edge = new PositionedEdge {
							Name = property.Name, Source = property, Target = edgeTarget
						};
					}
				}
			}
			positionedGraph.Root = positionedNodeFor[objectGraph.Root];
			return positionedGraph;
		}

		void CalculateLayout(PositionedGraph positionedGraph)
		{
			// impose a tree structure on the graph
			HashSet<PositionedEdge> treeEdges = DetermineTreeEdges(positionedGraph.Root);
			// first layout pass
			CalculateSubtreeSizesRecursive(positionedGraph.Root, treeEdges);
			// second layout pass
			CalculateNodePosRecursive(positionedGraph.Root, treeEdges, MarginTop, MarginBottom);
		}
		
		
		HashSet<PositionedEdge> DetermineTreeEdges(PositionedNode root)
		{
			var treeEdges = new HashSet<PositionedEdge>();
			
			var seenNodes = new HashSet<PositionedNode>();
			var q = new Queue<PositionedNode>();
			q.Enqueue(root);
			seenNodes.Add(root);
			
			while (q.Count > 0) {
				var node = q.Dequeue();
				foreach (var property in node.Properties) {
					var edge = property.Edge;
					if (edge != null && edge.Target != null) {
						if (!seenNodes.Contains(edge.Target)) {
							treeEdges.Add(edge);
							seenNodes.Add(edge.Target);
							q.Enqueue(edge.Target);
						}
					}
				}
			}
			return treeEdges;
		}
		
		void CalculateSubtreeSizesRecursive(PositionedNode root, HashSet<PositionedEdge> treeEdges)
		{
			double subtreeSize = 0;
			foreach (var child in TreeChildNodes(root, treeEdges)) {
				CalculateSubtreeSizesRecursive(child, treeEdges);
				// just sum up the sizes of children
				subtreeSize += child.SubtreeSize;
			}
			root.SubtreeSize = Math.Max(GetLateralSizeWithMargin(root), subtreeSize);
		}


		/// <summary>
		/// Given SubtreeSize for each node, positions the nodes, in a left-to-right or top-to-bottom layout.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="lateralStart"></param>
		/// <param name="mainStart"></param>
		void CalculateNodePosRecursive(PositionedNode node, HashSet<PositionedEdge> treeEdges, double lateralBase, double mainBase)
		{
			double childsSubtreeSize = TreeChildNodes(node, treeEdges).Sum(child => child.SubtreeSize);
			double center = TreeEdges(node, treeEdges).Count() == 0 ? 0 : 0.5 * (childsSubtreeSize - (GetLateralSizeWithMargin(node)));
			if (center < 0)	{
				// if root is larger than subtree, it would be shifted below lateralStart
				// -> make whole layout start at lateralStart
				lateralBase -= center;
			}
			
			SetLateral(node, GetLateral(node) + lateralBase + center);
			SetMain(node, mainBase);
			
			double childLateral = lateralBase;
			double childsMainFixed = GetMain(node) + GetMainSizeWithMargin(node);
			foreach (var child in TreeChildNodes(node, treeEdges)) {
				CalculateNodePosRecursive(child, treeEdges, childLateral, childsMainFixed);
				childLateral += child.SubtreeSize;
			}
		}

		IEnumerable<PositionedEdge> TreeEdges(PositionedNode node, HashSet<PositionedEdge> treeEdges)
		{
			return node.Edges.Where(e => treeEdges.Contains(e));
		}

		IEnumerable<PositionedNode> TreeChildNodes(PositionedNode node, HashSet<PositionedEdge> treeEdges)
		{
			return TreeEdges(node, treeEdges).Select(e => e.Target);
		}

		#region Horizontal / vertical layout helpers

		double GetMainSizeWithMargin(PositionedNode node)
		{
			return (this.LayoutDirection == LayoutDirection.LeftRight) ? node.Width + NodeMarginH : node.Height + NodeMarginV;
		}

		double GetLateralSizeWithMargin(PositionedNode node)
		{
			return (this.LayoutDirection == LayoutDirection.LeftRight) ? node.Height + NodeMarginV : node.Width + NodeMarginH;
		}

		double GetMain(PositionedNode node)
		{
			return (this.LayoutDirection == LayoutDirection.LeftRight) ? node.Left : node.Top;
		}

		double GetLateral(PositionedNode node)
		{
			return (this.LayoutDirection == LayoutDirection.LeftRight) ? node.Top : node.Left;
		}

		void SetMain(PositionedNode node, double value)
		{
			if (this.LayoutDirection == LayoutDirection.LeftRight) {
				node.Left = value;
			} else {
				node.Top = value;
			}
		}

		void SetLateral(PositionedNode node, double value)
		{
			if (this.LayoutDirection == LayoutDirection.LeftRight) {
				node.Top = value;
			} else {
				node.Left = value;
			}
		}

		#endregion
	}
}
