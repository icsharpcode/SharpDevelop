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
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
namespace ICSharpCode.SharpDevelop.Dom
{
	sealed class NamespaceModel : INamespaceModel
	{
		IProject parentProject;
		string name;
		NamespaceModel parent;
		NullSafeSimpleModelCollection<NamespaceModel> childNamespaces;
		NullSafeSimpleModelCollection<ITypeDefinitionModel> typeDefinitions;
		
		public NamespaceModel(IProject parentProject, NamespaceModel parent, string name)
		{
			this.parentProject = parentProject;
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
			get { return parentProject; }
		}
		
		public DomRegion Region {
			get { return DomRegion.Empty; }
		}
	}
}
