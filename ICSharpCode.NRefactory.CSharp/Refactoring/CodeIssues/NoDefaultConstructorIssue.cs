using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;

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

		private class GatherVisitor : GatherVisitorBase<NoDefaultConstructorIssue>
		{
			private bool initializerInvoked;
			private ConstructorInitializer initializer;

			public GatherVisitor(BaseRefactoringContext context)
				: base(context)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration declaration)
			{
				var result = ctx.Resolve(declaration) as TypeResolveResult;
				var baseType = result.Type.DirectBaseTypes.FirstOrDefault(t => !t.IsKnownType(KnownTypeCode.Object) && t.Kind != TypeKind.Interface);

				if (baseType != null)
				{
					var baseConstructor = baseType.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();
					var memberLookup = new MemberLookup(result.Type.GetDefinition(), ctx.Compilation.MainAssembly, false);

					if (baseConstructor == null || !memberLookup.IsAccessible(baseConstructor, true)) {
						var constructor = result.Type.GetConstructors(f => !f.IsSynthetic).FirstOrDefault();

						if (constructor == null) {
							// If there are no constructors declared then the base constructor isn't being invoked
							this.AddIssue(declaration, baseType);
						}
					}
				}

				base.VisitTypeDeclaration(declaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration declaration)
			{
				var result = ctx.Resolve(declaration) as MemberResolveResult;
				if (result == null || result.IsError)
					return;

				var baseType = result.Member.DeclaringType.DirectBaseTypes.FirstOrDefault(t => !t.IsKnownType(KnownTypeCode.Object) && t.Kind != TypeKind.Interface);

				if (baseType != null) {
					var baseConstructor = baseType.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();
					var memberLookup = new MemberLookup(result.Member.DeclaringType.GetDefinition(), ctx.Compilation.MainAssembly, false);

					if (baseConstructor == null || !memberLookup.IsAccessible(baseConstructor, true)) {
						this.initializerInvoked = false;
						this.initializer = null;
				
						base.VisitConstructorDeclaration(declaration);

						if (!this.initializerInvoked) {
							int argumentCount = initializer != null ? initializer.Arguments.Count : 0;
							this.AddIssue(declaration, baseType, argumentCount);
						}
					}
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

			private void AddIssue(AstNode node, IType baseType, int argumentCount = 0)
			{
				var identifier = node.GetChildByRole(Roles.Identifier);
				this.AddIssue(
					identifier,
					string.Format(ctx.TranslateString("CS1729: The type '{0}' does not contain a constructor that takes '{1}' arguments"), baseType.Name, argumentCount));
			}
		}
	}
}

