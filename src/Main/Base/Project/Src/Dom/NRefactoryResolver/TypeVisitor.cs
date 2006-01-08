// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
	public class TypeVisitor : AbstractAstVisitor
	{
		NRefactoryResolver resolver;
		
		public TypeVisitor(NRefactoryResolver resolver)
		{
			this.resolver = resolver;
		}
		
		public override object Visit(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value == null) {
				return NullReturnType.Instance;
			} else {
				return ReflectionReturnType.CreatePrimitive(primitiveExpression.Value.GetType());
			}
		}
		
		public override object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			switch (binaryOperatorExpression.Op) {
				case BinaryOperatorType.NullCoalescing:
					return binaryOperatorExpression.Right.AcceptVisitor(this, data);
				case BinaryOperatorType.DivideInteger:
					return ReflectionReturnType.Int;
				case BinaryOperatorType.Concat:
					return ReflectionReturnType.String;
				case BinaryOperatorType.Equality:
				case BinaryOperatorType.InEquality:
				case BinaryOperatorType.ReferenceEquality:
				case BinaryOperatorType.ReferenceInequality:
				case BinaryOperatorType.LogicalAnd:
				case BinaryOperatorType.LogicalOr:
				case BinaryOperatorType.LessThan:
				case BinaryOperatorType.LessThanOrEqual:
				case BinaryOperatorType.GreaterThan:
				case BinaryOperatorType.GreaterThanOrEqual:
					return ReflectionReturnType.Bool;
				default:
					return MemberLookupHelper.GetCommonType(binaryOperatorExpression.Left.AcceptVisitor(this, data) as IReturnType,
					                                        binaryOperatorExpression.Right.AcceptVisitor(this, data) as IReturnType);
			}
		}
		
		public override object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object Visit(InvocationExpression invocationExpression, object data)
		{
			IMethodOrProperty m = GetMethod(invocationExpression);
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
		
		public override object Visit(IndexerExpression indexerExpression, object data)
		{
			IProperty i = GetIndexer(indexerExpression);
			if (i != null)
				return i.ReturnType;
			else
				return null;
		}
		
		public IMethod FindOverload(List<IMethod> methods, IReturnType[] typeParameters, IList<Expression> arguments, object data)
		{
			if (methods.Count <= 0) {
				return null;
			}
			// We can't use this shortcut because MemberLookupHelper does type inference and
			// type substitution for us
			//if (methods.Count == 1)
			//	return methods[0];
			
			IReturnType[] types = new IReturnType[arguments.Count];
			for (int i = 0; i < types.Length; ++i) {
				types[i] = arguments[i].AcceptVisitor(this, data) as IReturnType;
			}
			return MemberLookupHelper.FindOverload(methods, typeParameters, types);
		}
		
		/*
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
		public static int FindOverload(List<IMethodOrProperty> methods, IReturnType[] types, bool forceParameterCount, out bool singleBestOverload)
		{
			singleBestOverload = true;
			if (methods.Count == 0)
				return -1;
			if (methods.Count == 1)
				return 0;
			
			IMethodOrProperty bestMethod = methods[0];
			int bestIndex = 0;
			int bestScore = ScoreOverload(bestMethod, types, forceParameterCount);
			
			for (int i = 1; i < methods.Count; ++i) {
				IMethodOrProperty method = methods[i];
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
		public static int ScoreOverload(IMethodOrProperty method, IReturnType[] types, bool forceParameterCount)
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
		 */
		
		/// <summary>
		/// Gets the method called by the InvocationExpression. In Visual Basic, the result
		/// can also be an Indexer.
		/// </summary>
		public IMethodOrProperty GetMethod(InvocationExpression invocationExpression)
		{
			IReturnType[] typeParameters = CreateReturnTypes(invocationExpression.TypeArguments);
			if (invocationExpression.TargetObject is FieldReferenceExpression) {
				FieldReferenceExpression field = (FieldReferenceExpression)invocationExpression.TargetObject;
				IReturnType type = field.TargetObject.AcceptVisitor(this, null) as IReturnType;
				List<IMethod> methods = resolver.SearchMethod(type, field.FieldName);
				if (methods.Count == 0 && resolver.Language == SupportedLanguage.VBNet)
					return GetVisualBasicIndexer(invocationExpression);
				return FindOverload(methods, typeParameters, invocationExpression.Arguments, null);
			} else if (invocationExpression.TargetObject is IdentifierExpression) {
				string id = ((IdentifierExpression)invocationExpression.TargetObject).Identifier;
				if (resolver.CallingClass == null) {
					return null;
				}
				List<IMethod> methods = resolver.SearchMethod(id);
				if (methods.Count == 0 && resolver.Language == SupportedLanguage.VBNet)
					return GetVisualBasicIndexer(invocationExpression);
				return FindOverload(methods, typeParameters, invocationExpression.Arguments, null);
			}
			return null;
		}
		
		IProperty GetVisualBasicIndexer(InvocationExpression invocationExpression)
		{
			return GetIndexer(new IndexerExpression(invocationExpression.TargetObject, invocationExpression.Arguments));
		}
		
		public IProperty GetIndexer(IndexerExpression indexerExpression)
		{
			IReturnType type = (IReturnType)indexerExpression.TargetObject.AcceptVisitor(this, null);
			if (type == null) {
				return null;
			}
			List<IProperty> indexers = type.GetProperties();
			// remove non-indexers:
			for (int i = 0; i < indexers.Count; i++) {
				if (!indexers[i].IsIndexer)
					indexers.RemoveAt(i--);
			}
			IReturnType[] parameters = new IReturnType[indexerExpression.Indices.Count];
			for (int i = 0; i < parameters.Length; i++) {
				Expression expr = indexerExpression.Indices[i] as Expression;
				if (expr != null)
					parameters[i] = (IReturnType)expr.AcceptVisitor(this, null);
			}
			return MemberLookupHelper.FindOverload(indexers.ToArray(), parameters);
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
				if (returnType is NamespaceReturnType) {
					string name = returnType.FullyQualifiedName;
					string combinedName;
					if (name.Length == 0)
						combinedName = fieldReferenceExpression.FieldName;
					else
						combinedName = name + "." + fieldReferenceExpression.FieldName;
					if (resolver.ProjectContent.NamespaceExists(combinedName)) {
						return new NamespaceReturnType(combinedName);
					}
					IClass c = resolver.GetClass(combinedName);
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
			IClass c = resolver.SearchClass(identifierExpression.Identifier);
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
				case UnaryOperatorType.Star:       // dereference
					//--expressionType.PointerNestingLevel;
					break;
				case UnaryOperatorType.BitWiseAnd: // get reference
					//++expressionType.PointerNestingLevel;
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
			return resolver.CallingClass.BaseType;
		}
		
		public override object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			return CreateReturnType(objectCreateExpression.CreateType);
		}
		
		public override object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			return CreateReturnType(arrayCreateExpression.CreateType);
		}
		
		public override object Visit(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return ReflectionReturnType.Bool;
		}
		
		public override object Visit(DefaultValueExpression defaultValueExpression, object data)
		{
			return CreateReturnType(defaultValueExpression.TypeReference);
		}
		
		public override object Visit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			return new AnonymousMethodReturnType();
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
		
		IReturnType[] CreateReturnTypes(List<TypeReference> references)
		{
			if (references == null) return new IReturnType[0];
			IReturnType[] types = new IReturnType[references.Count];
			for (int i = 0; i < types.Length; i++) {
				types[i] = CreateReturnType(references[i]);
			}
			return types;
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
			if (reference is InnerClassTypeReference) {
				reference = ((InnerClassTypeReference)reference).CombineToNormalTypeReference();
			}
			LanguageProperties languageProperties = projectContent.Language;
			IReturnType t = null;
			if (callingClass != null && !reference.IsGlobal) {
				foreach (ITypeParameter tp in callingClass.TypeParameters) {
					if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType)) {
						t = new GenericReturnType(tp);
						break;
					}
				}
				if (t == null && callingMember is IMethod && (callingMember as IMethod).TypeParameters != null) {
					foreach (ITypeParameter tp in (callingMember as IMethod).TypeParameters) {
						if (languageProperties.NameComparer.Equals(tp.Name, reference.SystemType)) {
							t = new GenericReturnType(tp);
							break;
						}
					}
				}
			}
			if (t == null) {
				if (reference.Type != reference.SystemType) {
					// keyword-type like void, int, string etc.
					t = ProjectContentRegistry.Mscorlib.GetClass(reference.SystemType).DefaultReturnType;
				} else {
					int typeParameterCount = reference.GenericTypes.Count;
					if (useLazyReturnType) {
						if (reference.IsGlobal)
							t = new GetClassReturnType(projectContent, reference.SystemType, typeParameterCount);
						else
							t = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn, reference.SystemType, typeParameterCount);
					} else {
						IClass c;
						if (reference.IsGlobal) {
							c = projectContent.GetClass(reference.SystemType, typeParameterCount);
							t = (c != null) ? c.DefaultReturnType : null;
						} else {
							t = projectContent.SearchType(reference.SystemType, typeParameterCount, callingClass, caretLine, caretColumn);
						}
						if (t == null) {
							if (reference.GenericTypes.Count == 0 && !reference.IsArrayType) {
								// reference to namespace is possible
								if (reference.IsGlobal) {
									if (projectContent.NamespaceExists(reference.Type))
										return new NamespaceReturnType(reference.Type);
								} else {
									string name = projectContent.SearchNamespace(reference.Type, callingClass, (callingClass == null) ? null : callingClass.CompilationUnit, caretLine, caretColumn);
									if (name != null)
										return new NamespaceReturnType(name);
								}
							}
							return null;
						}
					}
				}
			}
			if (reference.GenericTypes.Count > 0) {
				List<IReturnType> para = new List<IReturnType>(reference.GenericTypes.Count);
				for (int i = 0; i < reference.GenericTypes.Count; ++i) {
					para.Add(CreateReturnType(reference.GenericTypes[i], callingClass, callingMember, caretLine, caretColumn, projectContent, useLazyReturnType));
				}
				t = new ConstructedReturnType(t, para);
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
		}
		
		static IReturnType CreateReturnType(Type type)
		{
			return ReflectionReturnType.Create(ProjectContentRegistry.Mscorlib, null, type, false);
		}
	}
}
