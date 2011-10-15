// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
