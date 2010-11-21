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
	/// Traverses the DOM and resolves expressions.
	/// </summary>
	/// <remarks>
	/// The ResolveVisitor does two jobs at the same time: it tracks the resolve context (properties on CSharpResolver)
	/// and it resolves the expressions visited.
	/// To allow using the context tracking without having to resolve every expression in the file (e.g.
	/// </remarks>
	public class ResolveVisitor : AbstractDomVisitor<object, ResolveResult>
	{
		static readonly ResolveResult errorResult = new ErrorResolveResult(SharedTypes.UnknownType);
		readonly CSharpResolver resolver;
		readonly ParsedFile parsedFile;
		readonly Dictionary<INode, ResolveResult> cache = new Dictionary<INode, ResolveResult>();
		
		readonly IResolveVisitorNavigator navigator;
		ResolveVisitorNavigationMode mode = ResolveVisitorNavigationMode.Scan;
		
		bool resolverEnabled {
			get { return mode != ResolveVisitorNavigationMode.Scan; }
		}
		
		public ResolveVisitor(CSharpResolver resolver, ParsedFile parsedFile, IResolveVisitorNavigator navigator = null)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			this.resolver = resolver;
			this.parsedFile = parsedFile;
			this.navigator = navigator;
			if (navigator == null)
				mode = ResolveVisitorNavigationMode.ResolveAll;
		}
		
		#region Scan / Resolve
		public void Scan(INode node, object data = null)
		{
			if (node == null)
				return;
			if (mode == ResolveVisitorNavigationMode.ResolveAll) {
				Resolve(node, data);
			} else {
				ResolveVisitorNavigationMode oldMode = mode;
				mode = navigator.Scan(node);
				switch (mode) {
					case ResolveVisitorNavigationMode.Skip:
						break;
					case ResolveVisitorNavigationMode.Scan:
						node.AcceptVisitor(this, data);
						break;
					case ResolveVisitorNavigationMode.Resolve:
					case ResolveVisitorNavigationMode.ResolveAll:
						Resolve(node, data);
						break;
					default:
						throw new Exception("Invalid value for ResolveVisitorNavigationMode");
				}
				mode = oldMode;
			}
		}
		
		public ResolveResult Resolve(INode node, object data = null)
		{
			if (node == null)
				return errorResult;
			bool wasScan = mode == ResolveVisitorNavigationMode.Scan;
			if (wasScan)
				mode = ResolveVisitorNavigationMode.Resolve;
			ResolveResult result;
			if (!cache.TryGetValue(node, out result)) {
				result = cache[node] = node.AcceptVisitor(this, data) ?? errorResult;
			}
			if (wasScan)
				mode = ResolveVisitorNavigationMode.Scan;
			return result;
		}
		
		void ScanChildren(INode node)
		{
			for (INode child = node.FirstChild; child != null; child = child.NextSibling) {
				Scan(child);
			}
		}
		
		protected override ResolveResult VisitChildren(INode node, object data)
		{
			ScanChildren(node);
			return null;
		}
		#endregion
		
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
				ScanChildren(unit);
				return null;
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
				ScanChildren(namespaceDeclaration);
				return new NamespaceResolveResult(resolver.UsingScope.NamespaceName);
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
				ITypeDefinition newTypeDefinition = null;
				if (resolver.CurrentTypeDefinition != null) {
					foreach (ITypeDefinition innerClass in resolver.CurrentTypeDefinition.InnerClasses) {
						if (innerClass.Region.IsInside(typeDeclaration.StartLocation.Line, typeDeclaration.StartLocation.Column)) {
							newTypeDefinition = innerClass;
							break;
						}
					}
				} else if (parsedFile != null) {
					newTypeDefinition = parsedFile.GetTopLevelTypeDefinition(typeDeclaration.StartLocation);
				}
				if (newTypeDefinition != null)
					resolver.CurrentTypeDefinition = newTypeDefinition;
				ScanChildren(typeDeclaration);
				return newTypeDefinition != null ? new TypeResolveResult(newTypeDefinition) : errorResult;
			} finally {
				resolver.CurrentTypeDefinition = previousTypeDefinition;
			}
		}
		#endregion
		
		#region Track CurrentMember
		public override ResolveResult VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			int initializerCount = fieldDeclaration.Variables.Count();
			ResolveResult result = null;
			foreach (INode node in fieldDeclaration.Children) {
				if (node.Role == FieldDeclaration.Roles.Initializer) {
					if (resolver.CurrentTypeDefinition != null) {
						resolver.CurrentMember = resolver.CurrentTypeDefinition.Fields.FirstOrDefault(f => f.Region.IsInside(node.StartLocation));
					}
					
					if (resolverEnabled && initializerCount == 1) {
						result = Resolve(node);
					} else {
						Scan(node);
					}
					
					resolver.CurrentMember = null;
				} else {
					Scan(node);
				}
			}
			return result;
		}
		
		public override ResolveResult VisitVariableInitializer(VariableInitializer variableInitializer, object data)
		{
			ScanChildren(variableInitializer);
			if (resolverEnabled) {
				if (variableInitializer.Parent is FieldDeclaration) {
					if (resolver.CurrentMember != null)
						return new MemberResolveResult(resolver.CurrentMember, resolver.CurrentMember.ReturnType.Resolve(resolver.Context));
				} else {
					string identifier = variableInitializer.Name;
					foreach (IVariable v in resolver.LocalVariables) {
						if (v.Name == identifier) {
							object constantValue = v.IsConst ? v.ConstantValue.GetValue(resolver.Context) : null;
							return new VariableResolveResult(v, v.Type.Resolve(resolver.Context), constantValue);
						}
					}
				}
				return errorResult;
			} else {
				return null;
			}
		}
		
		ResolveResult VisitMethodMember(AbstractMemberBase member, object data)
		{
			try {
				if (resolver.CurrentTypeDefinition != null) {
					resolver.CurrentMember = resolver.CurrentTypeDefinition.Methods.FirstOrDefault(m => m.Region.IsInside(member.StartLocation));
				}
				
				ScanChildren(member);
				
				if (resolverEnabled && resolver.CurrentMember != null)
					return new MemberResolveResult(resolver.CurrentMember, resolver.CurrentMember.ReturnType.Resolve(resolver.Context));
				else
					return errorResult;
			} finally {
				resolver.CurrentMember = null;
			}
		}
		
		public override ResolveResult VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			return VisitMethodMember(methodDeclaration, data);
		}
		
		public override ResolveResult VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
		{
			return VisitMethodMember(operatorDeclaration, data);
		}
		
		public override ResolveResult VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			return VisitMethodMember(constructorDeclaration, data);
		}
		
		public override ResolveResult VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
		{
			return VisitMethodMember(destructorDeclaration, data);
		}
		
		public override ResolveResult VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			try {
				if (resolver.CurrentTypeDefinition != null) {
					resolver.CurrentMember = resolver.CurrentTypeDefinition.Properties.FirstOrDefault(p => p.Region.IsInside(propertyDeclaration.StartLocation));
				}
				
				foreach (INode node in propertyDeclaration.Children) {
					if (node.Role == PropertyDeclaration.PropertySetRole && resolver.CurrentMember != null) {
						resolver.PushBlock();
						try {
							resolver.AddVariable(resolver.CurrentMember.ReturnType, "value");
							Scan(node);
						} finally {
							resolver.PopBlock();
						}
					} else {
						Scan(node);
					}
				}
				if (resolverEnabled && resolver.CurrentMember != null)
					return new MemberResolveResult(resolver.CurrentMember, resolver.CurrentMember.ReturnType.Resolve(resolver.Context));
				else
					return errorResult;
			} finally {
				resolver.CurrentMember = null;
			}
		}
		
		public override ResolveResult VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
		{
			return VisitPropertyDeclaration(indexerDeclaration, data);
		}
		#endregion
		
		#region Track CheckForOverflow
		public override ResolveResult VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = true;
				if (resolverEnabled) {
					return Resolve(checkedExpression.Expression);
				} else {
					ScanChildren(checkedExpression);
					return null;
				}
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = false;
				if (resolverEnabled) {
					return Resolve(uncheckedExpression.Expression);
				} else {
					ScanChildren(uncheckedExpression);
					return null;
				}
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitCheckedStatement(CheckedStatement checkedStatement, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = true;
				ScanChildren(checkedStatement);
				return null;
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
		}
		
		public override ResolveResult VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
		{
			bool oldCheckForOverflow = resolver.CheckForOverflow;
			try {
				resolver.CheckForOverflow = false;
				ScanChildren(uncheckedStatement);
				return null;
			} finally {
				resolver.CheckForOverflow = oldCheckForOverflow;
			}
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
			ScanChildren(argListExpression);
			return new ResolveResult(resolver.Context.GetClass(typeof(RuntimeArgumentHandle)) ?? SharedTypes.UnknownType);
		}
		
		public override ResolveResult VisitArrayObjectCreateExpression(ArrayObjectCreateExpression arrayObjectCreateExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitAsExpression(AsExpression asExpression, object data)
		{
			if (resolverEnabled) {
				Scan(asExpression.Expression);
				return new ResolveResult(ResolveType(asExpression.TypeReference));
			} else {
				ScanChildren(asExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult left = Resolve(assignmentExpression.Left);
				Scan(assignmentExpression.Right);
				return new ResolveResult(left.Type);
			} else {
				ScanChildren(assignmentExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			if (resolverEnabled) {
				return resolver.ResolveBaseReference();
			} else {
				ScanChildren(baseReferenceExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult left = Resolve(binaryOperatorExpression.Left);
				ResolveResult right = Resolve(binaryOperatorExpression.Right);
				return resolver.ResolveBinaryOperator(binaryOperatorExpression.BinaryOperatorType, left, right);
			} else {
				ScanChildren(binaryOperatorExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitCastExpression(CastExpression castExpression, object data)
		{
			if (resolverEnabled) {
				return resolver.ResolveCast(ResolveType(castExpression.CastTo), Resolve(castExpression.Expression));
			} else {
				ScanChildren(castExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			if (resolverEnabled) {
				Scan(conditionalExpression.Condition);
				return resolver.ResolveConditional(Resolve(conditionalExpression.TrueExpression),
				                                   Resolve(conditionalExpression.FalseExpression));
			} else {
				ScanChildren(conditionalExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			if (resolverEnabled) {
				return new ConstantResolveResult(ResolveType(defaultValueExpression.TypeReference), null);
			} else {
				ScanChildren(defaultValueExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult rr = Resolve(directionExpression.Expression);
				return new ByReferenceResolveResult(rr.Type, directionExpression.FieldDirection == FieldDirection.Out);
			} else {
				ScanChildren(directionExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (resolverEnabled) {
				// TODO: type arguments?
				return resolver.ResolveSimpleName(identifierExpression.Identifier, EmptyList<IType>.Instance,
				                                  IsTargetOfInvocation(identifierExpression));
			} else {
				ScanChildren(identifierExpression);
				return null;
			}
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
			if (resolverEnabled) {
				ResolveResult target = Resolve(indexerExpression.Target);
				string[] argumentNames;
				ResolveResult[] arguments = GetArguments(indexerExpression.Arguments, out argumentNames);
				return resolver.ResolveIndexer(target, arguments, argumentNames);
			} else {
				ScanChildren(indexerExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult target = Resolve(invocationExpression.Target);
				string[] argumentNames;
				ResolveResult[] arguments = GetArguments(invocationExpression.Arguments, out argumentNames);
				return resolver.ResolveInvocation(target, arguments, argumentNames);
			} else {
				ScanChildren(invocationExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitIsExpression(IsExpression isExpression, object data)
		{
			ScanChildren(isExpression);
			return new ResolveResult(TypeCode.Boolean.ToTypeReference().Resolve(resolver.Context));
		}
		
		public override ResolveResult VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public override ResolveResult VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult target = Resolve(memberReferenceExpression.Target);
				List<INode> typeArgumentNodes = memberReferenceExpression.TypeArguments.ToList();
				// TODO: type arguments?
				return resolver.ResolveMemberAccess(target, memberReferenceExpression.MemberName,
				                                    EmptyList<IType>.Instance,
				                                    IsTargetOfInvocation(memberReferenceExpression));
			} else {
				ScanChildren(memberReferenceExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, object data)
		{
			return resolver.ResolvePrimitive(null);
		}
		
		public override ResolveResult VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			if (resolverEnabled) {
				IType type = ResolveType(objectCreateExpression.Type);
				string[] argumentNames;
				ResolveResult[] arguments = GetArguments(objectCreateExpression.Arguments, out argumentNames);
				return resolver.ResolveObjectCreation(type, arguments, argumentNames);
			} else {
				ScanChildren(objectCreateExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			if (resolverEnabled) {
				return Resolve(parenthesizedExpression.Expression);
			} else {
				Scan(parenthesizedExpression.Expression);
				return null;
			}
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
			if (resolverEnabled) {
				return resolver.ResolveSizeOf(ResolveType(sizeOfExpression.Type));
			} else {
				ScanChildren(sizeOfExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			if (resolverEnabled) {
				Scan(stackAllocExpression.CountExpression);
				return new ResolveResult(new PointerType(ResolveType(stackAllocExpression.Type)));
			} else {
				ScanChildren(stackAllocExpression);
				return null;
			}
		}
		
		public override ResolveResult VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return resolver.ResolveThisReference();
		}
		
		static readonly GetClassTypeReference systemType = new GetClassTypeReference("System.Type", 0);
		
		public override ResolveResult VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			ScanChildren(typeOfExpression);
			if (resolverEnabled)
				return new ResolveResult(systemType.Resolve(resolver.Context));
			else
				return null;
		}
		
		public override ResolveResult VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			if (resolverEnabled) {
				ResolveResult expr = Resolve(unaryOperatorExpression.Expression);
				return resolver.ResolveUnaryOperator(unaryOperatorExpression.UnaryOperatorType, expr);
			} else {
				ScanChildren(unaryOperatorExpression);
				return null;
			}
		}
		#endregion
		
		public override ResolveResult VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			resolver.PushBlock();
			ScanChildren(blockStatement);
			resolver.PopBlock();
			return null;
		}
		
		public override ResolveResult VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, object data)
		{
			IType type = ResolveType(variableDeclarationStatement.ReturnType);
			if (variableDeclarationStatement.Variables.Count() == 1) {
				if (type == SharedTypes.UnknownType && IsVar(variableDeclarationStatement.ReturnType)) {
					ResolveResult rr = Resolve(variableDeclarationStatement.Variables.Single().Initializer);
					type = rr.Type;
				}
				return Resolve(variableDeclarationStatement.Variables.Single(), type);
			} else {
				foreach (VariableInitializer vi in variableDeclarationStatement.Variables)
					Resolve(vi, type);
				return null;
			}
		}
		
		bool IsVar(INode returnType)
		{
			return returnType is IdentifierExpression && ((IdentifierExpression)returnType).Identifier == "var";
		}
		
		public override ResolveResult VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, object data)
		{
			ScanChildren(parameterDeclaration);
			if (resolverEnabled) {
				IParameterizedMember pm = resolver.CurrentMember as IParameterizedMember;
				if (pm != null) {
					foreach (IParameter p in pm.Parameters) {
						if (p.Name == parameterDeclaration.Name) {
							return new VariableResolveResult(p, p.Type.Resolve(resolver.Context));
						}
					}
				}
				return errorResult;
			} else {
				return null;
			}
		}
	}
}
