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
	public class CodeCoverageMarkersInvalidStartLineTestFixture
	{
		List<CodeCoverageTextMarker> markers;
		
		[SetUp]
		public void Init()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			IDocument document = MockDocument.Create();
			MarkerStrategy markerStrategy = new MarkerStrategy(document);
			
			string xml = "<PartCoverReport>\r\n" +
				"\t<file id=\"1\" url=\"c:\\Projects\\XmlEditor\\Test\\Schema\\SingleElementSchemaTestFixture.cs\"/>\r\n" +
				"\t<type name=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\" asm=\"XmlEditor.Tests\">\r\n" +
				"\t\t<method name=\"GetSchema\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"0\" sc=\"3\" el=\"0\" ec=\"4\" />\r\n" +
				"\t\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"-1\" sc=\"4\" el=\"9\" ec=\"20\" />\r\n" +
				"\t\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"10\" sc=\"3\" el=\"10\" ec=\"4\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
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
		public void NoMarkersAdded()
		{
			Assert.AreEqual(0, markers.Count, 
				"Should not be any markers added since all sequence point start lines are invalid.");
		}
	}
}
