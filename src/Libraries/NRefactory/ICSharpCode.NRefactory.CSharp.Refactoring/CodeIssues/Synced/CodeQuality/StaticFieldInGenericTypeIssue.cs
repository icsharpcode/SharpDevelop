//
// StaticFieldInGenericTypeIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using System.Diagnostics.CodeAnalysis;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Static field in generic type",
	                  Description = "Warns about static fields in generic types.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
	                  SuppressMessageCategory = "Microsoft.Design",
	                  SuppressMessageCheckId  = "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
                      AnalysisDisableKeyword = "StaticFieldInGenericType"
	                  )]
	public class StaticFieldInGenericTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<StaticFieldInGenericTypeIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			IList<ITypeParameter> availableTypeParameters = new List<ITypeParameter>();

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var typeResolveResult = ctx.Resolve(typeDeclaration);
				var typeDefinition = typeResolveResult.Type.GetDefinition();
				if (typeDefinition == null)
					return;
				var newTypeParameters = typeDefinition.TypeParameters;

				var oldTypeParameters = availableTypeParameters; 
				availableTypeParameters = Concat(availableTypeParameters, newTypeParameters);

				base.VisitTypeDeclaration(typeDeclaration);

				availableTypeParameters = oldTypeParameters;
			}

			static IList<ITypeParameter> Concat(params IList<ITypeParameter>[] lists)
			{
				return lists.SelectMany(l => l).ToList();
			}

			bool UsesAllTypeParameters(FieldDeclaration fieldDeclaration)
			{
				if (availableTypeParameters.Count == 0)
					return true;

				var fieldType = ctx.Resolve(fieldDeclaration.ReturnType).Type as ParameterizedType;
				if (fieldType == null)
					return false;

				// Check that all current type parameters are used in the field type
				var fieldTypeParameters = fieldType.TypeArguments;
				foreach (var typeParameter in availableTypeParameters) {
					if (!fieldTypeParameters.Contains(typeParameter))
						return false;
				}
				return true;
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				base.VisitFieldDeclaration(fieldDeclaration);
				if (fieldDeclaration.Modifiers.HasFlag(Modifiers.Static) && !UsesAllTypeParameters(fieldDeclaration)) {
					AddIssue(new CodeIssue(fieldDeclaration, ctx.TranslateString("Static field in generic type")));
				}
			}
		}
	}
}

