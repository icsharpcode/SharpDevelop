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
		
		HashSet<PositionedGraphNode> seenNodes = new HashSet<PositionedGraphNode>();
		Dictionary<ObjectGraphNode, PositionedGraphNode> treeNodeFor = new Dictionary<ObjectGraphNode, PositionedGraphNode>();
		
		public TreeLayouter()
		{
		}
		
		/// <summary>
		/// Calculates layout for given <see cref="ObjectGraph" />.
		/// </summary>
		/// <param name="objectGraph"></param>
		/// <returns></returns>
		public PositionedGraph CalculateLayout(ObjectGraph objectGraph, LayoutDirection direction, ExpandedNodes expandedNodes)
		{
			layoutDirection = direction;

			treeNodeFor = new Dictionary<ObjectGraphNode, PositionedGraphNode>();
			seenNodes = new HashSet<PositionedGraphNode>();

			//TreeGraphNode tree = buildTreeRecursive(objectGraph.Root, expandedNodes);
			
			// convert ObjectGraph to PositionedGraph with TreeEdges
			PositionedGraph tree = buildTreeGraph(objectGraph, expandedNodes);
			// first pass
			calculateSubtreeSizes((TreeGraphNode)tree.Root);
			// second pass
			calculateNodePosRecursive((TreeGraphNode)tree.Root, 0, 0);
			
			var neatoRouter = new NeatoEdgeRouter();
			resultGraph = neatoRouter.CalculateEdges(resultGraph);
			
			return resultGraph;
		}
		
		private PositionedGraph buildTreeGraph(ObjectGraph objectGraph, ExpandedNodes expandedNodes)
		{
			resultGraph = new PositionedGraph();
			
			// create empty PosNodes
			foreach (ObjectGraphNode objectGraphNode in objectGraph.Nodes)
			{
				TreeGraphNode posNode = createNewTreeGraphNode(objectGraphNode); 
				resultGraph.AddNode(posNode);
				treeNodeFor[objectGraphNode] = posNode;
				posNode.InitView();
			}
			
			// copy Content for each node
			foreach (PositionedGraphNode posNode in resultGraph.Nodes)
			{
				posNode.InitContentFromObjectNode();
				posNode.FillView();
			}
			
			// create edges
			foreach (PositionedGraphNode posNode in resultGraph.Nodes)
			{
				// create edges outgoing from this posNode
				foreach (PositionedNodeProperty property in posNode.Properties)
				{
					property.IsExpanded = expandedNodes.IsExpanded(property.Expression.Code);
					
					if (property.ObjectGraphProperty.TargetNode != null)
					{
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
		
		private void calculateSubtreeSizes(TreeGraphNode root)
		{
			seenNodes.Add(root);
			double subtreeSize = 0;
			
			foreach (PositionedNodeProperty property in root.Properties)
			{
				var edge = property.Edge as TreeGraphEdge;	// we know that these egdes are TreeEdges
				if (edge != null)
				{
					var neigborNode = (TreeGraphNode)edge.Target;
					if (seenNodes.Contains(neigborNode))
					{
						edge.IsTreeEdge = false;
					}
					else
					{
						edge.IsTreeEdge = true;
						calculateSubtreeSizes(neigborNode);
						subtreeSize += neigborNode.SubtreeSize;
					}
				}
			}
			
			root.Measure();
			root.SubtreeSize = Math.Max(root.LateralSizeWithMargin, subtreeSize);
		}
		
		
		private TreeGraphNode createNewTreeGraphNode(ObjectGraphNode objectGraphNode)
		{
			var newGraphNode = TreeGraphNode.Create(this.layoutDirection, objectGraphNode);
			newGraphNode.HorizontalMargin = horizNodeMargin;
			newGraphNode.VerticalMargin = vertNodeMargin;
			return newGraphNode;
		}
		
		/*private TreeGraphNode buildTreeRecursive(ObjectGraphNode objectGraphNode, ExpandedNodes expandedNodes)
		{
			seenNodes.Add(objectGraphNode, null);
			
			TreeGraphNode newTreeNode = createNewTreeGraphNode(); 
			resultGraph.AddNode(newTreeNode);
			treeNodeFor[objectGraphNode] = newTreeNode;
			
			double subtreeSize = 0;
			foreach	(AbstractNode absNode in objectGraphNode.Content.Children)
			{
				var newTreeNodeContent = new NestedNodeViewModel();
				
				
				ObjectGraphProperty property = ((PropertyNode)absNode).Property;
				
				if (property.TargetNode != null)
				{
					ObjectGraphNode neighbor = property.TargetNode;
					TreeGraphNode targetTreeNode = null;
					bool newEdgeIsTreeEdge = false;
					if (seenNodes.ContainsKey(neighbor))
					{
						targetTreeNode = treeNodeFor[neighbor];
						newEdgeIsTreeEdge = false;
					}
					else
					{
						targetTreeNode = buildTreeRecursive(neighbor, expandedNodes);
						newEdgeIsTreeEdge = true;
						subtreeSize += targetTreeNode.SubtreeSize;
					}
					var posNodeProperty = newTreeNode.AddProperty(property, expandedNodes.IsExpanded(property.Expression.Code));
					posNodeProperty.Edge = new TreeGraphEdge { IsTreeEdge = newEdgeIsTreeEdge, Name = property.Name, Source = posNodeProperty, Target = targetTreeNode };
				}
				else
				{
					// property.Edge stays null
					newTreeNode.AddProperty(property, expandedNodes.IsExpanded(property.Expression.Code));
				}
			}
			
			newTreeNode.Measure();
			newTreeNode.SubtreeSize = Math.Max(newTreeNode.LateralSizeWithMargin, subtreeSize);
			
			return newTreeNode;
		}*/
		
		/// <summary>
		/// Given SubtreeSize for each node, positions the nodes, in a left-to-right or top-to-bottom fashion.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="lateralStart"></param>
		/// <param name="mainStart"></param>
		private void calculateNodePosRecursive(TreeGraphNode node, double lateralStart, double mainStart)
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
			foreach (TreeGraphNode child in node.Childs)
			{
				calculateNodePosRecursive(child, childLateral, childsMainFixed);
				childLateral += child.SubtreeSize;
			}
		}
	}
}
