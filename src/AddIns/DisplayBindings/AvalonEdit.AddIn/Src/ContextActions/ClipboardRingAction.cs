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
		string Text;
		
		public string DisplayName { get; private set; }
		
		public IEntity Entity { get; private set; }
		
		public IContextActionProvider Provider { get { return null; } }
		
		public ClipboardRingAction(string text) 
		{
			string entry = text.Trim();
			if(entry.Length > 50)
				entry = entry.Substring(0,47) + "...";
			
			this.DisplayName = entry;			
			this.Text = text;
		}
		
		public string GetDisplayName(EditorRefactoringContext context)
		{
			return DisplayName;
		}
		
		public void Execute(EditorRefactoringContext context)
		{
			ITextEditor editor = context.Editor;
			editor.Document.Insert(context.CaretOffset, this.Text);
		}
	}
}
