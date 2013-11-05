// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
