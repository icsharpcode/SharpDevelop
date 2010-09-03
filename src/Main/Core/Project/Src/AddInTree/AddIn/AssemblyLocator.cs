// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.Core
{
	// Based on http://ayende.com/Blog/archive/2006/05/22/SolvingTheAssemblyLoadContextProblem.aspx
	// This class ensures that assemblies loaded into the LoadFrom context are also available
	// in the Load context.
	static class AssemblyLocator
	{
		static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
		static bool initialized;
		
		public static void Init()
		{
			lock (assemblies) {
				if (initialized)
					return;
				initialized = true;
				AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			}
		}
		
		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			lock (assemblies) {
				Assembly assembly = null;
				assemblies.TryGetValue(args.Name, out assembly);
				return assembly;
			}
		}
		
		static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			Assembly assembly = args.LoadedAssembly;
			lock (assemblies) {
				assemblies[assembly.FullName] = assembly;
			}
		}
	}
}
