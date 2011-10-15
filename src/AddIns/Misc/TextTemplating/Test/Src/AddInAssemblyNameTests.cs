// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class AddInAssemblyNameTests
	{
		AddInAssemblyName addInAssemblyName;
		
		void CreateAssemblyName(string assemblyName)
		{
			addInAssemblyName = new AddInAssemblyName(assemblyName);
		}
		
		FakeAddIn CreateFakeAddIn(string id)
		{
			return new FakeAddIn(id);
		}
		
		[Test]
		public void Matches_AssemblyNameIsPackageManagementAndAddInIsICSharpCodePackageManagement_ReturnsTrue()
		{
			CreateAssemblyName("PackageManagement, Version=4.0, Culture=neutral, PublicKeyToken=null");
			FakeAddIn addIn = CreateFakeAddIn("ICSharpCode.PackageManagement");
			
			bool matches = addInAssemblyName.Matches(addIn);
			
			Assert.IsTrue(matches);
		}
		
		[Test]
		public void Matches_AssemblyNameIsTestAndAddInIsICSharpCodePackageManagement_ReturnsFalse()
		{
			CreateAssemblyName("Test, Version=4.0, Culture=neutral, PublicKeyToken=null");
			FakeAddIn addIn = CreateFakeAddIn("ICSharpCode.PackageManagement");
			
			bool matches = addInAssemblyName.Matches(addIn);
			
			Assert.IsFalse(matches);
		}
		
		[Test]
		public void Matches_AssemblyNameIsPackageManagementAndAddInIsICSharpCodePackageManagementInDifferentCase_ReturnsTrue()
		{
			CreateAssemblyName("PackageManagement, Version=4.0, Culture=neutral, PublicKeyToken=null");
			FakeAddIn addIn = CreateFakeAddIn("icsharpcode.packagemanagement");
			
			bool matches = addInAssemblyName.Matches(addIn);
			
			Assert.IsTrue(matches);
		}
		
		[Test]
		public void Matches_NullAddInPrimaryIdentifier_ReturnsFalse()
		{
			CreateAssemblyName("Test, Version=1.0");
			FakeAddIn addIn = CreateFakeAddIn(null);
			
			bool matches = addInAssemblyName.Matches(addIn);
			
			Assert.IsFalse(matches);
		}
		
		[Test]
		public void Matches_NullAssemblyName_ReturnsFalse()
		{
			CreateAssemblyName(null);
			FakeAddIn addIn = CreateFakeAddIn("Test");
			
			bool matches = addInAssemblyName.Matches(addIn);
			
			Assert.IsFalse(matches);
		}
	}
}
