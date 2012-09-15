// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Reflection;
using ICSharpCode.NRefactory.TypeSystem;

namespace UnitTesting.Tests.Utils
{
	public class AssemblyLoader
	{
		static ConcurrentDictionary<string, IUnresolvedAssembly> dict = new ConcurrentDictionary<string, IUnresolvedAssembly>();
		
		public static IUnresolvedAssembly Load(Assembly assembly)
		{
			return dict.GetOrAdd(assembly.Location, loc => new CecilLoader { LazyLoad = true }.LoadAssemblyFile(loc));
		}
		
		public static IUnresolvedAssembly Corlib {
			get { return Load(typeof(object).Assembly); }
		}
		
		public static IUnresolvedAssembly NUnitFramework {
			get { return Load(typeof(global::NUnit.Framework.TestAttribute).Assembly); }
		}
	}
}
