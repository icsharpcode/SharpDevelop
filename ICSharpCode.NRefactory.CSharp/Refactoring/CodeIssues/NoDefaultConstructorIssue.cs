using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("CS1729: Base class does not contain a 0 argument constructor",
					   Description = "CS1729: Base class does not contain a 0 argument constructor",
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
			private IType baseType;
			private ConstructorInitializer initializer;

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
					// Store the base type to use when visiting individual constructors
					this.baseType = baseType;

					var baseConstructor = baseType.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();

					if (baseConstructor == null) {
						var constructor = result.Type.GetConstructors(f => !f.IsSynthetic).FirstOrDefault();

						if (constructor == null) {
							// If there are no constructors declared then the base constructor isn't being invoked
							this.AddIssue(declaration, baseType);
						} else {
							base.VisitTypeDeclaration(declaration);
						}
					}
				}
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration declaration)
			{
				this.initializerInvoked = false;
				this.initializer = null;
				
				base.VisitConstructorDeclaration(declaration);

				if (!this.initializerInvoked) {
					int argumentCount = initializer != null ? initializer.Arguments.Count : 0;
					this.AddIssue(declaration, baseType, argumentCount);
				}
			}

			public override void VisitConstructorInitializer(ConstructorInitializer initializer)
			{
				var result = ctx.Resolve(initializer);

				if (!result.IsError) {
					this.initializerInvoked = true;
				} else {
					this.initializer = initializer;
				}
			}

			private void AddIssue(AstNode node, IType baseClass, int argumentCount = 0)
			{
				var identifier = node.GetChildByRole(Roles.Identifier);
				var errorMessage = string.Format("The type '{0}' does not contain a constructor that takes '{1}' arguments", baseClass.FullName, argumentCount);

				this.AddIssue(identifier, ctx.TranslateString(errorMessage));
			}
		}
	}
}

