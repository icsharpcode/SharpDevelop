// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Provides facilities to load and cache the contents of resource files.
	/// </summary>
	public static class ResourceFileContentRegistry
	{
		/// <summary>
		/// The AddIn tree path where the resource file content factories are registered.
		/// </summary>
		public const string ResourceFileContentFactoriesAddInTreePath = "/AddIns/ResourceToolkit/ResourceFileContentFactories";
		
		static List<IResourceFileContentFactory> factories;
		
		/// <summary>
		/// Gets a list of all registered resource file content factories.
		/// </summary>
		public static IEnumerable<IResourceFileContentFactory> Factories {
			get {
				if (factories == null) {
					factories = AddInTree.BuildItems<IResourceFileContentFactory>(ResourceFileContentFactoriesAddInTreePath, null, false);
				}
				return factories;
			}
		}
		
		
		static Dictionary<string, IResourceFileContent> resourceFileContents = new Dictionary<string, IResourceFileContent>();
		
		/// <summary>
		/// Gets the resource content for the specified file.
		/// </summary>
		/// <param name="fileName">The name of the file to get a resource content for.</param>
		/// <returns>The resource content for the specified file, or <c>null</c> if the format of the specified resource file cannot be handled.</returns>
		public static IResourceFileContent GetResourceFileContent(string fileName)
		{
			IResourceFileContent c;
			if (!resourceFileContents.TryGetValue(fileName, out c)) {
				c = CreateResourceFileContent(fileName);
				if (c == null) {
					return null;
				}
				resourceFileContents[fileName] = c;
			}
			return c;
		}
		
		/// <summary>
		/// Creates the resource content for the specified file.
		/// </summary>
		/// <param name="fileName">The name of the file to create a resource content for.</param>
		/// <returns>The resource content for the specified file, or <c>null</c>, if the resource file format cannot be handled.</returns>
		static IResourceFileContent CreateResourceFileContent(string fileName)
		{
			IResourceFileContentFactory factory = GetResourceFileContentFactory(fileName);
			if (factory == null) {
				return null;
			} else {
				return factory.CreateContentForFile(fileName);
			}
		}
		
		/// <summary>
		/// Gets a <see cref="IResourceFileContentFactory"/> that can create the resource file content
		/// for the specified resource file.
		/// </summary>
		/// <param name="fileName">The resource file to get a <see cref="IResourceFileContentFactory"/> for.</param>
		/// <returns>A <see cref="IResourceFileContentFactory"/> that can create the resource file content for the specified file, or <c>null</c> if the specified file is not supported by any registered resource file content factory.</returns>
		public static IResourceFileContentFactory GetResourceFileContentFactory(string fileName)
		{
			foreach (IResourceFileContentFactory factory in Factories) {
				if (factory.CanCreateContentForFile(fileName)) {
					return factory;
				}
			}
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// The AddIn tree path where the localized resource finders are registered.
		/// </summary>
		public const string LocalizedResourcesFindersAddInTreePath = "/AddIns/ResourceToolkit/LocalizedResourcesFinders";
		
		static List<ILocalizedResourcesFinder> localizedResourcesFinders;
		
		/// <summary>
		/// Gets a list of all registered localized resources finders.
		/// </summary>
		public static IEnumerable<ILocalizedResourcesFinder> LocalizedResourcesFinders {
			get {
				if (localizedResourcesFinders == null) {
					localizedResourcesFinders = AddInTree.BuildItems<ILocalizedResourcesFinder>(LocalizedResourcesFindersAddInTreePath, null, false);
				}
				return localizedResourcesFinders;
			}
		}
		
		/// <summary>
		/// Gets localized resources that belong to the master resource file.
		/// </summary>
		/// <param name="fileName">The name of the master resource file.</param>
		/// <returns>A dictionary of culture names and associated resource file contents.</returns>
		public static IDictionary<string, IResourceFileContent> GetLocalizedContents(string fileName)
		{
			Dictionary<string, IResourceFileContent> list = new Dictionary<string, IResourceFileContent>();
			foreach (ILocalizedResourcesFinder finder in LocalizedResourcesFinders) {
				IDictionary<string, IResourceFileContent> l = finder.GetLocalizedContents(fileName);
				if (l != null) {
					foreach (KeyValuePair<string, IResourceFileContent> entry in l) {
						if (!list.ContainsKey(entry.Key)) {
							list.Add(entry.Key, entry.Value);
						}
					}
				}
			}
			return list;
		}
		
	}
}
