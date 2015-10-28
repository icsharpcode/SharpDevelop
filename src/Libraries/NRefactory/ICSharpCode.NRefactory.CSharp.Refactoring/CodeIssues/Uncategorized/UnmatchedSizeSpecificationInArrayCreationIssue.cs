// 
// UnmatchedSizeSpecificationInArrayCreationIssue.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using ICSharpCode.NRefactory.Refactoring;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// When array initializer has the different number of elements as specified in size creation, it is an error.
	/// </summary>
//	[IssueDescription("Correct size specification in array creation",
//	                  Description= "When array initializer has the different number of elements as specified in size creation, it is an error.",
//	                  Category = IssueCategories.CompilerErrors,
//	                  Severity = Severity.Error,
//	                  AnalysisDisableKeyword = "UnmatchedSizeSpecificationInArrayCreation")]
	public class UnmatchedSizeSpecificationInArrayCreationIssue : GatherVisitorCodeIssueProvider
	{	
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<UnmatchedSizeSpecificationInArrayCreationIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base (ctx)
			{
			}
			
			public override void VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
			{
				base.VisitArrayCreateExpression(arrayCreateExpression);
				
				if (arrayCreateExpression == null)
					return;
				if (arrayCreateExpression.Arguments == null || !arrayCreateExpression.Arguments.Any()) {
					return;
				}
				
				var argument = arrayCreateExpression.Arguments.FirstOrNullObject();
				if (argument == null || !(argument is PrimitiveExpression))
					return;
				
				int arraySize = (Int32)((PrimitiveExpression)argument).Value;
				
				if (arraySize < 1)
					return;
				
				var initializer = arrayCreateExpression.Initializer;
				if (initializer.IsNull)
					return;
				
				if (arraySize != initializer.Elements.Count) {
					AddIssue(new CodeIssue(argument, ctx.TranslateString("Unmatched size specification with array initializer"), ctx.TranslateString("Correct array size specification"), script => {
						var newArgument = new PrimitiveExpression(initializer.Elements.Count);
						script.Replace(argument, newArgument);}));
				}
			}
		}
	}
}

