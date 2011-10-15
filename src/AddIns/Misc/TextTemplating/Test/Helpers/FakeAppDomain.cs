// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeAppDomain : IAppDomain
	{
		public event ResolveEventHandler AssemblyResolve;
		
		public Assembly FireAssemblyResolveEvent(string assemblyName)
		{
			var eventArgs = new ResolveEventArgs(assemblyName);
			return FireAssemblyResolveEvent(eventArgs);
		}
		
		public Assembly FireAssemblyResolveEvent(ResolveEventArgs e)
		{
			if (AssemblyResolve != null) {
				return AssemblyResolve(this, e);
			}
			return null;
		}
	}
}
