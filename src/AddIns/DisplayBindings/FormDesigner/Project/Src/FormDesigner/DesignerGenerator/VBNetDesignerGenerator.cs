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

namespace ICSharpCode.FormDesigner
{
	public class VBNetDesignerGenerator : AbstractDesignerGenerator
	{
		protected override string GenerateFieldDeclaration(Type fieldType, string name)
		{
			return "Private " + name + " As " + fieldType;
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.VisualBasic.VBCodeProvider();
		}
		
		protected override DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method)
		{
			DomRegion r = method.BodyRegion;
			return new DomRegion(r.BeginLine + 1, 1, r.EndLine, 1);
		}
		
		protected override string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body)
		{
			string param = GenerateParams(edesc);
			
			return "Sub " + eventMethodName + "(" + param + ")\n" +
				body +
				"\nEnd Sub\n\n";
		}
		
		protected static string GenerateParams(EventDescriptor edesc)
		{
			System.Type type =  edesc.EventType;
			MethodInfo mInfo = type.GetMethod("Invoke");
			string param = "";
			IAmbience csa = null;
			try {
				csa = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/VBNet", null);
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("VB ambience not found");
			}
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				param += pInfo.Name;
				param += " As ";
				
				string typeStr = pInfo.ParameterType.ToString();
				if (csa != null) {
					typeStr = csa.GetIntrinsicTypeName(typeStr);
				}
				param += typeStr;
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
	}
}
