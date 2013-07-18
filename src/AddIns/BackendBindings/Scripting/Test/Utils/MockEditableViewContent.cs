// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Scripting.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IEditable and IViewContent.
	/// </summary>
	public class MockEditableViewContent : MockViewContent, IEditable
	{
		public MockTextEditor MockTextEditor = new MockTextEditor();
		
		public MockEditableViewContent()
		{
			Text = String.Empty;
		}
		
		public string Text { get; set; }
		
		public ITextSource CreateSnapshot()
		{
			return new StringTextSource(Text);
		}
		
		public ITextEditorOptions TextEditorOptions {
			get { return MockTextEditor.Options; }
		}
		
		public MockTextEditorOptions MockTextEditorOptions {
			get { return MockTextEditor.MockTextEditorOptions; }
			set { MockTextEditor.MockTextEditorOptions = value; }
		}
		
		public override object GetService(Type serviceType)
		{
			if (serviceType == typeof(ITextEditor)) {
				return MockTextEditor;
			}
			return null;
		}
	}
}
