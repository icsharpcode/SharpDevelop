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
	public class SearchFileNode : SearchNode
	{
		public FileName FileName { get; private set; }
		
		public SearchFileNode(FileName fileName, System.Collections.Generic.List<SearchResultNode> resultNodes)
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
}
