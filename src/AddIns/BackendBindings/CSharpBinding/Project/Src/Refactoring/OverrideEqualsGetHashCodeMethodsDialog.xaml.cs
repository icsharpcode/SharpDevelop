// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for OverrideEqualsGetHashCodeMethodsDialog.xaml
	/// </summary>
	public partial class OverrideEqualsGetHashCodeMethodsDialog : AbstractInlineRefactorDialog
	{
		ITypeDefinition selectedClass;
		IMethod selectedMethod;
		AstNode baseCallNode;
		
		public OverrideEqualsGetHashCodeMethodsDialog(InsertionContext context, ITextEditor editor, ITextAnchor endAnchor,
		                                              ITextAnchor insertionPosition, ITypeDefinition selectedClass, IMethod selectedMethod, AstNode baseCallNode)
			: base(context, editor, insertionPosition)
		{
			if (selectedClass == null)
				throw new ArgumentNullException("selectedClass");
			
			InitializeComponent();
			
			this.selectedClass = selectedClass;
			this.insertionEndAnchor = endAnchor;
			this.selectedMethod = selectedMethod;
			this.baseCallNode = baseCallNode;
			
			addIEquatable.Content = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.OverrideEqualsGetHashCodeMethods.AddInterface}"),
			                                      "IEquatable<" + selectedClass.Name + ">");
			
			string otherMethod = selectedMethod.Name == "Equals" ? "GetHashCode" : "Equals";
			
			addOtherMethod.Content = StringParser.Parse("${res:AddIns.SharpRefactoring.OverrideEqualsGetHashCodeMethods.AddOtherMethod}", new StringTagPair("otherMethod", otherMethod));
			
			addIEquatable.IsEnabled = !selectedClass.GetAllBaseTypes().Any(
				type => {
					if (!type.IsParameterized || (type.TypeParameterCount != 1))
						return false;
					if (type.FullName != "System.IEquatable")
						return false;
					return type.TypeArguments.First().FullName == selectedClass.FullName;
				}
			);
		}
		
		static int[] largePrimes = {
			1000000007, 1000000009, 1000000021, 1000000033,
			1000000087, 1000000093, 1000000097, 1000000103,
			1000000123, 1000000181, 1000000207, 1000000223,
			1000000241, 1000000271, 1000000289, 1000000297,
			1000000321, 1000000349, 1000000363, 1000000403,
			1000000409, 1000000411, 1000000427, 1000000433,
			1000000439, 1000000447, 1000000453, 1000000459,
			1000000483, 1000000513, 1000000531, 1000000579
		};
		
		static bool CanCompareEqualityWithOperator(IType type)
		{
			// return true for value types except float and double
			// return false for reference types except string.
			return type != null
				&& type.FullName != "System.Single"
				&& type.FullName != "System.Double"
				&& (!IsReferenceType(type)
				    || type.FullName == "System.String");
		}
		
		Expression TestEquality(string other, IField field)
		{
			if (CanCompareEqualityWithOperator(field.ReturnType)) {
				return new BinaryOperatorExpression(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name),
				                                    BinaryOperatorType.Equality,
				                                    new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(ConvertType(KnownTypeCode.Object)), "Equals")
				);
				ie.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name));
				ie.Arguments.Add(new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
				return ie;
			}
		}
		
		Expression TestEquality(string other, IProperty property)
		{
			if (CanCompareEqualityWithOperator(property.ReturnType)) {
				return new BinaryOperatorExpression(new MemberReferenceExpression(new ThisReferenceExpression(), property.Name),
				                                    BinaryOperatorType.Equality,
				                                    new MemberReferenceExpression(new IdentifierExpression(other), property.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(ConvertType(KnownTypeCode.Object)), "Equals")
				);
				ie.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), property.Name));
				ie.Arguments.Add(new MemberReferenceExpression(new IdentifierExpression(other), property.Name));
				return ie;
			}
		}
		
		static bool IsReferenceType(IType type)
		{
			if (type.IsReferenceType.HasValue) {
				return type.IsReferenceType.Value;
			}
			
			return false;
		}
		
		protected override string GenerateCode(ITypeDefinition currentClass)
		{
			MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
			if (insertedOverrideMethod == null)
			{
				// We are not inside of a method declaration
				return null;
			}
			
			using (Script script = refactoringContext.StartScript()) {
				NewLineNode nextNewLineNode = insertedOverrideMethod.NextSibling as NewLineNode;
				
//			if (Options.AddIEquatableInterface) {
				// TODO : add IEquatable<T> to class
//				IAmbience ambience = currentClass.CompilationUnit.Language.GetAmbience();
//
//				IReturnType baseRType = currentClass.CompilationUnit.ProjectContent.GetClass("System.IEquatable", 1).DefaultReturnType;
//
//				IClass newClass = new DefaultClass(currentClass.CompilationUnit, currentClass.FullyQualifiedName, currentClass.Modifiers, currentClass.Region, null);
//
//				foreach (IReturnType type in currentClass.BaseTypes) {
//					newClass.BaseTypes.Add(type);
//				}
//
//
//				newClass.BaseTypes.Add(new ConstructedReturnType(baseRType, new List<IReturnType>() { currentClass.DefaultReturnType }));
//
//				ambience.ConversionFlags = ConversionFlags.IncludeBody;
//
//				string a = ambience.Convert(currentClass);
//
//				int startOffset = editor.Document.PositionToOffset(currentClass.Region.BeginLine, currentClass.Region.BeginColumn);
//				int endOffset = editor.Document.PositionToOffset(currentClass.BodyRegion.EndLine, currentClass.BodyRegion.EndColumn);
//
//				editor.Document.Replace(startOffset, endOffset - startOffset, a);
//			}
				
				if (Options.SurroundWithRegion) {
					script.InsertBefore(insertedOverrideMethod, new PreProcessorDirective(PreProcessorDirectiveType.Region, "Equals and GetHashCode implementation"));
				}
				
				if ("Equals".Equals(selectedMethod.Name, StringComparison.Ordinal)) {
					IList<MethodDeclaration> equalsOverrides = CreateEqualsOverrides(currentClass);
					MethodDeclaration defaultOverride = equalsOverrides.First();
					equalsOverrides = equalsOverrides.Skip(1).ToList();
					
					// Insert children of default Equals method into
					foreach (AstNode element in defaultOverride.Body.Children) {
						script.AddTo(insertedOverrideMethod.Body, element.Clone());
					}
					
					// Add other Equals() overrides after our main inserted method
					if (addOtherMethod.IsChecked == true) {
						if (equalsOverrides.Any()) {
							foreach (var equalsMethod in equalsOverrides) {
								AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
								script.InsertAfter(insertedOverrideMethod, equalsMethod);
							}
						}
						
						AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
						script.InsertAfter(insertedOverrideMethod, CreateGetHashCodeOverride(currentClass));
					}
				} else {
					StringBuilder builder = new StringBuilder();
					
					foreach (AstNode element in CreateGetHashCodeOverride(currentClass).Body.Children) {
						script.AddTo(insertedOverrideMethod.Body, element.Clone());
					}
					
					if (addOtherMethod.IsChecked == true) {
						foreach (var equalsMethod in CreateEqualsOverrides(currentClass)) {
							AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
							script.InsertAfter(insertedOverrideMethod, equalsMethod);
						}
					}
				}
				
				if (Options.AddOperatorOverloads) {
					var checkStatements = new[] {
						new IfElseStatement(
							new InvocationExpression(
								new IdentifierExpression("ReferenceEquals"),
								new List<Expression>() { new IdentifierExpression("lhs"), new IdentifierExpression("rhs") }
							),
							new ReturnStatement(new PrimitiveExpression(true))
						),
						new IfElseStatement(
							new BinaryOperatorExpression(
								new InvocationExpression(
									new IdentifierExpression("ReferenceEquals"),
									new List<Expression>() { new IdentifierExpression("lhs"), new PrimitiveExpression(null) }
								),
								BinaryOperatorType.ConditionalOr,
								new InvocationExpression(
									new IdentifierExpression("ReferenceEquals"),
									new List<Expression>() { new IdentifierExpression("rhs"), new PrimitiveExpression(null) }
								)
							),
							new ReturnStatement(new PrimitiveExpression(false))
						)
					};
					
					BlockStatement equalsOpBody = new BlockStatement();
					
					if (currentClass.Kind == TypeKind.Class) {
						foreach (var statement in checkStatements) {
							equalsOpBody.Add(statement);
						}
					}
					
					equalsOpBody.Add(
						new ReturnStatement(
							new InvocationExpression(
								new MemberReferenceExpression(new IdentifierExpression("lhs"), "Equals"),
								new List<Expression>() { new IdentifierExpression("rhs") }
							)
						)
					);

					BlockStatement notEqualsOpBody = new BlockStatement();
					notEqualsOpBody.Add(new ReturnStatement(
						new UnaryOperatorExpression(
							UnaryOperatorType.Not,
							new ParenthesizedExpression(
								new BinaryOperatorExpression(
									new IdentifierExpression("lhs"),
									BinaryOperatorType.Equality,
									new IdentifierExpression("rhs")
								)
							)
						)
					)
					                   );
					
					AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
					script.InsertAfter(insertedOverrideMethod, CreateOperatorOverload(OperatorType.Equality, currentClass, equalsOpBody));
					AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
					script.InsertAfter(insertedOverrideMethod, CreateOperatorOverload(OperatorType.Inequality, currentClass, notEqualsOpBody));
				}
				
				if (Options.SurroundWithRegion) {
					AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
					script.InsertAfter(insertedOverrideMethod, new PreProcessorDirective(PreProcessorDirectiveType.Endregion));
				}
			}
			
			return null;
		}
		
		List<MethodDeclaration> CreateEqualsOverrides(IType currentClass)
		{
			List<MethodDeclaration> methods = new List<MethodDeclaration>();
			
			AstType boolReference = ConvertType(KnownTypeCode.Boolean);
			AstType objectReference = ConvertType(KnownTypeCode.Object);
			
			MethodDeclaration method = new MethodDeclaration {
				Name = "Equals",
				Modifiers = Modifiers.Public | Modifiers.Override,
				ReturnType = boolReference
			};
			method.Parameters.Add(new ParameterDeclaration(objectReference, "obj"));
			method.Body = new BlockStatement();
			
			AstType currentType = ConvertType(currentClass);
			Expression expr = null;
			
			if (currentClass.Kind == TypeKind.Struct) {
				// return obj is CurrentType && Equals((CurrentType)obj);
				expr = new IsExpression() {
					Expression = new IdentifierExpression("obj"),
					Type = currentType.Clone()
				};
				expr = new ParenthesizedExpression(expr);
				expr = new BinaryOperatorExpression(
					expr, BinaryOperatorType.ConditionalAnd,
					new InvocationExpression(
						new IdentifierExpression("Equals"),
						new List<Expression> {
							new CastExpression(currentType.Clone(), new IdentifierExpression("obj"))
						}));
				method.Body.Add(new ReturnStatement(expr));
				
				methods.Add(method);
				
				// IEquatable implementation:
				method = new MethodDeclaration {
					Name = "Equals",
					Modifiers = Modifiers.Public,
					ReturnType = boolReference.Clone()
				};
				method.Parameters.Add(new ParameterDeclaration(currentType, "other"));
				method.Body = new BlockStatement();
			} else {
				method.Body.Add(new VariableDeclarationStatement(
					currentType.Clone(),
					"other",
					new IdentifierExpression("obj").CastAs(currentType.Clone())));
				method.Body.Add(new IfElseStatement(
					new BinaryOperatorExpression(new IdentifierExpression("other"), BinaryOperatorType.Equality, new PrimitiveExpression(null, "null")),
					new ReturnStatement(new PrimitiveExpression(false, "false"))));
			}
			
			expr = null;
			foreach (IField field in currentClass.GetFields()) {
				if (field.IsStatic) continue;
				
				if (expr == null) {
					expr = TestEquality("other", field);
				} else {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.ConditionalAnd,
					                                    TestEquality("other", field));
				}
			}
			
			foreach (IProperty property in currentClass.GetProperties()) {
				if (property.IsStatic || !property.IsAutoImplemented()) continue;
				if (expr == null) {
					expr = TestEquality("other", property);
				} else {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.ConditionalAnd,
					                                    TestEquality("other", property));
				}
			}
			
			method.Body.Add(new ReturnStatement(expr ?? new PrimitiveExpression(true, "true")));
			
			methods.Add(method);
			
			return methods;
		}

		MethodDeclaration CreateGetHashCodeOverride(ITypeDefinition currentClass)
		{
			const string hashCodeVarName = "hashCode";
			AstType intReference = ConvertType(KnownTypeCode.Int32);
			VariableDeclarationStatement hashCodeVar = new VariableDeclarationStatement(intReference, hashCodeVarName, new PrimitiveExpression(0, "0"));
			
			// Create new method declaration (to insert after main inserted method)
			MethodDeclaration getHashCodeMethod = new MethodDeclaration {
				Name = "GetHashCode",
				Modifiers = Modifiers.Public | Modifiers.Override,
				ReturnType = intReference.Clone(),
				Body = new BlockStatement()
			};
			
			getHashCodeMethod.Body.Add(hashCodeVar);
			
			if (currentClass.Fields.Any(f => !f.IsStatic) || currentClass.Properties.Any(p => !p.IsStatic && p.IsAutoImplemented())) {
				bool usePrimeMultiplication = true; // Always leave true for C#?
				BlockStatement hashCalculationBlock = new BlockStatement();
				getHashCodeMethod.Body.Add(new UncheckedStatement(hashCalculationBlock));
//					hashCalculationBlock = getHashCodeMethod.Body;
				
				int fieldIndex = 0;
				
				foreach (IField field in currentClass.Fields) {
					if (field.IsStatic) continue;
					
					AddToBlock(hashCodeVarName, usePrimeMultiplication, hashCalculationBlock, ref fieldIndex, field);
				}
				
				foreach (IProperty property in currentClass.Properties) {
					if (property.IsStatic || !property.IsAutoImplemented()) continue;
					
					AddToBlock(hashCodeVarName, usePrimeMultiplication, hashCalculationBlock, ref fieldIndex, property);
				}
			}
			
			getHashCodeMethod.Body.Add(new ReturnStatement(new IdentifierExpression(hashCodeVarName)));
			return getHashCodeMethod;
		}
		
		void AddToBlock(string hashCodeVarName, bool usePrimeMultiplication, BlockStatement hashCalculationBlock, ref int fieldIndex, IField field)
		{
			Expression expr = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(field.Name), "GetHashCode"));
			if (usePrimeMultiplication) {
				int prime = largePrimes[fieldIndex++ % largePrimes.Length];
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVarName), AssignmentOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(prime, prime.ToString()), BinaryOperatorType.Multiply, expr));
			} else {
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVarName), AssignmentOperatorType.ExclusiveOr, expr);
			}
			if (IsReferenceType(field.ReturnType)) {
				hashCalculationBlock.Add(new IfElseStatement(new BinaryOperatorExpression(new IdentifierExpression(field.Name), BinaryOperatorType.InEquality, new PrimitiveExpression(null, "null")), new ExpressionStatement(expr)));
			} else {
				hashCalculationBlock.Add(new ExpressionStatement(expr));
			}
		}
		
		void AddToBlock(string hashCodeVarName, bool usePrimeMultiplication, BlockStatement hashCalculationBlock, ref int fieldIndex, IProperty property)
		{
			Expression expr = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(property.Name), "GetHashCode"));
			if (usePrimeMultiplication) {
				int prime = largePrimes[fieldIndex++ % largePrimes.Length];
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVarName), AssignmentOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(prime, prime.ToString()), BinaryOperatorType.Multiply, expr));
			} else {
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVarName), AssignmentOperatorType.ExclusiveOr, expr);
			}
			if (IsReferenceType(property.ReturnType)) {
				hashCalculationBlock.Add(new IfElseStatement(new BinaryOperatorExpression(new IdentifierExpression(property.Name), BinaryOperatorType.InEquality, new PrimitiveExpression(null, "null")), new ExpressionStatement(expr)));
			} else {
				hashCalculationBlock.Add(new ExpressionStatement(expr));
			}
		}
		
		OperatorDeclaration CreateOperatorOverload(OperatorType op, ITypeDefinition currentClass, BlockStatement body)
		{
			return new OperatorDeclaration() {
				OperatorType = op,
				ReturnType = ConvertType(KnownTypeCode.Boolean),
				Parameters = {
					new ParameterDeclaration(ConvertType(currentClass), "lhs"),
					new ParameterDeclaration(ConvertType(currentClass), "rhs")
				},
				Modifiers = Modifiers.Public | Modifiers.Static,
				Body = body
			};
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
//			editor.Document.Insert(anchor.Offset, baseCall);
//			editor.Select(anchor.Offset, baseCall.Length);
			
			if (baseCallNode != null) {
				// Insert at least the base call
				MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
				if (insertedOverrideMethod == null)
				{
					// We are not inside of a method declaration
					return;
				}
				using (Script script = refactoringContext.StartScript()) {
					script.AddTo(insertedOverrideMethod.Body, baseCallNode);
				}
			}
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
			
//			editor.Caret.Offset = insertionEndAnchor.Offset;
		}
	}
}
