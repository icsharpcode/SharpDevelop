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
		
		public class TopologicalSort
		{
			List<Codon> codons;
			bool[] visited;
			List<Codon> sortedCodons;
			Dictionary<string, int> indexOfName;
			
			public TopologicalSort(List<Codon> codons)
			{
				this.codons = codons;
				visited = new bool[codons.Count];
				sortedCodons = new List<Codon>(codons.Count);
				indexOfName = new Dictionary<string, int>(codons.Count);
				// initialize visited to false and fill the indexOfName dictionary
				for (int i = 0; i < codons.Count; ++i) {
					visited[i] = false;
					indexOfName[codons[i].Id] = i;
				}
			}
			
			void InsertEdges()
			{
				// add the InsertBefore to the corresponding InsertAfter
				for (int i = 0; i < codons.Count; ++i) {
					string before = codons[i].InsertBefore;
					if (before != null && before != "") {
						if (indexOfName.ContainsKey(before)) {
							string after = codons[indexOfName[before]].InsertAfter;
							if (after == null || after == "") {
								codons[indexOfName[before]].InsertAfter = codons[i].Id;
							} else {
								codons[indexOfName[before]].InsertAfter = after + ',' + codons[i].Id;
							}
						} else {
							Console.WriteLine("Codon ({0}) specified in the insertbefore of the {1} codon does not exist!", before, codons[i]);
						}
					}
				}
			}
			
			public List<Codon> Execute()
			{
				InsertEdges();
				
				// Visit all codons
				for (int i = 0; i < codons.Count; ++i) {
					Visit(i);
				}
				return sortedCodons;
			}
			
			void Visit(int codonIndex)
			{
				if (visited[codonIndex]) {
					return;
				}
				string[] after = codons[codonIndex].InsertAfter.Split(new char[] {','});
				foreach (string s in after) {
					if (s == null || s.Length == 0) {
						continue;
					}
					if (indexOfName.ContainsKey(s)) {
						Visit(indexOfName[s]);
					} else {
						Console.WriteLine("Codon ({0}) specified in the insertafter of the {1} codon does not exist!", codons[codonIndex].InsertAfter, codons[codonIndex]);
					}
				}
				sortedCodons.Add(codons[codonIndex]);
				visited[codonIndex] = true;
			}
		}
		
		public ArrayList BuildChildItems(object caller)
		{
			ArrayList items = new ArrayList(codons.Count);
			if (!isSorted) {
				codons = (new TopologicalSort(codons)).Execute();
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
