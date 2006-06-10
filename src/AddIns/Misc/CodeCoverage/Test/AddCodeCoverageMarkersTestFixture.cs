// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.CodeCoverage;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class AddCodeCoverageMarkersTestFixture
	{
		MarkerStrategy markerStrategy;
		CodeCoverageTextMarker markerOne;
		CodeCoverageTextMarker markerTwo;
		CodeCoverageTextMarker markerThree;
		MockDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			document = new MockDocument();
			string code = "\t\t{\r\n" +
				"\t\t\tint count = 0;\r\n" +
				"\t\t}\r\n";
			document.AddLines(code);
			markerStrategy = new MarkerStrategy(document);
			
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"3\" endline=\"1\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"2\" column=\"4\" endline=\"2\" endcolumn=\"18\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"3\" column=\"3\" endline=\"3\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
			CodeCoverageResults results = new CodeCoverageResults(new StringReader(xml));
			CodeCoverageMethod method = results.Modules[0].Methods[0];
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.AddMarkers(markerStrategy, method.SequencePoints);
			
			foreach (CodeCoverageTextMarker marker in markerStrategy.TextMarker) {
				if (markerOne == null) {
					markerOne = marker;
				} else if (markerTwo == null) {
					markerTwo = marker;
				} else if (markerThree == null) {
					markerThree = marker;
				}
			}
		}
		
		[Test]
		public void MarkerCount()
		{
			int count = 0;
			foreach (CodeCoverageTextMarker marker in markerStrategy.TextMarker) {
				count++;
			}
			
			Assert.AreEqual(3, count);
		}
		
		[Test]
		public void MarkerOneOffset()
		{
			Assert.AreEqual(2, markerOne.Offset);
		}
		
		[Test]
		public void MarkerOneLength()
		{
			Assert.AreEqual(1, markerOne.Length);
		}
		
		[Test]
		public void MarkerOneType()
		{
			Assert.AreEqual(TextMarkerType.SolidBlock, markerOne.TextMarkerType);
		}
		
		[Test]
		public void MarkerOneForeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.VisitedForeColor, markerOne.ForeColor);
		}
		
		[Test]
		public void MarkerOneColor()
		{
			Assert.AreEqual(CodeCoverageOptions.VisitedColor, markerOne.Color);
		}
		
		[Test]
		public void MarkerTwoOffset()
		{
			Assert.AreEqual(8, markerTwo.Offset);
		}
		
		[Test]
		public void MarkerTwoLength()
		{
			Assert.AreEqual(14, markerTwo.Length);
		}
		
		[Test]
		public void MarkerThreeForeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.NotVisitedForeColor, markerThree.ForeColor);
		}
		
		[Test]
		public void MarkerThreeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.NotVisitedColor, markerThree.Color);
		}
		
		[Test]
		public void MarkerThreeOffset()
		{
			Assert.AreEqual(26, markerThree.Offset);
		}
		
		[Test]
		public void MarkerThreeLength()
		{
			Assert.AreEqual(1, markerThree.Length);
		}
	}
}
