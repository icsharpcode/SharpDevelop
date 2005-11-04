// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.FormDesigner;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerGenerator : AbstractDesignerGenerator
	{
		protected override string GenerateFieldDeclaration(Type fieldType, string name)
		{
			return "private " + name + " as " + fieldType;
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Boo.Lang.CodeDom.BooCodeProvider();
		}
		
		protected override string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body)
		{
			if (string.IsNullOrEmpty(body)) body = "\tpass";
			string param = GenerateParams(edesc);
			return "private def " + eventMethodName + "(" + param + "):\n" +
				body +
				"\n";
		}
		
		protected override DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine + 1, 1);
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
