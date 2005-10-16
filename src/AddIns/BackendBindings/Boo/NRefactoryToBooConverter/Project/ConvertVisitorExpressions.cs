// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using Boo.Lang.Compiler;
using B = Boo.Lang.Compiler.Ast;

namespace NRefactoryToBooConverter
{
	partial class ConvertVisitor
	{
		void ConvertExpressions(IEnumerable input, B.ExpressionCollection output)
		{
			foreach (Expression e in input) {
				B.Expression expr = ConvertExpression(e);
				if (expr != null) {
					output.Add(expr);
				}
			}
		}
		
		B.Expression ConvertExpression(Expression expr)
		{
			if (expr.IsNull)
				return null;
			return (B.Expression)expr.AcceptVisitor(this, null);
		}
		
		B.ReferenceExpression MakeReferenceExpression(string fullName)
		{
			string[] parts = fullName.Split('.');
			B.ReferenceExpression r = new B.ReferenceExpression(lastLexicalInfo, parts[0]);
			for (int i = 1; i < parts.Length; i++)
				r = new B.MemberReferenceExpression(lastLexicalInfo, r, parts[i]);
			return r;
		}
		
		B.MethodInvocationExpression MakeMethodCall(string fullName, params B.Expression[] arguments)
		{
			return new B.MethodInvocationExpression(MakeReferenceExpression(fullName), arguments);
		}
		
