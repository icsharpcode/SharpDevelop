// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;

namespace PythonBinding.Tests.Utils
{
	public class PythonComponentWalkerHelper
	{
		public static IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return new PythonComponentWalker(componentCreator);
		}
	}
}
