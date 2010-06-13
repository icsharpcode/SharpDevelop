// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestFrameworkDescriptorSupportsCSharpProjectsTestFixture
	{
		TestFrameworkDescriptor descriptor;
		
		[SetUp]
		public void Init()
		{
			MockTestFrameworkFactory factory = new MockTestFrameworkFactory();
			Properties properties = new Properties();
			properties["id"] = "nunit";
			properties["supportedProjects"] = ".csproj";
			
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
		public void IsSupportedProjectReturnsFalseForVBNetProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\myproj.vbproj";
			
			Assert.IsFalse(descriptor.IsSupportedProject(project));
		}
		
		[Test]
		public void IsSupportedProjectReturnsFalseForNullProject()
		{
			Assert.IsFalse(descriptor.IsSupportedProject(null));
		}
		
		[Test]
		public void IsSupportedProjectReturnsTrueForCSharpProjectFileExtensionInUpperCase()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.FileName = @"d:\projects\myproj.CSPROJ";
			
			Assert.IsTrue(descriptor.IsSupportedProject(project));
		}
	}
}
