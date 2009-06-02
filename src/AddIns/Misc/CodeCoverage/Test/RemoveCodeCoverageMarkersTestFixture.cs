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
	public class RemoveCodeCoverageMarkersTestFixture
	{
		IDocument document;
		ITextMarkerService markerStrategy;
	
		[TestFixtureSetUp]
		public void SetUpTestFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			document = MockTextMarkerService.CreateDocumentWithMockService();
			markerStrategy = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			string code = "\t\t{\r\n" +
				"\t\t\tint count = 0;\r\n" +
				"\t\t}\r\n";
			document.Text = code;
			
			string xml = "<PartCoverReport>\r\n" +
				"\t<file id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\"/>\r\n" +
				"\t<type name=\"Foo.Tests.FooTestFixture\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"1\" sc=\"3\" el=\"1\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"2\" sc=\"4\" el=\"2\" ec=\"18\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"3\" sc=\"3\" el=\"3\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"</PartCoverReport>";
			CodeCoverageResults results = new CodeCoverageResults(new StringReader(xml));
			CodeCoverageMethod method = results.Modules[0].Methods[0];
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.AddMarkers(document, method.SequencePoints);	
			
			// Add non-code coverage markers.
			markerStrategy.Create(0, 2);
			markerStrategy.Create(4, 5);
		}
		
		[Test]
		public void RemoveCodeCoverageMarkers()
		{
			// Check that code coverage markers exist.
			Assert.IsTrue(ContainsCodeCoverageMarkers(markerStrategy));
			
			// Remove code coverage markers.
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.RemoveMarkers(document);
			
			// Check that code coverage markers have been removed.
			Assert.IsFalse(ContainsCodeCoverageMarkers(markerStrategy));	
			
			// Check that non-code coverage markers still exist.
			Assert.IsTrue(ContainsNonCodeCoverageMarkers(markerStrategy));
		}
		
		static bool ContainsCodeCoverageMarkers(ITextMarkerService markerStrategy)
		{			
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				if (marker.Tag == typeof(CodeCoverageHighlighter)) {
					return true;
				}
			}
			return false;
		}
		
		static bool ContainsNonCodeCoverageMarkers(ITextMarkerService markerStrategy)
		{			
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				if (marker.Tag != typeof(CodeCoverageHighlighter)) {
					return true;
				}
			}
			return false;
		}

	}
}
