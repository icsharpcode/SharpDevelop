// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class RunProjectTestsInPadTestFixture
	{
		RunProjectTestsInPadCommand runProjectTestsInPadCommand;
		
		[SetUp]
		public void Init()
		{
			MockRunTestCommandContext context = new MockRunTestCommandContext();
			runProjectTestsInPadCommand = new RunProjectTestsInPadCommand(context);
			runProjectTestsInPadCommand.Owner = this;
			runProjectTestsInPadCommand.Run();
		}
		
		[Test]
		public void OwnerIsSetToCommandWhenRunMethodIsCalled()
		{
			Assert.AreEqual(runProjectTestsInPadCommand, runProjectTestsInPadCommand.Owner);
		}
			
		[Test]
		public void SelectedMethodIsNull()
		{
			Assert.IsNull(runProjectTestsInPadCommand.SelectedMethod);
		}
		
		[Test]
		public void SelectedClassIsNull()
		{
			Assert.IsNull(runProjectTestsInPadCommand.SelectedClass);
		}
		
		[Test]
		public void SelectedNamespaceIsNull()
		{
			Assert.IsNull(runProjectTestsInPadCommand.SelectedNamespace);
		}
		
		[Test]
		public void SelectedProjectMatchesProjectServiceCurrentProject()
		{
			MockCSharpProject project = new MockCSharpProject();
			ProjectService.CurrentProject = project;
			
			Assert.AreEqual(project, runProjectTestsInPadCommand.SelectedProject);
		}
	}
}
