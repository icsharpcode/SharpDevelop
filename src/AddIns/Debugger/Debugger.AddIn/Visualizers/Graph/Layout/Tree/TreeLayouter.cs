// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
	public class TreeLayouter
	{
		private static readonly double horizNodeMargin = 30;
		private static readonly double vertNodeMargin = 30;
		
		GraphEdgeRouter edgeRouter = new GraphEdgeRouter();
		
		public TreeLayouter()
		{
		}
		
		/// <summary>
		/// Calculates layout for given <see cref="ObjectGraph" />.
		/// </summary>
		/// <param name="objectGraph"></param>
		/// <returns></returns>
		public PositionedGraph CalculateLayout(ObjectGraph objectGraph, LayoutDirection direction, Expanded expanded)
		{
			var positionedGraph = BuildPositionedGraph(objectGraph, direction, expanded);
			CalculateLayout(positionedGraph);
			this.edgeRouter.RouteEdges(positionedGraph);
			
			return positionedGraph;
		}
		
		PositionedGraph BuildPositionedGraph(ObjectGraph objectGraph, LayoutDirection direction, Expanded expanded)// Expanded is passed so that the correct ContentNodes are expanded in the PositionedNode
		{
			var treeNodeFor = new Dictionary<ObjectGraphNode, PositionedGraphNode>();
			var resultGraph = new PositionedGraph();
			
			// create empty PositionedNodes
			foreach (ObjectGraphNode objectGraphNode in objectGraph.ReachableNodes) {
				TreeGraphNode posNode = TreeGraphNode.Create(direction, objectGraphNode);
				posNode.InitContentFromObjectNode(expanded);
				posNode.HorizontalMargin = horizNodeMargin;
				posNode.VerticalMargin = vertNodeMargin;
				resultGraph.AddNode(posNode);
				treeNodeFor[objectGraphNode] = posNode;
			}
			
			// create edges
			foreach (PositionedGraphNode posNode in resultGraph.Nodes)
			{
				foreach (PositionedNodeProperty property in posNode.Properties)	{
					if (property.ObjectGraphProperty.TargetNode != null) {
						ObjectGraphNode targetObjectNode = property.ObjectGraphProperty.TargetNode;
						PositionedGraphNode edgeTarget = treeNodeFor[targetObjectNode];
						property.Edge = new TreeGraphEdge
						{ IsTreeEdge = false, Name = property.Name, Source = property, Target = edgeTarget };
					}
				}
			}
			resultGraph.Root = treeNodeFor[objectGraph.Root];
			return resultGraph;
		}

		void CalculateLayout(PositionedGraph resultGraph)
		{
			HashSet<PositionedGraphNode> seenNodes = new HashSet<PositionedGraphNode>();
			// first layout pass
			CalculateSubtreeSizes((TreeGraphNode)resultGraph.Root, seenNodes);
			// second layout pass
			CalculateNodePosRecursive((TreeGraphNode)resultGraph.Root, 0, 0);
		}
		
		// determines which edges are tree edges, and calculates subtree size for each node
		private void CalculateSubtreeSizes(TreeGraphNode root, HashSet<PositionedGraphNode> seenNodes)
		{
			seenNodes.Add(root);
			double subtreeSize = 0;
			
			foreach (PositionedNodeProperty property in root.Properties) {
				var edge = property.Edge as TreeGraphEdge;	// we know that these egdes are TreeEdges
				if (edge != null) {
					var neigborNode = (TreeGraphNode)edge.Target;
					if (seenNodes.Contains(neigborNode)) {
						edge.IsTreeEdge = false;
					} else {
						edge.IsTreeEdge = true;
						CalculateSubtreeSizes(neigborNode, seenNodes);
						subtreeSize += neigborNode.SubtreeSize;
					}
				}
			}
			root.Measure();
			root.SubtreeSize = Math.Max(root.LateralSizeWithMargin, subtreeSize);
		}
		
		
		/// <summary>
		/// Given SubtreeSize for each node, positions the nodes, in a left-to-right or top-to-bottom fashion.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="lateralStart"></param>
		/// <param name="mainStart"></param>
		private void CalculateNodePosRecursive(TreeGraphNode node, double lateralStart, double mainStart)
		{
			double childsSubtreeSize = node.Childs.Sum(child => child.SubtreeSize);
			// center this node
			double center = node.ChildEdges.Count() == 0 ? 0 : 0.5 * (childsSubtreeSize - (node.LateralSizeWithMargin));
			if (center < 0)	{
				// if root is larger than subtree, it would be shifted below lateralStart
				// -> make whole layout start at lateralStart
				lateralStart -= center;
			}
			
			// design alternatives
			// node.MainPos += center;  // used this
			// Adapt(node).PosLateral += center;    // TreeNodeAdapterLR + TreeNodeAdapterTB
			// SetMainPos(node, GetMainPos(node) + 10)  // TreeNodeAdapterLR + TreeNodeAdapterTB, no creation
			
			node.LateralCoord += lateralStart + center;
			node.MainCoord = mainStart;
			
			double childLateral = lateralStart;
			double childsMainFixed = node.MainCoord + node.MainSizeWithMargin;
			foreach (TreeGraphNode child in node.Childs) {
				CalculateNodePosRecursive(child, childLateral, childsMainFixed);
				childLateral += child.SubtreeSize;
			}
		}
	}
}
