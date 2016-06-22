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
