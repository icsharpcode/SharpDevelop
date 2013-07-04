// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Represents a namespace within a single project.
	/// </summary>
	public interface INamespaceModel : ISymbolModel
	{
		string FullName { get; }
		
		INamespaceModel ParentNamespace { get; }
		IModelCollection<INamespaceModel> ChildNamespaces { get; }
		IModelCollection<ITypeDefinitionModel> Types { get; }
	}
	
	public sealed class EmptyNamespaceModel : INamespaceModel
	{
		public readonly static INamespaceModel Instance = new EmptyNamespaceModel();
		
		EmptyNamespaceModel()
		{
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