		public object Visit(PrimitiveExpression pe, object data)
		{
			object val = pe.Value;
			if (val == null) {
				return new B.NullLiteralExpression(GetLexicalInfo(pe));
			}
			if (val is string) {
				return new B.StringLiteralExpression(GetLexicalInfo(pe), (string)val);
			}
			if (val is char) {
				return new B.CharLiteralExpression(GetLexicalInfo(pe), ((char)val).ToString());
			}
			if (val is bool) {
				return new B.BoolLiteralExpression(GetLexicalInfo(pe), (bool)val);
			}
			if (val is byte) {
				AddWarning(pe, "Converting byte literal to int literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (byte)val, false);
			}
			if (val is short) {
				AddWarning(pe, "Converting short literal to int literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (short)val, false);
			}
			if (val is int) {
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (int)val, false);
			}
			if (val is long) {
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (long)val, true);
			}
			if (val is sbyte) {
				AddWarning(pe, "Converting sbyte literal to int literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (sbyte)val, false);
			}
			if (val is ushort) {
				AddWarning(pe, "Converting ushort literal to int literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (ushort)val, false);
			}
			if (val is uint) {
				AddWarning(pe, "Converting uint literal to int/long literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (uint)val);
			}
			if (val is ulong) {
				AddWarning(pe, "Converting ulong literal to long literal");
				return new B.IntegerLiteralExpression(GetLexicalInfo(pe), (long)((ulong)val), true);
			}
			if (val is float) {
				return new B.DoubleLiteralExpression(GetLexicalInfo(pe), (float)val, true);
			}
			if (val is double) {
				return new B.DoubleLiteralExpression(GetLexicalInfo(pe), (double)val, false);
			}
			AddError(pe, "Unknown primitive literal of type " + val.GetType().FullName);
			return null;
		}
		
		public object Visit(IdentifierExpression identifierExpression, object data)
		{
			return new B.ReferenceExpression(GetLexicalInfo(identifierExpression), identifierExpression.Identifier);
		}
		
		public object Visit(FieldReferenceExpression fre, object data)
		{
			B.Expression target = null;
			if (fre.TargetObject is TypeReferenceExpression) {
				// not typeof, so this is something like int.Parse()
				TypeReference typeRef = ((TypeReferenceExpression)fre.TargetObject).TypeReference;
				if (!typeRef.IsArrayType)
					target = MakeReferenceExpression(typeRef.SystemType);
			}
			if (target == null) {
				target = (B.Expression)fre.TargetObject.AcceptVisitor(this, data);
				if (target == null) return null;
			}
			return new B.MemberReferenceExpression(GetLexicalInfo(fre), target, fre.FieldName);
		}
		
		public object Visit(ClassReferenceExpression classReferenceExpression, object data)
		{
			// VB's MyClass.Method references methods in the CURRENT class, ignoring overrides!!!
			// that is supported neither by C# nor Boo.
			// Most of the time, "Me"="self" should also do the job.
			AddWarning(classReferenceExpression, "Class reference is not supported, replaced with self reference.");
			return new B.SelfLiteralExpression(GetLexicalInfo(classReferenceExpression));
		}
		
		B.BinaryOperatorType ConvertOperator(AssignmentOperatorType op, out bool isInPlace)
		{
			isInPlace = true;
			switch (op) {
				case AssignmentOperatorType.Add:
					return B.BinaryOperatorType.InPlaceAddition;
				case AssignmentOperatorType.Assign:
					return B.BinaryOperatorType.Assign;
				case AssignmentOperatorType.BitwiseAnd:
					return B.BinaryOperatorType.InPlaceBitwiseAnd;
				case AssignmentOperatorType.BitwiseOr:
					return B.BinaryOperatorType.InPlaceBitwiseOr;
				case AssignmentOperatorType.ConcatString:
					return B.BinaryOperatorType.InPlaceAddition;
				case AssignmentOperatorType.Divide:
					return B.BinaryOperatorType.InPlaceDivision;
				case AssignmentOperatorType.DivideInteger:
					return B.BinaryOperatorType.InPlaceDivision;
				case AssignmentOperatorType.ExclusiveOr:
					return B.BinaryOperatorType.InPlaceExclusiveOr;
				case AssignmentOperatorType.Modulus:
					isInPlace = false;
					return B.BinaryOperatorType.Modulus;
				case AssignmentOperatorType.Multiply:
					return B.BinaryOperatorType.InPlaceMultiply;
				case AssignmentOperatorType.Power:
					isInPlace = false;
					return B.BinaryOperatorType.Exponentiation;
				case AssignmentOperatorType.ShiftLeft:
					return B.BinaryOperatorType.InPlaceShiftLeft;
				case AssignmentOperatorType.ShiftRight:
					return B.BinaryOperatorType.InPlaceShiftRight;
				case AssignmentOperatorType.Subtract:
					return B.BinaryOperatorType.InPlaceSubtraction;
				default:
					return B.BinaryOperatorType.None;
			}
		}
		
		public object Visit(AssignmentExpression assignmentExpression, object data)
		{
			B.Expression left = ConvertExpression(assignmentExpression.Left);
			B.Expression right = ConvertExpression(assignmentExpression.Right);
			bool isInPlace;
			B.BinaryOperatorType op = ConvertOperator(assignmentExpression.Op, out isInPlace);
			if (op == B.BinaryOperatorType.None) {
				AddError(assignmentExpression, "Unknown operator.");
				return null;
			}
			if (!isInPlace) {
				// convert L <OP>= R to L = L OP R
				right = new B.BinaryExpression(GetLexicalInfo(assignmentExpression), op, left, right);
				op = B.BinaryOperatorType.Assign;
			}
			return new B.BinaryExpression(GetLexicalInfo(assignmentExpression), op, left, right);
		}
		
		B.BinaryOperatorType ConvertOperator(BinaryOperatorType op)
		{
			switch (op) {
				case BinaryOperatorType.Add:
					return B.BinaryOperatorType.Addition;
					//case BinaryOperatorType.AsCast: special case: converted to AsExpression
				case BinaryOperatorType.BitwiseAnd:
					return B.BinaryOperatorType.BitwiseAnd;
				case BinaryOperatorType.BitwiseOr:
					return B.BinaryOperatorType.BitwiseOr;
				case BinaryOperatorType.Concat:
					return B.BinaryOperatorType.Addition;
				case BinaryOperatorType.Divide:
					return B.BinaryOperatorType.Division;
				case BinaryOperatorType.DivideInteger:
					return B.BinaryOperatorType.Division;
				case BinaryOperatorType.Equality:
					return B.BinaryOperatorType.Equality;
				case BinaryOperatorType.ExclusiveOr:
					return B.BinaryOperatorType.ExclusiveOr;
				case BinaryOperatorType.GreaterThan:
					return B.BinaryOperatorType.GreaterThan;
				case BinaryOperatorType.GreaterThanOrEqual:
					return B.BinaryOperatorType.GreaterThanOrEqual;
				case BinaryOperatorType.InEquality:
					return B.BinaryOperatorType.Inequality;
				case BinaryOperatorType.LessThan:
					return B.BinaryOperatorType.LessThan;
				case BinaryOperatorType.LessThanOrEqual:
					return B.BinaryOperatorType.LessThanOrEqual;
				case BinaryOperatorType.Like:
					return B.BinaryOperatorType.Match;
				case BinaryOperatorType.LogicalAnd:
					return B.BinaryOperatorType.And;
				case BinaryOperatorType.LogicalOr:
					return B.BinaryOperatorType.Or;
				case BinaryOperatorType.Modulus:
					return B.BinaryOperatorType.Modulus;
				case BinaryOperatorType.Multiply:
					return B.BinaryOperatorType.Multiply;
				case BinaryOperatorType.Power:
					return B.BinaryOperatorType.Exponentiation;
				case BinaryOperatorType.ReferenceEquality:
					return B.BinaryOperatorType.ReferenceEquality;
				case BinaryOperatorType.ReferenceInequality:
					return B.BinaryOperatorType.ReferenceInequality;
				case BinaryOperatorType.ShiftLeft:
					return B.BinaryOperatorType.ShiftLeft;
				case BinaryOperatorType.ShiftRight:
					return B.BinaryOperatorType.ShiftRight;
				case BinaryOperatorType.Subtract:
					return B.BinaryOperatorType.Subtraction;
				case BinaryOperatorType.TypeCheck:
					return B.BinaryOperatorType.TypeTest;
				default:
					return B.BinaryOperatorType.None;
			}
		}
		
		
		B.UnaryOperatorType ConvertOperator(UnaryOperatorType op)
		{
			switch (op) {
				case UnaryOperatorType.BitNot:
					return B.UnaryOperatorType.OnesComplement;
				case UnaryOperatorType.Not:
					return B.UnaryOperatorType.LogicalNot;
				case UnaryOperatorType.Decrement:
					return B.UnaryOperatorType.Decrement;
				case UnaryOperatorType.Increment:
					return B.UnaryOperatorType.Increment;
				case UnaryOperatorType.Minus:
					return B.UnaryOperatorType.UnaryNegation;
				case UnaryOperatorType.PostDecrement:
					return B.UnaryOperatorType.PostDecrement;
				case UnaryOperatorType.PostIncrement:
					return B.UnaryOperatorType.PostIncrement;
				default:
					return B.UnaryOperatorType.None;
			}
		}
		
		public object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			B.Expression left = ConvertExpression(binaryOperatorExpression.Left);
			B.Expression right = ConvertExpression(binaryOperatorExpression.Right);
			B.BinaryOperatorType op = ConvertOperator(binaryOperatorExpression.Op);
			if (op == B.BinaryOperatorType.None) {
				if (binaryOperatorExpression.Op == BinaryOperatorType.AsCast) {
					return new B.AsExpression(GetLexicalInfo(binaryOperatorExpression), left, ConvertTypeReference(right));
				} else {
					AddError(binaryOperatorExpression, "Unknown operator.");
					return null;
				}
			}
//			if (binaryOperatorExpression.Op == BinaryOperatorType.DivideInteger) {
//				AddWarning(binaryOperatorExpression, "Integer division converted to normal division.");
//			}
			return new B.BinaryExpression(GetLexicalInfo(binaryOperatorExpression), op, left, right);
		}
		
		public object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			B.Expression expr = ConvertExpression(unaryOperatorExpression.Expression);
			if (unaryOperatorExpression.Op == UnaryOperatorType.Plus)
				return expr;
			B.UnaryOperatorType op = ConvertOperator(unaryOperatorExpression.Op);
			if (op == B.UnaryOperatorType.None) {
				AddError(unaryOperatorExpression, "Unknown operator.");
				return null;
			}
			return new B.UnaryExpression(GetLexicalInfo(unaryOperatorExpression), op, expr);
		}
		
