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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.TypeSystem
{
	/// <summary>
	/// Represents a reference which could point to a type or namespace.
	/// </summary>
	[Serializable]
	public abstract class TypeOrNamespaceReference : ITypeReference
	{
		/// <summary>
		/// Resolves the reference and returns the ResolveResult.
		/// </summary>
		public abstract ResolveResult Resolve(CSharpResolver resolver);
		
		/// <summary>
		/// Returns the namespace that is referenced; or null if no such namespace is found.
		/// </summary>
		public INamespace ResolveNamespace(CSharpResolver resolver)
		{
			NamespaceResolveResult nrr = Resolve(resolver) as NamespaceResolveResult;
			return nrr != null ? nrr.Namespace : null;
		}
		
		/// <summary>
		/// Returns the type that is referenced; or <see cref="SpecialTypes.UnknownType"/> if the type isn't found.
		/// </summary>
		public IType ResolveType(CSharpResolver resolver)
		{
			TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
			return trr != null ? trr.Type : SpecialType.UnknownType;
		}
		
		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (context.CurrentAssembly != context.Compilation.MainAssembly) {
				// The type needs to be resolved in a different compilation.
				throw new NotImplementedException();
			} else {
				// Resolve in current context.
				return ResolveType(new CSharpResolver((CSharpTypeResolveContext)context));
			}
		}
	}
}
