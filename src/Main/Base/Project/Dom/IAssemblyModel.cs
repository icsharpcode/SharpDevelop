// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IAssemblyModel
	{
		string Name { get; }
		
		ITypeDefinitionModelCollection TopLevelTypeDefinitions { get; }
		IModelCollection<INamespaceModel> Namespaces { get; }
		INamespaceModel RootNamespace { get; }
	}
	
	public interface IUpdateableAssemblyModel : IAssemblyModel
	{
		void Update(IUnresolvedFile oldFile, IUnresolvedFile newFile);
	}
	
	public sealed class EmptyAssemblyModel : IAssemblyModel
	{
		public readonly static IAssemblyModel Instance = new EmptyAssemblyModel();
		
		EmptyAssemblyModel()
		{
		}
		
		public string Name {
			get { return string.Empty; }
		}

		public ITypeDefinitionModelCollection TopLevelTypeDefinitions {
			get { return EmptyTypeDefinitionModelCollection.Instance; }
		}

		public IModelCollection<INamespaceModel> Namespaces {
			get { return ImmutableModelCollection<INamespaceModel>.Empty; }
		}

		public INamespaceModel RootNamespace {
			get { return EmptyNamespaceModel.Instance; }
		}
	}
}


