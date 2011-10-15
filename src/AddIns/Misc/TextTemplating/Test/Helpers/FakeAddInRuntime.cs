// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeAddInRuntime : IAddInRuntime
	{
		public FakeAddInRuntime(string assembly, Assembly loadedAssembly)
		{
			this.Assembly = assembly;
			this.LoadedAssembly = loadedAssembly;
		}
		
		public string Assembly { get; set; }
		public Assembly LoadedAssembly { get; set; }
	}
}
