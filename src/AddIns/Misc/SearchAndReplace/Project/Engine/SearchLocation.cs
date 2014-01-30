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

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace SearchAndReplace
{
	/// <summary>
	/// Describes a set of files or a file + selection to search in.
	/// </summary>
	public class SearchLocation
	{
		public SearchTarget Target { get; private set; }
		public string BaseDirectory { get; private set; }
		public string Filter { get; private set; }
		public bool SearchSubdirs { get; private set; }
		public ISegment Selection { get; private set; }
		
		public SearchLocation(SearchTarget target, string baseDirectory, string filter, bool searchSubdirs, ISegment selection)
		{
			this.Target = target;
			this.BaseDirectory = baseDirectory;
			this.Filter = filter ?? "*.*";
			this.SearchSubdirs = searchSubdirs;
			this.Selection = selection;
		}
		
		public bool EqualsWithoutSelection(SearchLocation other)
		{
			return other != null &&
				Target == other.Target &&
				BaseDirectory == other.BaseDirectory &&
				Filter == other.Filter &&
				SearchSubdirs == other.SearchSubdirs;
		}
		
		public virtual IEnumerable<FileName> GenerateFileList()
		{
			List<FileName> files = new List<FileName>();
			
			ITextEditor editor;
			switch (Target) {
				case SearchTarget.CurrentDocument:
				case SearchTarget.CurrentSelection:
					editor = SD.GetActiveViewContentService<ITextEditor>();
					if (editor != null)
						files.Add(editor.FileName);
					break;
				case SearchTarget.AllOpenFiles:
					foreach (var vc in SD.Workbench.ViewContentCollection) {
						editor = vc.GetService<ITextEditor>();
						if (editor != null)
							files.Add(editor.FileName);
					}
					break;
				case SearchTarget.WholeProject:
					if (ProjectService.CurrentProject == null)
						break;
					foreach (FileProjectItem item in ProjectService.CurrentProject.Items.OfType<FileProjectItem>())
						files.Add(item.FileName);
					break;
				case SearchTarget.WholeSolution:
					if (ProjectService.OpenSolution == null)
						break;
					foreach (var item in ProjectService.OpenSolution.AllItems.OfType<ISolutionFileItem>())
						files.Add(item.FileName);
					foreach (var item in ProjectService.OpenSolution.Projects.SelectMany(p => p.Items).OfType<FileProjectItem>())
						files.Add(item.FileName);
					break;
				case SearchTarget.Directory:
					if (!Directory.Exists(BaseDirectory))
						break;
					var options = SearchSubdirs ? DirectorySearchOptions.IncludeSubdirectories : DirectorySearchOptions.None;
					return SD.FileSystem.GetFiles(DirectoryName.Create(BaseDirectory), Filter, options);
				default:
					throw new Exception("Invalid value for FileListType");
			}
			
			return files.Distinct();
		}
	}
}
