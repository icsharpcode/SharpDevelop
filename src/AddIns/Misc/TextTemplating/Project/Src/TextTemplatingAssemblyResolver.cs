// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAssemblyResolver : ITextTemplatingAssemblyResolver
	{
		ITextTemplatingAssemblyPathResolver assemblyPathResolver;
		ITextTemplatingHostAppDomainAssemblyResolver appDomainAssemblyResolver;
		
		public TextTemplatingAssemblyResolver(IProject project)
			: this(
				new TextTemplatingAssemblyPathResolver(project),
				new TextTemplatingHostAppDomainAssemblyResolver())
		{
		}
		
		public TextTemplatingAssemblyResolver(
			ITextTemplatingAssemblyPathResolver assemblyPathResolver,
			ITextTemplatingHostAppDomainAssemblyResolver appDomainAssemblyResolver)
		{
			this.assemblyPathResolver = assemblyPathResolver;
			this.appDomainAssemblyResolver = appDomainAssemblyResolver;
		}
		
		public string ResolvePath(string assemblyReference)
		{
			return assemblyPathResolver.ResolvePath(assemblyReference);
		}
		
		public void Dispose()
		{
			appDomainAssemblyResolver.Dispose();
		}
	}
}
