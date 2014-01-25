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
