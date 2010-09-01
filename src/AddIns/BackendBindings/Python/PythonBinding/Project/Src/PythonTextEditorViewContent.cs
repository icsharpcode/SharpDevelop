// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	public class PythonTextEditorViewContent
	{
		IViewContent view;
		IEditable editable;
		ITextEditorProvider textEditorProvider;
		ITextEditor textEditor;
		ITextEditorOptions textEditorOptions;
		
		public PythonTextEditorViewContent(IPythonWorkbench workbench)
		{
			Init(workbench.ActiveViewContent);
		}
		
		public PythonTextEditorViewContent(IViewContent view)
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
