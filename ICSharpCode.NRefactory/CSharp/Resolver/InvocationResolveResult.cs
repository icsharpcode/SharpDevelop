// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents the result of a method invocation.
	/// </summary>
	public class InvocationResolveResult : MemberResolveResult
	{
		public readonly OverloadResolutionErrors OverloadResolutionErrors;
		public readonly IList<IType> TypeArguments;
		
		public readonly IList<ResolveResult> Arguments;
		public readonly IList<Conversion> ArgumentConversions;
		
		/// <summary>
		/// Gets whether this invocation is calling an extension method using extension method syntax.
		/// </summary>
		public readonly bool IsExtensionMethodInvocation;
		
		/// <summary>
		/// Gets whether a params-Array is being used in its expanded form.
		/// </summary>
		public readonly bool IsExpandedForm;
		
		readonly IList<int> argumentToParameterMap;
		
		public InvocationResolveResult(ResolveResult targetResult, OverloadResolution or, ITypeResolveContext context)
			: base(
				or.IsExtensionMethodInvocation ? new TypeResolveResult(or.BestCandidate.DeclaringType) : targetResult,
				or.BestCandidate,
				GetReturnType(or, context))
		{
			this.OverloadResolutionErrors = or.BestCandidateErrors;
			this.TypeArguments = or.InferredTypeArguments;
			this.Arguments = or.Arguments;
			this.ArgumentConversions = or.ArgumentConversions;
			this.IsExtensionMethodInvocation = or.IsExtensionMethodInvocation;
			this.IsExpandedForm = or.BestCandidateIsExpandedForm;
			this.argumentToParameterMap = or.GetArgumentToParameterMap();
		}
		
		static IType GetReturnType(OverloadResolution or, ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			IType returnType;
			if (or.BestCandidate.EntityType == EntityType.Constructor)
				returnType = or.BestCandidate.DeclaringType;
			else
				returnType = or.BestCandidate.ReturnType.Resolve(context);
			
			var typeArguments = or.InferredTypeArguments;
			if (typeArguments.Count > 0)
				return returnType.AcceptVisitor(new MethodTypeParameterSubstitution(typeArguments));
			else
				return returnType;
		}
		
		public override bool IsError {
			get { return this.OverloadResolutionErrors != OverloadResolutionErrors.None; }
		}
		
		/// <summary>
		/// Gets an array that maps argument indices to parameter indices.
		/// For arguments that could not be mapped to any parameter, the value will be -1.
		/// 
		/// parameterIndex = ArgumentToParameterMap[argumentIndex]
		/// </summary>
		public IList<int> GetArgumentToParameterMap()
		{
			return argumentToParameterMap;
		}
		
		public new IParameterizedMember Member {
			get { return (IParameterizedMember)base.Member; }
		}
	}
}
