// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.WpfDesign.AddIn
{
	sealed class CSharpEventHandlerService : AbstractEventHandlerService
	{
		public CSharpEventHandlerService(WpfViewContent viewContent) : base(viewContent)
		{
		}
		
		protected override void CreateEventHandlerInternal(Type eventHandlerType, string handlerName)
		{
			IClass c = GetDesignedClass();
			if (c != null) {
				foreach (IMethod m in c.Methods) {
					if (m.Name == handlerName) {
						FileService.JumpToFilePosition(m.DeclaringType.CompilationUnit.FileName,
						                               m.Region.BeginLine, m.Region.BeginColumn);
						return;
					}
				}
			}
			c = GetDesignedClassCodeBehindPart(c);
			if (c != null) {
				ITextEditorProvider tecp = FileService.OpenFile(c.CompilationUnit.FileName) as ITextEditorProvider;
				if (tecp != null) {
					int lineNumber;
					FormsDesigner.CSharpDesignerGenerator.CreateComponentEvent(
						c, tecp.TextEditor, eventHandlerType, handlerName, null,
						out lineNumber);
					tecp.TextEditor.JumpTo(lineNumber, 0);
				}
			}
		}
	}
}
