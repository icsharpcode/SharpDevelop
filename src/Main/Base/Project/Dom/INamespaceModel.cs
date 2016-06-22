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
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Represents a namespace within a single project.
	/// </summary>
	public interface INamespaceModel : ISymbolModel
	{
		/// <summary>
		/// Gets the full name of the namespace.
		/// </summary>
		string FullName { get; }
		
		/// <summary>
		/// Gets the parent namespace. Returns null only if this is the namespace without a name.
		/// </summary>
		INamespaceModel ParentNamespace { get; }
		
		/// <summary>
		/// Gets a collection of all child namespaces.
		/// </summary>
		IModelCollection<INamespaceModel> ChildNamespaces { get; }
		
		/// <summary>
		/// Gets a collection of all top-level types in this namespace.
		/// </summary>
		IModelCollection<ITypeDefinitionModel> Types { get; }
		
		/// <inheritdoc/>
		/// <remarks>
		/// Always returns <see cref="DomRegion.Empty"/> for namespaces,
		/// because they are not defined in one single location.
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Never)]
		new DomRegion Region { get; }
	}
	
	public sealed class EmptyNamespaceModel : INamespaceModel
	{
		public readonly static INamespaceModel Instance = new EmptyNamespaceModel();
		
		EmptyNamespaceModel()
		{
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add {} remove {} }

		public string FullName {
			get { return string.Empty; }
		}

		public INamespaceModel ParentNamespace {
			get { return null; }
		}

		public IModelCollection<INamespaceModel> ChildNamespaces {
			get { return ImmutableModelCollection<INamespaceModel>.Empty; }
		}

		public IModelCollection<ITypeDefinitionModel> Types {
			get { return EmptyTypeDefinitionModelCollection.Instance; }
		}

		public string Name {
			get { return string.Empty; }
		}

		public SymbolKind SymbolKind {
			get { return SymbolKind.Namespace; }
		}

		public ICSharpCode.SharpDevelop.Project.IProject ParentProject {
			get { return null; }
		}

		public DomRegion Region {
			get { return DomRegion.Empty; }
		}
	}
}
