// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingAssemblyPathResolver : ITextTemplatingAssemblyPathResolver
	{
		public string AssemblyReferencePassedToResolvePath;
		public string ResolvePathReturnValue;
		
		public string ResolvePath(string assemblyReference)
		{
			AssemblyReferencePassedToResolvePath = assemblyReference;
			return ResolvePathReturnValue;
		}
	}
}
