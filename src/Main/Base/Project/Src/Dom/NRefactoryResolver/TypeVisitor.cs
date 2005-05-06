// created on 22.08.2003 at 19:02

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class TypeVisitor : AbstractASTVisitor
	{
		NRefactoryResolver resolver;
		
		public TypeVisitor(NRefactoryResolver resolver)
		{
			this.resolver = resolver;
		}
		
		public override object Visit(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value != null) {
				return CreateReturnType(primitiveExpression.Value.GetType());
			}
			return null;
		}
		
		public override object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			// TODO : Operators
			return binaryOperatorExpression.Left.AcceptVisitor(this, data);
		}
		
		public override object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			if (parenthesizedExpression == null) {
				return null;
			}
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(InvocationExpression invocationExpression, object data)
		{
			IMethod m = GetMethod(invocationExpression, data);
			if (m == null)
				return null;
			else
				return m.ReturnType;
		}
		
		IMethod FindOverload(ArrayList methods, InvocationExpression invocationExpression, object data)
		{
			if (methods.Count <= 0) {
				return null;
			}
			IMethod bestMethod = (IMethod)methods[0]; // when in doubt, use first method
			if (methods.Count == 1)
				return bestMethod;
			
			ArrayList arguments = invocationExpression.Parameters;
			IReturnType[] types = new IReturnType[arguments.Count];
			for (int i = 0; i < types.Length; ++i) {
				types[i] = ((Expression)arguments[i]).AcceptVisitor(this, data) as IReturnType;
			}
			int bestScore = ScoreOverload(bestMethod, types);
			
			foreach (IMethod method in methods) {
				if (method == bestMethod) continue;
				int score = ScoreOverload(method, types);
				if (score > bestScore) {
					bestScore = score;
					bestMethod = method;
				}
			}
			
			return bestMethod;
		}
		
		int ScoreOverload(IMethod method, IReturnType[] types)
		{
			if (method.Parameters.Count == types.Length) {
				int points = types.Length;
				for (int i = 0; i < types.Length; ++i) {
					IReturnType type = method.Parameters[i].ReturnType;
					if (type != null && types[i] != null) {
						if (type.Equals(types[i]))
							points += 1;
					}
				}
				return points;
			} else {
				return types.Length - Math.Abs(method.Parameters.Count - types.Length);
			}
		}
		
		public IMethod GetMethod(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is FieldReferenceExpression) {
				FieldReferenceExpression field = (FieldReferenceExpression)invocationExpression.TargetObject;
				IReturnType type = field.TargetObject.AcceptVisitor(this, data) as IReturnType;
				ArrayList methods = resolver.SearchMethod(type, field.FieldName);
				return FindOverload(methods, invocationExpression, data);
			} else if (invocationExpression.TargetObject is IdentifierExpression) {
				string id = ((IdentifierExpression)invocationExpression.TargetObject).Identifier;
				if (resolver.CallingClass == null) {
					return null;
				}
				ArrayList methods = resolver.SearchMethod(resolver.CallingClass.DefaultReturnType, id);
				return FindOverload(methods, invocationExpression, data);
			}
			// invocationExpression is delegate call
			IReturnType t = invocationExpression.AcceptChildren(this, data) as IReturnType;
			if (t == null) {
				return null;
			}
			IClass c = resolver.SearchType(t.FullyQualifiedName, resolver.CallingClass, resolver.CompilationUnit);
			if (c.ClassType == ClassType.Delegate) {
				ArrayList methods = resolver.SearchMethod(t, "Invoke");
				return FindOverload(methods, invocationExpression, data);
			}
			return null;
		}
		
		public override object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (fieldReferenceExpression == null) {
				return null;
			}
			// int. generates a FieldreferenceExpression with TargetObject TypeReferenceExpression and no FieldName
			if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName == "") {
				if (fieldReferenceExpression.TargetObject is TypeReferenceExpression) {
					return CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference);
				}
			}
			IReturnType returnType = fieldReferenceExpression.TargetObject.AcceptVisitor(this, data) as IReturnType;
			if (returnType != null) {
				string name = resolver.SearchNamespace(returnType.FullyQualifiedName, resolver.CompilationUnit);
				if (name != null) {
					string n = resolver.SearchNamespace(name + "." + fieldReferenceExpression.FieldName, null);
					if (n != null) {
						return CreateNamespaceReturnType(n);
					}
					IClass c = resolver.SearchType(name + "." + fieldReferenceExpression.FieldName, resolver.CallingClass, resolver.CompilationUnit);
					if (c != null) {
						return c.DefaultReturnType;
					}
					return null;
				}
				return resolver.SearchMember(returnType, fieldReferenceExpression.FieldName);
			}
			return null;
		}
		
		public override object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			return null;
			/*
			ReturnType type = pointerReferenceExpression.TargetObject.AcceptVisitor(this, data) as ReturnType;
			if (type == null) {
				return null;
			}
			type = type.Clone();
			--type.PointerNestingLevel;
			if (type.PointerNestingLevel != 0) {
				return null;
			}
			return resolver.SearchMember(type, pointerReferenceExpression.Identifier);
			 */
		}
		
		public override object Visit(IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression == null) {
				return null;
			}
			string name = resolver.SearchNamespace(identifierExpression.Identifier, resolver.CompilationUnit);
			if (name != null && name != "") {
				return CreateNamespaceReturnType(name);
			}
			IClass c = resolver.SearchType(identifierExpression.Identifier, resolver.CallingClass, resolver.CompilationUnit);
			if (c != null) {
				return c.DefaultReturnType;
			}
			return resolver.DynamicLookup(identifierExpression.Identifier);
		}
		
		public override object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return CreateReturnType(typeReferenceExpression.TypeReference);
		}
		
		public override object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			if (unaryOperatorExpression == null) {
				return null;
			}
			IReturnType expressionType = unaryOperatorExpression.Expression.AcceptVisitor(this, data) as IReturnType;
			// TODO: Little bug: unary operator MAY change the return type,
			//                   but that is only a minor issue
			switch (unaryOperatorExpression.Op) {
				case UnaryOperatorType.Not:
					break;
				case UnaryOperatorType.BitNot:
					break;
				case UnaryOperatorType.Minus:
					break;
				case UnaryOperatorType.Plus:
					break;
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
					break;
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					break;
				case UnaryOperatorType.Star:       // dereference
					//--expressionType.PointerNestingLevel;
					break;
				case UnaryOperatorType.BitWiseAnd: // get reference
					//++expressionType.PointerNestingLevel;
					break;
				case UnaryOperatorType.None:
					break;
			}
			return expressionType;
		}
		
		public override object Visit(AssignmentExpression assignmentExpression, object data)
		{
			return assignmentExpression.Left.AcceptVisitor(this, data);
		}
		
		public override object Visit(SizeOfExpression sizeOfExpression, object data)
		{
			return CreateReturnType(typeof(int));
		}
		
		public override object Visit(TypeOfExpression typeOfExpression, object data)
		{
			return CreateReturnType(typeof(Type));
		}
		
		public override object Visit(CheckedExpression checkedExpression, object data)
		{
			return checkedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(UncheckedExpression uncheckedExpression, object data)
		{
			return uncheckedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(CastExpression castExpression, object data)
		{
			return CreateReturnType(castExpression.CastTo);
		}
		
		public override object Visit(StackAllocExpression stackAllocExpression, object data)
		{
			/*ReturnType returnType = new ReturnType(stackAllocExpression.TypeReference);
			++returnType.PointerNestingLevel;
			return returnType;*/
			return null;
		}
		
		public override object Visit(IndexerExpression indexerExpression, object data)
		{
			return null;
			/*
			IReturnType type = (IReturnType)indexerExpression.TargetObject.AcceptVisitor(this, data);
			if (type == null) {
				return null;
			}
			if (type.ArrayDimensions == null || type.ArrayDimensions.Length == 0) {
				// check if ther is an indexer
				if (indexerExpression.TargetObject is ThisReferenceExpression) {
					if (resolver.CallingClass == null) {
						return null;
					}
					type = new ReturnType(resolver.CallingClass.FullyQualifiedName);
				}
				ArrayList indexer = resolver.SearchIndexer(type);
				if (indexer.Count == 0) {
					return null;
				}
				// TODO: get the right indexer
				return ((IIndexer)indexer[0]).ReturnType;
			}
			
			// TODO: what is a[0] if a is pointer to array or array of pointer ?
			if (type.ArrayDimensions[type.ArrayDimensions.Length - 1] != indexerExpression.Indices.Count) {
				return null;
			}
			int[] newArray = new int[type.ArrayDimensions.Length - 1];
			Array.Copy(type.ArrayDimensions, 0, newArray, 0, type.ArrayDimensions.Length - 1);
			return new ReturnType(type.Name, newArray, type.PointerNestingLevel);
			 */
		}
		
		public override object Visit(ClassReferenceExpression classReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			return resolver.CallingClass.DefaultReturnType;
		}
		
		public override object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			return resolver.CallingClass.DefaultReturnType;
		}
		
		public override object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			IClass baseClass = resolver.CallingClass.BaseClass;
			if (baseClass == null) {
				return null;
			}
			return baseClass.DefaultReturnType;
		}
		
		public override object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			string name = objectCreateExpression.CreateType.Type;
			IClass c = resolver.SearchType(name, resolver.CallingClass, resolver.CompilationUnit);
			if (c != null)
				name = c.FullyQualifiedName;
			return c.DefaultReturnType;
			//return new ReturnType(name, objectCreateExpression.CreateType.RankSpecifier, objectCreateExpression.CreateType.PointerNestingLevel);
		}
		
		public override object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			IReturnType type = CreateReturnType(arrayCreateExpression.CreateType);
			/*
			if (arrayCreateExpression.Parameters != null && arrayCreateExpression.Parameters.Count > 0) {
				int[] newRank = new int[arrayCreateExpression.Rank.Length + 1];
				newRank[0] = arrayCreateExpression.Parameters.Count - 1;
				Array.Copy(type.ArrayDimensions, 0, newRank, 1, type.ArrayDimensions.Length);
				type.ArrayDimensions = newRank;
			}
			 */
			return type;
		}
		
		public override object Visit(DirectionExpression directionExpression, object data)
		{
			// no calls allowed !!!
			return null;
		}
		
		public override object Visit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			// no calls allowed !!!
			return null;
		}
		
		IReturnType CreateReturnType(TypeReference reference)
		{
			return CreateReturnType(reference, resolver);
		}
		
		public static IReturnType CreateReturnType(TypeReference reference, NRefactoryResolver resolver)
		{
			if (reference.IsNull) return null;
			IResolveContext context = new SearchClassResolveContext(resolver.CallingClass, resolver.CaretLine, resolver.CaretColumn);
			IReturnType t = new LazyReturnType(context, reference.SystemType);
			if (reference.GenericTypes.Count > 0) {
				List<IReturnType> para = new List<IReturnType>(reference.GenericTypes.Count);
				for (int i = 0; i < reference.GenericTypes.Count; ++i) {
					para.Add(CreateReturnType(reference.GenericTypes[i], resolver));
				}
				t = new SpecificReturnType(t, para);
			}
			if (reference.IsArrayType) {
				for (int i = 0; i < reference.RankSpecifier.Length; ++i) {
					t = new ArrayReturnType(t, reference.RankSpecifier[i]);
				}
			}
			return t;
		}
		
		IReturnType CreateNamespaceReturnType(string namespaceName)
		{
			return null;
		}
		
		IReturnType CreateReturnType(Type type)
		{
			return ReflectionReturnType.Create(ProjectContentRegistry.GetMscorlibContent(),
			                                   type);
		}
	}
}
