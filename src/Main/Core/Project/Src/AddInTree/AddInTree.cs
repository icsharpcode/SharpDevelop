// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of AddInTree.
	/// </summary>
	public sealed class AddInTree
	{
		readonly static string defaultCoreDirectory;
		
		static List<AddIn>   addIns   = new List<AddIn>();
		static AddInTreeNode rootNode = new AddInTreeNode();
		
		static Dictionary<string, IDoozer> doozers = new Dictionary<string, IDoozer>();
		static Dictionary<string, IConditionEvaluator> conditionEvaluators = new Dictionary<string, IConditionEvaluator>();
		
		static AddInTree()
		{
			defaultCoreDirectory = FileUtility.Combine(FileUtility.SharpDevelopRootPath, "AddIns");
			
			doozers.Add("Class", new ClassDoozer());
			doozers.Add("FileFilter", new FileFilterDoozer());
			doozers.Add("Icon", new IconDoozer());
			doozers.Add("MenuItem", new MenuItemDoozer());
			doozers.Add("ToolbarItem", new ToolbarItemDoozer());
			doozers.Add("Include", new IncludeDoozer());
			
			conditionEvaluators.Add("Compare", new CompareConditionEvaluator());
			conditionEvaluators.Add("Ownerstate", new OwnerStateConditionEvaluator());
		}
		
		public static List<AddIn> AddIns {
			get {
				return addIns;
			}
		}
		
		public static Dictionary<string, IDoozer> Doozers {
			get {
				return doozers;
			}
		}
		
		public static Dictionary<string, IConditionEvaluator> ConditionEvaluators {
			get {
				return conditionEvaluators;
			}
		}
		
		public static bool ExistsTreeNode(string path)
		{
			if (path == null || path.Length == 0) {
				return true;
			}
			
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			int i = 0;
			while (i < splittedPath.Length) {
				if (!curPath.ChildNodes.ContainsKey(splittedPath[i])) {
					return false;
				}
				curPath = curPath.ChildNodes[splittedPath[i]];
				++i;
			}
			return true;
		}
		
		public static AddInTreeNode GetTreeNode(string path)
		{
			return GetTreeNode(path, true);
		}
		
		public static AddInTreeNode GetTreeNode(string path, bool throwOnNotFound)
		{
			if (path == null || path.Length == 0) {
				return rootNode;
			}
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			int i = 0;
			while (i < splittedPath.Length) {
				if (!curPath.ChildNodes.ContainsKey(splittedPath[i])) {
					if (throwOnNotFound)
						throw new TreePathNotFoundException(path);
					else
						return null;
				}
				curPath = curPath.ChildNodes[splittedPath[i]];
				++i;
			}
			return curPath;
		}
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		public static object BuildItem(string path, object caller)
		{
			int pos = path.LastIndexOf('/');
			string parent = path.Substring(0, pos);
			string child = path.Substring(pos + 1);
			AddInTreeNode node = GetTreeNode(parent);
			return node.BuildChildItem(child, caller, BuildItems(path, caller, false));
		}
		
		/// <summary>
		/// Builds the items in the path.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="caller">The owner used to create the objects.</param>
		/// <param name="throwOnNotFound">If true, throws an TreePathNotFoundException
		/// if the path is not found. If false, an empty ArrayList is returned when the
		/// path is not found.</param>
		public static ArrayList BuildItems(string path, object caller, bool throwOnNotFound)
		{
			AddInTreeNode node = GetTreeNode(path, throwOnNotFound);
			if (node == null)
				return new ArrayList();
			else
				return node.BuildChildItems(caller);
		}
		
		static AddInTreeNode CreatePath(AddInTreeNode localRoot, string path)
		{
			if (path == null || path.Length == 0) {
				return localRoot;
			}
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = localRoot;
			int i = 0;
			while (i < splittedPath.Length) {
				if (!curPath.ChildNodes.ContainsKey(splittedPath[i])) {
					curPath.ChildNodes[splittedPath[i]] = new AddInTreeNode();
				}
				curPath = curPath.ChildNodes[splittedPath[i]];
				++i;
			}
			
			return curPath;
		}
		
		static void AddExtensionPath(ExtensionPath path)
		{
			AddInTreeNode treePath = CreatePath(rootNode, path.Name);
			foreach (Codon codon in path.Codons) {
				treePath.Codons.Add(codon);
			}
		}
		
		static void InsertAddIn(AddIn addIn)
		{
			foreach (ExtensionPath path in addIn.Paths.Values) {
				AddExtensionPath(path);
			}
			addIns.Add(addIn);
		}
		
		// As long as the show form takes 10 times of loading the xml representation I'm not implementing
		// binary serialization.
//		static Dictionary<string, ushort> nameLookupTable = new Dictionary<string, ushort>();
//		static Dictionary<AddIn, ushort> addInLookupTable = new Dictionary<AddIn, ushort>();
//
//		public static ushort GetAddInOffset(AddIn addIn)
//		{
//			return addInLookupTable[addIn];
//		}
//
//		public static ushort GetNameOffset(string name)
//		{
//			if (!nameLookupTable.ContainsKey(name)) {
//				nameLookupTable[name] = (ushort)nameLookupTable.Count;
//			}
//			return nameLookupTable[name];
//		}
//
//		public static void BinarySerialize(string fileName)
//		{
//			for (int i = 0; i < addIns.Count; ++i) {
//				addInLookupTable[addIns] = (ushort)i;
//			}
//			using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(fileName))) {
//				rootNode.BinarySerialize(writer);
//				writer.Write((ushort)addIns.Count);
//				for (int i = 0; i < addIns.Count; ++i) {
//					addIns[i].BinarySerialize(writer);
//				}
//				writer.Write((ushort)nameLookupTable.Count);
//				foreach (string name in nameLookupTable.Keys) {
//					writer.Write(name);
//				}
//			}
//		}
		public static void Load()
		{
			List<string> addInFiles = FileUtility.SearchDirectory(defaultCoreDirectory, "*.addin");
			foreach (string fileName in addInFiles) {
				InsertAddIn(AddIn.Load(fileName));
			}
		}
	}
}
