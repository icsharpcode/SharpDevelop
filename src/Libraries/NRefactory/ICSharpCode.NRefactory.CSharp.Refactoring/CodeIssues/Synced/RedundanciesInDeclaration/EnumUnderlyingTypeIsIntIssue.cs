//
// EnumUnderlyingTypeIsIntIssue.cs
//
// Author:
//       Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Underlying type of enum is int",
	                  Description = "The default underlying type of enums is int, so defining it explicitly is redundant.",
	                  Category = IssueCategories.RedundanciesInDeclarations,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "EnumUnderlyingTypeIsInt")]
	public class EnumUnderlyingTypeIsIntIssue : CodeIssueProvider
	{
		public override IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context, string subIssue)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<EnumUnderlyingTypeIsIntIssue>
		{
			public GatherVisitor(BaseRefactoringContext context)
				: base(context)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				switch (typeDeclaration.ClassType) {
					case ClassType.Class:
					case ClassType.Struct:
						//Visit nested types
						base.VisitTypeDeclaration(typeDeclaration);
						return;
					case ClassType.Interface:
						return;
				}

				var underlyingType = typeDeclaration.BaseTypes.FirstOrDefault();

				if (underlyingType != null && ctx.ResolveType(underlyingType).FullName == "System.Int32") {
					var colonToken = typeDeclaration.ColonToken;
					var startLocation = colonToken.StartLocation;
					var endLocation = underlyingType.EndLocation;

					AddIssue(new CodeIssue(startLocation,
					         endLocation,
					         ctx.TranslateString("Default underlying type of enums is already int"),
					         ctx.TranslateString("Remove redundant ': int'"),
					         script =>
					{
						script.ChangeBaseTypes(typeDeclaration, Enumerable.Empty<AstType>());
						}) { IssueMarker = IssueMarker.GrayOut });
				}
			}

			CodeAction GetFixAction(TypeDeclaration typeDeclaration, TextLocation start, TextLocation end)
			{
				return new CodeAction(ctx.TranslateString("Remove redundant underlying type"),
				                      script => {

					script.ChangeBaseTypes(typeDeclaration, Enumerable.Empty<AstType>());

				}, start, end);
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				//No need to visit statements
			}
		}
	}
}

