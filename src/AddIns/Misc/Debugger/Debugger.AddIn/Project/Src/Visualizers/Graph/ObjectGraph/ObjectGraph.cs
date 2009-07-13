// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Object graph built by <see cref="ObjectGraphBuilder"/>
	/// </summary>
    public class ObjectGraph
    {
    	/// <summary>
    	/// Root of the graph.
    	/// </summary>
    	public ObjectGraphNode Root { get; internal set; }
    	
    	/// <summary>
    	/// Adds node to the graph.
    	/// </summary>
    	/// <param name="node">node to be added</param>
    	internal void AddNode(ObjectGraphNode node)
    	{
    		_nodes.Add(node);
    	}
    	
    	private List<ObjectGraphNode> _nodes = new List<ObjectGraphNode>();
        /// <summary>
        /// All nodes in the graph.
        /// </summary>
        public IEnumerable<ObjectGraphNode> Nodes 
        {
            get { return _nodes; }
        }
       
        // HACK to support expanding/collapsing, because expanding is done by modifying ObjectGraph and rebuiling PosGraph
        public IEnumerable<ObjectGraphNode> ReachableNodes 
        {
        	get 
        	{
        		var seenNodes = new HashSet<ObjectGraphNode>();
        		determineReachableNodes(this.Root, seenNodes);
        		foreach	(var node in seenNodes)
        		{
        			yield return node;
        		}
        	}
        }
        private void determineReachableNodes(ObjectGraphNode root, HashSet<ObjectGraphNode> seenNodes)
        {
        	seenNodes.Add(root);
        	
        	foreach(var prop in root.Properties)
        	{
        		if (prop.TargetNode != null && !seenNodes.Contains(prop.TargetNode))
        			determineReachableNodes(prop.TargetNode, seenNodes);
        	}
        }
    }
}
