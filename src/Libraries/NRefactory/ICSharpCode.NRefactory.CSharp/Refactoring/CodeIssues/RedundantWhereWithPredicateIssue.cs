using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Any()/First()/etc. should be used with predicate and Where() removed",
	                  Description= "Detects redundant Where() with predicate calls followed by Any().",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Hint)]
	public class RedundantWhereWithPredicateIssue : ICodeIssueProvider
	{
		static readonly AstNode pattern =
			new InvocationExpression (
				new MemberReferenceExpression (
					new NamedNode ("whereInvoke",
					               new InvocationExpression (
					               	new MemberReferenceExpression (new AnyNode ("target"), "Where"),
					               	new AnyNode ())),
					Pattern.AnyString));
		
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}
			
			public override void VisitInvocationExpression (InvocationExpression anyInvoke)
			{
				base.VisitInvocationExpression (anyInvoke);
				
				var match = pattern.Match (anyInvoke);
				if (!match.Success)
					return;
				
				var anyResolve = ctx.Resolve (anyInvoke) as InvocationResolveResult;
				if (anyResolve == null || !HasPredicateVersion(anyResolve.Member))
					return;
				var whereInvoke = match.Get<InvocationExpression> ("whereInvoke").Single ();
				var whereResolve = ctx.Resolve (whereInvoke) as InvocationResolveResult;
				if (whereResolve == null || whereResolve.Member.Name != "Where" || !IsQueryExtensionClass(whereResolve.Member.DeclaringTypeDefinition))
					return;
				if (whereResolve.Member.Parameters.Count != 2)
					return;
				var predResolve = whereResolve.Member.Parameters [1];
				if (predResolve.Type.TypeParameterCount != 2)
					return;
				
				AddIssue (
					anyInvoke, string.Format("Redundant Where() call with predicate followed by {0}()", anyResolve.Member.Name),
				    script => {
						var arg = whereInvoke.Arguments.Single ().Clone ();
						var target = match.Get<Expression> ("target").Single ().Clone ();
						script.Replace (anyInvoke, new InvocationExpression (new MemberReferenceExpression (target, anyResolve.Member.Name), arg));
					});
			}
			
			bool IsQueryExtensionClass(ITypeDefinition typeDef)
			{
				if (typeDef == null || typeDef.Namespace != "System.Linq")
					return false;
				switch (typeDef.Name) {
					case "Enumerable":
					case "ParallelEnumerable":
					case "Queryable":
						return true;
					default:
						return false;
				}
			}
			
			bool HasPredicateVersion(IParameterizedMember member)
			{
				if (!IsQueryExtensionClass(member.DeclaringTypeDefinition))
					return false;
				switch (member.Name) {
					case "Any":
					case "Count":
					case "First":
					case "FirstOrDefault":
					case "Last":
					case "LastOrDefault":
					case "LongCount":
					case "Single":
					case "SingleOrDefault":
						return true;
					default:
						return false;
				}
			}
		}
	}
}
