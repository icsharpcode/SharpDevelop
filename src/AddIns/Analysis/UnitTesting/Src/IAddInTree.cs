// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.UnitTesting
{
	public interface IAddInTree
	{
		List<T> BuildItems<T>(string path, object caller);
	}
}
