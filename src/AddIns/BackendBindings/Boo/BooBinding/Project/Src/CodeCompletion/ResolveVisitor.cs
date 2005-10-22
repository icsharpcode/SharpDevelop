// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using Boo.Lang.Compiler.Ast;

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
			resolveResult = new ResolveResult(callingClass, resolver.CallingMember,
			                                  new GetClassReturnType(projectContent, fullTypeName, 0));
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
			resolveResult = new TypeResolveResult(callingClass, resolver.CallingMember, c);
		}
		
		void MakeTypeResult(IReturnType rt)
		{
			resolveResult = new TypeResolveResult(callingClass, resolver.CallingMember, rt);
		}
		
		void MakeMethodResult(IReturnType type, string methodName)
		{
			resolveResult = new MethodResolveResult(callingClass, resolver.CallingMember, type, methodName);
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
			IReturnType t = projectContent.SearchType(identifier, 0, callingClass, cu, resolver.CaretLine, resolver.CaretColumn);
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
				
				IMethodOrProperty method = resolver.CallingMember as IMethodOrProperty;
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
			if (callingClass != null) {
				if (ResolveMember(callingClass.DefaultReturnType, identifier))
					return true;
			}
			
			string namespaceName = projectContent.SearchNamespace(identifier, callingClass, cu, resolver.CaretLine, resolver.CaretColumn);
			if (namespaceName != null && namespaceName.Length > 0) {
				MakeNamespaceResult(namespaceName);
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
				IClass c = projectContent.GetClass(combinedName);
				if (c != null) {
					MakeTypeResult(c);
					return;
				}
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
					ResolveMember(resolveResult.ResolvedType, node.Name);
				}
			}
		}
		
		bool ResolveMember(IReturnType type, string memberName)
		{
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
			return false;
		}
		#endregion
		
		#region Resolve Method Invocation
		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			ClearResult();
			node.Target.Accept(this);
			if (resolveResult == null)
				return;
			if (resolveResult is MethodResolveResult) {
				// normal method call
				string methodName = ((MethodResolveResult)resolveResult).Name;
				IReturnType containingType = ((MethodResolveResult)resolveResult).ContainingType;
				
				ResolveMethodInType(containingType, methodName, node.Arguments);
			} else if (resolveResult is TypeResolveResult || resolveResult is MixedResolveResult) {
				TypeResolveResult trr = resolveResult as TypeResolveResult;
				if (trr == null)
					trr = (resolveResult as MixedResolveResult).TypeResult;
				if (trr != null && trr.ResolvedClass != null) {
					if (trr.ResolvedClass.FullyQualifiedName == "array") {
						ResolveArrayCreation(node.Arguments);
						return;
					}
					
					List<IMethod> methods = new List<IMethod>();
					bool isClassInInheritanceTree = false;
					if (callingClass != null)
						isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(trr.ResolvedClass);
					
					foreach (IMethod m in trr.ResolvedClass.Methods) {
						if (m.IsConstructor && !m.IsStatic
						    && m.IsAccessible(callingClass, isClassInInheritanceTree))
						{
							methods.Add(m);
						}
					}
					if (methods.Count == 0) {
						methods.Add(ICSharpCode.SharpDevelop.Dom.Constructor.CreateDefault(trr.ResolvedClass));
					}
					ResolveInvocation(methods, node.Arguments);
				} else {
					ClearResult();
				}
			} else if (resolveResult.ResolvedType != null) {
				// maybe event or callable call or call on System.Type -> constructor by reflection
				IClass c = resolveResult.ResolvedType.GetUnderlyingClass();
				if (c != null) {
					if (c.ClassType == ClassType.Delegate) {
						// find the delegate's invoke method
						IMethod invoke = c.Methods.Find(delegate(IMethod innerMethod) { return innerMethod.Name == "Invoke"; });
						if (invoke != null) {
							resolveResult.ResolvedType = invoke.ReturnType;
						}
					} else if (c.FullyQualifiedName == "System.Type") {
						resolveResult.ResolvedType = ReflectionReturnType.Object;
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
					MakeResult(new ArrayReturnType(trr.ResolvedType, 1));
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
			MakeResult(MemberLookupHelper.FindOverload(methods, new IReturnType[0], types));
		}
		#endregion
		
		#region Resolve Slice Expression
		public override void OnSlicingExpression(SlicingExpression node)
		{
			ClearResult();
			Visit(node.Target);
			Slice slice = node.Indices[0];
			if (slice.End != null) {
				// Boo slice, returns a part of the source -> same type as source
				return;
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
		
		public override void OnBinaryExpression(BinaryExpression node)
		{
			switch (node.Operator) {
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
				case BinaryOperatorType.Inequality:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.Match:
				case BinaryOperatorType.Member:
				case BinaryOperatorType.NotMatch:
				case BinaryOperatorType.NotMember:
				case BinaryOperatorType.Or:
				case BinaryOperatorType.And:
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
				case BinaryOperatorType.TypeTest:
					MakeResult(ReflectionReturnType.Bool);
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
					MakeResult(MemberLookupHelper.GetCommonType(left, right));
					break;
			}
		}
		
		protected override void OnError(Node node, Exception error)
		{
			MessageService.ShowError(error, "ResolveVisitor: error processing " + node);
		}
		
		public override void OnCallableBlockExpression(CallableBlockExpression node)
		{
			MakeResult(new AnonymousMethodReturnType());
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
			IReturnType elementType = null;
			foreach (Expression expr in node.Items) {
				ClearResult();
				node.Items[0].Accept(this);
				IReturnType thisType = (resolveResult != null) ? resolveResult.ResolvedType : null;
				if (elementType == null)
					elementType = thisType;
				else if (thisType != null)
					elementType = MemberLookupHelper.GetCommonType(elementType, thisType);
			}
			if (elementType == null)
				elementType = ReflectionReturnType.Object;
			MakeResult(new ArrayReturnType(elementType, 1));
		}
		
		public override void OnAsExpression(AsExpression node)
		{
			MakeResult(ConvertType(node.Type));
		}
		
		public override void OnCastExpression(CastExpression node)
		{
			MakeResult(ConvertType(node.Type));
		}
		
		public override void OnBoolLiteralExpression(BoolLiteralExpression node)
		{
			MakeResult(ReflectionReturnType.Bool);
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
				MakeResult(ReflectionReturnType.Int);
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
			MakeResult(ReflectionReturnType.String);
		}
		
		public override void OnTimeSpanLiteralExpression(TimeSpanLiteralExpression node)
		{
			MakeLiteralResult("System.TimeSpan");
		}
		
		public override void OnTypeofExpression(TypeofExpression node)
		{
			MakeLiteralResult("System.Type");
		}
		
		IReturnType ConvertType(TypeReference typeRef)
		{
			return resolver.ConvertType(typeRef);
		}
	}
}
