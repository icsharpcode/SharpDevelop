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
				return ReflectionReturnType.CreatePrimitive(primitiveExpression.Value.GetType());
			}
			return null;
		}
		
		public override object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			switch (binaryOperatorExpression.Op) {
				case BinaryOperatorType.AsCast:
					return binaryOperatorExpression.Right.AcceptVisitor(this, data);
				case BinaryOperatorType.DivideInteger:
					return ReflectionReturnType.Int;
				case BinaryOperatorType.Concat:
					return ReflectionReturnType.String;
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.InEquality:
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
				case BinaryOperatorType.TypeCheck:
				case BinaryOperatorType.LogicalAnd:
				case BinaryOperatorType.LogicalOr:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
					return ReflectionReturnType.Bool;
				default:
					return binaryOperatorExpression.Left.AcceptVisitor(this, data);
			}
		}
		
		public override object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(InvocationExpression invocationExpression, object data)
		{
			IMethod m = GetMethod(invocationExpression, data);
			if (m == null) {
				// This might also be a delegate invocation:
				// get the delegate's Invoke method
				IReturnType targetType = invocationExpression.TargetObject.AcceptVisitor(this, data) as IReturnType;
				if (targetType != null) {
					IClass c = targetType.GetUnderlyingClass();
					if (c != null && c.ClassType == ClassType.Delegate) {
						// find the delegate's return type
						m = c.Methods.Find(delegate(IMethod innerMethod) { return innerMethod.Name == "Invoke"; });
					}
				}
			}
			if (m != null)
				return m.ReturnType;
			else
				return null;
		}
		
		public IMethod FindOverload(ArrayList methods, ArrayList arguments, object data)
		{
			if (methods.Count <= 0) {
				return null;
			}
			if (methods.Count == 1)
				return (IMethod)methods[0];
			
			IReturnType[] types = new IReturnType[arguments.Count];
			for (int i = 0; i < types.Length; ++i) {
				types[i] = ((Expression)arguments[i]).AcceptVisitor(this, data) as IReturnType;
			}
			bool tmp;
			return (IMethod)methods[FindOverload(methods, types, true, out tmp)];
		}
		
		/// <summary>
		/// Finds the index of the overload in <paramref name="methods"/> that is the best
		/// match for a call with the specified return types.
		/// </summary>
		/// <param name="methods">ArrayList containing IMethodOrIndexers</param>
		/// <param name="types">Array containing the types of the parameters.</param>
		/// <param name="forceParameterCount">True when the parameter count should exactly be
		/// types.Length; False when more parameters are possible</param>
		/// <param name="singleBestOverload">Returns true when the overload returned is
		/// the only overload that had the highest score or false when there were
		/// multiple overloads with an equal score.</param>
		public static int FindOverload(ArrayList methods, IReturnType[] types, bool forceParameterCount, out bool singleBestOverload)
		{
			singleBestOverload = true;
			if (methods.Count == 1)
				return 0;
			
			IMethodOrIndexer bestMethod = (IMethodOrIndexer)methods[0];
			int bestIndex = 0;
			int bestScore = ScoreOverload(bestMethod, types, forceParameterCount);
			
			for (int i = 1; i < methods.Count; ++i) {
				IMethodOrIndexer method = (IMethodOrIndexer)methods[i];
				int score = ScoreOverload(method, types, forceParameterCount);
				if (score > bestScore) {
					bestScore = score;
					bestMethod = method;
					bestIndex = i;
					singleBestOverload = true;
				} else if (score == bestScore) {
					singleBestOverload = false;
				}
			}
			
			return bestIndex;
		}
		
		/// <summary>
		/// Calculates a score how good the specified <paramref name="method"/> matches
		/// the <paramref name="types"/>.
		/// </summary>
		/// <param name="methods">ArrayList containing IMethodOrIndexers</param>
		/// <param name="types">Array containing the types of the parameters.</param>
		/// <param name="forceParameterCount">True when the parameter count should exactly be
		/// types.Length; False when more parameters are possible</param>
		/// <returns>
		/// Integer score. When the parameter count matches, score is between 0 (no types matches)
		/// and types.Length (all types matched).
		/// When there were too many parameters but forceParameterCount was false, score is
		/// between -1 for no matches and types.Length - 1 for all matches.
		/// When the parameter count didn't match, score is -(Difference between parameter counts)
		/// </returns>
		public static int ScoreOverload(IMethodOrIndexer method, IReturnType[] types, bool forceParameterCount)
		{
			if (method == null) return -1;
			if (forceParameterCount
			    ? (method.Parameters.Count == types.Length)
			    : (method.Parameters.Count >= types.Length))
			{
				int points = 0;
				for (int i = 0; i < types.Length; ++i) {
					IReturnType type = method.Parameters[i].ReturnType;
					if (type != null && types[i] != null) {
						if (type.Equals(types[i]))
							points += 1;
					}
				}
				if (method.Parameters.Count == types.Length)
					return points;
				else
					return points - 1;
			} else {
				return -Math.Abs(method.Parameters.Count - types.Length);
			}
		}
		
		public IMethod GetMethod(InvocationExpression invocationExpression, object data)
		{
			if (invocationExpression.TargetObject is FieldReferenceExpression) {
				FieldReferenceExpression field = (FieldReferenceExpression)invocationExpression.TargetObject;
				IReturnType type = field.TargetObject.AcceptVisitor(this, data) as IReturnType;
				ArrayList methods = resolver.SearchMethod(type, field.FieldName);
				InjectMethodTypeParameters(methods, invocationExpression);
				return FindOverload(methods, invocationExpression.Parameters, data);
			} else if (invocationExpression.TargetObject is IdentifierExpression) {
				string id = ((IdentifierExpression)invocationExpression.TargetObject).Identifier;
				if (resolver.CallingClass == null) {
					return null;
				}
				ArrayList methods = resolver.SearchMethod(resolver.CallingClass.DefaultReturnType, id);
				InjectMethodTypeParameters(methods, invocationExpression);
				return FindOverload(methods, invocationExpression.Parameters, data);
			}
			return null;
		}
		
		void InjectMethodTypeParameters(ArrayList methods, InvocationExpression invocationExpression)
		{
			if (invocationExpression.TypeParameters == null) return;
			if (invocationExpression.TypeParameters.Count == 0) return;
			List<IReturnType> typeParameters = CreateReturnType(invocationExpression.TypeParameters);
			for (int i = 0; i < methods.Count; ++i) {
				IMethod m = methods[i] as IMethod;
				if (m == null) continue; // ignore Events
				if (m.TypeParameters.Count == 0) {
					m = null; // this is not the correct overload, ignore this method
				} else {
					m = (IMethod)m.Clone();
					m.ReturnType = SpecificReturnType.TranslateType(m.ReturnType, typeParameters, true);
					for (int j = 0; j < m.Parameters.Count; ++j) {
						m.Parameters[j].ReturnType = SpecificReturnType.TranslateType(m.Parameters[j].ReturnType, typeParameters, true);
					}
				}
				methods[i] = m;
			}
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
				NamespaceReturnType namespaceRT = returnType as NamespaceReturnType;
				if (namespaceRT != null) {
					string name = namespaceRT.FullyQualifiedName;
					string combinedName = name + "." + fieldReferenceExpression.FieldName;
					if (resolver.ProjectContent.NamespaceExists(combinedName)) {
						return new NamespaceReturnType(combinedName);
					}
					IClass c = resolver.ProjectContent.GetClass(combinedName);
					if (c != null) {
						return c.DefaultReturnType;
					}
					if (resolver.LanguageProperties.ImportModules) {
						// go through the members of the modules
						foreach (object o in resolver.ProjectContent.GetNamespaceContents(name)) {
							IMember member = o as IMember;
							if (member != null && resolver.IsSameName(member.Name, fieldReferenceExpression.FieldName))
								return member.ReturnType;
						}
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
			string name = resolver.SearchNamespace(identifierExpression.Identifier);
			if (name != null && name.Length > 0) {
				return new NamespaceReturnType(name);
			}
			IClass c = resolver.SearchType(identifierExpression.Identifier);
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
			IReturnType type = (IReturnType)indexerExpression.TargetObject.AcceptVisitor(this, data);
			if (type == null) {
				return null;
			}
			List<IIndexer> indexers = type.GetIndexers();
			if (indexers.Count > 0)
				return indexers[0].ReturnType;
			else
				return null;
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
			return CreateReturnType(objectCreateExpression.CreateType);
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
		
		List<IReturnType> CreateReturnType(List<TypeReference> references)
		{
			return references.ConvertAll<IReturnType>(CreateReturnType);
		}
		
		IReturnType CreateReturnType(TypeReference reference)
		{
			return CreateReturnType(reference, resolver);
		}
		
		public static IReturnType CreateReturnType(TypeReference reference, NRefactoryResolver resolver)
		{
			return CreateReturnType(reference,
			                        resolver.CallingClass, resolver.CallingMember,
			                        resolver.CaretLine, resolver.CaretColumn,
			                        resolver.ProjectContent, false);
		}
		
		public static IReturnType CreateReturnType(TypeReference reference, IClass callingClass,
		                                           IMember callingMember, int caretLine, int caretColumn,
		                                           IProjectContent projectContent,
		                                           bool useLazyReturnType)
		{
			if (reference == null) return null;
			if (reference.IsNull) return null;
			LanguageProperties languageProperties = projectContent.Language;
			IReturnType t = null;
			if (callingClass != null) {
				foreach (ITypeParameter tp in callingClass.TypeParameters) {
					if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType)) {
						t = new GenericReturnType(tp);
						break;
					}
				}
			}
			if (callingMember is IMethod && (callingMember as IMethod).TypeParameters != null) {
				foreach (ITypeParameter tp in (callingMember as IMethod).TypeParameters) {
					if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType))
						return new GenericReturnType(tp);
				}
			}
			if (t == null) {
				if (reference.Type != reference.SystemType) {
					// keyword-type like void, int, string etc.
					t = ProjectContentRegistry.Mscorlib.GetClass(reference.SystemType).DefaultReturnType;
				} else {
					if (useLazyReturnType) {
						t = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn, reference.SystemType);
					} else {
						IClass c = projectContent.SearchType(reference.SystemType, callingClass, caretLine, caretColumn);
						if (c == null)
							return null;
						t = c.DefaultReturnType;
					}
				}
			}
			if (reference.GenericTypes.Count > 0) {
				List<IReturnType> para = new List<IReturnType>(reference.GenericTypes.Count);
				for (int i = 0; i < reference.GenericTypes.Count; ++i) {
					para.Add(CreateReturnType(reference.GenericTypes[i], callingClass, callingMember, caretLine, caretColumn, projectContent, useLazyReturnType));
				}
				t = new SpecificReturnType(t, para);
			}
			return WrapArray(t, reference);
		}
		
		static IReturnType WrapArray(IReturnType t, TypeReference reference)
		{
			if (reference.IsArrayType) {
				for (int i = reference.RankSpecifier.Length - 1; i >= 0; --i) {
					t = new ArrayReturnType(t, reference.RankSpecifier[i] + 1);
				}
			}
			return t;
		}
		
		public class NamespaceReturnType : AbstractReturnType
		{
			public NamespaceReturnType(string fullName)
			{
				this.FullyQualifiedName = fullName;
			}
			
			public override IClass GetUnderlyingClass() {
				return null;
			}
			
			public override List<IMethod> GetMethods() {
				return new List<IMethod>();
			}
			
			public override List<IProperty> GetProperties() {
				return new List<IProperty>();
			}
			
			public override List<IField> GetFields() {
				return new List<IField>();
			}
			
			public override List<IEvent> GetEvents() {
				return new List<IEvent>();
			}
			
			public override List<IIndexer> GetIndexers() {
				return new List<IIndexer>();
			}
		}
		
		static IReturnType CreateReturnType(Type type)
		{
			return ReflectionReturnType.Create(ProjectContentRegistry.Mscorlib, type, false);
		}
	}
}
