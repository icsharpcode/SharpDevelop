// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
