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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Static class containing the AddInTree. Contains methods for accessing tree nodes and building items.
	/// </summary>
	public static class AddInTree
	{
		public static List<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true)
		{
			var addInTree = ServiceSingleton.GetRequiredService<IAddInTree>();
			return addInTree.BuildItems<T>(path, parameter, throwOnNotFound).ToList();
		}
		
		public static AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true)
		{
			var addInTree = ServiceSingleton.GetRequiredService<IAddInTree>();
			return addInTree.GetTreeNode(path, throwOnNotFound);
		}
	}
	
	/// <summary>
	/// Class containing the AddInTree. Contains methods for accessing tree nodes and building items.
	/// </summary>
	public class AddInTreeImpl : IAddInTree
	{
		List<AddIn>   addIns   = new List<AddIn>();
		AddInTreeNode rootNode = new AddInTreeNode();
		
		ConcurrentDictionary<string, IDoozer> doozers = new ConcurrentDictionary<string, IDoozer>();
		ConcurrentDictionary<string, IConditionEvaluator> conditionEvaluators = new ConcurrentDictionary<string, IConditionEvaluator>();
		
		public AddInTreeImpl(ApplicationStateInfoService applicationStateService)
		{
			doozers.TryAdd("Class", new ClassDoozer());
			doozers.TryAdd("Static", new StaticDoozer());
			doozers.TryAdd("FileFilter", new FileFilterDoozer());
			doozers.TryAdd("String", new StringDoozer());
			doozers.TryAdd("Icon", new IconDoozer());
			doozers.TryAdd("MenuItem", new MenuItemDoozer());
			doozers.TryAdd("ToolbarItem", new ToolbarItemDoozer());
			doozers.TryAdd("Include", new IncludeDoozer());
			doozers.TryAdd("Service", new ServiceDoozer());
			
			conditionEvaluators.TryAdd("Compare", new CompareConditionEvaluator());
			conditionEvaluators.TryAdd("Ownerstate", new OwnerStateConditionEvaluator());
			
			if (applicationStateService != null)
				applicationStateService.RegisterStateGetter("Installed 3rd party AddIns", GetInstalledThirdPartyAddInsListAsString);
		}
		
		string GetInstalledThirdPartyAddInsListAsString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (AddIn addIn in AddIns) {
				// Skip preinstalled AddIns (show only third party AddIns)
				if (addIn.IsPreinstalled)
					continue;
				
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
		public IReadOnlyList<AddIn> AddIns {
			get {
				return addIns;
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered doozers.
		/// </summary>
		public ConcurrentDictionary<string, IDoozer> Doozers {
			get {
				return doozers;
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered condition evaluators.
		/// </summary>
		public ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators {
			get {
				return conditionEvaluators;
			}
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
		public AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true)
		{
			if (path == null || path.Length == 0) {
				return rootNode;
			}
			string[] splittedPath = path.Split('/');
			AddInTreeNode curPath = rootNode;
			for (int i = 0; i < splittedPath.Length; i++) {
				if (!curPath.ChildNodes.TryGetValue(splittedPath[i], out curPath)) {
					if (throwOnNotFound)
						throw new TreePathNotFoundException(path);
					else
						return null;
				}
			}
			return curPath;
		}
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		/// <param name="path">A path to the item in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <exception cref="TreePathNotFoundException">The path does not
		/// exist or does not point to an item.</exception>
		public object BuildItem(string path, object parameter)
		{
			return BuildItem(path, parameter, null);
		}
		
		public object BuildItem(string path, object parameter, IEnumerable<ICondition> additionalConditions)
		{
			int pos = path.LastIndexOf('/');
			string parent = path.Substring(0, pos);
			string child = path.Substring(pos + 1);
			AddInTreeNode node = GetTreeNode(parent);
			return node.BuildChildItem(child, parameter, additionalConditions);
		}
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="parameter">The owner used to create the objects.</param>
		/// <param name="throwOnNotFound">If true, throws a <see cref="TreePathNotFoundException"/>
		/// if the path is not found. If false, an empty ArrayList is returned when the
		/// path is not found.</param>
		public IReadOnlyList<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true)
		{
			AddInTreeNode node = GetTreeNode(path, throwOnNotFound);
			if (node == null)
				return new List<T>();
			else
				return node.BuildChildItems<T>(parameter);
		}
		
		AddInTreeNode CreatePath(AddInTreeNode localRoot, string path)
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
		
		void AddExtensionPath(ExtensionPath path)
		{
			AddInTreeNode treePath = CreatePath(rootNode, path.Name);
			foreach (IEnumerable<Codon> innerCodons in path.GroupedCodons)
				treePath.AddCodons(innerCodons);
		}
		
		/// <summary>
		/// The specified AddIn is added to the <see cref="AddIns"/> collection.
		/// If the AddIn is enabled, its doozers, condition evaluators and extension
		/// paths are added to the AddInTree and its resources are added to the
		/// <see cref="ResourceService"/>.
		/// </summary>
		public void InsertAddIn(AddIn addIn)
		{
			if (addIn.Enabled) {
				foreach (ExtensionPath path in addIn.Paths.Values) {
					AddExtensionPath(path);
				}
				
				foreach (Runtime runtime in addIn.Runtimes) {
					if (runtime.IsActive) {
						foreach (var pair in runtime.DefinedDoozers) {
							if (!doozers.TryAdd(pair.Key, pair.Value))
								throw new AddInLoadException("Duplicate doozer: " + pair.Key);
						}
						foreach (var pair in runtime.DefinedConditionEvaluators) {
							if (!conditionEvaluators.TryAdd(pair.Key, pair.Value))
								throw new AddInLoadException("Duplicate condition evaluator: " + pair.Key);
						}
					}
				}
				
				string addInRoot = Path.GetDirectoryName(addIn.FileName);
				foreach(string bitmapResource in addIn.BitmapResources)
				{
					string path = Path.Combine(addInRoot, bitmapResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					ServiceSingleton.GetRequiredService<IResourceService>().RegisterNeutralImages(resourceManager);
				}
				
				foreach(string stringResource in addIn.StringResources)
				{
					string path = Path.Combine(addInRoot, stringResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					ServiceSingleton.GetRequiredService<IResourceService>().RegisterNeutralStrings(resourceManager);
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
		public void RemoveAddIn(AddIn addIn)
		{
			if (addIn.Enabled) {
				throw new ArgumentException("Cannot remove enabled AddIns at runtime.");
			}
			addIns.Remove(addIn);
		}
		
		// used by Load(): disables an addin and removes it from the dictionaries.
		void DisableAddin(AddIn addIn, Dictionary<string, Version> dict, Dictionary<string, AddIn> addInDict)
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
		public void Load(List<string> addInFiles, List<string> disabledAddIns)
		{
			List<AddIn> list = new List<AddIn>();
			Dictionary<string, Version> dict = new Dictionary<string, Version>();
			Dictionary<string, AddIn> addInDict = new Dictionary<string, AddIn>();
			var nameTable = new System.Xml.NameTable();
			foreach (string fileName in addInFiles) {
				AddIn addIn;
				try {
					addIn = AddIn.Load(this, fileName, nameTable);
				} catch (AddInLoadException ex) {
					LoggingService.Error(ex);
					if (ex.InnerException != null) {
						MessageService.ShowError("Error loading AddIn " + fileName + ":\n"
						                         + ex.InnerException.Message);
					} else {
						MessageService.ShowError("Error loading AddIn " + fileName + ":\n"
						                         + ex.Message);
					}
					addIn = new AddIn(this);
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
