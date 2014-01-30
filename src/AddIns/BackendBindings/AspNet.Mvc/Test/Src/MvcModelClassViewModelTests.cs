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
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcModelClassViewModelTests
	{
		FakeMvcClass fakeClass;
		MvcModelClassViewModel viewModel;
		
		void CreateViewModel(string classNamespace, string className)
		{
			fakeClass = new FakeMvcClass(classNamespace, className);
			viewModel = new MvcModelClassViewModel(fakeClass);
		}
		
		void CreateViewModel(string fullyQualifiedClassName)
		{
			fakeClass = new FakeMvcClass(fullyQualifiedClassName);
			viewModel = new MvcModelClassViewModel(fakeClass);	
		}
		
		[Test]
		public void Name_ClassHasNamespaceAndName_ReturnsClassNameFollowedByNamespace()
		{
			CreateViewModel("ICSharpCode.Tests", "MyClass");
			string text = viewModel.Name;
			
			Assert.AreEqual("MyClass (ICSharpCode.Tests)", text);
		}
		
		[Test]
		public void ToString_ClassHasNamespaceAndName_ReturnsClassNameFollowedByNamespace()
		{
			CreateViewModel("ICSharpCode.Tests", "MyClass");
			string text = viewModel.ToString();
			
			Assert.AreEqual("MyClass (ICSharpCode.Tests)", text);
		}
		
		[Test]
		public void FullName_ClassHasNamespaceAndName_ReturnsFullyQualifiedClassName()
		{
			CreateViewModel("ICSharpCode.Tests.MyClass");
			string name = viewModel.FullName;
			
			Assert.AreEqual("ICSharpCode.Tests.MyClass", name);
		}
		
		[Test]
		public void Name_ClassNameButHasEmptyNamespace_ReturnsClassNameOnly()
		{
			CreateViewModel(String.Empty, "MyClass");
			string text = viewModel.Name;
			
			Assert.AreEqual("MyClass", text);
		}
		
		[Test]
		public void AssemblyLocation_ClassHasAssemblyLocation_ReturnsClassAssemblyLocation()
		{
			CreateViewModel("Test.MyClass");
			fakeClass.AssemblyLocation = "TestAssemblyLocation";
			
			Assert.AreEqual("TestAssemblyLocation", viewModel.AssemblyLocation);
		}
	}
}
