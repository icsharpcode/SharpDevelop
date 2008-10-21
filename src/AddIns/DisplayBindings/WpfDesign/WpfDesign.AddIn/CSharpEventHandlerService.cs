// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.WpfDesign.AddIn
{
	sealed class CSharpEventHandlerService : AbstractEventHandlerService
	{
		public CSharpEventHandlerService(WpfPrimaryViewContent viewContent)
			: base(viewContent)
		{
		}

		protected override void CreateEventHandlerInternal(Type eventHandlerType, string handlerName)
		{
			IClass c = GetDesignedClass();
			if (c != null) {
				foreach (IMethod m in c.Methods) {
					if (m.Name == handlerName) {
						FileService.JumpToFilePosition(m.DeclaringType.CompilationUnit.FileName,
													   m.Region.BeginLine - 1, m.Region.BeginColumn - 1);
						return;
					}
				}
			}
			c = GetDesignedClassCodeBehindPart(c);
			if (c != null) {
				ITextEditorControlProvider tecp = FileService.OpenFile(c.CompilationUnit.FileName) as ITextEditorControlProvider;
				if (tecp != null) {
					int lineNumber;
					FormsDesigner.CSharpDesignerGenerator.CreateComponentEvent(
						c, tecp.TextEditorControl.Document, eventHandlerType, handlerName, null,
						out lineNumber);
					tecp.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber - 1);
				}
			}
		}
	}
}
