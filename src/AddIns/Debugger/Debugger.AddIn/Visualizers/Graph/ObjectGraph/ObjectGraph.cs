// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Object graph built by <see cref="ObjectGraphBuilder"/>.
	/// </summary>
	/// <remarks>
	/// The graph is never empty.
	/// </remarks>
    public class ObjectGraph
    {
    	/// <summary>
    	/// Root of the graph. Should never be null.
    	/// </summary>
    	public ObjectGraphNode Root { get; internal set; }
    	
    	/// <summary>
    	/// Adds node to the graph.
    	/// </summary>
    	/// <param name="node">node to be added</param>
    	internal void AddNode(ObjectGraphNode node)
    	{
    		nodes.Add(node);
    	}
    	
    	private List<ObjectGraphNode> nodes = new List<ObjectGraphNode>();
        /// <summary>
        /// All nodes in the graph. Should always contain at least one node.
        /// </summary>
        public IEnumerable<ObjectGraphNode> Nodes 
        {
            get { return nodes; }
        }
       
        // HACK to support expanding/collapsing, because expanding is done by modifying ObjectGraph and rebuiling PosGraph
        public IEnumerable<ObjectGraphNode> ReachableNodes 
        {
        	get 
        	{
        		var reachableNodes = new HashSet<ObjectGraphNode>();
        		FindReachableNodesRecursive(this.Root, reachableNodes);
        		foreach	(var node in reachableNodes)	{
        			yield return node;
        		}
        	}
        }
        
        void FindReachableNodesRecursive(ObjectGraphNode root, HashSet<ObjectGraphNode> seenNodes)
        {
        	seenNodes.Add(root);
        	foreach(var prop in root.Properties) {
        		if (prop.TargetNode != null && !seenNodes.Contains(prop.TargetNode))
        			FindReachableNodesRecursive(prop.TargetNode, seenNodes);
        	}
        }
    }
}
