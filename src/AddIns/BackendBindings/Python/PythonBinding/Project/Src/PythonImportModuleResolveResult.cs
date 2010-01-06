// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonImportModuleResolveResult : ResolveResult
	{
		PythonImportExpression expression;
		
		public PythonImportModuleResolveResult(PythonImportExpression expression)
			: base(null, null, null)
		{
			this.expression = expression;
		}
		
		public string Name { 
			get { return expression.Module; }
		}
		
		public override ArrayList GetCompletionData(IProjectContent projectContent)
		{
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			if (expression.HasFromAndImport) {
				if (expression.HasIdentifier) {
					return new ArrayList();
				} else {
					return completion.GetCompletionItemsFromModule(expression.Module);
				}
			}
			return completion.GetCompletionItems(expression.Module);
		}
		
		public override ResolveResult Clone()
		{
			return new PythonImportModuleResolveResult(expression);
		}
	}
}
