using System;
using System.IO;
using System.Reflection;
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
		
		static Dictionary<string, IErbauer> erbauer = new Dictionary<string, IErbauer>();
		static Dictionary<string, IAuswerter> auswerter = new Dictionary<string, IAuswerter>();
		
		static AddInTree()
		{
			try {
				// might fail in unit test mode
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				defaultCoreDirectory = FileUtility.Combine(Path.GetDirectoryName(entryAssembly.Location), "..", "AddIns");
			} catch (Exception) {
				defaultCoreDirectory = "";
			}
			
			erbauer.Add("Class", new ClassErbauer());
			erbauer.Add("FileFilter", new FileFilterErbauer());
			erbauer.Add("Icon", new IconErbauer());
			erbauer.Add("MenuItem", new MenuItemErbauer());
			erbauer.Add("ToolbarItem", new ToolbarItemErbauer());
			
			auswerter.Add("Compare", new CompareAuswerter());
			auswerter.Add("Ownerstate", new OwnerStateAuswerter());
		}
		
		public static List<AddIn> AddIns {
			get {
				return addIns;
			}
		}
		
		public static Dictionary<string, IErbauer> Erbauer {
			get {
				return erbauer;
			}
		}
		
		public static Dictionary<string, IAuswerter> Auswerter {
			get {
				return auswerter;
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
			if (path == null || path.Length == 0) {
				return rootNode;
			}
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			int i = 0;
			while (i < splittedPath.Length) {
				if (!curPath.ChildNodes.ContainsKey(splittedPath[i])) {
					throw new TreePathNotFoundException(path);
				}
				curPath = curPath.ChildNodes[splittedPath[i]];
				++i;
			}
			return curPath;
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
