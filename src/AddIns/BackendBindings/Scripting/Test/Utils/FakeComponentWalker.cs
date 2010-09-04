// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeComponentWalker : IComponentWalker
	{		
		public string CodePassedToCreateComponent;
		
		public IComponent CreateComponent(string code)
		{
			CodePassedToCreateComponent = code;
			return null;
		}
	}
}
