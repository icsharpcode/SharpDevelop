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
	public class CSharpDesignerGenerator : AbstractDesignerGenerator
	{
		protected override DomRegion GetReplaceRegion(IDocument document, IMethod method)
		{
			return new DomRegion(GetCursorLine(document, method), 1, method.BodyRegion.EndLine, 1);
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.CSharp.CSharpCodeProvider();
		}
		
		protected override string CreateEventHandler(Type eventType, string eventMethodName, string body, string indentation)
		{
			string param = GenerateParams(eventType, true);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "void " + eventMethodName + "(" + param + ")");
			b.AppendLine(indentation + "{");
			if (string.IsNullOrEmpty(body)) {
				if (ICSharpCode.FormsDesigner.Gui.OptionPanels.GeneralOptionsPanel.InsertTodoComment) {
					body = "// TODO: Implement " + eventMethodName;
				}
			}
			string singleIndent = EditorControlService.GlobalOptions.IndentationString;
			b.AppendLine(indentation + singleIndent + body);
			b.AppendLine(indentation + "}");
			return b.ToString();
		}
		
		protected override int GetCursorLineAfterEventHandlerCreation()
		{
			return 3;
		}
		
		protected override int GetCursorLine(IDocument document, IMethod method)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (method == null)
				throw new ArgumentNullException("method");
			
			DomRegion r = method.BodyRegion;
			int offset = document.PositionToOffset(r.BeginLine, r.BeginColumn);
			while (offset < document.TextLength) {
				char c = document.GetCharAt(offset++);
				if (c == '{') {
					return r.BeginLine + 1;
				}
				if (c != ' ') {
					break;
				}
			}
			return r.BeginLine + 2;
		}
		
		protected string GenerateParams(Type eventType, bool paramNames)
		{
			CSharpOutputVisitor v = new CSharpOutputVisitor();
			MethodDeclaration md = ConvertEventInvokeMethodToNRefactory(CurrentClassPart, eventType, "name");
			if (md != null) {
				v.AppendCommaSeparatedList(md.Parameters);
			}
			return v.Text;
		}
		
		// static method that for use by the WPF designer
		public static void CreateComponentEvent(
			IClass c, ITextEditor editor,
			Type eventType, string eventMethodName, string body, out int lineNumber)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (eventType == null)
				throw new ArgumentNullException("edesc");
			
			IDocument document = editor.Document;
			
			CSharpDesignerGenerator gen = new CSharpDesignerGenerator();
			
			gen.CurrentClassPart = c;
			int line = gen.GetEventHandlerInsertionLine(c);
			if (line > document.TotalNumberOfLines) {
				lineNumber = document.TotalNumberOfLines;
				return;
			}
			
			int offset = document.GetLine(line).Offset;
			
			string tabs = editor.Options.IndentationString;
			tabs += tabs;
			
			document.Insert(offset, gen.CreateEventHandler(eventType, eventMethodName, body, tabs));
			lineNumber = line + gen.GetCursorLineAfterEventHandlerCreation();
		}
	}
}
