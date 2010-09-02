// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Testing
{
	[TestFixture]
	public class PythonTestFrameworkIsTestProjectTests
	{
		PythonTestFramework testFramework;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MSBuildEngineHelper.InitMSBuildEngine();
		}
		
		[SetUp]
		public void Init()
		{
			testFramework = new PythonTestFramework();
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNull()
		{
			Assert.IsFalse(testFramework.IsTestProject(null));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForPythonProject()
		{
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution();
			createInfo.OutputProjectFileName = @"C:\projects\test.pyproj";
			PythonProject project = new PythonProject(createInfo);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNonPythonProject()
		{
			MockProject project = new MockProject();
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNullPythonProject()
		{
			PythonProject project = null;
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
	}
}
