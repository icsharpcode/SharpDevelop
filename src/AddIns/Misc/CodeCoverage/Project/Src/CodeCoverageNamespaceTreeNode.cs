// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageNamespaceTreeNode : CodeCoverageTreeNode
	{
		public CodeCoverageNamespaceTreeNode(string name) : base(name, CodeCoverageImageListIndex.Namespace)
		{
		}
	}
}
