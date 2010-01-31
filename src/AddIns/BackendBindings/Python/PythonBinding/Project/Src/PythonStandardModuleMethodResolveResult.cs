// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleMethodGroupResolveResult : MethodGroupResolveResult
	{
		public PythonStandardModuleMethodGroupResolveResult(IReturnType containingType, string methodName, MethodGroup[] methodGroups)
			: base(null, null, containingType, methodName, methodGroups)
		{
		}
		
		public static PythonStandardModuleMethodGroupResolveResult Create(PythonStandardModuleType type, string methodName)
		{
			PythonModuleCompletionItems completionItems = PythonModuleCompletionItemsFactory.Create(type);
			MethodGroup methods = completionItems.GetMethods(methodName);
			if (methods.Count > 0) {
				return Create(methods);
			}
			return null;
		}
		
		static PythonStandardModuleMethodGroupResolveResult Create(MethodGroup methods)
		{
			MethodGroup[] methodGroups = new MethodGroup[] { methods };
			IMethod method = methods[0];
			IReturnType returnType = new DefaultReturnType(method.DeclaringType);
			return new PythonStandardModuleMethodGroupResolveResult(returnType, method.Name, methodGroups);
		}
	}
}
