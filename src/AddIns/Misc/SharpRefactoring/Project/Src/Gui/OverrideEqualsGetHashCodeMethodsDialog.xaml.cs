// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for OverrideEqualsGetHashCodeMethodsDialog.xaml
	/// </summary>
	public partial class OverrideEqualsGetHashCodeMethodsDialog : AbstractInlineRefactorDialog
	{
		IClass selectedClass;
		ITextAnchor startAnchor;
		IMethod selectedMethod;
		string baseCall;
		
		public OverrideEqualsGetHashCodeMethodsDialog(InsertionContext context, ITextEditor editor, ITextAnchor startAnchor, ITextAnchor endAnchor,
		                                              ITextAnchor insertionPosition, IClass selectedClass, IMethod selectedMethod, string baseCall)
			: base(context, editor, insertionPosition)
		{
			if (selectedClass == null)
				throw new ArgumentNullException("selectedClass");
			
			InitializeComponent();
			
			this.selectedClass = selectedClass;
			this.startAnchor = startAnchor;
			this.insertionEndAnchor = endAnchor;
			this.selectedMethod = selectedMethod;
			this.baseCall = baseCall;
			
			addIEquatable.Content = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.OverrideEqualsGetHashCodeMethods.AddInterface}"),
			                                      "IEquatable<" + selectedClass.Name + ">");
			
			string otherMethod = selectedMethod.Name == "Equals" ? "GetHashCode" : "Equals";
			
			addOtherMethod.Content = StringParser.Parse("${res:AddIns.SharpRefactoring.OverrideEqualsGetHashCodeMethods.AddOtherMethod}", new StringTagPair("otherMethod", otherMethod));
			
			addIEquatable.IsEnabled = !selectedClass.BaseTypes.Any(
				type => {
					if (!type.IsGenericReturnType)
						return false;
					var genericType = type.CastToGenericReturnType();
					var boundTo = genericType.TypeParameter.BoundTo;
					if (boundTo == null)
						return false;
					return boundTo.Name == selectedClass.Name;
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
		
		static bool IsValueType(IReturnType type)
		{
			IClass c = type.GetUnderlyingClass();
			return c != null && (c.ClassType == Dom.ClassType.Struct || c.ClassType == Dom.ClassType.Enum);
		}
		
		static bool CanCompareEqualityWithOperator(IReturnType type)
		{
			// return true for value types except float and double
			// return false for reference types except string.
			IClass c = type.GetUnderlyingClass();
			return c != null
				&& c.FullyQualifiedName != "System.Single"
				&& c.FullyQualifiedName != "System.Double"
				&& (c.ClassType == Dom.ClassType.Struct
				    || c.ClassType == Dom.ClassType.Enum
				    || c.FullyQualifiedName == "System.String");
		}
		
		static Expression TestEquality(string other, IField field)
		{
			if (CanCompareEqualityWithOperator(field.ReturnType)) {
				return new BinaryOperatorExpression(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name),
				                                    BinaryOperatorType.Equality,
				                                    new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(new TypeReference("System.Object", true)), "Equals")
				);
				ie.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name));
				ie.Arguments.Add(new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
				return ie;
			}
		}
		
		static Expression TestEquality(string other, IProperty property)
		{
			if (CanCompareEqualityWithOperator(property.ReturnType)) {
				return new BinaryOperatorExpression(new MemberReferenceExpression(new ThisReferenceExpression(), property.Name),
				                                    BinaryOperatorType.Equality,
				                                    new MemberReferenceExpression(new IdentifierExpression(other), property.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(new TypeReference("System.Object", true)), "Equals")
				);
				ie.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), property.Name));
				ie.Arguments.Add(new MemberReferenceExpression(new IdentifierExpression(other), property.Name));
				return ie;
			}
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			StringBuilder code = new StringBuilder();
			
			var line = editor.Document.GetLineForOffset(startAnchor.Offset);
			
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset);
			
			CodeGenerator generator = language.CodeGenerator;
			
			if (Options.AddIEquatableInterface) {
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
			}
			
			if (Options.SurroundWithRegion) {
				editor.Document.InsertNormalized(startAnchor.Offset, "#region Equals and GetHashCode implementation\n" + indent);
			}
			
			string codeForMethodBody;
			
			if ("Equals".Equals(selectedMethod.Name, StringComparison.Ordinal)) {
				IList<MethodDeclaration> equalsOverrides = CreateEqualsOverrides(currentClass);
				MethodDeclaration defaultOverride = equalsOverrides.First();
				equalsOverrides = equalsOverrides.Skip(1).ToList();
				
				StringBuilder builder = new StringBuilder();
				
				foreach (AbstractNode element in defaultOverride.Body.Children.OfType<AbstractNode>()) {
					builder.Append(language.CodeGenerator.GenerateCode(element, indent + "\t"));
				}
				
				codeForMethodBody = builder.ToString().Trim();
				
				if (addOtherMethod.IsChecked == true) {
					if (equalsOverrides.Any())
						code.Append(indent + "\n" + string.Join("\n", equalsOverrides.Select(item => generator.GenerateCode(item, indent))));
					code.Append(indent + "\n" + generator.GenerateCode(CreateGetHashCodeOverride(currentClass), indent));
				}
			} else {
				StringBuilder builder = new StringBuilder();
				
				foreach (AbstractNode element in CreateGetHashCodeOverride(currentClass).Body.Children.OfType<AbstractNode>()) {
					builder.Append(language.CodeGenerator.GenerateCode(element, indent + "\t"));
				}
				
				codeForMethodBody = builder.ToString().Trim();
				
				if (addOtherMethod.IsChecked == true)
					code.Append(indent + "\n" + string.Join("\n", CreateEqualsOverrides(currentClass).Select(item => generator.GenerateCode(item, indent))));
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
							BinaryOperatorType.LogicalOr,
							new InvocationExpression(
								new IdentifierExpression("ReferenceEquals"),
								new List<Expression>() { new IdentifierExpression("rhs"), new PrimitiveExpression(null) }
							)
						),
						new ReturnStatement(new PrimitiveExpression(false))
					)
				};
				
				BlockStatement equalsOpBody = new BlockStatement() {
					Children = {
						new ReturnStatement(
							new InvocationExpression(
								new MemberReferenceExpression(new IdentifierExpression("lhs"), "Equals"),
								new List<Expression>() { new IdentifierExpression("rhs") }
							)
						)
					}
				};
				
				if (currentClass.ClassType == Dom.ClassType.Class) {
					equalsOpBody.Children.InsertRange(0, checkStatements);
				}

				BlockStatement notEqualsOpBody = new BlockStatement() {
					Children = {
						new ReturnStatement(
							new UnaryOperatorExpression(
								new ParenthesizedExpression(
									new BinaryOperatorExpression(
										new IdentifierExpression("lhs"),
										BinaryOperatorType.Equality,
										new IdentifierExpression("rhs")
									)
								),
								UnaryOperatorType.Not
							)
						)
					}
				};
				
				code.Append(indent + "\n" + generator.GenerateCode(CreateOperatorOverload(OverloadableOperatorType.Equality, currentClass, equalsOpBody), indent));
				code.Append(indent + "\n" + generator.GenerateCode(CreateOperatorOverload(OverloadableOperatorType.InEquality, currentClass, notEqualsOpBody), indent));
			}
			
			if (Options.SurroundWithRegion) {
				code.AppendLine(indent + "#endregion");
			}
			
			editor.Document.InsertNormalized(insertionEndAnchor.Offset, code.ToString());
			
			return codeForMethodBody;
		}
		
		List<MethodDeclaration> CreateEqualsOverrides(IClass currentClass)
		{
			List<MethodDeclaration> methods = new List<MethodDeclaration>();
			
			TypeReference boolReference = new TypeReference("System.Boolean", true);
			TypeReference objectReference = new TypeReference("System.Object", true);
			
			MethodDeclaration method = new MethodDeclaration {
				Name = "Equals",
				Modifier = Modifiers.Public | Modifiers.Override,
				TypeReference = boolReference
			};
			method.Parameters.Add(new ParameterDeclarationExpression(objectReference, "obj"));
			method.Body = new BlockStatement();
			
			TypeReference currentType = ConvertType(currentClass.DefaultReturnType);
			
			Expression expr = null;
			
			if (currentClass.ClassType == Dom.ClassType.Struct) {
				// return obj is CurrentType && Equals((CurrentType)obj);
				expr = new TypeOfIsExpression(new IdentifierExpression("obj"), currentType);
				expr = new ParenthesizedExpression(expr);
				expr = new BinaryOperatorExpression(
					expr, BinaryOperatorType.LogicalAnd,
					new InvocationExpression(
						new IdentifierExpression("Equals"),
						new List<Expression> {
							new CastExpression(currentType, new IdentifierExpression("obj"), CastType.Cast)
						}));
				method.Body.AddChild(new ReturnStatement(expr));
				
				methods.Add(method);
				
				// IEquatable implementation:
				method = new MethodDeclaration {
					Name = "Equals",
					Modifier = Modifiers.Public,
					TypeReference = boolReference
				};
				method.Parameters.Add(new ParameterDeclarationExpression(currentType, "other"));
				method.Body = new BlockStatement();
			} else {
				method.Body.AddChild(new LocalVariableDeclaration(new VariableDeclaration(
					"other",
					new CastExpression(currentType, new IdentifierExpression("obj"), CastType.TryCast),
					currentType)));
				method.Body.AddChild(new IfElseStatement(
					new BinaryOperatorExpression(new IdentifierExpression("other"), BinaryOperatorType.ReferenceEquality, new PrimitiveExpression(null, "null")),
					new ReturnStatement(new PrimitiveExpression(false, "false"))));
				
//				expr = new BinaryOperatorExpression(new ThisReferenceExpression(),
//				                                    BinaryOperatorType.ReferenceEquality,
//				                                    new IdentifierExpression("obj"));
//				method.Body.AddChild(new IfElseStatement(expr, new ReturnStatement(new PrimitiveExpression(true, "true"))));
			}
			
			
			expr = null;
			foreach (IField field in currentClass.Fields) {
				if (field.IsStatic) continue;
				
				if (expr == null) {
					expr = TestEquality("other", field);
				} else {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.LogicalAnd,
					                                    TestEquality("other", field));
				}
			}
			
			foreach (IProperty property in currentClass.Properties) {
				if (property.IsStatic || !property.IsAutoImplemented()) continue;
				if (expr == null) {
					expr = TestEquality("other", property);
				} else {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.LogicalAnd,
					                                    TestEquality("other", property));
				}
			}
			
			method.Body.AddChild(new ReturnStatement(expr ?? new PrimitiveExpression(true, "true")));
			
			methods.Add(method);
			
			return methods;
		}

		MethodDeclaration CreateGetHashCodeOverride(IClass currentClass)
		{
			TypeReference intReference = new TypeReference("System.Int32", true);
			VariableDeclaration hashCodeVar = new VariableDeclaration("hashCode", new PrimitiveExpression(0, "0"), intReference);
			
			MethodDeclaration getHashCodeMethod = new MethodDeclaration {
				Name = "GetHashCode",
				Modifier = Modifiers.Public | Modifiers.Override,
				TypeReference = intReference,
				Body = new BlockStatement()
			};
			
			getHashCodeMethod.Body.AddChild(new LocalVariableDeclaration(hashCodeVar));
			
			if (currentClass.Fields.Any(f => !f.IsStatic) || currentClass.Properties.Any(p => !p.IsStatic && p.IsAutoImplemented())) {
				bool usePrimeMultiplication = currentClass.ProjectContent.Language == LanguageProperties.CSharp;
				BlockStatement hashCalculationBlock;
				
				if (usePrimeMultiplication) {
					hashCalculationBlock = new BlockStatement();
					getHashCodeMethod.Body.AddChild(new UncheckedStatement(hashCalculationBlock));
				} else {
					hashCalculationBlock = getHashCodeMethod.Body;
				}
				
				int fieldIndex = 0;
				
				foreach (IField field in currentClass.Fields) {
					if (field.IsStatic) continue;
					
					AddToBlock(hashCodeVar, getHashCodeMethod, usePrimeMultiplication, hashCalculationBlock, ref fieldIndex, field);
				}
				
				foreach (IProperty property in currentClass.Properties) {
					if (property.IsStatic || !property.IsAutoImplemented()) continue;
					
					AddToBlock(hashCodeVar, getHashCodeMethod, usePrimeMultiplication, hashCalculationBlock, ref fieldIndex, property);
				}
			}
			
			getHashCodeMethod.Body.AddChild(new ReturnStatement(new IdentifierExpression(hashCodeVar.Name)));
			return getHashCodeMethod;
		}

		void AddToBlock(VariableDeclaration hashCodeVar, MethodDeclaration getHashCodeMethod, bool usePrimeMultiplication, BlockStatement hashCalculationBlock, ref int fieldIndex, IField field)
		{
			Expression expr = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(field.Name), "GetHashCode"));
			if (usePrimeMultiplication) {
				int prime = largePrimes[fieldIndex++ % largePrimes.Length];
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVar.Name), AssignmentOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(prime, prime.ToString()), BinaryOperatorType.Multiply, expr));
			} else {
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVar.Name), AssignmentOperatorType.ExclusiveOr, expr);
			}
			if (IsValueType(field.ReturnType)) {
				hashCalculationBlock.AddChild(new ExpressionStatement(expr));
			} else {
				hashCalculationBlock.AddChild(new IfElseStatement(new BinaryOperatorExpression(new IdentifierExpression(field.Name), BinaryOperatorType.ReferenceInequality, new PrimitiveExpression(null, "null")), new ExpressionStatement(expr)));
			}
		}
		
		void AddToBlock(VariableDeclaration hashCodeVar, MethodDeclaration getHashCodeMethod, bool usePrimeMultiplication, BlockStatement hashCalculationBlock, ref int fieldIndex, IProperty property)
		{
			Expression expr = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(property.Name), "GetHashCode"));
			if (usePrimeMultiplication) {
				int prime = largePrimes[fieldIndex++ % largePrimes.Length];
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVar.Name), AssignmentOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(prime, prime.ToString()), BinaryOperatorType.Multiply, expr));
			} else {
				expr = new AssignmentExpression(new IdentifierExpression(hashCodeVar.Name), AssignmentOperatorType.ExclusiveOr, expr);
			}
			if (IsValueType(property.ReturnType)) {
				hashCalculationBlock.AddChild(new ExpressionStatement(expr));
			} else {
				hashCalculationBlock.AddChild(new IfElseStatement(new BinaryOperatorExpression(new IdentifierExpression(property.Name), BinaryOperatorType.ReferenceInequality, new PrimitiveExpression(null, "null")), new ExpressionStatement(expr)));
			}
		}
		
		OperatorDeclaration CreateOperatorOverload(OverloadableOperatorType op, IClass currentClass, BlockStatement body)
		{
			return new OperatorDeclaration() {
				OverloadableOperator = op,
				TypeReference = new TypeReference("System.Boolean", true),
				Parameters = {
					new ParameterDeclarationExpression(ConvertType(currentClass.DefaultReturnType), "lhs"),
					new ParameterDeclarationExpression(ConvertType(currentClass.DefaultReturnType), "rhs")
				},
				Modifier = Modifiers.Public | Modifiers.Static,
				Body = body
			};
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
			editor.Document.Insert(anchor.Offset, baseCall);
			editor.Select(anchor.Offset, baseCall.Length);
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
			
			editor.Caret.Offset = insertionEndAnchor.Offset;
		}
	}
}
