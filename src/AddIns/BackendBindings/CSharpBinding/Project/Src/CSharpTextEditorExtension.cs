// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Refactoring;

namespace CSharpBinding
{
	public class CSharpTextEditorExtension : ITextEditorExtension
	{
		ITextEditor editor;
		IssueManager inspectionManager;
		IList<IContextActionProvider> contextActionProviders;
		CodeManipulation codeManipulation;
		public void Attach(ITextEditor editor)
		{
			this.editor = editor;
			inspectionManager = new IssueManager(editor);
			codeManipulation = new CodeManipulation(editor);
			if (!editor.ContextActionProviders.IsReadOnly) {
				contextActionProviders = AddInTree.BuildItems<IContextActionProvider>("/SharpDevelop/ViewContent/TextEditor/C#/ContextActions", null);
				editor.ContextActionProviders.AddRange(contextActionProviders);
			}
		}
		public void Detach()
		{
			codeManipulation.Dispose();
			if (inspectionManager != null) {
				inspectionManager.Dispose();
				inspectionManager = null;
			}
			if (contextActionProviders != null) {
				editor.ContextActionProviders.RemoveAll(contextActionProviders.Contains);
			}
			this.editor = null;
		}
	}
}

