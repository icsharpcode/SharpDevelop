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
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingHostAppDomainAssemblyResolverTests
	{
		TextTemplatingHostAppDomainAssemblyResolver resolver;
		FakeAppDomain fakeAppDomain;
		FakeAddInTree fakeAddInTree;
		
		void CreateResolver()
		{
			fakeAppDomain = new FakeAppDomain();
			fakeAddInTree = new FakeAddInTree();
			resolver = new TextTemplatingHostAppDomainAssemblyResolver(fakeAppDomain, fakeAddInTree);
		}
		
		FakeAddIn AddFakeAddInToTree(string id)
		{
			return fakeAddInTree.AddFakeAddIn(id);
		}
		
		void AddFakeRuntime(FakeAddIn fakeAddIn, string runtimeFileName)
		{
			AddFakeRuntime(fakeAddIn, runtimeFileName, null);
		}
		
		void AddFakeRuntime(FakeAddIn fakeAddIn, string runtimeFileName, Assembly assembly)
		{
			fakeAddIn.AddFakeAddInRuntime(runtimeFileName, assembly);
		}
		
		[Test]
		public void AssemblyResolve_EventFiredForAssemblyThatMatchesAddInAssembly_ReturnsAddInAssembly()
		{
			CreateResolver();
			FakeAddIn fakeAddIn = AddFakeAddInToTree("ICSharpCode.PackageManagement");
			Assembly expectedAssembly = typeof(string).Assembly;
			AddFakeRuntime(fakeAddIn, "PackageManagement.dll", expectedAssembly);
			
			string assemblyName = "PackageManagement, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.AreEqual(expectedAssembly, assembly);
		}
		

		
		[Test]
		public void Dispose_EventFiredForAssemblyThatMatchesAddInAssemblyAfter_DoesNotReturnAddInAssembly()
		{
			CreateResolver();
			FakeAddIn fakeAddIn = AddFakeAddInToTree("ICSharpCode.PackageManagement");
			Assembly expectedAssembly = typeof(string).Assembly;
			AddFakeRuntime(fakeAddIn, "PackageManagement.dll", expectedAssembly);
			
			resolver.Dispose();
			
			string assemblyName = "PackageManagement, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.IsNull(assembly);
		}
		
		[Test]
		public void AssemblyResolve_EventFiredForAssemblyThatMatchesSecondAddInAssembly_ReturnsSecondAddInAssembly()
		{
			CreateResolver();
			AddFakeAddInToTree("ICSharpCode.Test");
			FakeAddIn fakeAddIn = AddFakeAddInToTree("ICSharpCode.PackageManagement");
			Assembly expectedAssembly = this.GetType().Assembly;
			AddFakeRuntime(fakeAddIn, "PackageManagement.dll", expectedAssembly);
			
			string assemblyName = "PackageManagement, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.AreEqual(expectedAssembly, assembly);
		}
		
		[Test]
		public void AssemblyResolve_EventFiredForAssemblyThatMatchesFirstOutOfTwoAddInAssemblies_ReturnsFirstAddInAssembly()
		{
			CreateResolver();
			FakeAddIn fakeAddIn = AddFakeAddInToTree("ICSharpCode.PackageManagement");
			Assembly expectedAssembly = this.GetType().Assembly;
			AddFakeRuntime(fakeAddIn, "PackageManagement.dll", expectedAssembly);
			
			AddFakeAddInToTree("ICSharpCode.Test");
			
			string assemblyName = "PackageManagement, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.AreEqual(expectedAssembly, assembly);
		}
		
		[Test]
		public void AssemblyResolve_EventFiredForAssemblyThatDoesNotMatchAnyAddIns_ReturnsNull()
		{
			CreateResolver();
			AddFakeAddInToTree("ICSharpCode.Test");
			
			string assemblyName = "Unknown, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.IsNull(assembly);
		}
		
		[Test]
		public void AssemblyResolve_MatchedAddInHasNoRuntimes_ReturnsNull()
		{
			CreateResolver();
			AddFakeAddInToTree("ICSharpCode.Test");
			
			string assemblyName = "Test, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.IsNull(assembly);
		}
		
		[Test]
		public void AssemblyResolve_AddInAssemblyIsSecondOfThreeRuntimes_ReturnsAddInAssemblyFromSecondRuntime()
		{
			CreateResolver();
			FakeAddIn fakeAddIn = AddFakeAddInToTree("ICSharpCode.PackageManagement");
			AddFakeRuntime(fakeAddIn, ":ICSharpCode.SharpDevelop");
			Assembly expectedAssembly = this.GetType().Assembly;
			AddFakeRuntime(fakeAddIn, "PackageManagement.dll", expectedAssembly);
			AddFakeRuntime(fakeAddIn, "IronPython.dll");
			
			string assemblyName = "PackageManagement, Version=4.1.0.0, Culture=neutral, PublicKeyToken=null";
			Assembly assembly = fakeAppDomain.FireAssemblyResolveEvent(assemblyName);
			
			Assert.AreEqual(expectedAssembly, assembly);
		}
	}
}
