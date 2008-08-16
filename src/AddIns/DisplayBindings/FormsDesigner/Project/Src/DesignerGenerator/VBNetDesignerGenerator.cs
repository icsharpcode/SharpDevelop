// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.FormsDesigner
{
	public class VBNetDesignerGenerator : AbstractDesignerGenerator
	{
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.VisualBasic.VBCodeProvider();
		}
		
		protected override DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine, 1);
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
			b.AppendLine(indentation + "\t" + body);
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
	}
}
