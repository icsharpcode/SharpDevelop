// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Creates code completion bindings that manage code completion for one language.
	/// </summary>
	/// <attribute name="resourcePath" use="optional">
	/// Namespace containing .xft and .xpt files as embedded resources.
	/// </attribute>
	/// <attribute name="path" use="optional">
	/// Path to directory containing .xft and .xpt files.
	/// </attribute>
	/// <usage>In the category subpaths of /SharpDevelop/BackendBindings/Templates</usage>
	/// <returns>
	/// The <see cref="TemplateBase"/> instance loaded from the template file.
	/// </returns>
	class TemplateFileDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		IReadOnlyFileSystem GetFileSystem(BuildItemArgs args)
		{
			string resourceNamespace = args.Codon["resourceNamespace"];
			if (!string.IsNullOrEmpty(resourceNamespace)) {
				args.AddIn.LoadRuntimeAssemblies();
				var assemblies = args.AddIn.Runtimes.Select(r => r.LoadedAssembly).Where(asm => asm != null).ToArray();
				return new EmbeddedResourceFileSystem(assemblies, resourceNamespace);
			}
			string path = args.Codon["path"];
			if (!string.IsNullOrEmpty(path)) {
				path = Path.Combine(Path.GetDirectoryName(args.AddIn.FileName), path);
				return new ReadOnlyChrootFileSystem(SD.FileSystem, DirectoryName.Create(path));
			}
			throw new InvalidOperationException("Missing 'resourceNamespace' or 'path' attribute.");
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			var fileSystem = GetFileSystem(args);
			var templates = new List<TemplateBase>();
			var xpt = fileSystem.GetFiles(DirectoryName.Create("."), "*.xpt", DirectorySearchOptions.IncludeSubdirectories);
			var xft = fileSystem.GetFiles(DirectoryName.Create("."), "*.xft", DirectorySearchOptions.IncludeSubdirectories);
			foreach (var fileName in xpt.Concat(xft)) {
				using (var stream = fileSystem.OpenRead(fileName)) {
					var relFileSystem = new ReadOnlyChrootFileSystem(fileSystem, fileName.GetParentDirectory());
					templates.Add(SD.Templates.LoadTemplate(stream, relFileSystem));
				}
			}
			if (templates.Count == 1)
				return templates[0];
			else
				return new MultipleItems(templates);
		}
		
		sealed class MultipleItems : IBuildItemsModifier
		{
			readonly IEnumerable items;
			
			public MultipleItems(IEnumerable items)
			{
				this.items = items;
			}
			
			public void Apply(IList outputItems)
			{
				foreach (var item in items)
					outputItems.Add(item);
			}
		}
	}
}
