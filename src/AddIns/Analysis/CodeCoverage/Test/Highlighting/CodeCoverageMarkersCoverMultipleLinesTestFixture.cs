// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Highlighting
{
	[TestFixture]
	public class CodeCoverageMarkersCoverMultipleLinesTestFixture
	{
		List<ITextMarker> markers;
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			document = MockTextMarkerService.CreateDocumentWithMockService();
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
			document.Text = code;
			ITextMarkerService markerStrategy = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
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
			highlighter.AddMarkers(document, method.SequencePoints);

			markers = new List<ITextMarker>();
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				markers.Add(marker);
			}
		}
		
		[Test]
		public void MarkerCount()
		{
			Assert.AreEqual(3, markers.Count);
		}
		
		[Test]
		public void FirstMarkerPosition()
		{
			Assert.AreEqual(new Location(3, 1), document.OffsetToPosition(markers[0].StartOffset));
			Assert.AreEqual(new Location(4, 1), document.OffsetToPosition(markers[0].EndOffset));
		}
		
		[Test]
		public void SecondMarkerPosition()
		{
			Assert.AreEqual(new Location(4, 2), document.OffsetToPosition(markers[1].StartOffset));
			Assert.AreEqual(new Location(20, 9), document.OffsetToPosition(markers[1].EndOffset));
		}
		
		[Test]
		public void ThirdMarkerPosition()
		{
			Assert.AreEqual(new Location(3, 10), document.OffsetToPosition(markers[2].StartOffset));
			Assert.AreEqual(new Location(4, 10), document.OffsetToPosition(markers[2].EndOffset));
		}
	}
}
