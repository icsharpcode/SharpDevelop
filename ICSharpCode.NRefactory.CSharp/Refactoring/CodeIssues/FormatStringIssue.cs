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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Format string syntax error",
	                  Description = "Finds issues with format strings.",
	                  Category = IssueCategories.ConstraintViolations,
	                  Severity = Severity.Error)]
	public class FormatStringIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase
		{
			readonly BaseRefactoringContext context;
			readonly IType stringType;
			readonly IType objectType;
			
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
				this.context = context;
				stringType = context.Compilation.FindType(KnownTypeCode.String);
				objectType = context.Compilation.FindType(KnownTypeCode.Object);
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				Console.WriteLine(invocationExpression.GetText());
				base.VisitInvocationExpression(invocationExpression);
				var invocationResolveResult = context.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (invocationResolveResult == null)
					return;
				string format;
				IList<Expression> formatArguments;
				TextLocation formatStart;
				if (!TryGetFormattingParameters(invocationResolveResult, invocationExpression, out format, out formatStart, out formatArguments))
					return;
				var parsingResult = context.ParseFormatString(format);
				CheckSegments(parsingResult.Segments, formatStart, formatArguments, invocationExpression);
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
							AddIssue(segmentStart, segmentEnd, string.Format(outOfBounds, formatItem.Index));
						}
						if (formatItem.HasErrors) {
							var errorMessage = string.Join(Environment.NewLine, errors.Select(error => error.Message).ToArray());
							string messageFormat;
							if (errors.Count > 1) {
								messageFormat = context.TranslateString("Multiple:\n{0}");
							} else {
								messageFormat = context.TranslateString("{0}");
							}
							AddIssue(segmentStart, segmentEnd, string.Format(messageFormat, errorMessage));
						}
					} else if (segment.HasErrors) {
						foreach (var error in errors) {
							var errorStart = new TextLocation(formatStart.Line, formatStart.Column + error.StartLocation + 1);
							var errorEnd = new TextLocation(formatStart.Line, formatStart.Column + error.EndLocation + 1);
							AddIssue(errorStart, errorEnd, error.Message);
						}
					}
				}
			}

			static string[] parameterNames = { "format", "frmt", "fmt" };

			bool TryGetFormattingParameters(CSharpInvocationResolveResult invocationResolveResult, InvocationExpression invocationExpression,
			                                out string format, out TextLocation formatStart, out IList<Expression> arguments)
			{
				format = null;
				formatStart = default(TextLocation);
				arguments = new List<Expression>();
				var argumentToParameterMap = invocationResolveResult.GetArgumentToParameterMap();
				var resolvedParameters = invocationResolveResult.Member.Parameters;
				var allArguments = invocationExpression.Arguments.ToArray();
				for (int i = 0; i < allArguments.Length; i++) {
					var parameterIndex = argumentToParameterMap[i];
					if (parameterIndex < 0 || parameterIndex >= resolvedParameters.Count) {
						// No valid mapping for this parameter, skip it
						continue;
					}
					var parameter = resolvedParameters[parameterIndex];
					var argument = allArguments[i];
					if (parameter.Type.Equals(stringType) && parameterNames.Contains(parameter.Name) && argument is PrimitiveExpression) {
						format = (string)((PrimitiveExpression)argument).Value;
						formatStart = argument.StartLocation;
					} else if (parameter.IsParams || parameter.Type.Equals(objectType)) {
						arguments.Add(argument);
					}
				}
				return format != null;
			}
		}
	}
}

