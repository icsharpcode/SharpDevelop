// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
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
		
		public override List<ICompletionEntry> GetCompletionData(IProjectContent projectContent)
		{
			PythonImportCompletion completion = new PythonImportCompletion(projectContent);
			if (expression.HasFromAndImport) {
				if (expression.HasIdentifier) {
					return new List<ICompletionEntry>();
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
