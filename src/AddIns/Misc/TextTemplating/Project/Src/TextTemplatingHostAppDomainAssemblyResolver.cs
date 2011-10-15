// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Reflection;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingHostAppDomainAssemblyResolver : ITextTemplatingHostAppDomainAssemblyResolver
	{
		IAppDomain hostAppDomain;
		IAddInTree addInTree;
		
		public TextTemplatingHostAppDomainAssemblyResolver()
			: this(
				new TextTemplatingHostAppDomain(),
				new TextTemplatingAddInTree())
		{
		}
		
		public TextTemplatingHostAppDomainAssemblyResolver(
			IAppDomain hostAppDomain,
			IAddInTree addInTree)
		{
			this.hostAppDomain = hostAppDomain;
			this.addInTree = addInTree;
			hostAppDomain.AssemblyResolve += ResolveAssembly;
		}

		Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		{
			var assemblyName = new AddInAssemblyName(args.Name);
			return ResolveAssembly(assemblyName);
		}
		
		Assembly ResolveAssembly(AddInAssemblyName assemblyName)
		{
			IAddIn addIn = FindAddIn(assemblyName);
			if (addIn != null) {
				return FindAddInAssemblyFromRuntimes(addIn);
			}
			return null;
		}
		
		IAddIn FindAddIn(AddInAssemblyName assemblyName)
		{
			return addInTree
				.GetAddIns()
				.SingleOrDefault(addIn => assemblyName.Matches(addIn));
		}
		
		Assembly FindAddInAssemblyFromRuntimes(IAddIn addIn)
		{
			IAddInRuntime runtime = FindRuntime(addIn);
			if (runtime != null) {
				return runtime.LoadedAssembly;
			}
			return null;
		}
		
		IAddInRuntime FindRuntime(IAddIn addIn)
		{
			var addinRuntime = new AddInAssemblyRuntime(addIn);
			return addinRuntime.Runtime;
		}
		
		public void Dispose()
		{
			hostAppDomain.AssemblyResolve -= ResolveAssembly;
		}
	}
}
