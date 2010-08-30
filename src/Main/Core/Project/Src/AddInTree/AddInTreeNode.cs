// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents an extension path in the <see cref="AddInTree"/>.
	/// </summary>
	public sealed class AddInTreeNode
	{
		readonly object lockObj = new object();
		Dictionary<string, AddInTreeNode> childNodes = new Dictionary<string, AddInTreeNode>();
		ReadOnlyCollection<Codon> codons;
		List<IEnumerable<Codon>> codonInput;
		
		/// <summary>
		/// A dictionary containing the child paths.
		/// </summary>
		public Dictionary<string, AddInTreeNode> ChildNodes {
			get {
				return childNodes;
			}
		}
		
		public void AddCodons(IEnumerable<Codon> newCodons)
		{
			if (newCodons == null)
				throw new ArgumentNullException("newCodons");
			lock (lockObj) {
				if (codonInput == null) {
					codonInput = new List<IEnumerable<Codon>>();
					if (codons != null)
						codonInput.Add(codons);
				}
				codonInput.Add(newCodons);
			}
		}
		
		/// <summary>
		/// A list of child <see cref="Codon"/>s.
		/// </summary>
		public ReadOnlyCollection<Codon> Codons {
			get {
				lock (lockObj) {
					if (codons == null) {
						if (codonInput == null) {
							codons = new ReadOnlyCollection<Codon>(new Codon[0]);
						} else {
							codons = TopologicalSort.Sort(codonInput).AsReadOnly();
							codonInput = null;
						}
					}
					return codons;
				}
			}
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
		
		/// <summary>
		/// Builds the child items in this path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="caller">The owner used to create the objects.</param>
		public List<T> BuildChildItems<T>(object caller)
		{
			var codons = this.Codons;
			List<T> items = new List<T>(codons.Count);
			foreach (Codon codon in codons) {
				ArrayList subItems = null;
				if (childNodes.ContainsKey(codon.Id)) {
					subItems = childNodes[codon.Id].BuildChildItems(caller);
				}
				object result = codon.BuildItem(caller, subItems);
				if (result == null)
					continue;
				IBuildItemsModifier mod = result as IBuildItemsModifier;
				if (mod != null) {
					mod.Apply(items);
				} else if (result is T) {
					items.Add((T)result);
				} else {
					throw new InvalidCastException("The AddInTreeNode <" + codon.Name + " id='" + codon.Id
					                               + "'> returned an instance of " + result.GetType().FullName
					                               + " but the type " + typeof(T).FullName + " is expected.");
				}
			}
			return items;
		}
		
		/// <summary>
		/// Builds the child items in this path.
		/// </summary>
		/// <param name="caller">The owner used to create the objects.</param>
		public ArrayList BuildChildItems(object caller)
		{
			var codons = this.Codons;
			ArrayList items = new ArrayList(codons.Count);
			foreach (Codon codon in codons) {
				ArrayList subItems = null;
				if (childNodes.ContainsKey(codon.Id)) {
					subItems = childNodes[codon.Id].BuildChildItems(caller);
				}
				object result = codon.BuildItem(caller, subItems);
				if (result == null)
					continue;
				IBuildItemsModifier mod = result as IBuildItemsModifier;
				if (mod != null) {
					mod.Apply(items);
				} else {
					items.Add(result);
				}
			}
			return items;
		}
		
		/// <summary>
		/// Builds a specific child items in this path.
		/// </summary>
		/// <param name="childItemID">
		/// The ID of the child item to build.
		/// </param>
		/// <param name="caller">The owner used to create the objects.</param>
		/// <param name="subItems">The subitems to pass to the doozer</param>
		/// <exception cref="TreePathNotFoundException">
		/// Occurs when <paramref name="childItemID"/> does not exist in this path.
		/// </exception>
		public object BuildChildItem(string childItemID, object caller, ArrayList subItems)
		{
			foreach (Codon codon in this.Codons) {
				if (codon.Id == childItemID) {
					return codon.BuildItem(caller, subItems);
				}
			}
			throw new TreePathNotFoundException(childItemID);
		}
	}
}
