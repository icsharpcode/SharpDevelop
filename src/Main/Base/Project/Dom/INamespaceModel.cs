// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
