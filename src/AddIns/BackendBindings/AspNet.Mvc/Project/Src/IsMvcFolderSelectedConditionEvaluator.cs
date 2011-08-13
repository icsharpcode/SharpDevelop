// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class IsMvcFolderSelectedConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object owner, Condition condition)
		{
			return IsMvcFolderNodeSelected(owner);
		}
		
		bool IsMvcFolderNodeSelected(object owner)
		{
			if (IsProjectNode(owner)) {
				return false;
			}
			
			var directoryNode = owner as DirectoryNode;
			if (directoryNode != null) {
				if (IsMatch(directoryNode)) {
					return true;
				}
				return IsMvcFolderNodeSelected(directoryNode.Parent);
			}
			return false;
		}
		
		bool IsProjectNode(object owner)
		{
			return owner is ProjectNode;
		}
		
		bool IsMatch(DirectoryNode directoryNode)
		{
			return MvcParentFolderName.Equals(directoryNode.Text, StringComparison.InvariantCultureIgnoreCase);
		}
		
		protected abstract string MvcParentFolderName { get; }
	}
}
