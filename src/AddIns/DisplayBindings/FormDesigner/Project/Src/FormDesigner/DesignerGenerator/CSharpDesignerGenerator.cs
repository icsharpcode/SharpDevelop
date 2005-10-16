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

namespace ICSharpCode.FormDesigner
{
	public class CSharpDesignerGenerator : AbstractDesignerGenerator
	{
		protected override string GenerateFieldDeclaration(Type fieldType, string name)
		{
			return "private " + fieldType + " " + name + ";";
		}
		
		protected override System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider()
		{
			return new Microsoft.CSharp.CSharpCodeProvider();
		}
		
		protected override string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body)
		{
			string param = GenerateParams(edesc, true);
			
			return "void " + eventMethodName + "(" + param + ")\n" +
				"{\n" + body +
				"\n}\n\n";
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
