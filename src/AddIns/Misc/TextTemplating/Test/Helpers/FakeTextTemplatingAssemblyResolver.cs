// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingAssemblyResolver : ITextTemplatingAssemblyResolver
	{
		public string AssembyReferencePassedToResolvePath;
		public string ResolvePathReturnValue = String.Empty;
		
		public string ResolvePath(string assemblyReference)
		{
			this.AssembyReferencePassedToResolvePath = assemblyReference;
			return ResolvePathReturnValue;
		}
		
		public bool IsDisposeCalled;
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
	}
}
