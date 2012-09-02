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

			public override void VisitTypeDeclaration(TypeDeclaration declaration)
			{
				var result = ctx.Resolve(declaration) as TypeResolveResult;
				var baseType = result.Type.GetNonInterfaceBaseTypes().FirstOrDefault(t => !t.IsKnownType(KnownTypeCode.Object) && !object.Equals(t, result.Type));

				if (baseType != null)
				{
					var baseConstructor = baseType.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();

					if (baseConstructor == null) {
						var constructor = result.Type.GetConstructors(f => !f.IsSynthetic).FirstOrDefault();

						if (constructor == null) {
							// If there are no constructors declared then the base constructor isn't being invoked
							this.AddIssue(declaration);
						} else {
							base.VisitTypeDeclaration(declaration);
						}
					}
				}
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration declaration)
			{
				this.initializerInvoked = false;
				
				base.VisitConstructorDeclaration(declaration);

				if (!this.initializerInvoked) {
					this.AddIssue(declaration);
				}
			}

			public override void VisitConstructorInitializer(ConstructorInitializer initializer)
			{
				var result = ctx.Resolve(initializer);

				if (!result.IsError) {
					this.initializerInvoked = true;
				}
			}

			private void AddIssue(AstNode node)
			{
				var identifier = node.GetChildByRole(Roles.Identifier);
				this.AddIssue(identifier, ctx.TranslateString("There is no default constructor in the base class"));
			}
		}
	}
}

