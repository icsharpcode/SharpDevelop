// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace UnitTesting.Tests.Utils
{
	public class NRefactoryHelper
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
		
		public static ICompilation CreateCompilation(string code)
		{
			return new CSharpProjectContent()
				.AddAssemblyReferences(Corlib, NUnitFramework)
				.AddOrUpdateFiles(new CSharpParser().Parse(code, "test.cs").ToTypeSystem())
				.CreateCompilation();
		}
		
		public static ITypeDefinition CreateTypeDefinition(string code)
		{
			return CreateCompilation(code).MainAssembly.TopLevelTypeDefinitions.First();
		}
	}
}
