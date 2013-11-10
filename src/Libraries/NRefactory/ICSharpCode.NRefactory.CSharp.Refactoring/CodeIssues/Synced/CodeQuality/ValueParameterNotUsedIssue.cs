//
// SetterDoesNotUseValueParameterTests.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Threading;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'value' parameter not used",
	       Description = "Warns about property or indexer setters and event adders or removers that do not use the value parameter.",
	       Category = IssueCategories.CodeQualityIssues,
	       Severity = Severity.Warning,
           AnalysisDisableKeyword = "ValueParameterNotUsed")]
	public class ValueParameterNotUsedIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}
		
		class GatherVisitor : GatherVisitorBase<ValueParameterNotUsedIssue>
		{
			public GatherVisitor(BaseRefactoringContext context, ValueParameterNotUsedIssue inspector) : base (context)
			{
			}

            public override void VisitAccessor(Accessor accessor)
		    {
		        if (accessor.Role == PropertyDeclaration.SetterRole) {
                    FindIssuesInAccessor(accessor, ctx.TranslateString("The setter does not use the 'value' parameter"));
                } else if (accessor.Role == CustomEventDeclaration.AddAccessorRole) {
                    FindIssuesInAccessor(accessor, ctx.TranslateString("The add accessor does not use the 'value' parameter"));
                } else if (accessor.Role == CustomEventDeclaration.RemoveAccessorRole) {
                    FindIssuesInAccessor(accessor, ctx.TranslateString("The remove accessor does not use the 'value' parameter"));
                }
		    }

		    public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
		    {
                if (eventDeclaration.AddAccessor.Body.Statements.Count == 0 && eventDeclaration.RemoveAccessor.Body.Statements.Count == 0)
                    return;
		        
		        base.VisitCustomEventDeclaration(eventDeclaration);
		    }

		    void FindIssuesInAccessor(Accessor accessor, string accessorName)
			{
				var body = accessor.Body;
				if (!IsEligible(body))
					return;

				var localResolveResult = ctx.GetResolverStateBefore(body)
					.LookupSimpleNameOrTypeName("value", new List<IType>(), NameLookupMode.Expression) as LocalResolveResult; 
				if (localResolveResult == null)
					return;

				bool referenceFound = false;
				foreach (var result in ctx.FindReferences (body, localResolveResult.Variable)) {
					var node = result.Node;
					if (node.StartLocation >= body.StartLocation && node.EndLocation <= body.EndLocation) {
						referenceFound = true;
						break;
					}
				}

				if(!referenceFound)
					AddIssue(new CodeIssue(accessor.Keyword, accessorName));
			}

			static bool IsEligible(BlockStatement body)
			{
				if (body == null || body.IsNull)
					return false;
				if (body.Statements.FirstOrNullObject() is ThrowStatement)
					return false;
				return true;
			}
		}
	}
}
