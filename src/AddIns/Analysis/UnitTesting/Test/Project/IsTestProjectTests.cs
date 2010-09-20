// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// A project is considered to be a test project if it contains
	/// a reference to the NUnit.Framework assembly.
	/// </summary>
	[TestFixture]
	public class IsTestProjectTests
	{
		[Test]
		public void ProjectWithNoReferences()
		{
			IProject project = new MockCSharpProject();
			Assert.IsFalse(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReference()
		{
			IProject project = new MockCSharpProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "NUnit.Framework";
			ProjectService.AddProjectItem(project, referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReferenceCaseInsensitive()
		{
			IProject project = new MockCSharpProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "nunit.framework";
			ProjectService.AddProjectItem(project, referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReferenceSpecificVersion()
		{
			IProject project = new MockCSharpProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "NUnit.Framework, Version=2.2.8.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77";
			ProjectService.AddProjectItem(project, referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void NullReferenceName()
		{
			Assert.IsFalse(TestProject.IsTestFrameworkReference(null));
		}
		
		[Test]
		public void NullProject()
		{
			Assert.IsFalse(TestProject.IsTestProject(null));
		}
	}
}
