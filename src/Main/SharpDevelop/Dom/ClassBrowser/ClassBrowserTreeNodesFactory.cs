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
