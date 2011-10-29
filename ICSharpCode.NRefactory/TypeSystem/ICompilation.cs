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
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.NRefactory.TypeSystem
{
	public interface ICompilation
	{
		/// <summary>
		/// Gets the current assembly.
		/// </summary>
		IAssembly MainAssembly { get; }
		
		/// <summary>
		/// Gets the type resolve context that specifies this compilation and no current assembly or entity.
		/// </summary>
		ITypeResolveContext TypeResolveContext { get; }
		
		/// <summary>
		/// Gets the referenced assemblies.
		/// This list does not include the current assembly.
		/// </summary>
		IList<IAssembly> ReferencedAssemblies { get; }
		
		/// <summary>
		/// Gets the root namespace of this compilation.
		/// </summary>
		INamespace RootNamespace { get; }
		
		/// <summary>
		/// Gets the root namespace for a given extern alias.
		/// </summary>
		INamespace GetNamespaceForExternAlias(string alias);
		
		/// <summary>
		/// Gets all type defininitions in this compilation, including nested types.
		/// </summary>
		IEnumerable<ITypeDefinition> GetAllTypeDefinitions();
		
		IType FindType(KnownTypeCode typeCode);
		
		CacheManager CacheManager { get; }
	}
	
	public interface IResolved
	{
		ICompilation Compilation { get; }
	}
}
