// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingAssemblyResolver : ITextTemplatingAssemblyResolver
	{
		public string AssembyReferencePassedToResolve;
		public string ResolveReturnValue = String.Empty;
		
		public string Resolve(string assemblyReference)
		{
			this.AssembyReferencePassedToResolve = assemblyReference;
			return ResolveReturnValue;
		}
	}
}
