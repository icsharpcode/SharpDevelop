// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestFrameworkDescriptorIgnoresProjectFileExtensionWhitespaceTestFixture
	{
		TestFrameworkDescriptor descriptor;
		
		[SetUp]
		public void Init()
		{
			MockTestFrameworkFactory factory = new MockTestFrameworkFactory();
			Properties properties = new Properties();
			properties["id"] = "nunit";
			properties["supportedProjects"] = "  .csproj;  .vbproj  ";
			
			descriptor = new TestFrameworkDescriptor(properties, factory);
		}
		
		[Test]
		public void IsSupportedProjectReturnsTrueForCSharpProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\myproj.csproj";
			
			Assert.IsTrue(descriptor.IsSupportedProject(project));
		}
		
		[Test]
		public void IsSupportedProjectReturnsTrueForVBNetProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\myproj.vbproj";
			
			Assert.IsTrue(descriptor.IsSupportedProject(project));
		}
	}
}
