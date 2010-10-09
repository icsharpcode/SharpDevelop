// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Contains the main resolver logic.
	/// </summary>
	public class CSharpResolver
	{
		static readonly ResolveResult ErrorResult = new ErrorResolveResult(SharedTypes.UnknownType);
		static readonly ResolveResult DynamicResult = new ResolveResult(SharedTypes.Dynamic);
		
		readonly ITypeResolveContext context;
		
		/// <summary>
		/// Gets/Sets whether the current context is <c>checked</c>.
		/// </summary>
		public bool IsCheckedContext { get; set; }
		
		public CSharpResolver(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
		}
		
		#region class OperatorMethod
		class OperatorMethod : Immutable, IMethod
		{
			IList<IParameter> parameters = new List<IParameter>();
			
			public IList<IParameter> Parameters {
				get { return parameters; }
			}
			
			public ITypeReference ReturnType {
				get; set;
			}
			
			IList<IAttribute> IMethod.ReturnTypeAttributes {
				get { return EmptyList<IAttribute>.Instance; }
			}
			
			IList<ITypeParameter> IMethod.TypeParameters {
				get { return EmptyList<ITypeParameter>.Instance; }
			}
			
			bool IMethod.IsExtensionMethod {
				get { return false; }
			}
			
			bool IMethod.IsConstructor {
				get { return false; }
			}
			
			bool IMethod.IsDestructor {
				get { return false; }
			}
			
			bool IMethod.IsOperator {
				get { return true; }
			}
			
			ITypeDefinition IEntity.DeclaringTypeDefinition {
				get { return null; }
			}
			
			ITypeDefinition IMember.DeclaringTypeDefinition {
				get { return null; }
			}
			
			IType IMember.DeclaringType {
				get { return null; }
			}
			
			IMember IMember.MemberDefinition {
				get { return null; }
			}
			
			IList<IExplicitInterfaceImplementation> IMember.InterfaceImplementations {
				get { return EmptyList<IExplicitInterfaceImplementation>.Instance; }
			}
			
			bool IMember.IsVirtual {
				get { return false; }
			}
			
			bool IMember.IsOverride {
				get { return false; }
			}
			
			bool IMember.IsOverridable {
				get { return false; }
			}
			
			EntityType IEntity.EntityType {
				get { return EntityType.Operator; }
			}
			
			DomRegion IEntity.Region {
				get { return DomRegion.Empty; }
			}
			
			DomRegion IEntity.BodyRegion {
				get { return DomRegion.Empty; }
			}
			
			IList<IAttribute> IEntity.Attributes {
				get { return EmptyList<IAttribute>.Instance; }
			}
			
			string IEntity.Documentation {
				get { return null; }
			}
			
			Accessibility IEntity.Accessibility {
				get { return Accessibility.Public; }
			}
			
			bool IEntity.IsStatic {
				get { return true; }
			}
			
			bool IEntity.IsAbstract {
				get { return false; }
			}
			
			bool IEntity.IsSealed {
				get { return false; }
			}
			
			bool IEntity.IsShadowing {
				get { return false; }
			}
			
			bool IEntity.IsSynthetic {
				get { return true; }
			}
			
			IProjectContent IEntity.ProjectContent {
				get { return null; }
			}
			
			string INamedElement.FullName {
				get { return "operator"; }
			}
			
			string INamedElement.Name {
				get { return "operator"; }
			}
			
			string INamedElement.Namespace {
				get { return string.Empty; }
			}
			
			string INamedElement.DotNetName {
				get { return "operator"; }
			}
		}
		#endregion
		
		#region ResolveUnaryOperator
		public ResolveResult ResolveUnaryOperator(UnaryOperatorType op, ResolveResult expression)
		{
			if (expression.Type == SharedTypes.Dynamic)
				return DynamicResult;
			
			// C# 4.0 spec: §7.3.3 Unary operator overload resolution
			string overloadableOperatorName = GetOverloadableOperatorName(op);
			if (overloadableOperatorName == null) {
				switch (op) {
					case UnaryOperatorType.Dereference:
						PointerType p = expression.Type as PointerType;
						if (p != null)
							return new ResolveResult(p.ElementType);
						else
							return ErrorResult;
					case UnaryOperatorType.AddressOf:
						return new ResolveResult(new PointerType(expression.Type));
					default:
						throw new ArgumentException("Invalid value for UnaryOperatorType", "op");
				}
			}
			// If the type is nullable, get the underlying type:
			IType type = NullableType.GetUnderlyingType(expression.Type);
			bool isNullable = type != null;
			if (!isNullable)
				type = expression.Type;
			
			// the operator is overloadable:
			// TODO: implicit support for user operators
			//var candidateSet = GetUnaryOperatorCandidates();
			
			UnaryOperatorMethod[] methodGroup;
			switch (op) {
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostIncrement:
				case UnaryOperatorType.PostDecrement:
					// C# 4.0 spec: §7.6.9 Postfix increment and decrement operators
					// C# 4.0 spec: §7.7.5 Prefix increment and decrement operators
					TypeCode code = ReflectionHelper.GetTypeCode(type);
					if (type.IsEnum() || (code >= TypeCode.SByte && code <= TypeCode.Decimal))
						return new ResolveResult(expression.Type);
					else
						return ErrorResult;
				case UnaryOperatorType.Plus:
					methodGroup = unaryPlusOperators;
					break;
				case UnaryOperatorType.Minus:
					methodGroup = IsCheckedContext ? checkedUnaryMinusOperators : uncheckedUnaryMinusOperators;
					break;
				case UnaryOperatorType.Not:
					methodGroup = logicalNegationOperator;
					break;
				case UnaryOperatorType.BitNot:
					if (type.IsEnum()) {
						if (expression.IsCompileTimeConstant && expression.ConstantValue != null) {
							// evaluate as (E)(~(U)x);
							var U = expression.ConstantValue.GetType().ToTypeReference().Resolve(context);
							var unpackedEnum = new ConstantResolveResult(U, expression.ConstantValue);
							return ResolveCast(expression.Type, ResolveUnaryOperator(op, unpackedEnum));
						} else {
							return new ResolveResult(expression.Type);
						}
					} else {
						methodGroup = bitwiseComplementOperators;
						break;
					}
				default:
					throw new InvalidOperationException();
			}
			throw new NotImplementedException();
		}
		
		object GetUserUnaryOperatorCandidates()
		{
			// C# 4.0 spec: §7.3.5 Candidate user-defined operators
			throw new NotImplementedException();
		}
		
		static string GetOverloadableOperatorName(UnaryOperatorType op)
		{
			switch (op) {
				case UnaryOperatorType.Not:
					return "op_LogicalNot";
				case UnaryOperatorType.BitNot:
					return "op_OnesComplement";
				case UnaryOperatorType.Minus:
					return "op_UnaryNegation";
				case UnaryOperatorType.Plus:
					return "op_UnaryPlus";
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
					return "op_Increment";
				case UnaryOperatorType.Decrement:
				case UnaryOperatorType.PostDecrement:
					return "op_Decrement";
				default:
					return null;
			}
		}
		
		abstract class UnaryOperatorMethod : OperatorMethod
		{
			public abstract object Invoke(object input);
		}
		
		sealed class LambdaUnaryOperatorMethod<T> : UnaryOperatorMethod
		{
			readonly Func<T, T> func;
			
			public LambdaUnaryOperatorMethod(Func<T, T> func)
			{
				this.func = func;
			}
			
			public override object Invoke(object input)
			{
				return func((T)input);
			}
		}
		
		sealed class LiftedUnaryOperatorMethod : UnaryOperatorMethod
		{
			UnaryOperatorMethod baseMethod;
			
			public LiftedUnaryOperatorMethod(UnaryOperatorMethod baseMethod)
			{
				this.baseMethod = baseMethod;
				this.ReturnType = NullableType.Create(baseMethod.ReturnType);
				this.Parameters.Add(new DefaultParameter(NullableType.Create(baseMethod.Parameters[0].Type), string.Empty));
			}
			
			public override object Invoke(object input)
			{
				if (input == null)
					return null;
				else
					return baseMethod.Invoke(input);
			}
		}
		
		static UnaryOperatorMethod[] Lift(params UnaryOperatorMethod[] methods)
		{
			UnaryOperatorMethod[] lifted = new UnaryOperatorMethod[methods.Length * 2];
			methods.CopyTo(lifted, 0);
			for (int i = 0; i < methods.Length; i++) {
				lifted[methods.Length + i] = new LiftedUnaryOperatorMethod(methods[i]);
			}
			return lifted;
		}
		
		// C# 4.0 spec: §7.7.1 Unary plus operator
		static readonly UnaryOperatorMethod[] unaryPlusOperators = Lift(
			new LambdaUnaryOperatorMethod<int>(i => +i),
			new LambdaUnaryOperatorMethod<uint>(i => +i),
			new LambdaUnaryOperatorMethod<long>(i => +i),
			new LambdaUnaryOperatorMethod<ulong>(i => +i),
			new LambdaUnaryOperatorMethod<float>(i => +i),
			new LambdaUnaryOperatorMethod<double>(i => +i),
			new LambdaUnaryOperatorMethod<decimal>(i => +i)
		);
		
		// C# 4.0 spec: §7.7.2 Unary minus operator
		static readonly UnaryOperatorMethod[] uncheckedUnaryMinusOperators = Lift(
			new LambdaUnaryOperatorMethod<int>(i => unchecked(-i)),
			new LambdaUnaryOperatorMethod<long>(i => unchecked(-i)),
			new LambdaUnaryOperatorMethod<float>(i => -i),
			new LambdaUnaryOperatorMethod<double>(i => -i),
			new LambdaUnaryOperatorMethod<decimal>(i => -i)
		);
		static readonly UnaryOperatorMethod[] checkedUnaryMinusOperators = Lift(
			new LambdaUnaryOperatorMethod<int>(i => checked(-i)),
			new LambdaUnaryOperatorMethod<long>(i => checked(-i)),
			new LambdaUnaryOperatorMethod<float>(i => -i),
			new LambdaUnaryOperatorMethod<double>(i => -i),
			new LambdaUnaryOperatorMethod<decimal>(i => -i)
		);
		
		// C# 4.0 spec: §7.7.3 Logical negation operator
		static readonly UnaryOperatorMethod[] logicalNegationOperator = Lift(new LambdaUnaryOperatorMethod<bool>(b => !b));
		
		// C# 4.0 spec: §7.7.4 Bitwise complement operator
		static readonly UnaryOperatorMethod[] bitwiseComplementOperators = Lift(
			new LambdaUnaryOperatorMethod<int>(i => ~i),
			new LambdaUnaryOperatorMethod<uint>(i => ~i),
			new LambdaUnaryOperatorMethod<long>(i => ~i),
			new LambdaUnaryOperatorMethod<ulong>(i => ~i)
		);
		#endregion
		
		#region ResolveCast
		public ResolveResult ResolveCast(IType targetType, ResolveResult expression)
		{
			if (expression.IsError)
				return new ErrorResolveResult(targetType);
			
			// C# 4.0 spec: §7.7.6 Cast expressions
			if (expression.IsCompileTimeConstant) {
				TypeCode code = ReflectionHelper.GetTypeCode(targetType);
				if (code >= TypeCode.Boolean && code <= TypeCode.Decimal) {
					try {
						return new ConstantResolveResult(targetType, Convert.ChangeType(expression.ConstantValue, code, CultureInfo.InvariantCulture));
					} catch (InvalidCastException) {
						return new ErrorResolveResult(targetType);
					}
				} else if (code == TypeCode.String) {
					if (expression.ConstantValue == null || expression.ConstantValue is string)
						return new ConstantResolveResult(targetType, expression.ConstantValue);
					else
						return new ErrorResolveResult(targetType);
				} else if (targetType.IsEnum()) {
					code = ReflectionHelper.GetTypeCode(targetType.GetEnumUnderlyingType(context));
					if (code >= TypeCode.SByte && code <= TypeCode.UInt64) {
						try {
							return new ConstantResolveResult(targetType, Convert.ChangeType(expression.ConstantValue, code, CultureInfo.InvariantCulture));
						} catch (InvalidCastException) {
							return new ErrorResolveResult(targetType);
						}
					}
				}
			}
			return new ResolveResult(targetType);
		}
		#endregion
	}
}
