// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public class ScriptingTextEditorViewContent
	{
		IViewContent view;
		IEditable editable;
		ITextEditorProvider textEditorProvider;
		ITextEditor textEditor;
		ITextEditorOptions textEditorOptions;
		
		public ScriptingTextEditorViewContent(IScriptingWorkbench workbench)
		{
			Init(workbench.ActiveViewContent);
		}
		
		public ScriptingTextEditorViewContent(IViewContent view)
		{
			Init(view);
		}
		
		void Init(IViewContent view)
		{
			this.view = view;
			editable = view as IEditable;
			textEditorProvider = view as ITextEditorProvider;
			textEditor = textEditorProvider.TextEditor;
			textEditorOptions = textEditor.Options;
		}
		
		public FileName PrimaryFileName {
			get { return view.PrimaryFileName; }
		}
		
		public IEditable EditableView {
			get { return editable; }
		}
		
		public ITextEditor TextEditor {
			get { return textEditor; }
		}
		
		public ITextEditorOptions TextEditorOptions {
			get { return textEditorOptions; }
		}
	}
}
