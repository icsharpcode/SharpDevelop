// 
// ConvertToStaticTypeIssue.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013  Ji Kun <jikun.nus@gmail.com>
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

using ICSharpCode.NRefactory.CSharp;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Class can be converted to static",
	                  Description = "If all fields, properties and methods members are static, the class can be made static.",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "ConvertToStaticType")]
	public class ConvertToStaticTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertToStaticTypeIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static bool IsMainMethod(IMember f)
			{
				return 
					f.SymbolKind == SymbolKind.Method &&
					(f.ReturnType.IsKnownType(KnownTypeCode.Void) || f.ReturnType.IsKnownType(KnownTypeCode.Int32)) &&
					f.IsStatic &&
					f.Name == "Main";
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				base.VisitTypeDeclaration(typeDeclaration);

				if (typeDeclaration == null || typeDeclaration.ClassType != ClassType.Class || typeDeclaration.HasModifier(Modifiers.Static))
					return;
				if (!typeDeclaration.Members.Any() || typeDeclaration.HasModifier(Modifiers.Abstract) || typeDeclaration.HasModifier(Modifiers.Partial))
					return;
				if (typeDeclaration.Members.Where(m => !(m is TypeDeclaration)).Any(f => !f.HasModifier(Modifiers.Static) && !f.HasModifier(Modifiers.Const)))
					return;
				var rr = ctx.Resolve(typeDeclaration);
				if (rr.IsError || rr.Type.GetMembers().Any(IsMainMethod))
					return;
				AddIssue(new CodeIssue(
					typeDeclaration.NameToken, 
					ctx.TranslateString("This class is recommended to be defined as static"),
					ctx.TranslateString("Make class static"),
					s => s.ChangeModifier(typeDeclaration, (typeDeclaration.Modifiers & ~Modifiers.Sealed) | Modifiers.Static)
				));
			}
		}
	}
}