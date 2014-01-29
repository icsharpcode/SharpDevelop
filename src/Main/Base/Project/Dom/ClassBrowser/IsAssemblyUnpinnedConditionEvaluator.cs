// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class IsAssemblyUnpinnedConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			var assemblyModel = parameter as IAssemblyModel;
			if (assemblyModel != null) {
				return assemblyModel.IsUnpinned();
			}
			
			return false;
		}
	}
}
