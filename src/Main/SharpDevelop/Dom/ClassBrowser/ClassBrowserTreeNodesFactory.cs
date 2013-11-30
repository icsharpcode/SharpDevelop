// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	class ClassBrowserTreeNodesFactory : ITreeNodeFactory
	{
		public Type GetSupportedType(object model)
		{
			if (model is ISolutionAssemblyList)
				return typeof(ISolutionAssemblyList);
			if (model is IProject)
				return typeof(IProject);
			if (model is INamespaceModel)
				return typeof(INamespaceModel);
			if (model is ITypeDefinitionModel)
				return typeof(ITypeDefinitionModel);
			if (model is IMemberModel)
				return typeof(IMemberModel);
			if (model is IAssemblyModel)
				return typeof(IAssemblyModel);
			if (model is IAssemblyReferenceModel)
				return typeof(IAssemblyReferenceModel);
			if (model is IAssemblyReferencesModel)
				return typeof(IAssemblyReferencesModel);
			return null;
		}
		
		public SharpTreeNode CreateTreeNode(object model)
		{
			if (model is ISolutionAssemblyList)
				return new SolutionTreeNode((ISolutionAssemblyList)model);
			if (model is IProject)
				return new ProjectTreeNode((IProject)model);
			if (model is INamespaceModel)
				return new NamespaceTreeNode((INamespaceModel)model);
			if (model is ITypeDefinitionModel)
				return new TypeDefinitionTreeNode((ITypeDefinitionModel)model);
			if (model is IMemberModel)
				return new MemberTreeNode((IMemberModel)model);
			if (model is IAssemblyModel)
				return new AssemblyTreeNode((IAssemblyModel) model);
			if (model is IAssemblyReferenceModel)
				return new AssemblyReferenceTreeNode((IAssemblyReferenceModel) model);
			if (model is IAssemblyReferencesModel)
				return new AssemblyReferencesTreeNode((IAssemblyReferencesModel) model);
			return null;
		}
	}
}


