// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.Core.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests.Utils.Tests
{
	[TestFixture]
	public class DerivedRuntimeTests
	{
		[Test]
		public void LoadAssemblyFromReturnsICSharpCoreTestsAssemblyForUnknownAssemblyFileName()
		{
			DerivedRuntime runtime = new DerivedRuntime("MyAddIn.dll", String.Empty);
			Assembly loadedAssembly = runtime.CallLoadAssemblyFrom("tests.dll");
			
			string expectedLoadedAssemblyName = "ICSharpCode.Core.Tests.dll";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyFromReturnsKnownAssemblyFromAssemblyFileNamesCollection()
		{
			DerivedRuntime runtime = new DerivedRuntime("MyAddIn.dll", String.Empty);
			runtime.AssemblyFileNames.Add("MyAddIn.dll", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.CallLoadAssemblyFrom(@"d:\projects\test\MyAddIn.dll");
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyReturnsICSharpCoreTestsAssemblyForUnknownAssemblyName()
		{
			DerivedRuntime runtime = new DerivedRuntime(":ICSharpCode.SharpDevelop", String.Empty);
			Assembly loadedAssembly = runtime.CallLoadAssembly("Unknown");
			
			string expectedLoadedAssemblyName = "ICSharpCode.Core.Tests.dll";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyReturnsKnownAssemblyFromAssemblyNamesCollection()
		{
			DerivedRuntime runtime = new DerivedRuntime(":ICSharpCode.SharpDevelop", String.Empty);
			runtime.AssemblyNames.Add("ICSharpCode.SharpDevelop", typeof(string).Assembly);
			Assembly loadedAssembly = runtime.CallLoadAssembly("ICSharpCode.SharpDevelop");
			
			string expectedLoadedAssemblyName = "CommonLanguageRuntimeLibrary";
			Assert.AreEqual(expectedLoadedAssemblyName, loadedAssembly.ManifestModule.ToString());
		}
		
		[Test]
		public void LoadAssemblyThrowsPredefinedException()
		{
			DerivedRuntime runtime = new DerivedRuntime(":ICSharpCode.SharpDevelop", String.Empty);
			ApplicationException expectedException = new ApplicationException("Test");
			runtime.LoadAssemblyExceptionToThrow = expectedException;
			
			ApplicationException ex = Assert.Throws<ApplicationException>(delegate { runtime.CallLoadAssembly("test"); });
			Assert.AreEqual("Test", ex.Message);
		}
		
		[Test]
		public void LoadAssemblyFromThrowsPredefinedException()
		{
			DerivedRuntime runtime = new DerivedRuntime(":ICSharpCode.SharpDevelop", String.Empty);
			ApplicationException expectedException = new ApplicationException("Test");
			runtime.LoadAssemblyFromExceptionToThrow = expectedException;
			
			ApplicationException ex = Assert.Throws<ApplicationException>(delegate { runtime.CallLoadAssemblyFrom("test"); });
			Assert.AreEqual("Test", ex.Message);
		}
	}
}
