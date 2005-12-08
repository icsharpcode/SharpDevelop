// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using System.CodeDom;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.FormsDesigner;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerGenerator : AbstractDesignerGenerator
	{
		protected override string GenerateFieldDeclaration(CodeDOMGenerator domGenerator, CodeMemberField field)
		{
			// TODO: add support for modifiers
			// (or implement code generation for fields in the Boo CodeDomProvider)
			return "private " + field.Name + " as " + field.Type.BaseType;
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Boo.Lang.CodeDom.BooCodeProvider();
		}
		
		protected override string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body, string indentation)
		{
			if (string.IsNullOrEmpty(body)) body = "pass";
			string param = GenerateParams(edesc);
			
			StringBuilder b = new StringBuilder();
			b.AppendLine(indentation);
			b.AppendLine(indentation + "private def " + eventMethodName + "(" + param + "):");
			b.AppendLine(indentation + "\t" + body);
			return b.ToString();
		}
		
		protected override DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine + 1, 1);
		}
		
		protected override int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine + 1;
		}
		
		protected static string GenerateParams(EventDescriptor edesc)
		{
			Type type =  edesc.EventType;
			MethodInfo mInfo = type.GetMethod("Invoke");
			string param = "";
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				param += pInfo.Name;
				param += " as ";
				
				string typeStr = pInfo.ParameterType.ToString();
				typeStr = BooAmbience.Instance.GetIntrinsicTypeName(typeStr);
				param += typeStr;
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
	}
}
