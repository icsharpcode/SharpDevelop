// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// created on 22.08.2003 at 19:02

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class TypeVisitor : AbstractAstVisitor
	{
		NRefactoryResolver resolver;
		
		public TypeVisitor(NRefactoryResolver resolver)
		{
			this.resolver = resolver;
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value == null) {
				return NullReturnType.Instance;
			} else {
				return resolver.ProjectContent.SystemTypes.CreatePrimitive(primitiveExpression.Value.GetType());
			}
		}
		
		public override object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			IReturnType type = queryExpression.SelectOrGroupClause.AcceptVisitor(this, data) as IReturnType;
			if (type != null) {
				return new ConstructedReturnType(
					new GetClassReturnType(resolver.ProjectContent, "System.Collections.Generic.IEnumerable", 1),
					new IReturnType[] { type }
				);
			} else {
				return null;
			}
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			switch (binaryOperatorExpression.Op) {
				case BinaryOperatorType.NullCoalescing:
					return binaryOperatorExpression.Right.AcceptVisitor(this, data);
				case BinaryOperatorType.DivideInteger:
					return resolver.ProjectContent.SystemTypes.Int32;
				case BinaryOperatorType.Concat:
					return resolver.ProjectContent.SystemTypes.String;
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
					return resolver.ProjectContent.SystemTypes.Boolean;
				default:
					return MemberLookupHelper.GetCommonType(resolver.ProjectContent,
					                                        binaryOperatorExpression.Left.AcceptVisitor(this, data) as IReturnType,
					                                        binaryOperatorExpression.Right.AcceptVisitor(this, data) as IReturnType);
			}
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
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
		
		public override object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			IProperty i = GetIndexer(indexerExpression);
			if (i != null)
				return i.ReturnType;
			else
				return null;
		}
		
		static readonly IMethod[] emptyMethodsArray = new IMethod[0];
		
		/// <summary>
		/// Find the correct overload from a list of overloads.
		/// </summary>
		/// <param name="methods">The list of available methods to call.</param>
		/// <param name="typeParameters">The type parameters used for the method call.</param>
		/// <param name="arguments">The arguments passed to the method.</param>
		/// <returns>The chosen overload.</returns>
		public IMethod FindOverload(IList<IMethod> methods, IReturnType[] typeParameters, IList<Expression> arguments)
		{
			return FindOverload(methods, null, null, typeParameters, arguments);
		}
		
		/// <summary>
		/// Find the correct overload from a list of overloads.
		/// </summary>
		/// <param name="methods">The list of available methods to call.</param>
		/// <param name="extensionMethods">The list of available extension methods to call.</param>
		/// <param name="targetType">The type of the expression on which the method is called. Used as first argument to the extension methods.</param>
		/// <param name="typeParameters">The type parameters used for the method call.</param>
		/// <param name="arguments">The arguments passed to the method.</param>
		/// <returns>The chosen overload.</returns>
		public IMethod FindOverload(IList<IMethod> methods, IList<IMethod> extensionMethods, IReturnType targetType, IReturnType[] typeParameters, IList<Expression> arguments)
		{
			if (extensionMethods == null) extensionMethods = emptyMethodsArray;
			
			if (methods.Count == 0 && extensionMethods.Count == 0) {
				return null;
			}
			
			// We must call FindOverload even if there is only one possible match
			// because MemberLookupHelper does type inference and type substitution for us
			
			
			
			IReturnType[] types = new IReturnType[arguments.Count];
			for (int i = 0; i < types.Length; ++i) {
				types[i] = arguments[i].AcceptVisitor(this, null) as IReturnType;
			}
			
			bool matchIsAcceptable;
			IMethod result = MemberLookupHelper.FindOverload(methods, typeParameters, types, out matchIsAcceptable);
			if (matchIsAcceptable) return result;
			
			if (extensionMethods.Count > 0) {
				IReturnType[] extendedTypes = new IReturnType[types.Length + 1];
				extendedTypes[0] = targetType;
				types.CopyTo(extendedTypes, 1);
				
				IMethod extensionResult = MemberLookupHelper.FindOverload(extensionMethods, typeParameters, extendedTypes, out matchIsAcceptable);
				if (matchIsAcceptable)
					return extensionResult;
				else
					return result ?? extensionResult;
			} else {
				return result;
			}
		}
		
		/// <summary>
		/// Gets the method called by the InvocationExpression. In Visual Basic, the result
		/// can also be an Indexer.
		/// </summary>
		public IMethodOrProperty GetMethod(InvocationExpression invocationExpression)
		{
			IReturnType[] typeParameters = CreateReturnTypes(invocationExpression.TypeArguments);
			if (invocationExpression.TargetObject is FieldReferenceExpression) {
				FieldReferenceExpression field = (FieldReferenceExpression)invocationExpression.TargetObject;
				IReturnType targetType = field.TargetObject.AcceptVisitor(this, null) as IReturnType;
				
				List<IMethod> methods = resolver.SearchMethod(targetType, field.FieldName);
				
				// FindOverload does not need the extensionMethods if a normal method is applicable,
				// so we use a LazyList because SearchExtensionMethods is expensive and we don't want to call it
				// if it is not required.
				IList<IMethod> extensionMethods = new LazyList<IMethod>(
					delegate { return resolver.SearchExtensionMethods(field.FieldName); });
				
				if (methods.Count == 0 && extensionMethods.Count == 0 && resolver.Language == SupportedLanguage.VBNet)
					return GetVisualBasicIndexer(invocationExpression);
				return FindOverload(methods, extensionMethods, targetType, typeParameters, invocationExpression.Arguments);
			} else if (invocationExpression.TargetObject is IdentifierExpression) {
				string id = ((IdentifierExpression)invocationExpression.TargetObject).Identifier;
				if (resolver.CallingClass == null) {
					return null;
				}
				List<IMethod> methods = resolver.SearchMethod(id);
				if (methods.Count == 0 && resolver.Language == SupportedLanguage.VBNet)
					return GetVisualBasicIndexer(invocationExpression);
				return FindOverload(methods, typeParameters, invocationExpression.Arguments);
			} else {
				if (resolver.Language == SupportedLanguage.CSharp && resolver.CallingClass != null) {
					if (invocationExpression.TargetObject is ThisReferenceExpression) {
						// call to constructor
						return FindOverload(GetConstructors(resolver.CallingClass), typeParameters, invocationExpression.Arguments);
					} else if (invocationExpression.TargetObject is BaseReferenceExpression) {
						return FindOverload(GetConstructors(resolver.CallingClass.BaseClass), typeParameters, invocationExpression.Arguments);
					}
				}
				
				// this could be a nested indexer access
				if (resolver.Language == SupportedLanguage.VBNet)
					return GetVisualBasicIndexer(invocationExpression);
			}
			return null;
		}
		
		IList<IMethod> GetConstructors(IClass c)
		{
			if (c == null)
				return emptyMethodsArray;
			return c.Methods.FindAll(delegate(IMethod m) { return m.IsConstructor; });
		}
		
		public IProperty GetIndexer(IndexerExpression indexerExpression)
		{
			IReturnType targetObjectType = (IReturnType)indexerExpression.TargetObject.AcceptVisitor(this, null);
			return GetIndexer(indexerExpression, targetObjectType);
		}
		
		IProperty GetVisualBasicIndexer(InvocationExpression invocationExpression)
		{
			ResolveResult targetRR = resolver.ResolveInternal(invocationExpression.TargetObject, ExpressionContext.Default);
			if (targetRR != null) {
				// Visual Basic can call indexers in two ways:
				// collection(index) - use indexer
				// collection.Item(index) - use parametrized property
				
				if (invocationExpression.TargetObject is IdentifierExpression || invocationExpression.TargetObject is FieldReferenceExpression) {
					// only IdentifierExpression/FieldReferenceExpression can represent a parametrized property
					// - the check is necessary because collection.Items and collection.Item(index) both
					// resolve to the same property, but we want to use the default indexer for the second call in
					// collection.Item(index1)(index2)
					MemberResolveResult memberRR = targetRR as MemberResolveResult;
					if (memberRR != null)  {
						IProperty p = memberRR.ResolvedMember as IProperty;
						if (p != null && p.Parameters.Count > 0) {
							// this is a parametrized property
							return p;
						}
					}
				}
				// not a parametrized property - try normal indexer
				return GetIndexer(new IndexerExpression(invocationExpression.TargetObject, invocationExpression.Arguments),
				                  targetRR.ResolvedType);
			} else {
				return null;
			}
		}
		
		IProperty GetIndexer(IndexerExpression indexerExpression, IReturnType targetObjectType)
		{
			if (targetObjectType == null) {
				return null;
			}
			List<IProperty> indexers = targetObjectType.GetProperties();
			// remove non-indexers:
			for (int i = 0; i < indexers.Count; i++) {
				if (!indexers[i].IsIndexer)
					indexers.RemoveAt(i--);
			}
			IReturnType[] parameters = new IReturnType[indexerExpression.Indexes.Count];
			for (int i = 0; i < parameters.Length; i++) {
				Expression expr = indexerExpression.Indexes[i] as Expression;
				if (expr != null)
					parameters[i] = (IReturnType)expr.AcceptVisitor(this, null);
			}
			return MemberLookupHelper.FindOverload(indexers.ToArray(), parameters);
		}
		
		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
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
		
		public override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
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
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			if (identifierExpression == null) {
				return null;
			}
			IClass c = resolver.SearchClass(identifierExpression.Identifier, identifierExpression.StartLocation);
			if (c != null) {
				return c.DefaultReturnType;
			}
			return resolver.DynamicLookup(identifierExpression.Identifier, identifierExpression.StartLocation);
		}
		
		public override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return CreateReturnType(typeReferenceExpression.TypeReference);
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
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
		
		public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
		{
			return assignmentExpression.Left.AcceptVisitor(this, data);
		}
		
		public override object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			return resolver.ProjectContent.SystemTypes.Int32;
		}
		
		public override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return resolver.ProjectContent.SystemTypes.Type;
		}
		
		public override object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			return checkedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			return uncheckedExpression.Expression.AcceptVisitor(this, data);
		}
		
		public override object VisitCastExpression(CastExpression castExpression, object data)
		{
			return CreateReturnType(castExpression.CastTo);
		}
		
		public override object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			/*ReturnType returnType = new ReturnType(stackAllocExpression.TypeReference);
			++returnType.PointerNestingLevel;
			return returnType;*/
			return null;
		}
		
		public override object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			return resolver.CallingClass.DefaultReturnType;
		}
		
		public override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			return resolver.CallingClass.DefaultReturnType;
		}
		
		public override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			if (resolver.CallingClass == null) {
				return null;
			}
			return resolver.CallingClass.BaseType;
		}
		
		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			if (objectCreateExpression.IsAnonymousType) {
				return CreateAnonymousTypeClass(objectCreateExpression.ObjectInitializer).DefaultReturnType;
			} else {
				return CreateReturnType(objectCreateExpression.CreateType);
			}
		}
		
		DefaultClass CreateAnonymousTypeClass(CollectionInitializerExpression initializer)
		{
			List<IReturnType> fieldTypes = new List<IReturnType>();
			List<string> fieldNames = new List<string>();
			
			foreach (Expression expr in initializer.CreateExpressions) {
				if (expr is AssignmentExpression) {
					// use right part only
					fieldTypes.Add( ((AssignmentExpression)expr).Right.AcceptVisitor(this, null) as IReturnType );
				} else {
					fieldTypes.Add( expr.AcceptVisitor(this, null) as IReturnType );
				}
				
				fieldNames.Add(GetAnonymousTypeFieldName(expr));
			}
			
			StringBuilder nameBuilder = new StringBuilder();
			nameBuilder.Append('{');
			for (int i = 0; i < fieldTypes.Count; i++) {
				if (i > 0) nameBuilder.Append(", ");
				nameBuilder.Append(fieldNames[i]);
				nameBuilder.Append(" : ");
				if (fieldTypes[i] != null) {
					nameBuilder.Append(fieldTypes[i].DotNetName);
				}
			}
			nameBuilder.Append('}');
			
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(resolver.ProjectContent), nameBuilder.ToString());
			c.Modifiers = ModifierEnum.Internal | ModifierEnum.Synthetic | ModifierEnum.Sealed;
			for (int i = 0; i < fieldTypes.Count; i++) {
				c.Fields.Add(new DefaultField(fieldTypes[i], fieldNames[i], ModifierEnum.Public | ModifierEnum.Synthetic, DomRegion.Empty, c));
			}
			return c;
		}
		
		static string GetAnonymousTypeFieldName(Expression expr)
		{
			if (expr is FieldReferenceExpression) {
				return ((FieldReferenceExpression)expr).FieldName;
			}
			if (expr is AssignmentExpression) {
				expr = ((AssignmentExpression)expr).Left; // use left side if it is an IdentifierExpression
			}
			if (expr is IdentifierExpression) {
				return ((IdentifierExpression)expr).Identifier;
			} else {
				return "?";
			}
		}
		
		public override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (arrayCreateExpression.IsImplicitlyTyped) {
				return arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, data);
			} else {
				return CreateReturnType(arrayCreateExpression.CreateType);
			}
		}
		
		public override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return resolver.ProjectContent.SystemTypes.Boolean;
		}
		
		public override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			return CreateReturnType(defaultValueExpression.TypeReference);
		}
		
		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			AnonymousMethodReturnType amrt = new AnonymousMethodReturnType(resolver.CompilationUnit);
			if (anonymousMethodExpression.HasParameterList) {
				amrt.MethodParameters = new List<IParameter>();
				foreach (ParameterDeclarationExpression param in anonymousMethodExpression.Parameters) {
					amrt.MethodParameters.Add(NRefactoryASTConvertVisitor.CreateParameter(param, resolver.CallingMember as IMethod, resolver.CallingClass, resolver.CompilationUnit));
				}
			}
			return amrt;
		}
		
		public override object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data)
		{
			// used for implicitly typed arrays
			if (collectionInitializerExpression.CreateExpressions.Count == 0)
				return null;
			IReturnType combinedRT = collectionInitializerExpression.CreateExpressions[0].AcceptVisitor(this, data) as IReturnType;
			for (int i = 1; i < collectionInitializerExpression.CreateExpressions.Count; i++) {
				IReturnType rt = collectionInitializerExpression.CreateExpressions[i].AcceptVisitor(this, data) as IReturnType;
				combinedRT = MemberLookupHelper.GetCommonType(resolver.ProjectContent, combinedRT, rt);
			}
			return new ArrayReturnType(resolver.ProjectContent, combinedRT, 1);
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
					IClass c = projectContent.GetClass(reference.SystemType, 0);
					if (c != null)
						t = c.DefaultReturnType;
					else
						t = new GetClassReturnType(projectContent, reference.SystemType, 0);
				} else {
					int typeParameterCount = reference.GenericTypes.Count;
					if (useLazyReturnType) {
						if (reference.IsGlobal)
							t = new GetClassReturnType(projectContent, reference.SystemType, typeParameterCount);
						else if (callingClass != null)
							t = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn, reference.SystemType, typeParameterCount);
					} else {
						IClass c;
						if (reference.IsGlobal) {
							c = projectContent.GetClass(reference.SystemType, typeParameterCount);
							t = (c != null) ? c.DefaultReturnType : null;
						} else if (callingClass != null) {
							t = projectContent.SearchType(new SearchTypeRequest(reference.SystemType, typeParameterCount, callingClass, caretLine, caretColumn)).Result;
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
			return WrapArray(projectContent, t, reference);
		}
		
		static IReturnType WrapArray(IProjectContent pc, IReturnType t, TypeReference reference)
		{
			if (reference.IsArrayType) {
				for (int i = reference.RankSpecifier.Length - 1; i >= 0; --i) {
					int dimensions = reference.RankSpecifier[i] + 1;
					if (dimensions > 0) {
						t = new ArrayReturnType(pc, t, dimensions);
					}
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
	}
}
