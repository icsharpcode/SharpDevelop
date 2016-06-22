// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	public class ClipboardRingAction : IContextAction 
	{
		readonly int maxLength = 50;
		
		readonly string endString = "...";
		
		public string Text { get; private set; }
		
		public string DisplayName { get; private set; }
		
		public IEntity Entity { get; private set; }
		
		public IContextActionProvider Provider { get { return null; } }
		
		public ClipboardRingAction(string text) 
		{
			string entry = System.Text.RegularExpressions.Regex.Replace(text.Trim(), @"\s+", " ");
			if(entry.Length > maxLength)
				entry = entry.Substring(0, maxLength-endString.Length) + endString;
			
			this.DisplayName = entry;			
			this.Text = text;
		}
		
		public string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public void Execute(EditorRefactoringContext context)
		{
			/* insert to editor */
			ITextEditor editor = context.Editor;
			editor.Document.Insert(context.CaretOffset, this.Text);
			
			/* update clipboard ring */
			var clipboardRing = ICSharpCode.SharpDevelop.Gui.TextEditorSideBar.Instance;
			if (clipboardRing != null) {
				clipboardRing.PutInClipboardRing(this.Text);
			}
		}
	}
}
