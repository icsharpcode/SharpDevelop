using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("No default constructor in base class",
					   Description = "There is no default constructor in the base class",
					   Category = IssueCategories.CompilerErrors,
					   Severity = Severity.Error,
					   IssueMarker = IssueMarker.Underline)]
	public class NoDefaultConstructorIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		private class GatherVisitor : GatherVisitorBase
		{
			private bool initializerInvoked;

			public GatherVisitor(BaseRefactoringContext context)
				: base(context)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var result = ctx.Resolve(typeDeclaration) as TypeResolveResult;
				var baseType = result.Type.GetNonInterfaceBaseTypes().FirstOrDefault(t => !t.IsKnownType(KnownTypeCode.Object) && !object.Equals(t, result.Type));

				if (baseType != null)
				{
					var constructor = baseType.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();

					if (constructor == null) {
						base.VisitTypeDeclaration(typeDeclaration);

						if (!this.initializerInvoked) {
							var identifier = typeDeclaration.GetChildByRole(Roles.Identifier);
							this.AddIssue(identifier, ctx.TranslateString("There is no default constructor in the base class"));
						}
					}
				}
			}

			public override void VisitConstructorInitializer(ConstructorInitializer constructorInitializer)
			{
				var result = ctx.Resolve(constructorInitializer);

				if (!result.IsError) {
					this.initializerInvoked = true;
				}
			}
		}
	}
}

