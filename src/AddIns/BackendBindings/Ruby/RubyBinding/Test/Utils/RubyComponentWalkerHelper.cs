// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;

namespace RubyBinding.Tests.Utils
{
	public class RubyComponentWalkerHelper
	{
		public static IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return new RubyComponentWalker(componentCreator);
		}
	}
}
