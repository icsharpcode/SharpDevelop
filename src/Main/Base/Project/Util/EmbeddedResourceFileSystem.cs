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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Treats embedded resources as a file system.
	/// </summary>
	public class EmbeddedResourceFileSystem : IReadOnlyFileSystem
	{
		readonly Assembly[] assemblies;
		readonly string prefix;
		Lazy<List<FileName>> fileNames;
		
		public EmbeddedResourceFileSystem(Assembly[] assemblies, string resourceNamespace)
		{
			if (assemblies == null)
				throw new ArgumentNullException("assemblies");
			if (resourceNamespace == null)
				throw new ArgumentNullException("resourceNamespace");
			this.assemblies = assemblies;
			this.prefix = resourceNamespace + ".";
			this.fileNames = new Lazy<List<FileName>>(GetFileNames);
		}
		
		List<FileName> GetFileNames()
		{
			List<FileName> result = new List<FileName>();
			foreach (Assembly asm in assemblies) {
				foreach (string resourceName in asm.GetManifestResourceNames()) {
					if (resourceName.StartsWith(prefix, StringComparison.Ordinal)) {
						result.Add(FileName.Create(resourceName.Substring(prefix.Length)));
					}
				}
			}
			return result;
		}
		
		public bool FileExists(FileName path)
		{
			return fileNames.Value.Contains(path);
		}
		
		bool IReadOnlyFileSystem.DirectoryExists(DirectoryName path)
		{
			return false;
		}
		
		string ToResourceName(FileName fileName)
		{
			return prefix + fileName.ToString().Replace('\\', '.');
		}
		
		public Stream OpenRead(FileName fileName)
		{
			string name = ToResourceName(fileName);
			foreach (var asm in assemblies) {
				Stream stream = asm.GetManifestResourceStream(name);
				if (stream != null)
					return stream;
			}
			throw new FileNotFoundException("Resource '" + name + "' not found.");
		}
		
		public TextReader OpenText(FileName fileName)
		{
			return new StreamReader(OpenRead(fileName));
		}
		
		public IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern, DirectorySearchOptions searchOptions)
		{
			return fileNames.Value.Where(delegate(FileName fn) {
				if (!FileUtility.MatchesPattern(fn.GetFileName(), searchPattern))
					return false;
				if ((searchOptions & DirectorySearchOptions.IncludeSubdirectories) != 0)
					return FileUtility.IsBaseDirectory(directory, fn);
				else
					return directory.Equals(fn.GetParentDirectory());
			});
		}
	}
}
