// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.ILSpyAddIn.ViewContent
{
	/// <summary>
	/// Equivalent to AE.AddIn CodeEditor, but without editing capabilities.
	/// </summary>
	public class CodeView : Grid, IDisposable
	{
		readonly SharpDevelopTextEditor textEditor = new SharpDevelopTextEditor();
		
		public CodeView()
		{
			this.Children.Add(textEditor);
			textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
		}
		
		public TextDocument Document {
			get { return textEditor.Document; }
			set { textEditor.Document = value; }
		}
		
		public void Dispose()
		{
		}
	}
}
