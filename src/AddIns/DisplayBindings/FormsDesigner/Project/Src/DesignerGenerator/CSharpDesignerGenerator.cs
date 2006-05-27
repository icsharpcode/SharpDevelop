// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.FormsDesigner
{
	public class CSharpDesignerGenerator : AbstractDesignerGenerator
	{
		protected override DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			return new DomRegion(GetCursorLine(document, method), 1, method.BodyRegion.EndLine, 1);
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.CSharp.CSharpCodeProvider();
		}
		
		protected override string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body, string indentation)
		{
			string param = GenerateParams(edesc, true);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "void " + eventMethodName + "(" + param + ")");
			b.AppendLine(indentation + "{");
			b.AppendLine(indentation + "\t" + body);
			b.AppendLine(indentation + "}");
			return b.ToString();
		}
		
		protected override int GetCursorLineAfterEventHandlerCreation()
		{
			return 3;
		}
		
		protected override int GetCursorLine(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			int offset = document.PositionToOffset(new Point(r.BeginColumn - 1, r.BeginLine - 1));
			string tmp = document.GetText(offset, 10);
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
		
		protected static string GenerateParams(EventDescriptor edesc, bool paramNames)
		{
			System.Type type =  edesc.EventType;
			MethodInfo mInfo = type.GetMethod("Invoke");
			string param = "";
			IAmbience csa = null;
			try {
				csa = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/C#", null);
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("C# ambience not found");
			}
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				string typeStr = pInfo.ParameterType.ToString();
				if (csa != null) {
					typeStr = csa.GetIntrinsicTypeName(typeStr);
				}
				param += typeStr;
				if (paramNames == true) {
					param += " ";
					param += pInfo.Name;
				}
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
	}
}
