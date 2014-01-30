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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A collection of type definition models with the ability to look them up by name.
	/// </summary>
	public interface ITypeDefinitionModelCollection : IModelCollection<ITypeDefinitionModel>
	{
		/// <summary>
		/// Gets the type definition model with the specified type name.
		/// Returns null if no such model object exists.
		/// </summary>
		ITypeDefinitionModel this[FullTypeName fullTypeName] { get; }
		
		/// <summary>
		/// Gets the type definition model with the specified type name.
		/// Returns null if no such model object exists.
		/// </summary>
		ITypeDefinitionModel this[TopLevelTypeName topLevelTypeName] { get; }
		
		/// <summary>
		/// Updates the collection when the parse information has changed.
		/// </summary>
		void Update(IList<IUnresolvedTypeDefinition> oldFile, IList<IUnresolvedTypeDefinition> newFile);
	}
	
	public sealed class EmptyTypeDefinitionModelCollection : ITypeDefinitionModelCollection
	{
		public static readonly EmptyTypeDefinitionModelCollection Instance = new EmptyTypeDefinitionModelCollection();
		
		event ModelCollectionChangedEventHandler<ITypeDefinitionModel> IModelCollection<ITypeDefinitionModel>.CollectionChanged {
			add { }
			remove { }
		}
		
		IReadOnlyCollection<ITypeDefinitionModel> IModelCollection<ITypeDefinitionModel>.CreateSnapshot()
		{
			return this; // already immutable
		}
		
		ITypeDefinitionModel ITypeDefinitionModelCollection.this[FullTypeName name] {
			get { return null; }
		}
		
		ITypeDefinitionModel ITypeDefinitionModelCollection.this[TopLevelTypeName name] {
			get { return null; }
		}
		
		int IReadOnlyCollection<ITypeDefinitionModel>.Count {
			get { return 0; }
		}
		
		IEnumerator<ITypeDefinitionModel> IEnumerable<ITypeDefinitionModel>.GetEnumerator()
		{
			return Enumerable.Empty<ITypeDefinitionModel>().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumerable.Empty<ITypeDefinitionModel>().GetEnumerator();
		}
		
		void ITypeDefinitionModelCollection.Update(IList<IUnresolvedTypeDefinition> oldFile, IList<IUnresolvedTypeDefinition> newFile)
		{
			throw new NotSupportedException();
		}
	}
}
