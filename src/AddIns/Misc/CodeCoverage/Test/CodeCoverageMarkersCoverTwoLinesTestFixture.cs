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

			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\XmlEditor\\Tests\\XmlEditor.Tests.dll\" assembly=\"XmlEditor.Tests\">\r\n" +
				"\t\t<method name=\"NoteElementHasNoChildElements\" class=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"3\" endline=\"1\" endcolumn=\"4\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"2\" column=\"4\" endline=\"3\" endcolumn=\"57\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"4\" column=\"3\" endline=\"4\" endcolumn=\"4\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
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
