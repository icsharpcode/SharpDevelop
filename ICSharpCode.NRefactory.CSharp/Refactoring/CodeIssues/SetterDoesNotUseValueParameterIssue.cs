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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Property or indexer setter does not use the value parameter",
	       Description = "Warns about property or indexer setters that do not use the value parameter.",
	       Category = IssueCategories.CodeQualityIssues,
	       Severity = Severity.Warning)]
	public class SetterDoesNotUseValueParameterIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase
		{
			readonly BaseRefactoringContext context;
			
			public GatherVisitor(BaseRefactoringContext context, SetterDoesNotUseValueParameterIssue inspector) : base (context)
			{
				this.context = context;
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				FindIssuesInNode(indexerDeclaration.Setter.Body);
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				FindIssuesInNode(propertyDeclaration.Setter.Body);
			}

			void FindIssuesInNode(AstNode node)
			{
				var setterVisitor = new ReferenceFinderVisitor(context, "value");
				node.AcceptVisitor(setterVisitor);
				if (!setterVisitor.FoundReference)
					AddIssue(node, context.TranslateString("The setter does not use the 'value' parameter."));
			}
		}

		class ReferenceFinderVisitor: DepthFirstAstVisitor
		{
			readonly BaseRefactoringContext context;
			readonly string variableName;

			public ReferenceFinderVisitor(BaseRefactoringContext context, string variableName)
			{
				this.context = context;
				this.variableName = variableName;
			}

			public bool FoundReference { get; private set; }

			public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
			{
				var resolvedIdentifier = context.Resolve(identifierExpression) as LocalResolveResult;
				if (resolvedIdentifier != null)
					FoundReference |= resolvedIdentifier.Variable.Name == variableName;
				base.VisitIdentifierExpression(identifierExpression);
			}
		}
	}
}
