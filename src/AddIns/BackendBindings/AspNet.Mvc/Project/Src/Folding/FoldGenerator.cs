// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.Core;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class FoldGenerator : IFoldGenerator
	{
		ITextEditorWithParseInformationFolding textEditor;
		IFoldParser foldParser;
		
		public FoldGenerator(
			ITextEditorWithParseInformationFolding textEditor,
			IFoldParser foldParser)
		{
			this.textEditor = textEditor;
			this.foldParser = foldParser;
			IsParseInformationFoldingEnabled = false;
			this.textEditor.InstallFoldingManager();
		}
		
		bool IsParseInformationFoldingEnabled {
			get { return textEditor.IsParseInformationFoldingEnabled; }
			set { textEditor.IsParseInformationFoldingEnabled = value; }
		}
		
		public void Dispose()
		{
			textEditor.Dispose();
			IsParseInformationFoldingEnabled = true;
		}
		
		public void GenerateFolds()
		{
			try {
				textEditor.UpdateFolds(GetFolds());
			} catch (Exception ex) {
				LoggingService.Debug(ex.ToString());
			}
		}
		
		IEnumerable<NewFolding> GetFolds()
		{
			return foldParser.GetFolds(textEditor.GetTextSnapshot());
		}
	}
}
