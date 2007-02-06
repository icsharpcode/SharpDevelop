// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Ast;

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of CSharpWorkflowDesignerGenerator.
	/// </summary>
	public class CSharpWorkflowDesignerEventBindingService : WorkflowDesignerEventBindingService
	{
		public CSharpWorkflowDesignerEventBindingService(IServiceProvider provider, string codeSeparationFileName) : base(provider, codeSeparationFileName)
		{
		}

		/// <summary>
		/// Finds the start line for the cursor when positioning inside a method.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		protected override int GetCursorLine(IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			int offset = document.PositionToOffset(new Point(r.BeginColumn - 1, r.BeginLine - 1));
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
	
		protected override string CreateEventHandler(IClass completeClass, EventDescriptor edesc, string eventMethodName, string body, string indentation)
		{
			string param = GenerateParameters(completeClass, edesc, true);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "void " + eventMethodName + "(" + param + ")");
			b.AppendLine(indentation + "{");
			b.AppendLine(indentation + "\t" + body);
			b.AppendLine(indentation + "}");
			return b.ToString();
		}
		
		protected string GenerateParameters(IClass completeClass, EventDescriptor eventDescriptor, bool paramNames)
		{
			CSharpOutputVisitor v = new CSharpOutputVisitor();
			MethodDeclaration md = ConvertDescriptorToNRefactory(completeClass, eventDescriptor, "name");
			if (md != null) {
				v.AppendCommaSeparatedList(md.Parameters);
			}
			return v.Text;
		}
	}
}
