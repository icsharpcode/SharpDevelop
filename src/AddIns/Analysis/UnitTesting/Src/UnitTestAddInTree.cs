// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestAddInTree : IAddInTree
	{
		public List<T> BuildItems<T>(string path, object caller)
		{
			return AddInTree.BuildItems<T>(path, caller);
		}
	}
}
