using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of AddInTreeNode.
	/// </summary>
	public class AddInTreeNode
	{
		Dictionary<string, AddInTreeNode> childNodes = new Dictionary<string, AddInTreeNode>();
		List<Codon> codons = new List<Codon>();
		bool isSorted = false;
		
		public Dictionary<string, AddInTreeNode> ChildNodes {
			get {
				return childNodes;
			}
		}
		
		public List<Codon> Codons {
			get {
				return codons;
			}
		}
		
		public AddInTreeNode()
		{
		}
//		
//		public void BinarySerialize(BinaryWriter writer)
//		{
//			if (!isSorted) {
//				(new SortCodons(this)).Execute();
//				isSorted = true;
//			}
//			writer.Write((ushort)codons.Count);
//			foreach (Codon codon in codons) {
//				codon.BinarySerialize(writer);
//			}
//			
//			writer.Write((ushort)childNodes.Count);
//			foreach (KeyValuePair<string, AddInTreeNode> child in childNodes) {
//				writer.Write(AddInTree.GetNameOffset(child.Key));
//				child.Value.BinarySerialize(writer);
//			}
//		}
		
		class SortCodons
		{
			bool[] visited;
			List<Codon> sortedCodons;
			Dictionary<string, int> indexOfName;
			AddInTreeNode node;
			
			public SortCodons(AddInTreeNode node)
			{
				this.node = node;
				visited = new bool[node.codons.Count];
				sortedCodons = new List<Codon>(node.codons.Count);
				indexOfName = new Dictionary<string, int>(node.codons.Count);
				// initialize visited to false and fill the indexOfName dictionary
				for (int i = 0; i < node.codons.Count; ++i) {
					visited[i] = false;
					indexOfName[node.codons[i].Id] = i;
				}
			}
			
			void InsertEdges()
			{
				// add the InsertBefore to the corresponding InsertAfter
				for (int i = 0; i < node.codons.Count; ++i) {
					string before = node.codons[i].InsertBefore;
					if (before != null && before != "") {
						if (indexOfName.ContainsKey(before)) {
							string after = node.codons[indexOfName[before]].InsertAfter;
							if (after == null || after == "") {
								node.codons[indexOfName[before]].InsertAfter = node.codons[i].Id;
							} else {
								after += ',' + node.codons[i].Id;
							}
						} else {
							Console.WriteLine("Codon ({0}) specified in the insertbefore of the {1} codon does not exist!", before, node.codons[i]);
						}
					}
				}
			}
			
			public void Execute()
			{
				
				InsertEdges();
				
				// Visit all codons
				for (int i = 0; i < node.codons.Count; ++i) {
					Visit(i);
				}
				node.codons = sortedCodons;
			}
			
			void Visit(int codonIndex)
			{
				if (visited[codonIndex]) {
					return;
				}
				string[] after = node.codons[codonIndex].InsertAfter.Split(new char[] {','});
				foreach (string s in after) {
					if (s == null || s.Length == 0) {
						continue;
					}
					if (indexOfName.ContainsKey(s)) {
						Visit(indexOfName[s]);
					} else {
						Console.WriteLine("Codon ({0}) specified in the insertafter of the {1} codon does not exist!", node.codons[codonIndex].InsertAfter, node.codons[codonIndex]);
					}
				}
				sortedCodons.Add(node.codons[codonIndex]);
				visited[codonIndex] = true;
			}
		}
		
		public ArrayList BuildChildItems(object caller)
		{
			ArrayList items = new ArrayList(codons.Count);
			if (!isSorted) {
				(new SortCodons(this)).Execute();
				isSorted = true;
			}
			foreach (Codon codon in codons) {
				ArrayList subItems = null;
				if (childNodes.ContainsKey(codon.Id)) {
					subItems = childNodes[codon.Id].BuildChildItems(caller);
				}
				items.Add(codon.BuildItem(caller, subItems));
			}
			return items;
		}
		
		public object BuildChildItem(string childItemID, object caller)
		{
			foreach (Codon codon in codons) {
				if (codon.Id == childItemID) {
					return codon.BuildItem(caller, null);
				}
			}
			return null;
		}
	}
}
