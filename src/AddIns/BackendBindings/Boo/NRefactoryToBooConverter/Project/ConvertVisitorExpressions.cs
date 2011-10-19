// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.NRefactory.Ast;
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
		
		B.Expression MakeReferenceExpression(TypeReference typeRef)
		{
			if (typeRef.IsArrayType)
				return new B.TypeofExpression(GetLexicalInfo(typeRef), ConvertTypeReference(typeRef));
			B.SimpleTypeReference t = (B.SimpleTypeReference)ConvertTypeReference(typeRef);
			B.ReferenceExpression r = MakeReferenceExpression(t.Name);
			if (t is B.GenericTypeReference) {
				B.GenericReferenceExpression gr = new B.GenericReferenceExpression(GetLexicalInfo(typeRef));
				gr.Target = r;
				foreach (B.TypeReference tr in ((B.GenericTypeReference)t).GenericArguments) {
					gr.GenericArguments.Add(tr);
				}
				return gr;
			} else {
				return r;
			}
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
		
		public object VisitPrimitiveExpression(PrimitiveExpression pe, object data)
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
			if (val is decimal) {
				AddWarning(pe, "Converting decimal literal to double literal");
				return new B.DoubleLiteralExpression(GetLexicalInfo(pe), (double)(decimal)val);
			}
			AddError(pe, "Unknown primitive literal of type " + val.GetType().FullName);
			return null;
		}
		
		public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
		{
			AddError(namedArgumentExpression, "Named arguments are not supported in boo. (argument name was " + namedArgumentExpression.Name + ")");
			return namedArgumentExpression.Expression.AcceptVisitor(this, data);
		}
		
		public object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			return new B.ReferenceExpression(GetLexicalInfo(identifierExpression), identifierExpression.Identifier);
		}
		
		public object VisitMemberReferenceExpression(MemberReferenceExpression mre, object data)
		{
			B.Expression target = null;
			if (mre.TargetObject is TypeReferenceExpression) {
				// not typeof, so this is something like int.Parse() or Class<string>.StaticMethod
				TypeReference typeRef = ((TypeReferenceExpression)mre.TargetObject).TypeReference;
				if (!typeRef.IsArrayType)
					target = MakeReferenceExpression(typeRef);
			}
			if (target == null) {
				target = (B.Expression)mre.TargetObject.AcceptVisitor(this, data);
				if (target == null) return null;
			}
			return new B.MemberReferenceExpression(GetLexicalInfo(mre), target, mre.MemberName);
		}
		
		public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
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
		
		public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
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
				case BinaryOperatorType.NullCoalescing:
					return B.BinaryOperatorType.Or;
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
		
		public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			B.Expression left = ConvertExpression(binaryOperatorExpression.Left);
			B.Expression right = ConvertExpression(binaryOperatorExpression.Right);
			B.BinaryOperatorType op = ConvertOperator(binaryOperatorExpression.Op);
			if (op == B.BinaryOperatorType.None) {
				AddError(binaryOperatorExpression, "Unknown operator.");
				return null;
			}
