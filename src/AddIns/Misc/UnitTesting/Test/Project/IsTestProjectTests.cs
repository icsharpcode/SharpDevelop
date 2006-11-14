// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;

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
			MSBuildProject project = new MSBuildProject();
			Assert.IsFalse(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReference()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "NUnit.Framework";
			project.Items.Add(referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReferenceCaseInsensitive()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "nunit.framework";
			project.Items.Add(referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void ProjectWithNUnitFrameworkReferenceSpecificVersion()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = "NUnit.Framework, Version=2.2.8.0, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77";
			project.Items.Add(referenceProjectItem);
			
			Assert.IsTrue(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void NullReferenceName()
		{
			MSBuildProject project = new MSBuildProject();
			ReferenceProjectItem referenceProjectItem = new ReferenceProjectItem(project);
			referenceProjectItem.Include = null;
			project.Items.Add(referenceProjectItem);
			
			Assert.IsFalse(TestProject.IsTestProject(project));
		}
		
		[Test]
		public void NullProject()
		{
			Assert.IsFalse(TestProject.IsTestProject(null));
		}
	}
}
