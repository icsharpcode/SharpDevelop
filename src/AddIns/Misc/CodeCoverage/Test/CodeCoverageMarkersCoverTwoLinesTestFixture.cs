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
	public class CodeCoverageMarkersCoverTwoLinesTestFixture
	{
		CodeCoverageTextMarker markerOne;
		CodeCoverageTextMarker markerTwo;
		CodeCoverageTextMarker markerThree;
		CodeCoverageTextMarker markerFour;
	
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			IDocument document = MockDocument.Create();
			string code = "\t\t{\r\n" +
				"\t\t\tAssert.AreEqual(0, childElementCompletionData.Length, \"\" +\r\n" +
				"\t\t\t                \"Not expecting any child elements.\");\r\n" +
				"\t\t}\r\n";
			document.TextContent = code;
			MarkerStrategy markerStrategy = new MarkerStrategy(document);

			string xml = "<PartCoverReport>\r\n" +
				"\t<file id=\"1\" url=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t<Assembly id=\"1\" name=\"XmlEditor.Tests\" module=\"C:\\Projects\\Test\\XmlEditor.Tests\\bin\\XmlEditor.Tests.DLL\" domain=\"test-domain-XmlEditor.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type asmref=\"1\" name=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\">\r\n" +
				"\t\t<Method name=\"NoteElementHasNoChildElements\">\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"1\" sc=\"3\" el=\"1\" ec=\"4\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"2\" sc=\"4\" el=\"3\" ec=\"57\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"4\" sc=\"3\" el=\"4\" ec=\"4\" />\r\n" +
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
				} else if (markerFour == null) {
					markerFour = marker;
				}
			}
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
		public void MarkerTwoOffset()
		{
			Assert.AreEqual(8, markerTwo.Offset);
		}
		
		[Test]
		public void MarkerTwoLength()
		{
			Assert.AreEqual(58, markerTwo.Length);
		}
		
		[Test]
		public void MarkerThreeOffset()
		{
			Assert.AreEqual(68, markerThree.Offset);
		}
		
		[Test]
		public void MarkerThreeLength()
		{
			Assert.AreEqual(56, markerThree.Length);
		}
		
		[Test]
		public void MarkerFourLength()
		{
			Assert.AreEqual(1, markerFour.Length);
		}
		
		[Test]
		public void MarkerFourOffset()
		{
			Assert.AreEqual(128, markerFour.Offset);
		}
	}
}
