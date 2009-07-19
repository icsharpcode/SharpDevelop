// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Static class containing the AddInTree. Contains methods for accessing tree nodes and building items.
	/// </summary>
	public static class AddInTree
	{
		static List<AddIn>   addIns   = new List<AddIn>();
		static AddInTreeNode rootNode = new AddInTreeNode();
		
		static Dictionary<string, IDoozer> doozers = new Dictionary<string, IDoozer>();
		static Dictionary<string, IConditionEvaluator> conditionEvaluators = new Dictionary<string, IConditionEvaluator>();
		
		static AddInTree()
		{
			doozers.Add("Class", new ClassDoozer());
			doozers.Add("FileFilter", new FileFilterDoozer());
			doozers.Add("String", new StringDoozer());
			doozers.Add("Icon", new IconDoozer());
			doozers.Add("MenuItem", new MenuItemDoozer());
			doozers.Add("ToolbarItem", new ToolbarItemDoozer());
			doozers.Add("Include", new IncludeDoozer());
			
			conditionEvaluators.Add("Compare", new CompareConditionEvaluator());
			conditionEvaluators.Add("Ownerstate", new OwnerStateConditionEvaluator());
			
			ApplicationStateInfoService.RegisterStateGetter("Installed 3rd party AddIns", GetInstalledThirdPartyAddInsListAsString);
		}
		
		static object GetInstalledThirdPartyAddInsListAsString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (AddIn addIn in AddIns) {
				// Skip preinstalled AddIns (show only third party AddIns)
				if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)) {
					string hidden = addIn.Properties["addInManagerHidden"];
					if (string.Equals(hidden, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(hidden, "preinstalled", StringComparison.OrdinalIgnoreCase))
						continue;
				}
				if (sb.Length > 0) sb.Append(", ");
				sb.Append("[");
				sb.Append(addIn.Name);
				if (addIn.Version != null) {
					sb.Append(' ');
					sb.Append(addIn.Version.ToString());
				}
				if (!addIn.Enabled) {
					sb.Append(", Enabled=");
					sb.Append(addIn.Enabled);
				}
				if (addIn.Action != AddInAction.Enable) {
					sb.Append(", Action=");
					sb.Append(addIn.Action.ToString());
				}
				sb.Append("]");
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Gets the list of loaded AddIns.
		/// </summary>
		public static IList<AddIn> AddIns {
			get {
				return addIns.AsReadOnly();
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered doozers.
		/// </summary>
		public static Dictionary<string, IDoozer> Doozers {
			get {
				return doozers;
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered condition evaluators.
		/// </summary>
		public static Dictionary<string, IConditionEvaluator> ConditionEvaluators {
			get {
				return conditionEvaluators;
			}
		}
		
		/// <summary>
		/// Checks whether the specified path exists in the AddIn tree.
		/// </summary>
		public static bool ExistsTreeNode(string path)
		{
			if (path == null || path.Length == 0) {
				return true;
			}
			
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			int i = 0;
			while (i < splittedPath.Length) {
				// curPath = curPath.ChildNodes[splittedPath[i]] - check if child path exists
				if (!curPath.ChildNodes.TryGetValue(splittedPath[i], out curPath)) {
					return false;
				}
				++i;
			}
			return true;
		}
		
		/// <summary>
		/// Gets the <see cref="AddInTreeNode"/> representing the specified path.
		/// This method throws a <see cref="TreePathNotFoundException"/> when the
		/// path does not exist.
		/// </summary>
		public static AddInTreeNode GetTreeNode(string path)
		{
			return GetTreeNode(path, true);
		}
		
		/// <summary>
		/// Gets the <see cref="AddInTreeNode"/> representing the specified path.
		/// </summary>
		/// <param name="path">The path of the AddIn tree node</param>
		/// <param name="throwOnNotFound">
		/// If set to <c>true</c>, this method throws a
		/// <see cref="TreePathNotFoundException"/> when the path does not exist.
		/// If set to <c>false</c>, <c>null</c> is returned for non-existing paths.
		/// </param>
		public static AddInTreeNode GetTreeNode(string path, bool throwOnNotFound)
		{
			if (path == null || path.Length == 0) {
				return rootNode;
			}
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			int i = 0;
			while (i < splittedPath.Length) {
				if (!curPath.ChildNodes.TryGetValue(splittedPath[i], out curPath)) {
					if (throwOnNotFound)
						throw new TreePathNotFoundException(path);
					else
						return null;
				}
				// curPath = curPath.ChildNodes[splittedPath[i]]; already done by TryGetValue
				++i;
			}
			return curPath;
		}
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		/// <param name="path">A path to the item in the addin tree.</param>
		/// <param name="caller">The owner used to create the objects.</param>
		/// <exception cref="TreePathNotFoundException">The path does not
		/// exist or does not point to an item.</exception>
		public static object BuildItem(string path, object caller)
		{
			int pos = path.LastIndexOf('/');
			string parent = path.Substring(0, pos);
			string child = path.Substring(pos + 1);
			AddInTreeNode node = GetTreeNode(parent);
			return node.BuildChildItem(child, caller, new ArrayList(BuildItems<object>(path, caller, false)));
		}
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// Throws a <see cref="TreePathNotFoundException"/> if the path is not found.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="caller">The owner used to create the objects.</param>
		public static List<T> BuildItems<T>(string path, object caller)
		{
			return BuildItems<T>(path, caller, true);
		}
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="caller">The owner used to create the objects.</param>
		/// <param name="throwOnNotFound">If true, throws a <see cref="TreePathNotFoundException"/>
		/// if the path is not found. If false, an empty ArrayList is returned when the
		/// path is not found.</param>
		public static List<T> BuildItems<T>(string path, object caller, bool throwOnNotFound)
		{
			AddInTreeNode node = GetTreeNode(path, throwOnNotFound);
			if (node == null)
				return new List<T>();
			else
				return node.BuildChildItems<T>(caller);
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
		
		/// <summary>
		/// The specified AddIn is added to the <see cref="AddIns"/> collection.
		/// If the AddIn is enabled, its doozers, condition evaluators and extension
		/// paths are added to the AddInTree and its resources are added to the
		/// <see cref="ResourceService"/>.
		/// </summary>
		public static void InsertAddIn(AddIn addIn)
		{
			if (addIn.Enabled) {
				foreach (ExtensionPath path in addIn.Paths.Values) {
					AddExtensionPath(path);
				}
				
				foreach (Runtime runtime in addIn.Runtimes) {
					if (runtime.IsActive) {
						foreach (LazyLoadDoozer doozer in runtime.DefinedDoozers) {
							if (AddInTree.Doozers.ContainsKey(doozer.Name)) {
								throw new AddInLoadException("Duplicate doozer: " + doozer.Name);
							}
							AddInTree.Doozers.Add(doozer.Name, doozer);
						}
						foreach (LazyConditionEvaluator condition in runtime.DefinedConditionEvaluators) {
							if (AddInTree.ConditionEvaluators.ContainsKey(condition.Name)) {
								throw new AddInLoadException("Duplicate condition evaluator: " + condition.Name);
							}
							AddInTree.ConditionEvaluators.Add(condition.Name, condition);
						}
					}
				}
				
				string addInRoot = Path.GetDirectoryName(addIn.FileName);
				foreach(string bitmapResource in addIn.BitmapResources)
				{
					string path = Path.Combine(addInRoot, bitmapResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					ResourceService.RegisterNeutralImages(resourceManager);
				}
				
				foreach(string stringResource in addIn.StringResources)
				{
					string path = Path.Combine(addInRoot, stringResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					ResourceService.RegisterNeutralStrings(resourceManager);
				}
			}
			addIns.Add(addIn);
		}
		
		/// <summary>
		/// The specified AddIn is removed to the <see cref="AddIns"/> collection.
		/// This is only possible for disabled AddIns, enabled AddIns require
		/// a restart of the application to be removed.
		/// </summary>
		/// <exception cref="ArgumentException">Occurs when trying to remove an enabled AddIn.</exception>
		public static void RemoveAddIn(AddIn addIn)
		{
			if (addIn.Enabled) {
				throw new ArgumentException("Cannot remove enabled AddIns at runtime.");
			}
			addIns.Remove(addIn);
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
		
		// used by Load(): disables an addin and removes it from the dictionaries.
		static void DisableAddin(AddIn addIn, Dictionary<string, Version> dict, Dictionary<string, AddIn> addInDict)
		{
			addIn.Enabled = false;
			addIn.Action = AddInAction.DependencyError;
			foreach (string name in addIn.Manifest.Identities.Keys) {
				dict.Remove(name);
				addInDict.Remove(name);
			}
		}
		
		/// <summary>
		/// Loads a list of .addin files, ensuring that dependencies are satisfied.
		/// This method is normally called by <see cref="CoreStartup.RunInitialization"/>.
		/// </summary>
		/// <param name="addInFiles">
		/// The list of .addin file names to load.
		/// </param>
		/// <param name="disabledAddIns">
		/// The list of disabled AddIn identity names.
		/// </param>
		public static void Load(List<string> addInFiles, List<string> disabledAddIns)
		{
			List<AddIn> list = new List<AddIn>();
			Dictionary<string, Version> dict = new Dictionary<string, Version>();
			Dictionary<string, AddIn> addInDict = new Dictionary<string, AddIn>();
			foreach (string fileName in addInFiles) {
				AddIn addIn;
				try {
					addIn = AddIn.Load(fileName);
				} catch (AddInLoadException ex) {
					LoggingService.Error(ex);
					if (ex.InnerException != null) {
						MessageService.ShowError("Error loading AddIn " + fileName + ":\n"
						                         + ex.InnerException.Message);
					} else {
						MessageService.ShowError("Error loading AddIn " + fileName + ":\n"
						                         + ex.Message);
					}
					addIn = new AddIn();
					addIn.addInFileName = fileName;
					addIn.CustomErrorMessage = ex.Message;
				}
				if (addIn.Action == AddInAction.CustomError) {
					list.Add(addIn);
					continue;
				}
				addIn.Enabled = true;
				if (disabledAddIns != null && disabledAddIns.Count > 0) {
					foreach (string name in addIn.Manifest.Identities.Keys) {
						if (disabledAddIns.Contains(name)) {
							addIn.Enabled = false;
							break;
						}
					}
				}
				if (addIn.Enabled) {
					foreach (KeyValuePair<string, Version> pair in addIn.Manifest.Identities) {
						if (dict.ContainsKey(pair.Key)) {
							MessageService.ShowError("Name '" + pair.Key + "' is used by " +
							                         "'" + addInDict[pair.Key].FileName + "' and '" + fileName + "'");
							addIn.Enabled = false;
							addIn.Action = AddInAction.InstalledTwice;
							break;
						} else {
							dict.Add(pair.Key, pair.Value);
							addInDict.Add(pair.Key, addIn);
						}
					}
				}
				list.Add(addIn);
			}
		checkDependencies:
			for (int i = 0; i < list.Count; i++) {
				AddIn addIn = list[i];
				if (!addIn.Enabled) continue;
				
				Version versionFound;
				
				foreach (AddInReference reference in addIn.Manifest.Conflicts) {
					if (reference.Check(dict, out versionFound)) {
						MessageService.ShowError(addIn.Name + " conflicts with " + reference.ToString()
						                         + " and has been disabled.");
						DisableAddin(addIn, dict, addInDict);
						goto checkDependencies; // after removing one addin, others could break
					}
				}
				foreach (AddInReference reference in addIn.Manifest.Dependencies) {
					if (!reference.Check(dict, out versionFound)) {
						if (versionFound != null) {
							MessageService.ShowError(addIn.Name + " has not been loaded because it requires "
							                         + reference.ToString() + ", but version "
							                         + versionFound.ToString() + " is installed.");
						} else {
							MessageService.ShowError(addIn.Name + " has not been loaded because it requires "
							                         + reference.ToString() + ".");
						}
						DisableAddin(addIn, dict, addInDict);
						goto checkDependencies; // after removing one addin, others could break
					}
				}
			}
			foreach (AddIn addIn in list) {
				try {
					InsertAddIn(addIn);
				} catch (AddInLoadException ex) {
					LoggingService.Error(ex);
					MessageService.ShowError("Error loading AddIn " + addIn.FileName + ":\n"
					                         + ex.Message);
				}
			}
		}
	}
}
