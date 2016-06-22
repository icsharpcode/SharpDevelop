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
