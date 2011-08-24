// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.FormsDesigner
{
	public class VBNetDesignerGenerator : AbstractDesignerGenerator
	{
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.VisualBasic.VBCodeProvider();
		}
		
		protected override DomRegion GetReplaceRegion(IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine, 1);
		}
		
		protected override void RemoveFieldDeclaration(IDocument document, IField field)
		{
			// In VB, the field region begins at the start of the declaration
			// and ends on the first column of the line following the declaration.
			int startOffset = document.PositionToOffset(field.Region.BeginLine, 1);
			int endOffset   = document.PositionToOffset(field.Region.EndLine, 1);
			document.Remove(startOffset, endOffset - startOffset);
		}
		
		protected override void ReplaceFieldDeclaration(IDocument document, IField oldField, string newFieldDeclaration)
		{
			// In VB, the field region begins at the start of the declaration
			// and ends on the first column of the line following the declaration.
			int startOffset = document.PositionToOffset(oldField.Region.BeginLine, 1);
			int endOffset   = document.PositionToOffset(oldField.Region.EndLine, 1);
			document.Replace(startOffset, endOffset - startOffset, tabs + newFieldDeclaration + Environment.NewLine);
		}
		
		protected override string CreateEventHandler(Type eventType, string eventMethodName, string body, string indentation)
		{
			string param = GenerateParams(eventType);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "Sub " + eventMethodName + "(" + param + ")");
			if (string.IsNullOrEmpty(body)) {
				if (ICSharpCode.FormsDesigner.Gui.OptionPanels.GeneralOptionsPanel.InsertTodoComment) {
					body = "' TODO: Implement " + eventMethodName;
				}
			}
			string singleIndent = EditorControlService.GlobalOptions.IndentationString;
			b.AppendLine(indentation + singleIndent + body);
			b.AppendLine(indentation + "End Sub");
			return b.ToString();
		}
		
		protected string GenerateParams(Type eventType)
		{
			VBNetOutputVisitor v = new VBNetOutputVisitor();
			MethodDeclaration md = ConvertEventInvokeMethodToNRefactory(this.CurrentClassPart, eventType, "name");
			if (md != null) {
				v.AppendCommaSeparatedList(md.Parameters);
			}
			return v.Text;
		}
		
		protected override bool CompareMethodNames(string strA, string strB)
		{
			return String.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
		}
	}
}
