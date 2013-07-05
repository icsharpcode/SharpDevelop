// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class NamespaceModel : INamespaceModel
	{
		IEntityModelContext context;
		string name;
		NamespaceModel parent;
		NullSafeSimpleModelCollection<NamespaceModel> childNamespaces;
		NullSafeSimpleModelCollection<ITypeDefinitionModel> typeDefinitions;
		
		public NamespaceModel(IEntityModelContext context, NamespaceModel parent, string name)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.parent = parent;
			this.name = name;
			this.typeDefinitions = new NullSafeSimpleModelCollection<ITypeDefinitionModel>();
			this.childNamespaces = new NullSafeSimpleModelCollection<NamespaceModel>();
		}
		
		public event PropertyChangedEventHandler PropertyChanged { add {} remove {} }

		public string FullName {
			get {
				if (parent == null || string.IsNullOrEmpty(parent.name))
					return name;
				return parent.FullName + "." + name;
			}
		}
		
		public INamespaceModel ParentNamespace {
			get { return parent; }
		}
		
		IModelCollection<INamespaceModel> INamespaceModel.ChildNamespaces {
			get { return childNamespaces; }
		}
		
		IModelCollection<ITypeDefinitionModel> INamespaceModel.Types {
			get { return typeDefinitions; }
		}
		
		public IMutableModelCollection<NamespaceModel> ChildNamespaces {
			get { return childNamespaces; }
		}
		
		public IMutableModelCollection<ITypeDefinitionModel> Types {
			get { return typeDefinitions; }
		}

		public string Name {
			get { return name; }
		}
		
		public SymbolKind SymbolKind {
			get { return SymbolKind.Namespace; }
		}
		
		public IProject ParentProject {
			get { return context.Project; }
		}
		
		public DomRegion Region {
			get { return DomRegion.Empty; }
		}
	}
}