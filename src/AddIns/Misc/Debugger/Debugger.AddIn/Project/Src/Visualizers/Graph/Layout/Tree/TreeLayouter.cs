// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
		
		private LayoutDirection layoutDirection = LayoutDirection.TopBottom;
		
		PositionedGraph resultGraph = null;
		
		Dictionary<ObjectNode, TreeNode> treeNodeFor = new Dictionary<ObjectNode, TreeNode>();
		Dictionary<ObjectNode, object> seenNodes = new Dictionary<ObjectNode, object>();
		
		public TreeLayouter()
		{
		}
		
		/// <summary>
		/// Calculates layout for given <see cref="ObjectGraph" />.
		/// </summary>
		/// <param name="objectGraph"></param>
		/// <returns></returns>
		public PositionedGraph CalculateLayout(ObjectGraph objectGraph, LayoutDirection direction)
		{
			layoutDirection = direction;

			resultGraph = new PositionedGraph();
			treeNodeFor = new Dictionary<ObjectNode, TreeNode>();
			seenNodes = new Dictionary<ObjectNode, object>();

			TreeNode tree = buildTreeRecursive(objectGraph.Root);
			calculateNodePosRecursive(tree, 0, 0);
			
			var neatoRouter = new NeatoEdgeRouter();
			resultGraph = neatoRouter.CalculateEdges(resultGraph);
			
			return resultGraph;
		}
		
		private TreeNode buildTreeRecursive(ObjectNode objectGraphNode)
		{
			seenNodes.Add(objectGraphNode, null);
			
			TreeNode newTreeNode = TreeNode.Create(this.layoutDirection, objectGraphNode);
			newTreeNode.HorizontalMargin = horizNodeMargin;
			newTreeNode.VerticalMargin = vertNodeMargin;
			resultGraph.nodes.Add(newTreeNode);
			treeNodeFor[objectGraphNode] = newTreeNode;
			
			double subtreeSize = 0;
			foreach	(ObjectProperty property in objectGraphNode.Properties)
			{
				if (property.TargetNode != null)
				{
					ObjectNode neighbor = property.TargetNode;
					TreeNode targetTreeNode = null;
					bool newEdgeIsTreeEdge = false;
					if (seenNodes.ContainsKey(neighbor))
					{
						targetTreeNode = treeNodeFor[neighbor];
						newEdgeIsTreeEdge = false;
					}
					else
					{
						targetTreeNode = buildTreeRecursive(neighbor);
						newEdgeIsTreeEdge = true;
						subtreeSize += targetTreeNode.SubtreeSize;
					}
					var posNodeProperty = newTreeNode.AddProperty(property);
					posNodeProperty.Edge = new TreeGraphEdge { IsTreeEdge = newEdgeIsTreeEdge, Name = property.Name, Source = posNodeProperty, Target = targetTreeNode };
				}
				else
				{
					// PositionedNodeProperty.Edge is null
					newTreeNode.AddProperty(property);
				}
			}
			subtreeSize = Math.Max(newTreeNode.LateralSizeWithMargin, subtreeSize);
			newTreeNode.SubtreeSize = subtreeSize;
			
			return newTreeNode;
		}
		
		/// <summary>
		/// Given SubtreeSize for each node, positions the nodes, in a left-to-right or top-to-bottom fashion.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="lateralStart"></param>
		/// <param name="mainStart"></param>
		private void calculateNodePosRecursive(TreeNode node, double lateralStart, double mainStart)
		{
			double childsSubtreeSize = node.Childs.Sum(child => child.SubtreeSize);
			// center this node
			double center = node.ChildEdges.Count() == 0 ? 0 : 0.5 * (childsSubtreeSize - (node.LateralSizeWithMargin));
			if (center < 0)
			{
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
			foreach (TreeNode child in node.Childs)
			{
				calculateNodePosRecursive(child, childLateral, childsMainFixed);
				childLateral += child.SubtreeSize;
			}
		}
	}
}
