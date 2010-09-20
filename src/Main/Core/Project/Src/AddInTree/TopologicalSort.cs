// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Supports sorting codons using InsertBefore/InsertAfter
	/// </summary>
	static class TopologicalSort
	{
		sealed class Node
		{
			internal Codon codon;
			internal bool visited;
			internal List<Node> previous = new List<Node>();
			
			internal void Visit(List<Codon> output)
			{
				if (visited)
					return;
				visited = true;
				foreach (Node n in previous)
					n.Visit(output);
				output.Add(codon);
			}
		}
		
		public static List<Codon> Sort(IEnumerable<IEnumerable<Codon>> codonInput)
		{
			// Step 1: create nodes for graph
			Dictionary<string, Node> nameToNodeDict = new Dictionary<string, Node>();
			List<Node> allNodes = new List<Node>();
			foreach (IEnumerable<Codon> codonList in codonInput) {
				// create entries to preserve order within
				Node previous = null;
				foreach (Codon codon in codonList) {
					Node node = new Node();
					node.codon = codon;
					if (!string.IsNullOrEmpty(codon.Id))
						nameToNodeDict[codon.Id] = node;
					// add implicit edges
					if (previous != null)
						node.previous.Add(previous);
					
					allNodes.Add(node);
					previous = node;
				}
			}
			// Step 2: create edges from InsertBefore/InsertAfter values
			foreach (Node node in allNodes) {
				if (!string.IsNullOrEmpty(node.codon.InsertBefore)) {
					foreach (string beforeReference in node.codon.InsertBefore.Split(',')) {
						Node referencedNode;
						if (nameToNodeDict.TryGetValue(beforeReference, out referencedNode)) {
							referencedNode.previous.Add(node);
						} else {
							LoggingService.WarnFormatted("Codon ({0}) specified in the insertbefore of the {1} codon does not exist!", beforeReference, node.codon);
						}
					}
				}
				if (!string.IsNullOrEmpty(node.codon.InsertAfter)) {
					foreach (string afterReference in node.codon.InsertAfter.Split(',')) {
						Node referencedNode;
						if (nameToNodeDict.TryGetValue(afterReference, out referencedNode)) {
							node.previous.Add(referencedNode);
						} else {
							LoggingService.WarnFormatted("Codon ({0}) specified in the insertafter of the {1} codon does not exist!", afterReference, node.codon);
						}
					}
				}
			}
			// Step 3: Perform Topological Sort
			List<Codon> output = new List<Codon>();
			foreach (Node node in allNodes) {
				node.Visit(output);
			}
			return output;
		}
	}
}
