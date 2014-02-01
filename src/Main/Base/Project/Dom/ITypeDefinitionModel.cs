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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Observable model for a type definition.
	/// </summary>
	public interface ITypeDefinitionModel : IEntityModel
	{
		event EventHandler Updated;
		
		FullTypeName FullTypeName { get; }
		string Namespace { get; }
		TypeKind TypeKind { get; }
		IModelCollection<ITypeDefinitionModel> NestedTypes { get; }
		IModelCollection<IMemberModel> Members { get; }
		
		IEnumerable<DomRegion> GetPartRegions();
		
		/// <summary>
		/// Resolves the type definition in the current compilation.
		/// Returns null if the type definition could not be resolved.
		/// </summary>
		new ITypeDefinition Resolve();
		
		/// <summary>
		/// Resolves the type definition in the specified compilation.
		/// Returns null if the type definition could not be resolved.
		/// </summary>
		new ITypeDefinition Resolve(ICompilation compilation);
		
		/// <summary>
		/// Retrieves the nested type with the specified name and additional type parameter count
		/// </summary>
		ITypeDefinitionModel GetNestedType(string name, int additionalTypeParameterCount);
		
		bool IsPartial { get; }
	}
}
