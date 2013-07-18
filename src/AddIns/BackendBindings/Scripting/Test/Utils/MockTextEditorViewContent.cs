// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Scripting.Tests.Utils
{
	/// <summary>
	/// A mock IViewContent implementation that also implements the
	/// ITextEditorControlProvider interface.
	/// </summary>
	public class MockTextEditorViewContent : MockViewContent
	{
		ITextEditor textEditor;
		
		public MockTextEditorViewContent()
		{
			textEditor = new AvalonEditTextEditorAdapter(new TextEditor());
		}
	}
}
