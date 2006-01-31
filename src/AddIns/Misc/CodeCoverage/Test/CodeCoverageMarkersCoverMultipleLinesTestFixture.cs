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
	public class CodeCoverageMarkersCoverMultipleLinesTestFixture
	{
		List<CodeCoverageTextMarker> markers;
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			MockDocument document = new MockDocument();
			string code = "\t\t{\r\n" +
				"\t\t\treturn \"<?xml version=\\\"1.0\\\"?>\\r\\n\" +\r\n" +
				"\t\t\t\t\"<xs:schema xmlns:xs=\\\"http://www.w3.org/2001/XMLSchema\\\"\\r\\n\" +\r\n" +
				"\t\t\t\t\"targetNamespace=\\\"http://www.w3schools.com\\\"\\r\\n\" +\r\n" +
				"\t\t\t\t\"xmlns=\\\"http://www.w3schools.com\\\"\\r\\n\" +\r\n" +
				"\t\t\t\t\"elementFormDefault=\\\"qualified\\\">\\r\\n\" +\r\n" +
				"\t\t\t\t\"<xs:element name=\\\"note\\\">\\r\\n\" +\r\n" +
				"\t\t\t\t\"</xs:element>\\r\\n\" +\r\n" +
				"\t\t\t\t\"</xs:schema>\";\r\n" +
				"\t\t}\r\n";
			document.AddLines(code);
			MarkerStrategy markerStrategy = new MarkerStrategy(document);
			
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\XmlEditor\\Test\\XmlEditor.Tests.dll\" assembly=\"XmlEditor.Tests\">\r\n" +
				"\t\t<method name=\"GetSchema\" class=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"3\" endline=\"1\" endcolumn=\"4\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"2\" column=\"4\" endline=\"9\" endcolumn=\"20\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"10\" column=\"3\" endline=\"10\" endcolumn=\"4\" document=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
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
		public void MarkerCount()
		{
			Assert.AreEqual(10, markers.Count);
		}
		
		[Test]
		public void MarkerThreeOffset()
		{
			Assert.AreEqual(48, markers[2].Offset);
		}
		
		[Test]
		public void MarkerFourOffset()
		{
			Assert.AreEqual(118, markers[3].Offset);
		}
		
		[Test]
		public void MarkerNineOffset()
		{
			Assert.AreEqual(338, markers[8].Offset);
		}
	}
}
