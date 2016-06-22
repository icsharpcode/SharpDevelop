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
