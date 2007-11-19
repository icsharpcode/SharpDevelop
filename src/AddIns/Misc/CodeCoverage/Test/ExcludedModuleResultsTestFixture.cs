//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
////     <version>$Revision$</version>
//// </file>
//
//using ICSharpCode.CodeCoverage;
//using NUnit.Framework;
//using System;
//using System.IO;
//
//namespace ICSharpCode.CodeCoverage.Tests
//{
//	/// <summary>
//	/// Tests that results with the excluded attribute set are not included
//	/// in the code coverage results.
//	/// </summary>
//	[TestFixture]
//	public class ExcludedModuleResultsTestFixture
//	{
//		CodeCoverageResults results;
//		
//		[TestFixtureSetUp]
//		public void SetUpFixture()
//		{
//			string xml = "<coverage>\r\n" +
//				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
//				"\t\t<method name=\"SimpleTest\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
//				"\t\t\t<seqpnt visitcount=\"0\" line=\"20\" column=\"3\" endline=\"20\" excluded=\"true\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
//				"\t\t\t<seqpnt visitcount=\"0\" line=\"21\" column=\"13\" endline=\"21\" excluded=\"true\" endcolumn=\"32\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
//				"\t\t\t<seqpnt visitcount=\"0\" line=\"24\" column=\"3\" endline=\"24\" excluded=\"true\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
//				"\t\t</method>\r\n" +
//				"\t</module>\r\n" +
//				"</coverage>";
//			results = new CodeCoverageResults(new StringReader(xml));
//		}
//		
//		[Test]
//		public void NoModules()
//		{
//			Assert.AreEqual(0, results.Modules.Count, "All modules should be excluded");
//		}
//	}
//}
