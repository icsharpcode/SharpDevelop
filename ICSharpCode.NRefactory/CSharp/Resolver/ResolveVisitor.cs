// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Traverses the DOM and resolves every expression.
	/// </summary>
	public class ResolveVisitor : AbstractDomVisitor<object, ResolveResult>
	{
		static readonly ResolveResult errorResult = new ErrorResolveResult(SharedTypes.UnknownType);
		readonly CSharpResolver resolver;
		readonly ParsedFile parsedFile;
		readonly Dictionary<INode, ResolveResult> cache = new Dictionary<INode, ResolveResult>();
		
		/// <summary>
		/// Set this property to false to skip resolving all sub expressions.
		/// </summary>
		public bool FullyResolveSubExpressions { get; set; }
		
		public ResolveVisitor(CSharpResolver resolver, ParsedFile parsedFile)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			this.resolver = resolver;
			this.parsedFile = parsedFile;
			this.FullyResolveSubExpressions = true;
		}
		
		public ResolveResult Resolve(INode node)
		{
			ResolveResult result;
			if (!cache.TryGetValue(node, out result)) {
				result = cache[node] = node.AcceptVisitor(this, null) ?? errorResult;
			}
			return result;
		}
		
		public ResolveResult GetResolveResult(INode node)
		{
			ResolveResult result;
			if (cache.TryGetValue(node, out result))
				return result;
			else
				return null;
		}
		
		#region Track UsingScope
		public override ResolveResult VisitCompilationUnit(CompilationUnit unit, object data)
		{
			UsingScope previousUsingScope = resolver.UsingScope;
			try {
				if (parsedFile != null)
					resolver.UsingScope = parsedFile.RootUsingScope;
				return base.VisitCompilationUnit(unit, data);
			} finally {
				resolver.UsingScope = previousUsingScope;
			}
		}
		
		public override ResolveResult VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			UsingScope previousUsingScope = resolver.UsingScope;
			try {
				if (parsedFile != null) {
					resolver.UsingScope = parsedFile.GetUsingScope(namespaceDeclaration.StartLocation);
				}
				return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
			} finally {
				resolver.UsingScope = previousUsingScope;
			}
		}
		#endregion
		
		#region Track CurrentTypeDefinition
		public override ResolveResult VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			ITypeDefinition previousTypeDefinition = resolver.CurrentTypeDefinition;
			try {
				if (resolver.CurrentTypeDefinition != null) {
					foreach (ITypeDefinition innerClass in resolver.CurrentTypeDefinition.InnerClasses) {
						if (innerClass.Region.IsInside(typeDeclaration.StartLocation.Line, typeDeclaration.StartLocation.Column)) {
							resolver.CurrentTypeDefinition = innerClass;
							break;
						}
					}
				} else if (parsedFile != null) {
					resolver.CurrentTypeDefinition = parsedFile.GetTopLevelTypeDefinition(typeDeclaration.StartLocation);
				}
				return base.VisitTypeDeclaration(typeDeclaration, data);
			} finally {
				resolver.CurrentTypeDefinition = previousTypeDefinition;
			}
		}
		#endregion
		
		#region Track CheckForOverflow
		public override ResolveResult VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = true;
				return checkedExpression.Expression.AcceptVisitor(this, data);
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = false;
				return uncheckedExpression.Expression.AcceptVisitor(this, data);
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = true;
				return base.VisitCheckedStatement(checkedStatement, data);
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = true;
				return base.VisitUncheckedStatement(uncheckedStatement, data);
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		#endregion
		
		#region VisitChildren
		protected override ResolveResult VisitChildren(INode node, object data)
		{
			// this method is used to iterate through all children
			if (FullyResolveSubExpressions) {
				INode child = node.FirstChild;
				while (child != null) {
					ResolveResult rr = child.AcceptVisitor (this, data);
					if (rr != null)
						cache[child] = rr;
					child = child.NextSibling;
				}
			}
			return null;
		}
		#endregion
		
		#region Visit Expressions
		static bool IsTargetOfInvocation(INode node)
		{
			InvocationExpression ie = node.Parent as InvocationExpression;
			return ie != null && ie.Target == node;
		}
		
		IType ResolveType(INode node)
		{
			return SharedTypes.UnknownType;
		}
		
		public override ResolveResult VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitArgListExpression(ArgListExpression argListExpression, object data)
		{
			return new ResolveResult(resolver.Context.GetClass(typeof(RuntimeArgumentHandle)) ?? SharedTypes.UnknownType);
		}
		
		public override ResolveResult VisitArrayObjectCreateExpression(ArrayObjectCreateExpression arrayObjectCreateExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitAsExpression(AsExpression asExpression, object data)
		{
			if (FullyResolveSubExpressions)
				Resolve(asExpression.Expression);
			return new ResolveResult(ResolveType(asExpression.TypeReference));
		}
		
		public override ResolveResult VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			ResolveResult left = Resolve(assignmentExpression.Left);
			if (FullyResolveSubExpressions) {
				Resolve(assignmentExpression.Right);
			}
			return new ResolveResult(left.Type);
		}
		
		public override ResolveResult VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return resolver.ResolveBaseReference();
		}
		
		public override ResolveResult VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			ResolveResult left = Resolve(binaryOperatorExpression.Left);
			ResolveResult right = Resolve(binaryOperatorExpression.Right);
			return resolver.ResolveBinaryOperator(binaryOperatorExpression.BinaryOperatorType, left, right);
		}
		
		public override ResolveResult VisitCastExpression(CastExpression castExpression, object data)
		{
			return resolver.ResolveCast(ResolveType(castExpression.CastTo), Resolve(castExpression.Expression));
		}
		
		public override ResolveResult VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			if (FullyResolveSubExpressions)
				Resolve(conditionalExpression.Condition);
			return resolver.ResolveConditional(Resolve(conditionalExpression.TrueExpression),
			                                   Resolve(conditionalExpression.FalseExpression));
		}
		
		public override ResolveResult VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			return new ConstantResolveResult(ResolveType(defaultValueExpression.TypeReference), null);
		}
		
		public override ResolveResult VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			ResolveResult rr = Resolve(directionExpression.Expression);
			return new ByReferenceResolveResult(rr.Type, directionExpression.FieldDirection == FieldDirection.Out);
		}
		
		public override ResolveResult VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			// TODO: type arguments?
			return resolver.ResolveSimpleName(identifierExpression.Identifier.Name, EmptyList<IType>.Instance,
			                                  IsTargetOfInvocation(identifierExpression));
		}
		
		ResolveResult[] GetArguments(IEnumerable<INode> argumentExpressions, out string[] argumentNames)
		{
			argumentNames = null; // TODO: add support for named arguments
			ResolveResult[] arguments = new ResolveResult[argumentExpressions.Count()];
			int i = 0;
			foreach (INode argument in argumentExpressions) {
				arguments[i++] = Resolve(argument);
			}
			return arguments;
		}
		
		public override ResolveResult VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			ResolveResult target = Resolve(indexerExpression.Target);
			string[] argumentNames;
			ResolveResult[] arguments = GetArguments(indexerExpression.Arguments, out argumentNames);
			return resolver.ResolveIndexer(target, arguments, argumentNames);
		}
		
		public override ResolveResult VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			ResolveResult target = Resolve(invocationExpression.Target);
			string[] argumentNames;
			ResolveResult[] arguments = GetArguments(invocationExpression.Arguments, out argumentNames);
			return resolver.ResolveInvocation(target, arguments, argumentNames);
		}
		
		public override ResolveResult VisitIsExpression(IsExpression isExpression, object data)
		{
			if (FullyResolveSubExpressions)
				ResolveType(isExpression.TypeReference);
			return new ResolveResult(TypeCode.Boolean.ToTypeReference().Resolve(resolver.Context));
		}
		
		public override ResolveResult VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			ResolveResult target = Resolve(memberReferenceExpression.Target);
			List<INode> typeArgumentNodes = memberReferenceExpression.TypeArguments.ToList();
			// TODO: type arguments?
			return resolver.ResolveMemberAccess(target, memberReferenceExpression.MemberName,
			                                    EmptyList<IType>.Instance,
			                                    IsTargetOfInvocation(memberReferenceExpression));
		}
		
		public override ResolveResult VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, object data)
		{
			return resolver.ResolvePrimitive(null);
		}
		
		public override ResolveResult VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			IType type = ResolveType(objectCreateExpression.Type);
			string[] argumentNames;
			ResolveResult[] arguments = GetArguments(objectCreateExpression.Arguments, out argumentNames);
			return resolver.ResolveObjectCreation(type, arguments, argumentNames);
		}
		
		public override ResolveResult VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return Resolve(parenthesizedExpression.Expression);
		}
		
		public override ResolveResult VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			return resolver.ResolvePrimitive(primitiveExpression.Value);
		}
		
		public override ResolveResult VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			return resolver.ResolveSizeOf(ResolveType(sizeOfExpression.Type));
		}
		
		public override ResolveResult VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			if (FullyResolveSubExpressions)
				Resolve(stackAllocExpression.CountExpression);
			return new ResolveResult(new PointerType(ResolveType(stackAllocExpression.Type)));
		}
		
		public override ResolveResult VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return resolver.ResolveThisReference();
		}
		
		static readonly GetClassTypeReference systemType = new GetClassTypeReference("System.Type", 0);
		
		public override ResolveResult VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			if (FullyResolveSubExpressions)
				ResolveType(typeOfExpression.Type);
			return new ResolveResult(systemType.Resolve(resolver.Context));
		}
		
		public override ResolveResult VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			ResolveResult expr = Resolve(unaryOperatorExpression.Expression);
			return resolver.ResolveUnaryOperator(unaryOperatorExpression.UnaryOperatorType, expr);
		}
		#endregion
	}
}
