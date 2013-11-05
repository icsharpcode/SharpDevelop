// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace SearchAndReplace
{
	public class SearchFileNode : SearchNode
	{
		public FileName FileName { get; private set; }
		
		public SearchFileNode(FileName fileName, IList<SearchResultNode> resultNodes)
		{
			this.FileName = fileName;
			this.Children = resultNodes;
			this.IsExpanded = true;
		}
		
		protected override object CreateText()
		{
			return new TextBlock {
				Inlines = {
					new Bold(new Run(Path.GetFileName(FileName))),
					new Run(StringParser.Parse(" (${res:MainWindow.Windows.SearchResultPanel.In} ") + Path.GetDirectoryName(FileName) + ")")
				}
			};
		}
		
		public override void ActivateItem()
		{
			FileService.OpenFile(FileName);
		}
	}
	
	public class SearchProjectNode : SearchNode
	{
		public IProject Project { get; private set; }
		
		public SearchProjectNode(IProject project, IList<SearchNode> resultNodes)
		{
			this.Project = project;
			this.Children = resultNodes;
			this.IsExpanded = true;
		}
		
		protected override object CreateText()
		{
			string fileName = SD.ResourceService.GetString("MainWindow.Windows.SearchResultPanel.NoProject");
			string directory = null;
			
			if (Project != null) {
				fileName = Path.GetFileNameWithoutExtension(Project.FileName);
				directory = Path.GetDirectoryName(Project.FileName);
			}
			
			if (directory != null) {
				return new TextBlock {
					Inlines = {
						new Bold(new Run(fileName)),
						new Run(StringParser.Parse(" (${res:MainWindow.Windows.SearchResultPanel.In} ") + directory + ")")
					}
				};
			}
			
			return new TextBlock { Inlines = { new Bold(new Run(fileName)) } };
		}
		
		public override void ActivateItem()
		{
		}
	}
}
