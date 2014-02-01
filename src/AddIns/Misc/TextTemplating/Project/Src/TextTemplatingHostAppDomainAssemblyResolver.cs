// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
