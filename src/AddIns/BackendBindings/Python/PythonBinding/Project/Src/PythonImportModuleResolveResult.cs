// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