//			if (binaryOperatorExpression.Op == BinaryOperatorType.DivideInteger) {
//				AddWarning(binaryOperatorExpression, "Integer division converted to normal division.");
//			}
			return new B.BinaryExpression(GetLexicalInfo(binaryOperatorExpression), op, left, right);
		}
		
		public object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
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
		
		public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return ConvertExpression(parenthesizedExpression.Expression);
		}
		
		public object VisitInvocationExpression(InvocationExpression ie, object data)
		{
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
		
		public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
		{
			TypeReference t = objectCreateExpression.CreateType;
			if (t.IsArrayType) {
				throw new ApplicationException("ObjectCreateExpression cannot be called with an ArrayType");
			}
			// HACK: Tricking out event handlers
			if (t.Type.EndsWith("EventHandler") && objectCreateExpression.Parameters.Count == 1)
				return ConvertExpression((Expression)objectCreateExpression.Parameters[0]);
			
			B.MethodInvocationExpression mie = new B.MethodInvocationExpression(GetLexicalInfo(objectCreateExpression), MakeReferenceExpression(t));
			ConvertExpressions(objectCreateExpression.Parameters, mie.Arguments);
			return mie;
		}
		
		public object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return MakeReferenceExpression(typeReferenceExpression.TypeReference);
		}
		
		public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
		{
			AddError(sizeOfExpression, "sizeof is not supported.");
			return null;
		}
		
		public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
		{
			AddError(defaultValueExpression, "default() is not supported.");
			return null;
		}
		
		public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
		{
			return new B.TypeofExpression(GetLexicalInfo(typeOfExpression), ConvertTypeReference(typeOfExpression.TypeReference));
		}
		
		public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
		{
			return new B.BinaryExpression(GetLexicalInfo(typeOfIsExpression), B.BinaryOperatorType.TypeTest,
			                              ConvertExpression(typeOfIsExpression.Expression),
			                              new B.TypeofExpression(GetLexicalInfo(typeOfIsExpression), ConvertTypeReference(typeOfIsExpression.TypeReference)));
		}
		
		public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
		{
			// Boo can reference methods directly
			return ConvertExpression(addressOfExpression.Expression);
		}
		
		public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			AddError(pointerReferenceExpression, "Pointers are not supported.");
			return null;
		}
		
		public object VisitCastExpression(CastExpression castExpression, object data)
		{
			switch (castExpression.CastType) {
				case CastType.Cast:
				case CastType.Conversion:
				case CastType.PrimitiveConversion:
					return new B.CastExpression(GetLexicalInfo(castExpression),
					                            ConvertExpression(castExpression.Expression),
					                            ConvertTypeReference(castExpression.CastTo));
				case CastType.TryCast:
					return new B.TryCastExpression(GetLexicalInfo(castExpression),
					                               ConvertExpression(castExpression.Expression),
					                               ConvertTypeReference(castExpression.CastTo));
				default:
					AddError(castExpression, "Unknown cast: " + castExpression);
					return null;
			}
		}
		
		public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
		{
			AddError(stackAllocExpression, "StackAlloc is not supported.");
			return null;
		}
		
		public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return new B.SelfLiteralExpression(GetLexicalInfo(thisReferenceExpression));
		}
		
		public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new B.SuperLiteralExpression(GetLexicalInfo(baseReferenceExpression));
		}
		
		public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
		{
			// boo does not need to specify the direction when calling out/ref methods
			return ConvertExpression(directionExpression.Expression);
		}
		
		public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (!arrayCreateExpression.ArrayInitializer.IsNull) {
				B.ArrayLiteralExpression ale = ConvertArrayLiteralExpression(arrayCreateExpression.ArrayInitializer);
				if (!arrayCreateExpression.IsImplicitlyTyped) {
					ale.Type = (B.ArrayTypeReference)ConvertTypeReference(arrayCreateExpression.CreateType);
				}
				return ale;
			}
			string builtInName = (arrayCreateExpression.Arguments.Count > 1) ? "matrix" : "array";
			B.MethodInvocationExpression mie = new B.MethodInvocationExpression(GetLexicalInfo(arrayCreateExpression),
			                                                                    MakeReferenceExpression(builtInName));
			TypeReference elementType = arrayCreateExpression.CreateType.Clone();
			int[] newRank = new int[elementType.RankSpecifier.Length - 1];
			for (int i = 0; i < newRank.Length; i++)
				newRank[i] = elementType.RankSpecifier[i + 1];
			elementType.RankSpecifier = newRank;
			mie.Arguments.Add(MakeReferenceExpression(elementType));
			ConvertExpressions(arrayCreateExpression.Arguments, mie.Arguments);
			return mie;
		}
		
		public object VisitCollectionInitializerExpression(CollectionInitializerExpression aie, object data)
		{
			return ConvertArrayLiteralExpression(aie);
		}
		
		B.ArrayLiteralExpression ConvertArrayLiteralExpression(CollectionInitializerExpression aie)
		{
			B.ArrayLiteralExpression dims = new B.ArrayLiteralExpression(GetLexicalInfo(aie));
			ConvertExpressions(aie.CreateExpressions, dims.Items);
			return dims;
		}
		
		public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
		{
			B.SlicingExpression s = new B.SlicingExpression(GetLexicalInfo(indexerExpression));
			s.Target = ConvertExpression(indexerExpression.TargetObject);
			foreach (Expression expr in indexerExpression.Indexes) {
				s.Indices.Add(new B.Slice(ConvertExpression(expr)));
			}
			return s;
		}
		
		public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			B.BlockExpression cbe = new B.BlockExpression(GetLexicalInfo(anonymousMethodExpression));
			cbe.EndSourceLocation = GetLocation(anonymousMethodExpression.EndLocation);
			cbe.Body = ConvertBlock(anonymousMethodExpression.Body);
			ConvertParameters(anonymousMethodExpression.Parameters, cbe.Parameters);
			return cbe;
		}
		
		public object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			B.BlockExpression cbe = new B.BlockExpression(GetLexicalInfo(lambdaExpression));
			cbe.EndSourceLocation = GetLocation(lambdaExpression.EndLocation);
			if (lambdaExpression.StatementBody.IsNull) {
				cbe.Body = new B.Block();
				cbe.Body.Add(new B.ReturnStatement(ConvertExpression(lambdaExpression.ExpressionBody)));
			} else {
				cbe.Body = ConvertBlock(lambdaExpression.StatementBody);
			}
			ConvertParameters(lambdaExpression.Parameters, cbe.Parameters);
			return cbe;
		}
		
		public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
		{
			B.ConditionalExpression te = new B.ConditionalExpression(GetLexicalInfo(conditionalExpression));
			te.Condition = ConvertExpression(conditionalExpression.Condition);
			te.TrueValue = ConvertExpression(conditionalExpression.TrueExpression);
			te.FalseValue = ConvertExpression(conditionalExpression.FalseExpression);
			return te;
		}
		
		public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
		{
			AddError(checkedExpression, "Using 'checked' inside an expression is not supported by boo, " +
			         "use the checked {} block instead.");
			return MakeMethodCall("checked", ConvertExpression(checkedExpression.Expression));
		}
		
		public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
		{
			AddError(uncheckedExpression, "Using 'unchecked' inside an expression is not supported by boo, " +
			         "use the unchecked {} block instead.");
			return MakeMethodCall("unchecked", ConvertExpression(uncheckedExpression.Expression));
		}
	}
}