		public object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return ConvertExpression(parenthesizedExpression.Expression);
		}
		
		public object Visit(InvocationExpression ie, object data)
		{
			if (ie.TypeArguments != null && ie.TypeArguments.Count > 0) {
				AddError(ie, "Generic method calls are not supported.");
			}
			B.Expression e = ConvertExpression(ie.TargetObject);
			if (e == null)
				return null;
			if (settings.IsVisualBasic && ie.TargetObject is IdentifierExpression && currentStatement != null) {
				VariableResolver resolver = new VariableResolver(nameComparer);
				TypeReference typeRef = resolver.FindType((ie.TargetObject as IdentifierExpression).Identifier, currentStatement);
				if (typeRef != null && typeRef.IsArrayType) {
					// Visual Basic: indexer expression
					B.SlicingExpression s = new B.SlicingExpression(GetLexicalInfo(ie));
					s.Target = e;
					foreach (Expression expr in ie.Arguments) {
						s.Indices.Add(new B.Slice(ConvertExpression(expr)));
					}
					return s;
				}
			}
			B.MethodInvocationExpression r = new B.MethodInvocationExpression(GetLexicalInfo(ie), e);
			foreach (Expression expr in ie.Arguments) {
				e = ConvertExpression(expr);
				if (e != null) {
					r.Arguments.Add(e);
				}
			}
			return r;
		}
		
