// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestNodeFactory : ITreeNodeFactory
	{
		public Type GetSupportedType(object model)
		{
			if (model is ITest)
				return typeof(ITest);
			else
				return null;
		}
		
		public SharpTreeNode CreateTreeNode(object model)
		{
			ITest test = model as ITest;
			if (test != null)
				return new UnitTestNode(test);
			else
				return null;
		}
	}
}
