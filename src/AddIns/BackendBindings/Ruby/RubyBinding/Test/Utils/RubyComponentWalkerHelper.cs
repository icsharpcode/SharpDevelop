// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
