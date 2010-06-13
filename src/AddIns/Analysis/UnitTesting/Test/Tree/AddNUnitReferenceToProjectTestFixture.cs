// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class AddNUnitReferenceToProjectTestFixture
	{
		MockCSharpProject project;
		ReferenceProjectItem referenceProjectItem;
		AddNUnitReferenceCommand command = new AddNUnitReferenceCommand();
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			
			command.Run(project);
			
			if (project.Items.Count > 0) {
				referenceProjectItem = project.Items[0] as ReferenceProjectItem;
			}
		}
		
		[Test]
		public void ProjectHasNUnitFrameworkReferenceAdded()
		{
			Assert.AreEqual("nunit.framework", referenceProjectItem.Name);
		}
		
		[Test]
		public void NUnitReferenceProjectPropertyEqualsProjectPassedToRunMethod()
		{
			Assert.AreEqual(project, referenceProjectItem.Project);
		}
		
		[Test]
		public void ProjectIsSaved()
		{
			Assert.IsTrue(project.IsSaved);
		}
		
		[Test]
		public void NoExceptionThrownWhenProjectIsNull()
		{
			Assert.DoesNotThrow(delegate { command.Run(null); });
		}
	}
}
