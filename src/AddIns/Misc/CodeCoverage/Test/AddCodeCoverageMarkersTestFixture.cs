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
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			document = MockDocument.Create();
			string code = "\t\t{\r\n" +
				"\t\t\tint count = 0;\r\n" +
				"\t\t}\r\n";
			document.TextContent = code;
			markerStrategy = new MarkerStrategy(document);
			
			string xml = "<PartCoverReport>\r\n" +
				"<File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"<Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type asmref=\"1\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"\t\t<Method name=\"SimpleTest\">\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"1\" fid=\"1\" sc=\"3\" el=\"1\" ec=\"4\" document=\"c:\\Projects\\Foo\\Foo1TestFixture.cs\" />\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"2\" fid=\"1\" sc=\"4\" el=\"2\" ec=\"18\" document=\"c:\\Projects\\Foo\\Foo1TestFixture.cs\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"3\" fid=\"1\" sc=\"3\" el=\"3\" ec=\"4\" document=\"c:\\Projects\\Foo\\Foo1TestFixture.cs\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"</PartCoverReport>";
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
