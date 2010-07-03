// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
