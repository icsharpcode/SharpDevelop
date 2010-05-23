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
	public class BuildProjectBeforeTestRunTestFixture
	{
		BuildProjectBeforeTestRun buildProjectBeforeTestRun;
		MockSaveAllFilesCommand saveAllFilesCommand;
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeServiceForUnitTests();
			}
			saveAllFilesCommand = new MockSaveAllFilesCommand();
			project = new MockCSharpProject();
			BuildResults buildResults = new BuildResults();
			
			buildProjectBeforeTestRun = new BuildProjectBeforeTestRun(project, saveAllFilesCommand);
		}
		
		[Test]
		public void SaveAllFilesCommandNotCalledInitially()
		{
			Assert.IsFalse(saveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
		
		[Test]
		public void BeforeBuildMethodSavesAllFiles()
		{
			buildProjectBeforeTestRun.BeforeBuild();
			Assert.IsTrue(saveAllFilesCommand.IsSaveAllFilesMethodCalled);
		}
	}
}
