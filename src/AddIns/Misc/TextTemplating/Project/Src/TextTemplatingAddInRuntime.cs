// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAddInRuntime : IAddInRuntime
	{
		Runtime runtime;
		
		public TextTemplatingAddInRuntime(Runtime runtime)
		{
			this.runtime = runtime;
		}
		
		public string Assembly {
			get { return runtime.Assembly; }
		}
		
		public Assembly LoadedAssembly {
			get { return runtime.LoadedAssembly; }
		}
	}
}
