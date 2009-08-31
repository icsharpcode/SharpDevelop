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
			
			IDocument document = MockDocument.Create();
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
			document.TextContent = code;
			MarkerStrategy markerStrategy = new MarkerStrategy(document);
			
			string xml = "<PartCoverReport>\r\n" +
				"\t<File id=\"1\" url=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\" />\r\n" +
				"\t<Assembly id=\"1\" name=\"XmlEditor.Tests\" module=\"C:\\Projects\\Test\\XmlEditor.Tests\\bin\\XmlEditor.Tests.DLL\" domain=\"test-domain-XmlEditor.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type asmref=\"XmlEditor.Tests\" name=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\" flags=\"1232592\">\r\n" +
				"\t\t<Method name=\"GetSchema\">\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"1\" sc=\"3\" el=\"1\" ec=\"4\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"2\" sc=\"4\" el=\"9\" ec=\"20\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"10\" sc=\"3\" el=\"10\" ec=\"4\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"</PartCoverReport>";
			
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
