// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
