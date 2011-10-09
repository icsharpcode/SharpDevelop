// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public class ViewInBrowserConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			var node  = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				return false;
			}
			
			string fileName = Path.GetFileName(node.FullPath);
			
			string[] extensions = condition["extensions"].Split(',');
			if (!extensions.Any(e => fileName.EndsWith(e))) {
				return false;
			}
			
			return true;
		}
	}
}