		public object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			TypeReference t = objectCreateExpression.CreateType;
			if (t.IsArrayType) {
				throw new ApplicationException("ObjectCreateExpression cannot be called with an ArrayType");
			}
			// HACK: Tricking out event handlers
			if (t.SystemType.EndsWith("EventHandler") && objectCreateExpression.Parameters.Count == 1)
				return ConvertExpression((Expression)objectCreateExpression.Parameters[0]);
			
			B.MethodInvocationExpression mie = new B.MethodInvocationExpression(GetLexicalInfo(objectCreateExpression), MakeReferenceExpression(t.SystemType));
			ConvertExpressions(objectCreateExpression.Parameters, mie.Arguments);
			return mie;
		}
		
		public object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return WrapTypeReference(typeReferenceExpression.TypeReference);
		}
		
		public object Visit(SizeOfExpression sizeOfExpression, object data)
		{
			AddError(sizeOfExpression, "sizeof is not supported.");
			return null;
		}
		
		public object Visit(DefaultValueExpression defaultValueExpression, object data)
		{
			AddError(defaultValueExpression, "default() is not supported.");
			return null;
		}
		
		public object Visit(TypeOfExpression typeOfExpression, object data)
		{
			return new B.TypeofExpression(GetLexicalInfo(typeOfExpression), ConvertTypeReference(typeOfExpression.TypeReference));
		}
		
		public object Visit(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return new B.BinaryExpression(GetLexicalInfo(typeOfIsExpression), B.BinaryOperatorType.TypeTest,
			                              ConvertExpression(typeOfIsExpression.Expression),
			                              WrapTypeReference(typeOfIsExpression.TypeReference));
		}
		
		public object Visit(AddressOfExpression addressOfExpression, object data)
		{
			// Boo can reference methods directly
			return ConvertExpression(addressOfExpression.Expression);
		}
		
		public object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			AddError(pointerReferenceExpression, "Pointers are not supported.");
			return null;
		}
		
		public object Visit(CastExpression castExpression, object data)
		{
			return new B.CastExpression(GetLexicalInfo(castExpression),
			                            ConvertTypeReference(castExpression.CastTo),
			                            ConvertExpression(castExpression.Expression));
		}
		
		public object Visit(StackAllocExpression stackAllocExpression, object data)
		{
			AddError(stackAllocExpression, "StackAlloc is not supported.");
			return null;
		}
		
		public object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return new B.SelfLiteralExpression(GetLexicalInfo(thisReferenceExpression));
		}
		
		public object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new B.SuperLiteralExpression(GetLexicalInfo(baseReferenceExpression));
		}
		
		public object Visit(DirectionExpression directionExpression, object data)
		{
			// boo does not need to specify the direction when calling out/ref methods
			return ConvertExpression(directionExpression.Expression);
		}
		
		public object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (!arrayCreateExpression.ArrayInitializer.IsNull) {
				return arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, data);
			}
			ArrayCreationParameter acp = (ArrayCreationParameter)arrayCreateExpression.Parameters[0];
			string builtInName = (acp.Expressions.Count == 1) ? "array" : "matrix";
			B.MethodInvocationExpression mie = new B.MethodInvocationExpression(GetLexicalInfo(arrayCreateExpression),
			                                                                    MakeReferenceExpression(builtInName));
			if (arrayCreateExpression.Parameters.Count > 1) {
				arrayCreateExpression.CreateType.RankSpecifier = new int[arrayCreateExpression.Parameters.Count - 1];
				mie.Arguments.Add(WrapTypeReference(arrayCreateExpression.CreateType));
			} else {
				mie.Arguments.Add(MakeReferenceExpression(arrayCreateExpression.CreateType.SystemType));
			}
			if (acp.Expressions.Count == 1) {
				mie.Arguments.Add(ConvertExpression((Expression)acp.Expressions[0]));
			} else {
				B.ArrayLiteralExpression dims = new B.ArrayLiteralExpression(GetLexicalInfo(acp));
				ConvertExpressions(acp.Expressions, dims.Items);
				mie.Arguments.Add(dims);
			}
			return mie;
		}
		
		public object Visit(ArrayCreationParameter arrayCreationParameter, object data)
		{
			throw new ApplicationException("Visited ArrayCreationParameter.");
		}
		
		public object Visit(ArrayInitializerExpression aie, object data)
		{
			B.ArrayLiteralExpression dims = new B.ArrayLiteralExpression(GetLexicalInfo(aie));
			ConvertExpressions(aie.CreateExpressions, dims.Items);
			return dims;
		}
		
		public object Visit(IndexerExpression indexerExpression, object data)
		{
			B.SlicingExpression s = new B.SlicingExpression(GetLexicalInfo(indexerExpression));
			s.Target = ConvertExpression(indexerExpression.TargetObject);
			foreach (Expression expr in indexerExpression.Indices) {
				s.Indices.Add(new B.Slice(ConvertExpression(expr)));
			}
			return s;
		}
		
		public object Visit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			B.CallableBlockExpression cbe = new B.CallableBlockExpression(GetLexicalInfo(anonymousMethodExpression));
			cbe.EndSourceLocation = GetLocation(anonymousMethodExpression.EndLocation);
			cbe.Body = ConvertBlock(anonymousMethodExpression.Body);
			ConvertParameters(anonymousMethodExpression.Parameters, cbe.Parameters);
			return cbe;
		}
		
		public object Visit(ConditionalExpression conditionalExpression, object data)
		{
			B.TernaryExpression te = new B.TernaryExpression(GetLexicalInfo(conditionalExpression));
			te.Condition = ConvertExpression(conditionalExpression.Condition);
			te.TrueValue = ConvertExpression(conditionalExpression.TrueExpression);
			te.FalseValue = ConvertExpression(conditionalExpression.FalseExpression);
			return te;
		}
		
		public object Visit(CheckedExpression checkedExpression, object data)
		{
			AddError(checkedExpression, "Using 'checked' inside an expression is not supported by boo, " +
			         "use the checked {} block instead.");
			return null;
		}
		
		public object Visit(UncheckedExpression uncheckedExpression, object data)
		{
			AddError(uncheckedExpression, "Using 'unchecked' inside an expression is not supported by boo, " +
			         "use the unchecked {} block instead.");
			return null;
		}
	}
}
