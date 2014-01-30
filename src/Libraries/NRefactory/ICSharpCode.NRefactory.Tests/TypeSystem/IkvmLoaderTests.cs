// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class IkvmLoaderTests : BinaryLoaderTests
	{
		static readonly Lazy<IUnresolvedAssembly> mscorlib = new Lazy<IUnresolvedAssembly>(
			delegate {
				return new IkvmLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
			});
		
		static readonly Lazy<IUnresolvedAssembly> systemCore = new Lazy<IUnresolvedAssembly>(
			delegate {
			return new IkvmLoader().LoadAssemblyFile(typeof(System.Linq.Enumerable).Assembly.Location);
		});

		public static IUnresolvedAssembly Mscorlib { get { return mscorlib.Value; } }
		public static IUnresolvedAssembly SystemCore { get { return systemCore.Value; } }

		public override IUnresolvedAssembly MscorlibInstance { get { return mscorlib.Value; } }
		public override IUnresolvedAssembly SystemCoreInstance { get { return systemCore.Value; } }

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			try {
				// use "IncludeInternalMembers" so that Cecil results match C# parser results
				IkvmLoader loader = new IkvmLoader() { IncludeInternalMembers = true };
				IUnresolvedAssembly asm = loader.LoadAssemblyFile(typeof(TestCase.SimplePublicClass).Assembly.Location);
				compilation = new SimpleCompilation(asm, IkvmLoaderTests.Mscorlib);
			} catch (Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
