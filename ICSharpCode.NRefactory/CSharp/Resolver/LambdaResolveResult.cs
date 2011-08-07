// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents an anonymous method or lambda expression.
	/// </summary>
	public abstract class LambdaResolveResult : ResolveResult
	{
		protected LambdaResolveResult() : base(SharedTypes.UnknownType)
		{
		}
		
		/// <summary>
		/// Gets whether there is a parameter list.
		/// This property always returns true for C# 3.0-lambdas, but may return false
		/// for C# 2.0 anonymous methods.
		/// </summary>
		public abstract bool HasParameterList { get; }
		
		/// <summary>
		/// Gets whether this lambda is using the C# 2.0 anonymous method syntax.
		/// </summary>
		public abstract bool IsAnonymousMethod { get; }
		
		/// <summary>
		/// Gets whether the lambda parameters are implicitly typed.
		/// </summary>
		/// <remarks>This property returns false for anonymous methods without parameter list.</remarks>
		public abstract bool IsImplicitlyTyped { get; }
		
		/// <summary>
		/// Gets the return type inferred when the parameter types are inferred to be <paramref name="parameterTypes"/>
		/// </summary>
		public abstract IType GetInferredReturnType(IType[] parameterTypes);
		
		/// <summary>
		/// Gets the list of parameters.
		/// </summary>
		public abstract IList<IParameter> Parameters { get; }
		
		/// <summary>
		/// Gets whether the lambda body is valid for the given parameter types and return type.
		/// </summary>
		/// <returns>
		/// Produces a <see cref="Conversion.AnonymousFunctionConversion"/> if the lambda is valid;
		/// otherwise returns <see cref="Conversion.None"/>.
		/// </returns>
		public abstract Conversion IsValid(IType[] parameterTypes, IType returnType, Conversions conversions);
	}
}
