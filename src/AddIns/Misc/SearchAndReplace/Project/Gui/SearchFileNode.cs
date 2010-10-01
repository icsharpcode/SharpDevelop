// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	class SearchFileNode : SearchNode
	{
		FileName fileName;
		
		public SearchFileNode(FileName fileName, SearchResultNode[] resultNodes)
		{
			this.fileName = fileName;
			this.Children = resultNodes;
			this.IsExpanded = true;
		}
		
		protected override object CreateText()
		{
			return new TextBlock {
				Inlines = {
					new Bold(new Run(Path.GetFileName(fileName))),
					new Run(StringParser.Parse(" (${res:MainWindow.Windows.SearchResultPanel.In} ") + Path.GetDirectoryName(fileName) + ")")
				}
			};
		}
		
		public override void ActivateItem()
		{
			FileService.OpenFile(fileName);
		}
	}
}
