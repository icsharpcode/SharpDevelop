// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ClassBrowserTreeNodesFactory : ITreeNodeFactory
	{
		public Type GetSupportedType(object model)
		{
			if (model is ISolution)
				return typeof(ISolution);
			if (model is IProject)
				return typeof(IProject);
			if (model is INamespaceModel)
				return typeof(INamespaceModel);
			if (model is ITypeDefinitionModel)
				return typeof(ITypeDefinitionModel);
			if (model is IMemberModel)
				return typeof(IMemberModel);
			return null;
		}
		
		public SharpTreeNode CreateTreeNode(object model)
		{
			if (model is ISolution)
				return new SolutionTreeNode((ISolution)model);
			if (model is IProject)
				return new ProjectTreeNode((IProject)model);
			if (model is INamespaceModel)
				return new NamespaceTreeNode((INamespaceModel)model);
			if (model is ITypeDefinitionModel)
				return new TypeDefinitionTreeNode((ITypeDefinitionModel)model);
			if (model is IMemberModel)
				return new MemberTreeNode((IMemberModel)model);
			return null;
		}
	}
}


