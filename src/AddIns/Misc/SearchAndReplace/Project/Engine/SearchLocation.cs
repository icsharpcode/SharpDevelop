// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
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
			
			switch (Target) {
				case SearchTarget.CurrentDocument:
				case SearchTarget.CurrentSelection:
					ITextEditorProvider vc = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
					if (vc != null)
						files.Add(vc.TextEditor.FileName);
					break;
				case SearchTarget.AllOpenFiles:
					foreach (ITextEditorProvider editor in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<ITextEditorProvider>())
						files.Add(editor.TextEditor.FileName);
					break;
				case SearchTarget.WholeProject:
					if (ProjectService.CurrentProject == null)
						break;
					foreach (FileProjectItem item in ProjectService.CurrentProject.Items.OfType<FileProjectItem>())
						files.Add(new FileName(item.FileName));
					break;
				case SearchTarget.WholeSolution:
					if (ProjectService.OpenSolution == null)
						break;
					foreach (var item in ProjectService.OpenSolution.SolutionFolderContainers.Select(f => f.SolutionItems).SelectMany(si => si.Items))
						files.Add(new FileName(Path.Combine(ProjectService.OpenSolution.Directory, item.Location)));
					foreach (var item in ProjectService.OpenSolution.Projects.SelectMany(p => p.Items).OfType<FileProjectItem>())
						files.Add(new FileName(item.FileName));
					break;
				case SearchTarget.Directory:
					if (!Directory.Exists(BaseDirectory))
						break;
					return FileUtility.LazySearchDirectory(BaseDirectory, Filter, SearchSubdirs);
				default:
					throw new Exception("Invalid value for FileListType");
			}
			
			return files.Distinct();
		}
	}
}
