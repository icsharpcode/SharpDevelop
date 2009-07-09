// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Tests.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class CodeCoverageMarkersInvalidStartColumnTestFixture
	{
		List<ITextMarker> markers;
		
		[SetUp]
		public void Init()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			IDocument document = MockTextMarkerService.CreateDocumentWithMockService();
			ITextMarkerService markerStrategy = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			document.Text = "abcdefg\r\nabcdefg";
			
			string xml = "<PartCoverReport>\r\n" +
				"\t<file id=\"1\" url=\"c:\\Projects\\Test\\Test.cs\" />\r\n" +
				"\t<type name=\"XmlEditor.Tests.Schema.SingleElementSchemaTestFixture\" asm=\"XmlEditor.Tests\">\r\n" +
				"\t\t<method name=\"GetSchema\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"1\" sc=\"0\" el=\"1\" ec=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"1\" sc=\"-1\" el=\"1\" ec=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"1\" sc=\"8\" el=\"1\" ec=\"20\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
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
		public void NoMarkersAdded()
		{
			Assert.AreEqual(0, markers.Count, 
				"Should not be any markers added since all sequence point start columns are invalid.");
		}
	}
}
