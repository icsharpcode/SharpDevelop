//
// ParameterCouldBeDeclaredWithBaseTypeIssue.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using System;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System.Diagnostics;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Parameter can be declared with base type",
		Description = "Finds parameters that can be demoted to a base class.",
		Category = IssueCategories.PracticesAndImprovements,
		Severity = Severity.Hint,
		SuppressMessageCategory="Microsoft.Design",
		SuppressMessageCheckId="CA1011:ConsiderPassingBaseTypesAsParameters"
	)]
	public class ParameterCanBeDeclaredWithBaseTypeIssue : GatherVisitorCodeIssueProvider
	{
		bool tryResolve;

		public ParameterCanBeDeclaredWithBaseTypeIssue() : this (true)
		{
		}

		public ParameterCanBeDeclaredWithBaseTypeIssue(bool tryResolve)
		{
			this.tryResolve = tryResolve;
		}

		#region ICodeIssueProvider implementation
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, tryResolve);
		}
		#endregion

		class GatherVisitor : GatherVisitorBase<ParameterCanBeDeclaredWithBaseTypeIssue>
		{
			bool tryResolve;
			
			public GatherVisitor(BaseRefactoringContext context, bool tryResolve) : base (context)
			{
				this.tryResolve = tryResolve;
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				methodDeclaration.Attributes.AcceptVisitor(this);
				if (HasEntryPointSignature(methodDeclaration) || methodDeclaration.HasModifier(Modifiers.Public) || methodDeclaration.HasModifier(Modifiers.Protected))
					return;
				var eligibleParameters = methodDeclaration.Parameters
					.Where(p => p.ParameterModifier != ParameterModifier.Out && p.ParameterModifier != ParameterModifier.Ref)
					.ToList();
				if (eligibleParameters.Count == 0)
					return;
				var declarationResolveResult = ctx.Resolve(methodDeclaration) as MemberResolveResult;
				if (declarationResolveResult == null)
					return;
				var member = declarationResolveResult.Member;
				if (member.IsOverride || member.IsOverridable || member.ImplementedInterfaceMembers.Any())
					return;

				var collector = new TypeCriteriaCollector(ctx);
				methodDeclaration.AcceptVisitor(collector);

				foreach (var parameter in eligibleParameters) {
					ProcessParameter(parameter, methodDeclaration.Body, collector);
				}
			}

			bool HasEntryPointSignature(MethodDeclaration methodDeclaration)
			{
				if (!methodDeclaration.Modifiers.HasFlag(Modifiers.Static))
					return false;
				var returnType = ctx.Resolve(methodDeclaration.ReturnType).Type;
				if (!returnType.IsKnownType(KnownTypeCode.Int32) && !returnType.IsKnownType(KnownTypeCode.Void))
					return false;
				var parameterCount = methodDeclaration.Parameters.Count;
				if (parameterCount == 0)
					return true;
				if (parameterCount != 1)
					return false;
				var parameterType = ctx.Resolve(methodDeclaration.Parameters.First()).Type as ArrayType;
				if (parameterType == null || !parameterType.ElementType.IsKnownType(KnownTypeCode.String))
					return false;
				return true;
			}

			bool FilterOut(IType current, IType newType)
			{
				// Filter out some strange framework types like _Exception
				return newType.Namespace.StartsWith("System.", StringComparison.Ordinal) && 
					   newType.Name.StartsWith("_", StringComparison.Ordinal) ? true : false;
			}

			void ProcessParameter(ParameterDeclaration parameter, AstNode rootResolutionNode, TypeCriteriaCollector collector)
			{
				var localResolveResult = ctx.Resolve(parameter) as LocalResolveResult;
				if (localResolveResult == null)
					return;
				var variable = localResolveResult.Variable;
				var typeKind = variable.Type.Kind;
				if (!(typeKind == TypeKind.Class ||
					  typeKind == TypeKind.Struct ||
					  typeKind == TypeKind.Interface ||
					  typeKind == TypeKind.Array) ||
				    parameter.Type is PrimitiveType ||
					!collector.UsedVariables.Contains(variable)) {
					return;
				}

				var candidateTypes = localResolveResult.Type.GetAllBaseTypes().ToList();
				TypesChecked += candidateTypes.Count;
				var criterion = collector.GetCriterion(variable);

				var possibleTypes = 
					(from type in candidateTypes
					 where !type.Equals(localResolveResult.Type) && criterion.SatisfiedBy(type)
					 select type).ToList();

				TypeResolveCount += possibleTypes.Count;
				var validTypes = 
					(from type in possibleTypes
					 where (!tryResolve || TypeChangeResolvesCorrectly(ctx, parameter, rootResolutionNode, type)) && !FilterOut (variable.Type, type)
					 select type).ToList();
				if (validTypes.Any()) {
					AddIssue(new CodeIssue(parameter.Type, ctx.TranslateString("Parameter can be declared with base type"), GetActions(parameter, validTypes)) {
						IssueMarker = IssueMarker.DottedLine
					});
					MembersWithIssues++;
				}
			}

			internal int TypeResolveCount = 0;
			internal int TypesChecked = 0;
			internal int MembersWithIssues = 0;
			internal int MethodResolveCount = 0;

			IEnumerable<CodeAction> GetActions(ParameterDeclaration parameter, IEnumerable<IType> possibleTypes)
			{
				var csResolver = ctx.Resolver.GetResolverStateBefore(parameter);
				var astBuilder = new TypeSystemAstBuilder(csResolver);
				foreach (var type in possibleTypes) {
					var localType = type;
					var message = String.Format(ctx.TranslateString("Demote parameter to '{0}'"), type.FullName);
					yield return new CodeAction(message, script => {
						script.Replace(parameter.Type, astBuilder.ConvertType(localType));
					}, parameter.Type);
				}
			}
		}

	    public static bool TypeChangeResolvesCorrectly(BaseRefactoringContext ctx, ParameterDeclaration parameter, AstNode rootNode, IType type)
	    {
	        var resolver = ctx.GetResolverStateBefore(rootNode);
	        resolver = resolver.AddVariable(new DefaultParameter(type, parameter.Name));
	        var astResolver = new CSharpAstResolver(resolver, rootNode, ctx.UnresolvedFile);
	        var validator = new TypeChangeValidationNavigator();
	        astResolver.ApplyNavigator(validator, ctx.CancellationToken);
	        return !validator.FoundErrors;
	    }

	    class TypeChangeValidationNavigator : IResolveVisitorNavigator
		{
			public bool FoundErrors { get; private set; }

			#region IResolveVisitorNavigator implementation
			public ResolveVisitorNavigationMode Scan(AstNode node)
			{
				if (FoundErrors)
					return ResolveVisitorNavigationMode.Skip;
				return ResolveVisitorNavigationMode.Resolve;
			}

			public void Resolved(AstNode node, ResolveResult result)
			{
//				bool errors = result.IsError;
				FoundErrors |= result.IsError;
			}

			public void ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType)
			{
				// no-op
			}
			#endregion
			
		}
	}

	public interface ITypeCriterion
	{
		/// <summary>
		/// Checks if the given type satisfies the critrion.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if the type satisfies the criterion, <c>false</c> otherwise.
		/// </returns>
		/// <param name='type'>
		/// The type to check.
		/// </param>
		bool SatisfiedBy(IType type);
	}

	public class SupportsIndexingCriterion : ITypeCriterion
	{
		IType returnType;

		IList<IType> argumentTypes;

		CSharpConversions conversions;

		bool isWriteAccess;

		public SupportsIndexingCriterion(IType returnType, IEnumerable<IType> argumentTypes, CSharpConversions conversions, bool isWriteAccess = false)
		{
			if (returnType == null)
				throw new ArgumentNullException("returnType");
			if (argumentTypes == null)
				throw new ArgumentNullException("argumentTypes");
			if (conversions == null)
				throw new ArgumentNullException("conversions");

			this.returnType = returnType;
			this.argumentTypes = argumentTypes.ToList();
			this.conversions = conversions;
			this.isWriteAccess = isWriteAccess;
		}

		#region ITypeCriterion implementation

		public bool SatisfiedBy(IType type)
		{
			var accessors = type.GetAccessors().ToList();
			return accessors.Any(member => {
				var parameterizedMember = member as IParameterizedMember;
				if (parameterizedMember == null)
					return false;

				if (isWriteAccess) {
					var parameterCount = member.Parameters.Count;
					if (member.Name != "set_Item" || parameterCount < 2)
						return false;
					var indexerElementType = parameterizedMember.Parameters.Last().Type;
					var indexerParameterTypes = parameterizedMember.Parameters.Take(parameterCount - 1).Select(p => p.Type).ToList();
					return IsSignatureMatch(indexerElementType, indexerParameterTypes);
				} else {
					if (member.Name != "get_Item" || member.Parameters.Count < 1)
						return false;
					var indexerElementType = parameterizedMember.ReturnType;
					var indexerParameterTypes = parameterizedMember.Parameters.Select(p => p.Type).ToList();
					return IsSignatureMatch(indexerElementType, indexerParameterTypes);
				}
			});
		}

		#endregion

		bool IsSignatureMatch(IType indexerElementType, IList<IType> indexerParameterTypes)
		{
			indexerElementType.GetAllBaseTypes();
			if (indexerParameterTypes.Count != argumentTypes.Count)
				return false;
			var returnConversion = conversions.ImplicitConversion(indexerElementType, returnType);
			if (!returnConversion.IsValid)
				return false;
			for (int i = 0; i < argumentTypes.Count; i++) {
				var conversion = conversions.ImplicitConversion(indexerParameterTypes[i], argumentTypes[i]);
				if (!conversion.IsValid)
					return false;
			}
			return true;
		}
	}

	public class TypeCriteriaCollector : DepthFirstAstVisitor
	{
		BaseRefactoringContext context;

		public TypeCriteriaCollector(BaseRefactoringContext context)
		{
			this.context = context;
			TypeCriteria = new Dictionary<IVariable, IList<ITypeCriterion>>();
			UsedVariables = new HashSet<IVariable>();
		}

		IDictionary<IVariable, IList<ITypeCriterion>> TypeCriteria { get; set; }

		public HashSet<IVariable> UsedVariables { get; private set; }

		public ITypeCriterion GetCriterion(IVariable variable)
		{
			if (!TypeCriteria.ContainsKey(variable))
				return new ConjunctionCriteria(new List<ITypeCriterion>());
			return new ConjunctionCriteria(TypeCriteria[variable]);
		}

		void AddCriterion(IVariable variable, ITypeCriterion criterion)
		{
			if (!TypeCriteria.ContainsKey(variable))
				TypeCriteria[variable] = new List<ITypeCriterion>();
			TypeCriteria[variable].Add(criterion);
		}

		public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			base.VisitMemberReferenceExpression(memberReferenceExpression);

			var targetResolveResult = context.Resolve(memberReferenceExpression.Target) as LocalResolveResult;
			if (targetResolveResult == null)
				return;
			var variable = targetResolveResult.Variable;
			var conversion = context.GetConversion(memberReferenceExpression);
			if (conversion.IsMethodGroupConversion) {
				AddCriterion(variable, new HasMemberCriterion(conversion.Method));
			} else {
				var resolveResult = context.Resolve(memberReferenceExpression);
				var memberResolveResult = resolveResult as MemberResolveResult;
				if (memberResolveResult != null)
					AddCriterion(variable, new HasMemberCriterion(memberResolveResult.Member));
			}
		}

		public override void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			base.VisitIndexerExpression(indexerExpression);

			var localResolveResult = context.Resolve(indexerExpression.Target) as LocalResolveResult;
			if (localResolveResult == null)
				return;
			var resolveResult = context.Resolve(indexerExpression);
			if (localResolveResult == null)
				return;
			var parent = indexerExpression.Parent;
			while (parent is ParenthesizedExpression)
				parent = parent.Parent;
			if (parent is DirectionExpression) {
				// The only types which are indexable and where the indexing expression
				// results in a variable is an actual array type
				AddCriterion(localResolveResult.Variable, new IsArrayTypeCriterion());
			} else if (resolveResult is ArrayAccessResolveResult) {
				var arrayResolveResult = (ArrayAccessResolveResult)resolveResult;
				var arrayType = arrayResolveResult.Array.Type as ArrayType;
				if (arrayType != null) {
					var parameterTypes = arrayResolveResult.Indexes.Select(index => index.Type);
					var criterion = new SupportsIndexingCriterion(arrayType.ElementType, parameterTypes, CSharpConversions.Get(context.Compilation));
					AddCriterion(localResolveResult.Variable, criterion);
				}
			} else if (resolveResult is CSharpInvocationResolveResult) {
				var invocationResolveResult = (CSharpInvocationResolveResult)resolveResult;
				var parameterTypes = invocationResolveResult.Arguments.Select(arg => arg.Type);
				var criterion = new SupportsIndexingCriterion(invocationResolveResult.Member.ReturnType, parameterTypes, CSharpConversions.Get(context.Compilation));
				AddCriterion(localResolveResult.Variable, criterion);
			}
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);

			var resolveResult = context.Resolve(invocationExpression);
			var invocationResolveResult = resolveResult as InvocationResolveResult;
			if (invocationResolveResult == null)
				return;

			// invocationExpression.Target resolves to a method group and VisitMemberReferenceExpression
			// only handles members, so handle method groups here
			var targetResolveResult = invocationResolveResult.TargetResult as LocalResolveResult;
			if (targetResolveResult != null) {
				var variable = targetResolveResult.Variable;
				AddCriterion(variable, new HasMemberCriterion(invocationResolveResult.Member));
			}
		}

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			base.VisitMethodDeclaration(methodDeclaration);

			var lastParameter = methodDeclaration.Parameters.LastOrNullObject();
			if (lastParameter.IsNull || !lastParameter.ParameterModifier.HasFlag(ParameterModifier.Params))
				return;
			var localResolveResult = context.Resolve(lastParameter) as LocalResolveResult;
			if (localResolveResult == null)
				return;
			AddCriterion(localResolveResult.Variable, new IsArrayTypeCriterion());
		}

		Role[] roles = new [] {
			Roles.Expression,
			Roles.Argument,
			Roles.Condition,
			BinaryOperatorExpression.RightRole,
			BinaryOperatorExpression.LeftRole
		};

		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			var resolveResult = context.Resolve(identifierExpression);
			var localResolveResult = resolveResult as LocalResolveResult;
			if (localResolveResult == null)
				return;

			var variable = localResolveResult.Variable;
			if (!UsedVariables.Contains(variable))
				UsedVariables.Add(variable);

			// Assignment expressions are checked separately, see VisitAssignmentExpression
			if (!roles.Contains(identifierExpression.Role) || identifierExpression.Parent is AssignmentExpression)
				return;

			CheckForCriterion(identifierExpression, variable);
		}

		public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			base.VisitAssignmentExpression(assignmentExpression);

			// Only check the right side; The left side always has the type of
			// the variable, which is not what we want to check

			var rightResolveResult = context.Resolve(assignmentExpression.Right) as LocalResolveResult;
			if (rightResolveResult != null) {
				CheckForCriterion(assignmentExpression.Right, rightResolveResult.Variable);
			}
		}

		void CheckForCriterion(Expression expression, IVariable variable)
		{
			AddCriterion(variable, new IsTypeCriterion(context.GetExpectedType(expression)));
		}

		class ConjunctionCriteria : ITypeCriterion
		{
			IList<ITypeCriterion> criteria;

			public ConjunctionCriteria(IList<ITypeCriterion> criteria)
			{
				this.criteria = criteria;
			}

			public bool SatisfiedBy(IType type)
			{
				foreach (var criterion in criteria) {
					if (!criterion.SatisfiedBy(type)) {
						return false;
					}
				}
				return true;
			}
		}
	}

	public class IsTypeCriterion : ITypeCriterion
	{
		IType isType;

		public IsTypeCriterion(IType isType)
		{
			this.isType = isType;
		}

		#region ITypeCriterion implementation
		public bool SatisfiedBy (IType type)
		{
			return isType == type ||
				type.GetAllBaseTypes().Any(t => t == isType);
		}
		#endregion
	}

	public class IsArrayTypeCriterion : ITypeCriterion
	{
		#region ITypeCriterion implementation

		public bool SatisfiedBy(IType type)
		{
			return type is ArrayType;
		}

		#endregion
	}

	public class HasMemberCriterion : ITypeCriterion
	{
//		IMember neededMember;
		IList<IMember> acceptableMembers;

		public HasMemberCriterion(IMember neededMember)
		{
//			this.neededMember = neededMember;

			if (neededMember.ImplementedInterfaceMembers.Any()) {
				acceptableMembers = neededMember.ImplementedInterfaceMembers.ToList();
			} else if (neededMember.IsOverride) {
				acceptableMembers = new List<IMember>();
				foreach (var member in InheritanceHelper.GetBaseMembers(neededMember, true)) {
					acceptableMembers.Add(member);
					if (member.IsShadowing)
						break;
				}
				acceptableMembers.Add(neededMember);
			} else {
				acceptableMembers = new List<IMember> { neededMember };
			}
		}

		#region ITypeCriterion implementation
		public bool SatisfiedBy (IType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			var typeMembers = type.GetMembers();
			return typeMembers.Any(member => HasCommonMemberDeclaration(acceptableMembers, member));
		}
		#endregion

		static bool HasCommonMemberDeclaration(IEnumerable<IMember> acceptableMembers, IMember member)
		{
			var implementedInterfaceMembers = member.MemberDefinition.ImplementedInterfaceMembers;
			if (implementedInterfaceMembers.Any()) {
				return ContainsAny(acceptableMembers, implementedInterfaceMembers);
			} else {
				return acceptableMembers.Contains(member/*				.MemberDefinition*/);
			}
		}

		static bool ContainsAny<T>(IEnumerable<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items) {
				if (collection.Contains(item))
					return true;
			}
			return false;
		}
	}
}

