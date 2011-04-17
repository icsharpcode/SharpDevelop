// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.CSharp.Resolver.ConstantValues
{
	// Contains representations for constant C# expressions.
	// We use these instead of storing the full AST to reduce the memory usage.
	
	// The type system's SimpleConstantValue is used to represent PrimitiveExpressions.
	
	public abstract class ConstantExpression : Immutable, IConstantValue
	{
		public abstract ResolveResult Resolve(CSharpResolver resolver);
		
		public static ResolveResult Resolve(IConstantValue constantValue, CSharpResolver resolver)
		{
			ConstantExpression expr = constantValue as ConstantExpression;
			if (expr != null)
				return expr.Resolve(resolver);
			else
				return new ConstantResolveResult(constantValue.GetValueType(resolver.Context), constantValue.GetValue(resolver.Context));
		}
		
		public IType GetValueType(ITypeResolveContext context)
		{
			return Resolve(new CSharpResolver(context)).Type;
		}
		
		public object GetValue(ITypeResolveContext context)
		{
			return Resolve(new CSharpResolver(context)).ConstantValue;
		}
	}
	
	public class ConstantCast : ConstantExpression, ISupportsInterning
	{
		ITypeReference targetType;
		IConstantValue expression;
		readonly bool checkForOverflow;
		
		public ConstantCast(ITypeReference targetType, IConstantValue expression, bool checkForOverflow)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (expression == null)
				throw new ArgumentNullException("expression");
			this.targetType = targetType;
			this.expression = expression;
			this.checkForOverflow = checkForOverflow;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			ResolveResult rr = Resolve(expression, resolver);
			resolver.CheckForOverflow = checkForOverflow;
			return resolver.ResolveCast(targetType.Resolve(resolver.Context), rr);
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			targetType = provider.Intern(targetType);
			expression = provider.Intern(expression);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return targetType.GetHashCode() + expression.GetHashCode() * 1018829 + (checkForOverflow ? 614811 : 7125912);
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantCast cast = other as ConstantCast;
			return cast != null
				&& this.targetType == cast.targetType && this.expression == cast.expression
				&& this.checkForOverflow == cast.checkForOverflow;
		}
	}
	
	public class ConstantIdentifierReference : ConstantExpression, ISupportsInterning
	{
		string identifier;
		IList<ITypeReference> typeArguments;
		
		public ConstantIdentifierReference(string identifier, IList<ITypeReference> typeArguments = null)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			this.identifier = identifier;
			this.typeArguments = typeArguments;
		}
		
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			return resolver.ResolveSimpleName(identifier, ResolveTypes(resolver, typeArguments));
		}
		
		internal static IList<IType> ResolveTypes(CSharpResolver resolver, IList<ITypeReference> typeArguments)
		{
			if (typeArguments == null)
				return EmptyList<IType>.Instance;
			IType[] types = new IType[typeArguments.Count];
			for (int i = 0; i < types.Length; i++) {
				types[i] = typeArguments[i].Resolve(resolver.Context);
			}
			return types;
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			identifier = provider.Intern(identifier);
			typeArguments = provider.InternList(typeArguments);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				int hashCode = identifier.GetHashCode();
				if (typeArguments != null)
					hashCode ^= typeArguments.GetHashCode();
				return hashCode;
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantIdentifierReference cir = other as ConstantIdentifierReference;
			return cir != null &&
				this.identifier == cir.identifier && this.typeArguments == cir.typeArguments;
		}
	}
	
	public class ConstantMemberReference : ConstantExpression, ISupportsInterning
	{
		ITypeReference targetType;
		IConstantValue targetExpression;
		string memberName;
		IList<ITypeReference> typeArguments;
		
		public ConstantMemberReference(ITypeReference targetType, string memberName, IList<ITypeReference> typeArguments = null)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (memberName == null)
				throw new ArgumentNullException("memberName");
			this.targetType = targetType;
			this.memberName = memberName;
			this.typeArguments = typeArguments;
		}
		
		public ConstantMemberReference(IConstantValue targetExpression, string memberName, IList<ITypeReference> typeArguments = null)
		{
			if (targetExpression == null)
				throw new ArgumentNullException("targetExpression");
			if (memberName == null)
				throw new ArgumentNullException("memberName");
			this.targetExpression = targetExpression;
			this.memberName = memberName;
			this.typeArguments = typeArguments;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			ResolveResult rr;
			if (targetType != null)
				rr = new TypeResolveResult(targetType.Resolve(resolver.Context));
			else
				rr = Resolve(targetExpression, resolver);
			return resolver.ResolveMemberAccess(rr, memberName, ConstantIdentifierReference.ResolveTypes(resolver, typeArguments));
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			targetType = provider.Intern(targetType);
			targetExpression = provider.Intern(targetExpression);
			memberName = provider.Intern(memberName);
			typeArguments = provider.InternList(typeArguments);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				int hashCode;
				if (targetType != null)
					hashCode = targetType.GetHashCode();
				else
					hashCode = targetExpression.GetHashCode();
				hashCode ^= memberName.GetHashCode();
				if (typeArguments != null)
					hashCode ^= typeArguments.GetHashCode();
				return hashCode;
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantMemberReference cmr = other as ConstantMemberReference;
			return cmr != null
				&& this.targetType == cmr.targetType && this.targetExpression == cmr.targetExpression
				&& this.memberName == cmr.memberName && this.typeArguments == cmr.typeArguments;
		}
	}
	
	public class ConstantDefaultValue : ConstantExpression, ISupportsInterning
	{
		ITypeReference type;
		
		public ConstantDefaultValue(ITypeReference type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.type = type;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			return resolver.ResolveDefaultValue(type.Resolve(resolver.Context));
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			type = provider.Intern(type);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return type.GetHashCode();
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantDefaultValue o = other as ConstantDefaultValue;
			return o != null && this.type == o.type;
		}
	}
	
	public class ConstantUnaryOperator : ConstantExpression, ISupportsInterning
	{
		UnaryOperatorType operatorType;
		IConstantValue expression;
		bool checkForOverflow;
		
		public ConstantUnaryOperator(UnaryOperatorType operatorType, IConstantValue expression, bool checkForOverflow)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			this.operatorType = operatorType;
			this.expression = expression;
			this.checkForOverflow = checkForOverflow;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			ResolveResult rr = Resolve(expression, resolver);
			resolver.CheckForOverflow = checkForOverflow;
			return resolver.ResolveUnaryOperator(operatorType, rr);
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			expression = provider.Intern(expression);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return expression.GetHashCode() * 811 + operatorType.GetHashCode() + (checkForOverflow ? 1717211 : 12751265);
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantUnaryOperator uop = other as ConstantUnaryOperator;
			return uop != null
				&& this.operatorType == uop.operatorType
				&& this.expression == uop.expression
				&& this.checkForOverflow == uop.checkForOverflow;
		}
	}

	public class ConstantBinaryOperator : ConstantExpression, ISupportsInterning
	{
		IConstantValue left;
		BinaryOperatorType operatorType;
		IConstantValue right;
		bool checkForOverflow;
		
		public ConstantBinaryOperator(IConstantValue left, BinaryOperatorType operatorType, IConstantValue right, bool checkForOverflow)
		{
			if (left == null)
				throw new ArgumentNullException("left");
			if (right == null)
				throw new ArgumentNullException("right");
			this.left = left;
			this.operatorType = operatorType;
			this.right = right;
			this.checkForOverflow = checkForOverflow;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			ResolveResult lhs = Resolve(left, resolver);
			ResolveResult rhs = Resolve(right, resolver);
			resolver.CheckForOverflow = checkForOverflow;
			return resolver.ResolveBinaryOperator(operatorType, lhs, rhs);
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			left = provider.Intern(left);
			right = provider.Intern(right);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return left.GetHashCode() * 811 + operatorType.GetHashCode()
					+ right.GetHashCode() * 91781 + (checkForOverflow ? 1261561 : 174811);
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantBinaryOperator bop = other as ConstantBinaryOperator;
			return bop != null
				&& this.operatorType == bop.operatorType
				&& this.left == bop.left && this.right == bop.right
				&& this.checkForOverflow == bop.checkForOverflow;
		}
	}
	
	public class ConstantConditionalOperator : ConstantExpression, ISupportsInterning
	{
		IConstantValue condition, trueExpr, falseExpr;
		
		public ConstantConditionalOperator(IConstantValue condition, IConstantValue trueExpr, IConstantValue falseExpr)
		{
			if (condition == null)
				throw new ArgumentNullException("condition");
			if (trueExpr == null)
				throw new ArgumentNullException("trueExpr");
			if (falseExpr == null)
				throw new ArgumentNullException("falseExpr");
			this.condition = condition;
			this.trueExpr = trueExpr;
			this.falseExpr = falseExpr;
		}
		
		public override ResolveResult Resolve(CSharpResolver resolver)
		{
			return resolver.ResolveConditional(
				Resolve(condition, resolver),
				Resolve(trueExpr, resolver),
				Resolve(falseExpr, resolver)
			);
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			condition = provider.Intern(condition);
			trueExpr = provider.Intern(trueExpr);
			falseExpr = provider.Intern(falseExpr);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return condition.GetHashCode() * 182981713
					+ trueExpr.GetHashCode() * 917517169
					+ falseExpr.GetHashCode() * 611651;
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			ConstantConditionalOperator coo = other as ConstantConditionalOperator;
			return coo != null
				&& this.condition == coo.condition
				&& this.trueExpr == coo.trueExpr
				&& this.falseExpr == coo.falseExpr;
		}
	}
}
