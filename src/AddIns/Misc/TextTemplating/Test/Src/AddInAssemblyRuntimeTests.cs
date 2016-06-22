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
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class AddInAssemblyRuntimeTests
	{
		AddInAssemblyRuntime runtime;
		
		void CreateRuntime(string id)
		{
			var fakeAddIn = CreateFakeAddIn(id);
			CreateRuntime(fakeAddIn);
		}
		
		FakeAddIn CreateFakeAddIn(string id)
		{
			return new FakeAddIn(id);
		}
		
		void CreateRuntime(FakeAddIn fakeAddIn)
		{
			runtime = new AddInAssemblyRuntime(fakeAddIn);
		}
		
		[Test]
		public void FileName_AddInIdIsICSharpCodePackageManagement_ReturnsPackageManagementDll()
		{
			CreateRuntime("ICSharpCode.PackageManagement");
			
			string fileName = runtime.FileName;
			
			Assert.AreEqual("PackageManagement.dll", fileName);
		}
		
		[Test]
		public void FileName_AddInIdIsICSharpCodePackageManagementInLowerCase_ReturnsPackageManagementDll()
		{
			CreateRuntime("icsharpcode.packageManagement");
			
			string fileName = runtime.FileName;
			
			Assert.AreEqual("packageManagement.dll", fileName);
		}
		
		[Test]
		public void FileName_AddInIdIsMyAddIn_ReturnsMyAddInDll()
		{
			CreateRuntime("MyAddIn");
			
			string fileName = runtime.FileName;
			
			Assert.AreEqual("MyAddIn.dll", fileName);
		}
		
		[Test]
		public void Matches_FileNameMatchesAddInRuntimeFileName_ReturnsTrue()
		{
			CreateRuntime("ICSharpCode.PackageManagement");
			
			bool match = runtime.Matches("PackageManagement.dll");
			
			Assert.IsTrue(match);
		}
		
		[Test]
		public void Matches_FileNameDoesNotMatchAddInRuntimeFileName_ReturnsFalse()
		{
			CreateRuntime("ICSharpCode.PackageManagement");
			
			bool match = runtime.Matches("test.dll");
			
			Assert.IsFalse(match);
		}
		
		[Test]
		public void Matches_FileNameInDifferentCaseToAddInRuntimeFileName_ReturnsTrue()
		{
			CreateRuntime("ICSharpCode.PackageManagement");
			
			bool match = runtime.Matches("packagemanagement.dll");
			
			Assert.IsTrue(match);
		}
		
		[Test]
		public void Runtime_AddInHasMatchingAddInRuntime_ReturnsAddInRuntime()
		{
			FakeAddIn fakeAddIn = CreateFakeAddIn("ICSharpCode.PackageManagement");
			FakeAddInRuntime expectedRuntime = fakeAddIn.AddFakeAddInRuntime("PackageManagement.dll");
			CreateRuntime(fakeAddIn);
			
			IAddInRuntime addInRuntime = runtime.Runtime;
			
			Assert.AreEqual(expectedRuntime, addInRuntime);
		}
		
		[Test]
		public void Runtime_AddInHasThreeRuntimesWithSecondOneMatchingAddInRuntime_ReturnsSecondAddInRuntime()
		{
			FakeAddIn fakeAddIn = CreateFakeAddIn("ICSharpCode.PackageManagement");
			fakeAddIn.AddFakeAddInRuntime("Test.dll");
			FakeAddInRuntime expectedRuntime = fakeAddIn.AddFakeAddInRuntime("PackageManagement.dll");
			fakeAddIn.AddFakeAddInRuntime("AnotherTest.dll");
			CreateRuntime(fakeAddIn);
			
			IAddInRuntime addInRuntime = runtime.Runtime;
			
			Assert.AreEqual(expectedRuntime, addInRuntime);
		}
		
		[Test]
		public void Runtime_AddInHasMatchingAddInRuntimeButWthDifferentCase_ReturnsAddInRuntime()
		{
			FakeAddIn fakeAddIn = CreateFakeAddIn("ICSharpCode.PackageManagement");
			FakeAddInRuntime expectedRuntime = fakeAddIn.AddFakeAddInRuntime("packageManagement.dll");
			CreateRuntime(fakeAddIn);
			
			IAddInRuntime addInRuntime = runtime.Runtime;
			
			Assert.AreEqual(expectedRuntime, addInRuntime);
		}
	}
}
