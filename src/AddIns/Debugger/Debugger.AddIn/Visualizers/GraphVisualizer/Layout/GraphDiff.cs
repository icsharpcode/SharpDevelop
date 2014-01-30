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
using System.Collections.ObjectModel;
using ICSharpCode.SharpDevelop;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Describes changes which occured between 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphDiff
	{
		private List<PositionedNode> addedNodes = new List<PositionedNode>();
		private List<PositionedNode> deletedNodes = new List<PositionedNode>();
		private List<PositionedNode> changedNodes = new List<PositionedNode>();
		private Dictionary<PositionedNode, PositionedNode> matching = new Dictionary<PositionedNode, PositionedNode>();
		
		/// <summary>
		/// Nodes in the new graph that were added.
		/// </summary>
		public IList<PositionedNode> AddedNodes
		{
			get { return addedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that were removed.
		/// </summary>
		public IList<PositionedNode> RemovedNodes
		{
			get { return deletedNodes.AsReadOnly(); }
		}
		
		/// <summary>
		/// Nodes in the old graph that are present also in the new graph (they represent the same debuggee instance).
		/// The matching new nodes can be obtained by <see cref="GetMatchingNewNode"/>.
		/// </summary>
		public IList<PositionedNode> ChangedNodes
		{
			get { return changedNodes.AsReadOnly(); }
		}
		
		public PositionedNode GetMatchingNewNode(PositionedNode oldNode)
		{
			return matching.GetOrDefault(oldNode);
		}
		
		internal void SetAdded(PositionedNode addedNode)
		{
			addedNodes.Add(addedNode);
		}
		
		internal void SetRemoved(PositionedNode removeddNode)
		{
			deletedNodes.Add(removeddNode);
		}
		
		internal void SetMatching(PositionedNode matchFrom, PositionedNode matchTo)
		{
			matching[matchFrom] = matchTo;
			changedNodes.Add(matchFrom);
		}
	}
}
