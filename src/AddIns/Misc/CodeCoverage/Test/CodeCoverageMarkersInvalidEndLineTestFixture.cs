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
	public class CodeCoverageMarkersInvalidEndLineTestFixture
	{
		List<CodeCoverageTextMarker> markers;
		
		[SetUp]
		public void Init()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			MockDocument document = new MockDocument();
			// Give doc 3 lines (end line seems to be counted as an extra line).
			document.AddLines("abc\r\ndef"); 
			MarkerStrategy markerStrategy = new MarkerStrategy(document);
			
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\XmlEditor\\Test\\XmlEditor.Tests.dll\" assembly=\"XmlEditor.Tests\">\r\n" +
				"\t\t<method name=\"GetSchema\" class=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"3\" column=\"3\" endline=\"4\" endcolumn=\"4\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"4\" endline=\"5\" endcolumn=\"20\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"4\" endline=\"-1\" endcolumn=\"20\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"4\" endline=\"0\" endcolumn=\"20\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
			
			CodeCoverageResults results = new CodeCoverageResults(new StringReader(xml));
			CodeCoverageMethod method = results.Modules[0].Methods[0];
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.AddMarkers(markerStrategy, method.SequencePoints);
			
			markers = new List<CodeCoverageTextMarker>();
			foreach (CodeCoverageTextMarker marker in markerStrategy.TextMarker) {
				markers.Add(marker);
			}
		}
		
		[Test]
		public void NoMarkersAdded()
		{
			Assert.AreEqual(0, markers.Count, 
				"Should not be any markers added since all sequence point end lines are invalid.");
		}
	}
}
