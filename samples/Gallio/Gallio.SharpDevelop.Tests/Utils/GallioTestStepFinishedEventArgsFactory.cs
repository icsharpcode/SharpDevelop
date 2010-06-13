// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using Gallio.Model.Schema;
using Gallio.Runner.Events;
using Gallio.Runner.Reports.Schema;

namespace Gallio.SharpDevelop.Tests.Utils
{
	public class GallioTestStepFinishedEventArgsFactory
	{
		TestData gallioTest;
		Report gallioReport = new Report();
		TestStepRun gallioTestStepRun;
		
		public TestStepFinishedEventArgs Create(string name)
		{
			CreateTestData(name);
			CreateTestStepRun();
			return CreateTestStepFinishedEventArgs();
		}
		
		void CreateTestData(string name)
		{
			string gallioTestName = ConvertToGallioTestName(name);
			gallioTest = new TestData("a", "b", gallioTestName);
			gallioTest.IsTestCase = true;
		}
		
		void CreateTestStepRun()
		{
			TestStepData testStepData = new TestStepData("a", "b", "c", "d");
			gallioTestStepRun = new TestStepRun(testStepData);
		}
		
		string ConvertToGallioTestName(string name)
		{
			return name.Replace('.', '/');
		}
		
		TestStepFinishedEventArgs CreateTestStepFinishedEventArgs()
		{
			return new TestStepFinishedEventArgs(gallioReport, gallioTest, gallioTestStepRun);
		}
	}
}
