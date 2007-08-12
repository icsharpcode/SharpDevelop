// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// This class converts C# constructs to their VB.NET equivalents.
	/// </summary>
	public class VBNetToCSharpConvertVisitor : VBNetConstructsConvertVisitor
	{
		// Fixes identifier casing
		// Adds using statements for the default usings
		// Convert "ReDim" statement
		// Convert "WithEvents" fields/"Handles" clauses
		
		public string NamespacePrefixToAdd { get; set; }
		
		protected readonly IProjectContent projectContent;
		protected readonly NRefactoryResolver resolver;
		protected readonly ParseInformation parseInformation;
		
		public VBNetToCSharpConvertVisitor(IProjectContent pc, ParseInformation parseInfo)
		{
			resolver = new NRefactoryResolver(LanguageProperties.VBNet);
			projectContent = pc;
			parseInformation = parseInfo;
		}
		
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			if (!string.IsNullOrEmpty(NamespacePrefixToAdd)) {
				for (int i = 0; i < compilationUnit.Children.Count; i++) {
					NamespaceDeclaration ns = compilationUnit.Children[i] as NamespaceDeclaration;
					if (ns != null) {
						ns.Name = NamespacePrefixToAdd + "." + ns.Name;
					}
					TypeDeclaration td = compilationUnit.Children[i] as TypeDeclaration;
					if (td != null) {
						ns = new NamespaceDeclaration(NamespacePrefixToAdd);
						ns.AddChild(td);
						compilationUnit.Children[i] = ns;
					}
				}
			}
			
			ToCSharpConvertVisitor v = new ToCSharpConvertVisitor();
			compilationUnit.AcceptVisitor(v, data);
			if (projectContent != null && projectContent.DefaultImports != null) {
				int index = 0;
				foreach (string u in projectContent.DefaultImports.Usings) {
					compilationUnit.Children.Insert(index++, new UsingDeclaration(u));
				}
			}
			return null;
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			resolver.Initialize(parseInformation, typeDeclaration.BodyStartLocation.Line, typeDeclaration.BodyStartLocation.Column);
			
			if (resolver.CallingClass != null) {
				// add Partial modifier to all parts of the class
				IClass callingClass = resolver.CallingClass.GetCompoundClass();
				if (callingClass.IsPartial) {
					typeDeclaration.Modifier |= Modifiers.Partial;
				}
				// determine if the type contains handles clauses referring to the current type
				bool containsClassHandlesClauses = false;
				bool hasConstructors = false;
				foreach (IMethod method in callingClass.Methods) {
					// do not count compiler-generated constructors
					if (method.IsSynthetic) continue;
					
					hasConstructors |= method.IsConstructor;
					foreach (string handles in method.HandlesClauses) {
						containsClassHandlesClauses |= !handles.Contains(".");
					}
				}
				CompoundClass compoundClass = callingClass as CompoundClass;
				if (containsClassHandlesClauses && !hasConstructors) {
					// ensure the type has at least one constructor to which the AddHandlerStatements can be added
					// add constructor only to one part
					if (compoundClass == null || compoundClass.GetParts()[0] == resolver.CallingClass) {
						ConstructorDeclaration cd = new ConstructorDeclaration(typeDeclaration.Name, Modifiers.Public, null, null);
						cd.Body = new BlockStatement();
						typeDeclaration.AddChild(cd);
					}
				}
			}
			
			base.VisitTypeDeclaration(typeDeclaration, data);
			return null;
		}
		
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			// Initialize resolver for method:
			if (!methodDeclaration.Body.IsNull) {
				if (resolver.Initialize(parseInformation, methodDeclaration.Body.StartLocation.Line, methodDeclaration.Body.StartLocation.Column)) {
					resolver.RunLookupTableVisitor(methodDeclaration);
				}
			}
			
			methodDeclaration.HandlesClause.Clear();
			
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
		{
			resolver.Initialize(parseInformation, fieldDeclaration.StartLocation.Line, fieldDeclaration.StartLocation.Column);
			
			base.VisitFieldDeclaration(fieldDeclaration, data);
			
			if ((fieldDeclaration.Modifier & Modifiers.WithEvents) == Modifiers.WithEvents) {
				TransformWithEventsField(fieldDeclaration);
				if (fieldDeclaration.Fields.Count == 0) {
					RemoveCurrentNode();
				}
			}
			
			return null;
		}
		
		void TransformWithEventsField(FieldDeclaration fieldDeclaration)
		{
			if (resolver.CallingClass == null)
				return;
			
			INode insertAfter = fieldDeclaration;
			
			for (int i = 0; i < fieldDeclaration.Fields.Count;) {
				VariableDeclaration field = fieldDeclaration.Fields[i];
				
				IdentifierExpression backingFieldNameExpression = null;
				PropertyDeclaration createdProperty = null;
				foreach (IMethod m in resolver.CallingClass.GetCompoundClass().Methods) {
					foreach (string handlesClause in m.HandlesClauses) {
						int pos = handlesClause.IndexOf('.');
						if (pos > 0) {
							string fieldName = handlesClause.Substring(0, pos);
							string eventName = handlesClause.Substring(pos + 1);
							if (resolver.IsSameName(fieldName, field.Name)) {
								if (createdProperty == null) {
									FieldDeclaration backingField = new FieldDeclaration(null);
									backingField.Fields.Add(new VariableDeclaration(
										"withEventsField_" + field.Name, field.Initializer, fieldDeclaration.GetTypeForField(i)));
									backingField.Modifier = Modifiers.Private;
									InsertAfterSibling(insertAfter, backingField);
									createdProperty = new PropertyDeclaration(fieldDeclaration.Modifier, null, field.Name, null);
									createdProperty.TypeReference = fieldDeclaration.GetTypeForField(i);
									createdProperty.StartLocation = fieldDeclaration.StartLocation;
									createdProperty.EndLocation = fieldDeclaration.EndLocation;
									
									backingFieldNameExpression = new IdentifierExpression(backingField.Fields[0].Name);
									
									createdProperty.GetRegion = new PropertyGetRegion(new BlockStatement(), null);
									createdProperty.GetRegion.Block.AddChild(new ReturnStatement(
										backingFieldNameExpression));
									
									Expression backingFieldNotNullTest = new BinaryOperatorExpression(
										backingFieldNameExpression,
										BinaryOperatorType.InEquality,
										new PrimitiveExpression(null, "null"));
									
									createdProperty.SetRegion = new PropertySetRegion(new BlockStatement(), null);
									createdProperty.SetRegion.Block.AddChild(new IfElseStatement(
										backingFieldNotNullTest, new BlockStatement()
									));
									createdProperty.SetRegion.Block.AddChild(new ExpressionStatement(
										new AssignmentExpression(
											backingFieldNameExpression,
											AssignmentOperatorType.Assign,
											new IdentifierExpression("value"))));
									createdProperty.SetRegion.Block.AddChild(new IfElseStatement(
										backingFieldNotNullTest, new BlockStatement()
									));
									InsertAfterSibling(backingField, createdProperty);
									insertAfter = createdProperty;
								}
								
								// insert code to remove the event handler
								IfElseStatement ies = (IfElseStatement)createdProperty.SetRegion.Block.Children[0];
								ies.TrueStatement[0].AddChild(new RemoveHandlerStatement(
									new FieldReferenceExpression(backingFieldNameExpression, eventName),
									new AddressOfExpression(new IdentifierExpression(m.Name))));
								
								// insert code to add the event handler
								ies = (IfElseStatement)createdProperty.SetRegion.Block.Children[2];
								ies.TrueStatement[0].AddChild(new AddHandlerStatement(
									new FieldReferenceExpression(backingFieldNameExpression, eventName),
									new AddressOfExpression(new IdentifierExpression(m.Name))));
							}
						}
					}
				}
				
				if (createdProperty != null) {
					// field replaced with property
					fieldDeclaration.Fields.RemoveAt(i);
				} else {
					i++;
				}
			}
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
		{
			if (!constructorDeclaration.Body.IsNull) {
				if (resolver.Initialize(parseInformation, constructorDeclaration.Body.StartLocation.Line, constructorDeclaration.Body.StartLocation.Column)) {
					resolver.RunLookupTableVisitor(constructorDeclaration);
				}
			}
			base.VisitConstructorDeclaration(constructorDeclaration, data);
			if (resolver.CallingClass != null) {
				if (constructorDeclaration.ConstructorInitializer.IsNull
				    || constructorDeclaration.ConstructorInitializer.ConstructorInitializerType != ConstructorInitializerType.This)
				{
					AddClassEventHandlersToConstructor(constructorDeclaration);
				}
			}
			return null;
		}
		
		void AddClassEventHandlersToConstructor(ConstructorDeclaration constructorDeclaration)
		{
			foreach (IMethod method in resolver.CallingClass.GetCompoundClass().Methods) {
				foreach (string handles in method.HandlesClauses) {
					if (!handles.Contains(".")) {
						AddHandlerStatement ahs = new AddHandlerStatement(
							new IdentifierExpression(handles),
							new AddressOfExpression(new IdentifierExpression(method.Name))
						);
						constructorDeclaration.Body.Children.Insert(0, ahs);
						ahs.Parent = constructorDeclaration.Body;
					}
				}
			}
		}
		
		public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
		{
			if (resolver.Initialize(parseInformation, propertyDeclaration.BodyStart.Line, propertyDeclaration.BodyStart.Column)) {
				resolver.RunLookupTableVisitor(propertyDeclaration);
			}
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}

		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			base.VisitIdentifierExpression(identifierExpression, data);
			if (resolver.CompilationUnit == null)
				return null;
			
			ResolveResult rr = resolver.ResolveInternal(identifierExpression, ExpressionContext.Default);
			string ident = GetIdentifierFromResult(rr);
			if (ident != null) {
				identifierExpression.Identifier = ident;
			}
			FullyQualifyModuleMemberReference(identifierExpression, rr);
			return null;
		}

		public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
			
			if (resolver.CompilationUnit == null)
				return null;
			
			ResolveResult rr = resolver.ResolveInternal(fieldReferenceExpression, ExpressionContext.Default);
			string ident = GetIdentifierFromResult(rr);
			if (ident != null) {
				fieldReferenceExpression.FieldName = ident;
			}
			
			if (rr is MethodResolveResult
			    && !(fieldReferenceExpression.Parent is AddressOfExpression)
			    && !(fieldReferenceExpression.Parent is InvocationExpression))
			{
				InvocationExpression ie = new InvocationExpression(fieldReferenceExpression);
				ReplaceCurrentNode(ie);
			} else {
				FullyQualifyModuleMemberReference(fieldReferenceExpression, rr);
			}
			
			return rr;
		}
		
		IReturnType GetContainingTypeOfStaticMember(ResolveResult rr)
		{
			MethodResolveResult methodRR = rr as MethodResolveResult;
			if (methodRR != null) {
				return methodRR.ContainingType;
			}
			MemberResolveResult memberRR = rr as MemberResolveResult;
			if (memberRR != null && memberRR.ResolvedMember.IsStatic) {
				return memberRR.ResolvedMember.DeclaringTypeReference;
			}
			return null;
		}
		
		void FullyQualifyModuleMemberReference(IdentifierExpression ident, ResolveResult rr)
		{
			IReturnType containingType = GetContainingTypeOfStaticMember(rr);
			if (containingType == null)
				return;
			if (resolver.CallingClass != null) {
				if (resolver.CallingClass.IsTypeInInheritanceTree(containingType.GetUnderlyingClass()))
					return;
			}
			ReplaceCurrentNode(new FieldReferenceExpression(
				new TypeReferenceExpression(Refactoring.CodeGenerator.ConvertType(containingType, CreateContext())),
				ident.Identifier
			));
		}
		
		void FullyQualifyModuleMemberReference(FieldReferenceExpression fre, ResolveResult rr)
		{
			IReturnType containingType = GetContainingTypeOfStaticMember(rr);
			if (containingType == null)
				return;
			
			ResolveResult targetRR = resolver.ResolveInternal(fre.TargetObject, ExpressionContext.Default);
			if (targetRR is NamespaceResolveResult) {
				fre.TargetObject = new TypeReferenceExpression(Refactoring.CodeGenerator.ConvertType(containingType, CreateContext()));
			}
		}

		public override object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
		{
			base.VisitInvocationExpression(invocationExpression, data);
			
			if (resolver.CompilationUnit == null)
				return null;
			
			if (!(invocationExpression.Parent is ReDimStatement))
			{
				ProcessInvocationExpression(invocationExpression);
			}
			
			return null;
		}
		
		void ProcessInvocationExpression(InvocationExpression invocationExpression)
		{
			MemberResolveResult rr = resolver.ResolveInternal(invocationExpression, ExpressionContext.Default) as MemberResolveResult;
			if (rr != null) {
				IProperty p = rr.ResolvedMember as IProperty;
				if (p != null && invocationExpression.Arguments.Count > 0) {
					ReplaceCurrentNode(new IndexerExpression(invocationExpression.TargetObject, invocationExpression.Arguments));
				}
				IMethod m = rr.ResolvedMember as IMethod;
				if (m != null && invocationExpression.Arguments.Count == m.Parameters.Count) {
					for (int i = 0; i < m.Parameters.Count; i++) {
						if (m.Parameters[i].IsOut) {
							invocationExpression.Arguments[i] = new DirectionExpression(
								FieldDirection.Out, invocationExpression.Arguments[i]);
						} else if (m.Parameters[i].IsRef) {
							invocationExpression.Arguments[i] = new DirectionExpression(
								FieldDirection.Ref, invocationExpression.Arguments[i]);
						}
					}
				}
			}
		}

		ClassFinder CreateContext()
		{
			return new ClassFinder(resolver.CallingClass, resolver.CallingMember, resolver.CaretLine, resolver.CaretColumn);
		}

		public override object VisitReDimStatement(ReDimStatement reDimStatement, object data)
		{
			base.VisitReDimStatement(reDimStatement, data);
			
			if (resolver.CompilationUnit == null)
				return null;
			
			if (reDimStatement.ReDimClauses.Count != 1)
				return null;
			
			if (reDimStatement.IsPreserve) {
				if (reDimStatement.ReDimClauses[0].Arguments.Count > 1) {
					// multidimensional Redim Preserve
					// replace with:
					// MyArray = (int[,])Microsoft.VisualBasic.CompilerServices.Utils.CopyArray(MyArray, new int[dim1+1, dim2+1]);
					
					ResolveResult rr = resolver.ResolveInternal(reDimStatement.ReDimClauses[0].TargetObject, ExpressionContext.Default);
					if (rr != null && rr.ResolvedType != null && rr.ResolvedType.IsArrayReturnType) {
						ArrayCreateExpression ace = new ArrayCreateExpression(
							Refactoring.CodeGenerator.ConvertType(rr.ResolvedType, CreateContext())
						);
						foreach (Expression arg in reDimStatement.ReDimClauses[0].Arguments) {
							ace.Arguments.Add(Expression.AddInteger(arg, 1));
						}
						
						ReplaceCurrentNode(new ExpressionStatement(
							new AssignmentExpression(
								reDimStatement.ReDimClauses[0].TargetObject,
								AssignmentOperatorType.Assign,
								new CastExpression(
									ace.CreateType,
									new InvocationExpression(
										MakeFieldReferenceExpression("Microsoft.VisualBasic.CompilerServices.Utils.CopyArray"),
										new List<Expression> {
											reDimStatement.ReDimClauses[0].TargetObject,
											ace
										}
									),
									CastType.Cast
								)
							)));
					}
				}
			} else {
				// replace with array create expression
				
				ResolveResult rr = resolver.ResolveInternal(reDimStatement.ReDimClauses[0].TargetObject, ExpressionContext.Default);
				if (rr != null && rr.ResolvedType != null && rr.ResolvedType.IsArrayReturnType) {
					ArrayCreateExpression ace = new ArrayCreateExpression(
						Refactoring.CodeGenerator.ConvertType(rr.ResolvedType, CreateContext())
					);
					foreach (Expression arg in reDimStatement.ReDimClauses[0].Arguments) {
						ace.Arguments.Add(Expression.AddInteger(arg, 1));
					}
					
					ReplaceCurrentNode(new ExpressionStatement(
						new AssignmentExpression(reDimStatement.ReDimClauses[0].TargetObject, AssignmentOperatorType.Assign, ace)));
				}
			}
			return null;
		}

		protected Expression MakeFieldReferenceExpression(string name)
		{
			Expression e = null;
			foreach (string n in name.Split('.')) {
				if (e == null)
					e = new IdentifierExpression(n);
				else
					e = new FieldReferenceExpression(e, n);
			}
			return e;
		}

		public override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			base.VisitDefaultValueExpression(defaultValueExpression, data);
			
			IReturnType type = FixTypeReferenceCasing(defaultValueExpression.TypeReference, defaultValueExpression.StartLocation);
			// the VBNetConstructsConvertVisitor will initialize local variables to
			// default(TypeReference).
			// MyType m = null; looks nicer than MyType m = default(MyType))
			// so we replace default(ReferenceType) with null
			if (type != null && (type.IsDefaultReturnType || type.IsConstructedReturnType)) {
				IClass c = type.GetUnderlyingClass();
				if (c != null && (c.ClassType == ClassType.Class || c.ClassType == ClassType.Interface || c.ClassType == ClassType.Delegate)) {
					ReplaceCurrentNode(new PrimitiveExpression(null, "null"));
				}
			}
			return null;
		}

		public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
		{
			FixTypeReferenceCasing(variableDeclaration.TypeReference, variableDeclaration.StartLocation);
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}

		IReturnType FixTypeReferenceCasing(TypeReference tr, Location loc)
		{
			if (resolver.CompilationUnit == null) return null;
			if (tr.IsNull) return null;
			IReturnType rt = resolver.SearchType(tr.SystemType, loc);
			if (rt != null) {
				IClass c = rt.GetUnderlyingClass();
				if (c != null) {
					if (string.Equals(tr.Type, c.Name, StringComparison.InvariantCultureIgnoreCase)) {
						tr.Type = c.Name;
					}
				}
			}
			return rt;
		}

		string GetIdentifierFromResult(ResolveResult rr)
		{
			LocalResolveResult lrr = rr as LocalResolveResult;
			if (lrr != null)
				return lrr.Field.Name;
			MemberResolveResult mrr = rr as MemberResolveResult;
			if (mrr != null)
				return mrr.ResolvedMember.Name;
			MethodResolveResult mtrr = rr as MethodResolveResult;
			if (mtrr != null)
				return mtrr.Name;
			TypeResolveResult trr = rr as TypeResolveResult;
			if (trr != null && trr.ResolvedClass != null)
				return trr.ResolvedClass.Name;
			return null;
		}
	}
}
