//
// FormatStringIssue.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("String formatting problems",
	                  Description = "Finds issues with format strings.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "FormatStringProblem")]
	public class FormatStringProblemIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<FormatStringProblemIssue>
		{
			readonly BaseRefactoringContext context;

			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
				this.context = context;
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				// Speed up the inspector by discarding some invocations early
				var hasEligibleArgument = invocationExpression.Arguments.Any(argument => {
					var primitiveArg = argument as PrimitiveExpression;
					return primitiveArg != null && primitiveArg.Value is string;
				});
				if (!hasEligibleArgument)
					return;

				var invocationResolveResult = context.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (invocationResolveResult == null)
					return;
				Expression formatArgument;
				IList<Expression> formatArguments;
				if (!FormatStringHelper.TryGetFormattingParameters(invocationResolveResult, invocationExpression,
				                                                   out formatArgument, out formatArguments, null)) {
					return;
				}
				var primitiveArgument = formatArgument as PrimitiveExpression;
				if (primitiveArgument == null || !(primitiveArgument.Value is string))
					return;
				var format = (string)primitiveArgument.Value;
				var parsingResult = context.ParseFormatString(format);
				CheckSegments(parsingResult.Segments, formatArgument.StartLocation, formatArguments, invocationExpression);

				var argUsed = new HashSet<int> ();

				foreach (var item in parsingResult.Segments) {
					var fi = item as FormatItem;
					if (fi == null)
						continue;
					argUsed.Add(fi.Index);
				}
				for (int i = 0; i < formatArguments.Count; i++) {
					if (!argUsed.Contains(i)) {
						AddIssue(new CodeIssue(formatArguments[i], ctx.TranslateString("Argument is not used in format string")) { IssueMarker = IssueMarker.GrayOut });
					}
				}
			}

			void CheckSegments(IList<IFormatStringSegment> segments, TextLocation formatStart, IList<Expression> formatArguments, AstNode anchor)
			{
				int argumentCount = formatArguments.Count;
				foreach (var segment in segments) {
					var errors = segment.Errors.ToList();
					var formatItem = segment as FormatItem;
					if (formatItem != null) {
						var segmentEnd = new TextLocation(formatStart.Line, formatStart.Column + segment.EndLocation + 1);
						var segmentStart = new TextLocation(formatStart.Line, formatStart.Column + segment.StartLocation + 1);
						if (formatItem.Index >= argumentCount) {
							var outOfBounds = context.TranslateString("The index '{0}' is out of bounds of the passed arguments");
							AddIssue(new CodeIssue(segmentStart, segmentEnd, string.Format(outOfBounds, formatItem.Index)));
						}
						if (formatItem.HasErrors) {
							var errorMessage = string.Join(Environment.NewLine, errors.Select(error => error.Message).ToArray());
							string messageFormat;
							if (errors.Count > 1) {
								messageFormat = context.TranslateString("Multiple:\n{0}");
							} else {
								messageFormat = context.TranslateString("{0}");
							}
							AddIssue(new CodeIssue(segmentStart, segmentEnd, string.Format(messageFormat, errorMessage)));
						}
					} else if (segment.HasErrors) {
						foreach (var error in errors) {
							var errorStart = new TextLocation(formatStart.Line, formatStart.Column + error.StartLocation + 1);
							var errorEnd = new TextLocation(formatStart.Line, formatStart.Column + error.EndLocation + 1);
							AddIssue(new CodeIssue(errorStart, errorEnd, error.Message));
						}
					}
				}
			}
		}
	}
}

