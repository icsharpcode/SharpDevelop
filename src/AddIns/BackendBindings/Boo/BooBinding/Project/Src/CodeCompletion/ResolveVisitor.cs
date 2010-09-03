// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Boo.Lang.Compiler.Ast;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class ResolveVisitor : DepthFirstVisitor
	{
		#region Field + Constructor
		BooResolver resolver;
		ResolveResult resolveResult;
		IClass callingClass;
		IProjectContent projectContent;
		ICompilationUnit cu;
		
		public ResolveVisitor(BooResolver resolver)
		{
			this.resolver = resolver;
			this.callingClass = resolver.CallingClass;
			this.projectContent = resolver.ProjectContent;
			this.cu = resolver.CompilationUnit;
		}
		
		public ResolveResult ResolveResult {
			get {
				return resolveResult;
			}
		}
		#endregion
		
		#region Make Result
		void ClearResult()
		{
			resolveResult = null;
		}
		
		void MakeResult(IReturnType type)
		{
			if (type == null)
				ClearResult();
			else
				resolveResult = new ResolveResult(callingClass, resolver.CallingMember, type);
		}
		
		void MakeLiteralResult(string fullTypeName)
		{
			MakeResult(new GetClassReturnType(projectContent, fullTypeName, 0));
		}
		
		void MakeResult(IMember member)
		{
			IField field = member as IField;
			if (field != null && (field.IsLocalVariable || field.IsParameter)) {
				resolveResult = new LocalResolveResult(resolver.CallingMember, field);
			} else if (member != null) {
				resolveResult = new MemberResolveResult(callingClass, resolver.CallingMember, member);
			} else {
				ClearResult();
			}
		}
		
		void MakeTypeResult(IClass c)
		{
			if (c != null)
				resolveResult = new TypeResolveResult(callingClass, resolver.CallingMember, c);
			else
				ClearResult();
		}
		
		void MakeTypeResult(IReturnType rt)
		{
			if (rt != null)
				resolveResult = new TypeResolveResult(callingClass, resolver.CallingMember, rt);
			else
				ClearResult();
		}
		
		void MakeMethodResult(IReturnType type, string methodName)
		{
			resolveResult = new MethodGroupResolveResult(callingClass, resolver.CallingMember, type, methodName);
			IMethod m = (resolveResult as MethodGroupResolveResult).GetMethodIfSingleOverload();
			if (m != null) {
				AnonymousMethodReturnType amrt = new AnonymousMethodReturnType(cu);
				amrt.MethodReturnType = m.ReturnType;
				amrt.MethodParameters = m.Parameters;
				resolveResult.ResolvedType = amrt;
			}
		}
		
		void MakeNamespaceResult(string namespaceName)
		{
			resolveResult = new NamespaceResolveResult(callingClass, resolver.CallingMember, namespaceName);
		}
		
		static bool IsSameName(string name1, string name2)
		{
			// boo is currently always case sensitive
			return name1 == name2;
		}
		#endregion
		
		#region Resolve Identifier
		public override void OnReferenceExpression(ReferenceExpression node)
		{
			string identifier = node.Name;
			bool wasResolved = ResolveIdentifier(identifier);
			if (wasResolved && resolveResult is TypeResolveResult) {
				return;
			}
			// was not resolved or was resolved as local, member etc.
			ResolveResult oldResult = resolveResult;
			ClearResult();
			// Try to resolve as type:
			IReturnType t = projectContent.SearchType(new SearchTypeRequest(identifier, 0, callingClass, cu, resolver.CaretLine, resolver.CaretColumn)).Result;
			if (t != null) {
				MakeTypeResult(t);
			} else {
				if (callingClass != null) {
					if (resolver.CallingMember is IMethod) {
						foreach (ITypeParameter typeParameter in (resolver.CallingMember as IMethod).TypeParameters) {
							if (IsSameName(identifier, typeParameter.Name)) {
								MakeTypeResult(new GenericReturnType(typeParameter));
								return;
							}
						}
					}
					foreach (ITypeParameter typeParameter in callingClass.TypeParameters) {
						if (IsSameName(identifier, typeParameter.Name)) {
							MakeTypeResult(new GenericReturnType(typeParameter));
							return;
						}
					}
				}
			}
			if (!wasResolved)
				return; // return type result, if existant
			if (resolveResult == null) {
				resolveResult = oldResult;
			} else {
				// TODO: return type or mixed dependant on context!
				resolveResult = new MixedResolveResult(oldResult, resolveResult);
			}
		}
		
		bool ResolveIdentifier(string identifier)
		{
			IField local;
			ClearResult();
			if (resolver.CallingMember != null) {
				local = resolver.FindLocalVariable(identifier, false);
				if (local != null) {
					MakeResult(local);
					return true;
				}
				
				IMethodOrProperty method = resolver.CallingMember;
				if (method != null) {
					foreach (IParameter p in method.Parameters) {
						if (IsSameName(p.Name, identifier)) {
							MakeResult(new DefaultField.ParameterField(p.ReturnType, p.Name, p.Region, callingClass));
							return true;
						}
					}
					if (method is IProperty && IsSameName(identifier, "value")) {
						if (((IProperty)method).SetterRegion.IsInside(resolver.CaretLine, resolver.CaretColumn)) {
							MakeResult(new DefaultField.ParameterField(method.ReturnType, "value", method.Region, callingClass));
							return true;
						}
					}
				}
			}
			
			{ // Find members of this class or enclosing classes
				IClass tmp = callingClass;
				while (tmp != null) {
					if (ResolveMember(tmp.DefaultReturnType, identifier))
						return true;
					tmp = tmp.DeclaringType;
				}
			}
			
			SearchTypeResult searchTypeResult = projectContent.SearchType(new SearchTypeRequest(identifier, 0, callingClass, cu, resolver.CaretLine, resolver.CaretColumn));
			if (!string.IsNullOrEmpty(searchTypeResult.NamespaceResult)) {
				MakeNamespaceResult(searchTypeResult.NamespaceResult);
				return true;
			}
			
			// Boo can import classes+modules:
			foreach (object o in resolver.GetImportedNamespaceContents()) {
				IClass c = o as IClass;
				if (c != null && IsSameName(identifier, c.Name)) {
					MakeTypeResult(c);
					return true;
				}
				IMember member = o as IMember;
				if (member != null && IsSameName(identifier, member.Name)) {
					if (member is IMethod) {
						MakeMethodResult(member.DeclaringType.DefaultReturnType, member.Name);
					} else {
						MakeResult(member);
					}
					return true;
				}
			}
			
			local = resolver.FindLocalVariable(identifier, true);
			if (local != null) {
				MakeResult(local);
				return true;
			}
			
			return false;
		}
		#endregion
		
		#region OnGenericReferenceExpression
		public override void OnGenericReferenceExpression(GenericReferenceExpression node)
		{
			MakeTypeResult(ConstructTypeFromGenericReferenceExpression(node));
		}
		
		public ConstructedReturnType ConstructTypeFromGenericReferenceExpression(GenericReferenceExpression node)
		{
			Stack<Expression> stack = new Stack<Expression>();
			Expression expr = node;
			while (expr != null) {
				stack.Push(expr);
				if (expr is MemberReferenceExpression) {
					expr = ((MemberReferenceExpression)expr).Target;
				} else if (expr is GenericReferenceExpression) {
					expr = ((GenericReferenceExpression)expr).Target;
				} else {
					expr = null;
				}
			}
			StringBuilder name = new StringBuilder();
			List<IReturnType> typeArguments = new List<IReturnType>();
			while (stack.Count > 0) {
				expr = stack.Pop();
				if (expr is MemberReferenceExpression) {
					name.Append('.');
					name.Append(((MemberReferenceExpression)expr).Name);
				} else if (expr is GenericReferenceExpression) {
					foreach (TypeReference tr in ((GenericReferenceExpression)expr).GenericArguments) {
						typeArguments.Add(ConvertVisitor.CreateReturnType(tr, callingClass,
						                                                  resolver.CallingMember,
						                                                  resolver.CaretLine,
						                                                  resolver.CaretColumn,
						                                                  projectContent));
					}
				} else if (expr is ReferenceExpression) {
					name.Append(((ReferenceExpression)expr).Name);
				} else {
					LoggingService.Warn("Unknown expression in GenericReferenceExpression: " + expr);
				}
			}
			IReturnType rt = projectContent.SearchType(new SearchTypeRequest(name.ToString(), typeArguments.Count, callingClass,
			                                                                 cu, resolver.CaretLine, resolver.CaretColumn)).Result;
			if (rt != null) {
				return new ConstructedReturnType(rt, typeArguments);
			} else {
				return null;
			}
		}
		#endregion
		
		#region Resolve Member
		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			ClearResult();
			node.Target.Accept(this);
			if (resolveResult is NamespaceResolveResult) {
				string namespaceName = (resolveResult as NamespaceResolveResult).Name;
				string combinedName;
				if (namespaceName.Length == 0)
					combinedName = node.Name;
				else
					combinedName = namespaceName + "." + node.Name;
				if (projectContent.NamespaceExists(combinedName)) {
					MakeNamespaceResult(combinedName);
					return;
				}
				IClass c = projectContent.GetClass(combinedName, 0);
				if (c != null) {
					MakeTypeResult(c);
					return;
				}
				
				ClearResult();
				// go through the members of the modules in that namespace
				foreach (object o in projectContent.GetNamespaceContents(namespaceName)) {
					IMember member = o as IMember;
					if (member != null && IsSameName(member.Name, node.Name)) {
						if (member is IMethod) {
							MakeMethodResult(member.DeclaringType.DefaultReturnType, member.Name);
						} else {
							MakeResult(member);
						}
						break;
					}
				}
			} else {
				if (resolveResult != null) {
					if (resolveResult is TypeResolveResult) {
						IClass rClass = (resolveResult as TypeResolveResult).ResolvedClass;
						if (rClass != null) {
							foreach (IClass baseClass in rClass.ClassInheritanceTree) {
								foreach (IClass innerClass in baseClass.InnerClasses) {
									if (IsSameName(innerClass.Name, node.Name)) {
										MakeTypeResult(innerClass);
										return;
									}
								}
							}
						}
					}
					ResolveMember(resolveResult.ResolvedType, node.Name);
				}
			}
		}
		
		bool ResolveMember(IReturnType type, string memberName)
		{
			ClearResult();
			if (type == null)
				return false;
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
			foreach (IProperty p in type.GetProperties()) {
				if (IsSameName(p.Name, memberName)) {
					MakeResult(p);
					return true;
				}
			}
			foreach (IField f in type.GetFields()) {
				if (IsSameName(f.Name, memberName)) {
					MakeResult(f);
					return true;
				}
			}
			foreach (IEvent e in type.GetEvents()) {
				if (IsSameName(e.Name, memberName)) {
					MakeResult(e);
					return true;
				}
			}
			foreach (IMethod m in type.GetMethods()) {
				if (IsSameName(m.Name, memberName)) {
					MakeMethodResult(type, memberName);
					return true;
				}
			}
			if (callingClass != null) {
				List<IMethodOrProperty> list = new List<IMethodOrProperty>();
				ResolveResult.AddExtensions(callingClass.ProjectContent.Language, list.Add, callingClass, type);
				foreach (IMethodOrProperty mp in list) {
					if (IsSameName(mp.Name, memberName)) {
						if (mp is IMethod)
							MakeMethodResult(type, memberName);
						else
							MakeResult(mp);
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		
		#region Resolve Method Invocation
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			ClearResult();
			node.Target.Accept(this);
			if (resolveResult is MixedResolveResult) {
				MixedResolveResult mixed = (MixedResolveResult)resolveResult;
				resolveResult = mixed.TypeResult;
				foreach (ResolveResult rr in mixed.Results) {
					if (rr is MethodGroupResolveResult) {
						resolveResult = rr;
						break;
					}
				}
			}
			if (resolveResult == null)
				return;
			
			if (resolveResult is MethodGroupResolveResult) {
				// normal method call
				string methodName = ((MethodGroupResolveResult)resolveResult).Name;
				IReturnType containingType = ((MethodGroupResolveResult)resolveResult).ContainingType;
				
				ResolveMethodInType(containingType, methodName, node.Arguments);
			} else if (resolveResult is TypeResolveResult) {
				TypeResolveResult trr = (TypeResolveResult)resolveResult;
				if (trr.ResolvedClass != null) {
					if (trr.ResolvedClass.FullyQualifiedName == "array") {
						ResolveArrayCreation(node.Arguments);
						return;
					}
					
					List<IMethod> methods = new List<IMethod>();
					bool isClassInInheritanceTree = false;
					if (callingClass != null)
						isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(trr.ResolvedClass);
					
					foreach (IMethod m in trr.ResolvedClass.DefaultReturnType.GetMethods()) {
						if (m.IsConstructor && !m.IsStatic
						    && m.IsAccessible(callingClass, isClassInInheritanceTree))
						{
							methods.Add(m);
						}
					}
					ResolveInvocation(methods, node.Arguments);
					if (resolveResult != null)
						resolveResult.ResolvedType = trr.ResolvedType;
				} else {
					ClearResult();
				}
			} else if (resolveResult.ResolvedType != null) {
				// maybe event or callable call or call on System.Type -> constructor by reflection
				IClass c = resolveResult.ResolvedType.GetUnderlyingClass();
				if (c != null) {
					if (c.ClassType == ClassType.Delegate) {
						// find the delegate's invoke method
						IMethod invoke = c.Methods.FirstOrDefault((IMethod innerMethod) => innerMethod.Name == "Invoke");
						if (invoke != null) {
							resolveResult.ResolvedType = invoke.ReturnType;
						}
					} else if (c.FullyQualifiedName == "System.Type") {
						resolveResult.ResolvedType = projectContent.SystemTypes.Object;
					} else {
						ClearResult();
					}
				} else {
					ClearResult();
				}
			} else {
				ClearResult();
			}
		}
		
		void ResolveArrayCreation(ExpressionCollection arguments)
		{
			if (arguments.Count == 2) {
				ClearResult();
				arguments[0].Accept(this);
				TypeResolveResult trr = resolveResult as TypeResolveResult;
				if (trr != null) {
					MakeResult(new ArrayReturnType(projectContent, trr.ResolvedType, 1));
				}
			} else {
				ResolveMethodInType(new GetClassReturnType(projectContent, "Boo.Lang.Builtins", 0),
				                    "array", arguments);
			}
		}
		
		void ResolveMethodInType(IReturnType containingType, string methodName, ExpressionCollection arguments)
		{
			List<IMethod> methods = new List<IMethod>();
			bool isClassInInheritanceTree = false;
			if (callingClass != null)
				isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(containingType.GetUnderlyingClass());
			
			foreach (IMethod m in containingType.GetMethods()) {
				if (IsSameName(m.Name, methodName)
				    && m.IsAccessible(callingClass, isClassInInheritanceTree)
				   ) {
					methods.Add(m);
				}
			}
			if (methods.Count == 0) {
				List<IMethodOrProperty> list = new List<IMethodOrProperty>();
				ResolveResult.AddExtensions(callingClass.ProjectContent.Language, list.Add, callingClass, containingType);
				foreach (IMethodOrProperty mp in list) {
					if (IsSameName(mp.Name, methodName) && mp is IMethod) {
						IMethod m = (IMethod)mp.CreateSpecializedMember();
						m.Parameters.RemoveAt(0);
						methods.Add(m);
					}
				}
			}
			ResolveInvocation(methods, arguments);
		}
		
		void ResolveInvocation(List<IMethod> methods, ExpressionCollection arguments)
		{
			ClearResult();
			if (methods.Count == 0) {
				return;
			}
			// MemberLookupHelper does type argument inference and type substitution for us
			IReturnType[] types = new IReturnType[arguments.Count];
			for (int i = 0; i < types.Length; ++i) {
				arguments[i].Accept(this);
				types[i] = (resolveResult != null) ? resolveResult.ResolvedType : null;
				ClearResult();
			}
			bool resultIsAcceptable;
			MakeResult(MemberLookupHelper.FindOverload(methods, types, out resultIsAcceptable));
		}
		#endregion
		
		#region Resolve Slice Expression
		public override void OnSlicingExpression(SlicingExpression node)
		{
			ClearResult();
			Visit(node.Target);
			if (node.Indices.Count > 0) {
				Slice slice = node.Indices[0];
				if (slice.End != null) {
					// Boo slice, returns a part of the source -> same type as source
					return;
				}
			}
			IReturnType rt = (resolveResult != null) ? resolveResult.ResolvedType : null;
			if (rt == null) {
				ClearResult();
				return;
			}
			List<IProperty> indexers = rt.GetProperties();
			// remove non-indexers:
			for (int i = 0; i < indexers.Count; i++) {
				if (!indexers[i].IsIndexer)
					indexers.RemoveAt(i--);
			}
			IReturnType[] parameters = new IReturnType[node.Indices.Count];
			for (int i = 0; i < parameters.Length; i++) {
				Expression expr = node.Indices[i].Begin as Expression;
				if (expr != null) {
					ClearResult();
					Visit(expr);
					parameters[i] = (resolveResult != null) ? resolveResult.ResolvedType : null;
				}
			}
			MakeResult(MemberLookupHelper.FindOverload(indexers.ToArray(), parameters));
		}
		#endregion
		
		public override void OnGeneratorExpression(GeneratorExpression node)
		{
			ClearResult();
			node.Expression.Accept(this);
			
			if (resolveResult != null) {
				IReturnType enumerable = new GetClassReturnType(projectContent, "System.Collections.Generic.IEnumerable", 1);
				MakeResult(new ConstructedReturnType(enumerable, new IReturnType[] { resolveResult.ResolvedType }));
			} else {
				MakeResult(new GetClassReturnType(projectContent, "System.Collections.IEnumerable", 0));
			}
		}
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			switch (node.Operator) {
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.Inequality:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.Match:
				case BinaryOperatorType.Member:
				case BinaryOperatorType.NotMatch:
				case BinaryOperatorType.NotMember:
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
				case BinaryOperatorType.TypeTest:
					MakeResult(projectContent.SystemTypes.Boolean);
					break;
				default:
					if (node.Left == null) {
						if (node.Right == null) {
							ClearResult();
						} else {
							node.Right.Accept(this);
						}
						return;
					} else if (node.Right == null) {
						node.Left.Accept(this);
						return;
					}
					node.Left.Accept(this);
					IReturnType left = (resolveResult != null) ? resolveResult.ResolvedType : null;
					node.Right.Accept(this);
					IReturnType right = (resolveResult != null) ? resolveResult.ResolvedType : null;
					MakeResult(MemberLookupHelper.GetCommonType(projectContent, left, right));
					break;
			}
		}
		
		protected override void OnError(Node node, Exception error)
		{
			MessageService.ShowException(error, "ResolveVisitor: error processing " + node);
		}
		
		public override void OnBlockExpression(BlockExpression node)
		{
			AnonymousMethodReturnType amrt = new AnonymousMethodReturnType(cu);
			if (node.ReturnType != null) {
				amrt.MethodReturnType = ConvertType(node.ReturnType);
			} else {
				amrt.MethodReturnType = new BooInferredReturnType(node.Body, resolver.CallingClass,
				                                               node.ContainsAnnotation("inline"));
			}
			amrt.MethodParameters = new List<IParameter>();
			ConvertVisitor.AddParameters(node.Parameters, amrt.MethodParameters, resolver.CallingMember, resolver.CallingClass ?? new DefaultClass(resolver.CompilationUnit, "__Dummy"));
			MakeResult(amrt);
		}
		
		public override void OnCallableTypeReference(CallableTypeReference node)
		{
			MakeTypeResult(ConvertType(node));
		}
		
		public override void OnRELiteralExpression(RELiteralExpression node)
		{
			MakeLiteralResult("System.Text.RegularExpressions.Regex");
		}
		
		public override void OnCharLiteralExpression(CharLiteralExpression node)
		{
			MakeLiteralResult("System.Char");
		}
		
		public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			if (node.Type != null) {
				MakeResult(ConvertType(node.Type));
				return;
			}
			IReturnType elementType = null;
			foreach (Expression expr in node.Items) {
				ClearResult();
				expr.Accept(this);
				IReturnType thisType = (resolveResult != null) ? resolveResult.ResolvedType : null;
				if (elementType == null)
					elementType = thisType;
				else if (thisType != null)
					elementType = MemberLookupHelper.GetCommonType(projectContent, elementType, thisType);
			}
			if (elementType == null)
				elementType = ConvertVisitor.GetDefaultReturnType(projectContent);
			MakeResult(new ArrayReturnType(projectContent, elementType, 1));
		}
		
		public override void OnTryCastExpression(TryCastExpression node)
		{
			MakeResult(ConvertType(node.Type));
		}
		
		public override void OnCastExpression(CastExpression node)
		{
			MakeResult(ConvertType(node.Type));
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			MakeResult(projectContent.SystemTypes.Boolean);
		}
		
		public override void OnDoubleLiteralExpression(DoubleLiteralExpression node)
		{
			if (node.IsSingle)
				MakeLiteralResult("System.Single");
			else
				MakeLiteralResult("System.Double");
		}
		
		public override void OnListLiteralExpression(ListLiteralExpression node)
		{
			MakeLiteralResult("Boo.Lang.List");
		}
		
		public override void OnHashLiteralExpression(HashLiteralExpression node)
		{
			MakeLiteralResult("Boo.Lang.Hash");
		}
		
		public override void OnIntegerLiteralExpression(IntegerLiteralExpression node)
		{
			if (node.IsLong)
				MakeLiteralResult("System.Int64");
			else
				MakeResult(projectContent.SystemTypes.Int32);
		}
		
		public override void OnNullLiteralExpression(NullLiteralExpression node)
		{
			MakeResult(NullReturnType.Instance);
		}
		
		public override void OnSelfLiteralExpression(SelfLiteralExpression node)
		{
			if (callingClass == null)
				ClearResult();
			else
				MakeResult(callingClass.DefaultReturnType);
		}
		
		public override void OnSuperLiteralExpression(SuperLiteralExpression node)
		{
			if (callingClass == null)
				ClearResult();
			else
				MakeResult(callingClass.BaseType);
		}
		
		public override void OnSimpleTypeReference(SimpleTypeReference node)
		{
			MakeTypeResult(ConvertType(node));
		}
		
		public override void OnStringLiteralExpression(StringLiteralExpression node)
		{
			MakeResult(projectContent.SystemTypes.String);
		}
		
		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			MakeLiteralResult("System.TimeSpan");
		}
		
		public override void OnTypeofExpression(TypeofExpression node)
		{
			MakeResult(projectContent.SystemTypes.Type);
		}
		
		IReturnType ConvertType(TypeReference typeRef)
		{
			return resolver.ConvertType(typeRef);
		}
	}
}
