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
using System.Reflection;
using ICSharpCode.Core.Tests.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.Core.Tests.Utils.Tests
{
	[TestFixture]
	public class DerivedRuntimeTests
	{
		IAddInTree addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
		
		[Test]
		public void LoadAssemblyFromReturnsICSharpCoreTestsAssemblyForUnknownAssemblyFileName()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "MyAddIn.dll", String.Empty);
			Assembly loadedAssembly = runtime.CallLoadAssemblyFrom("tests.dll");
			
			string expectedLoadedAssemblyName = "ICSharpCode.Core.Tests.dll";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyFromReturnsKnownAssemblyFromAssemblyFileNamesCollection()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, "MyAddIn.dll", String.Empty);
			runtime.AssemblyFileNames.Add("MyAddIn.dll", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.CallLoadAssemblyFrom(@"d:\projects\test\MyAddIn.dll");
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyReturnsICSharpCoreTestsAssemblyForUnknownAssemblyName()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			Assembly loadedAssembly = runtime.CallLoadAssembly("Unknown");
			
			string expectedLoadedAssemblyName = "ICSharpCode.Core.Tests.dll";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyReturnsKnownAssemblyFromAssemblyNamesCollection()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			runtime.AssemblyNames.Add("ICSharpCode.SharpDevelop", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.CallLoadAssembly("ICSharpCode.SharpDevelop");
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyThrowsPredefinedException()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			ApplicationException expectedException = new ApplicationException("Test");
			runtime.LoadAssemblyExceptionToThrow = expectedException;
			
			ApplicationException ex = Assert.Throws<ApplicationException>(delegate { runtime.CallLoadAssembly("test"); });
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void LoadAssemblyFromThrowsPredefinedException()
		{
			DerivedRuntime runtime = new DerivedRuntime(addInTree, ":ICSharpCode.SharpDevelop", String.Empty);
			ApplicationException expectedException = new ApplicationException("Test");
			runtime.LoadAssemblyFromExceptionToThrow = expectedException;
			
			ApplicationException ex = Assert.Throws<ApplicationException>(delegate { runtime.CallLoadAssemblyFrom("test"); });
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
