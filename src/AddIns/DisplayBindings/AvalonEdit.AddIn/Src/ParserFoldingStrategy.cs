// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Uses the NRefactory type system to create parsing information.
	/// </summary>
	[TextEditorService]
	public class ParserFoldingStrategy : IDisposable
	{
		readonly FoldingManager foldingManager;
		
		TextArea textArea;
		bool isFirstUpdate = true;
		
		public FoldingManager FoldingManager {
			get { return foldingManager; }
		}
		
		public ParserFoldingStrategy(TextArea textArea)
		{
			this.textArea = textArea;
			foldingManager = FoldingManager.Install(textArea);
		}
		
		public void Dispose()
		{
			if (textArea != null) {
				FoldingManager.Uninstall(foldingManager);
				textArea = null;
			}
		}
		
		public void UpdateFoldings(ParseInformation parseInfo)
		{
			int firstErrorOffset = -1;
			IEnumerable<NewFolding> newFoldings = parseInfo.GetFoldings(textArea.Document, out firstErrorOffset);
			newFoldings = newFoldings.OrderBy(f => f.StartOffset);
			foldingManager.UpdateFoldings(newFoldings, firstErrorOffset);
			
			isFirstUpdate = false;
		}
	}
}
