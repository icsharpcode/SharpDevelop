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
	public class RemoveCodeCoverageMarkersTestFixture
	{
		MarkerStrategy markerStrategy;
	
		[TestFixtureSetUp]
		public void SetUpTestFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			IDocument document = MockDocument.Create();
			string code = "\t\t{\r\n" +
				"\t\t\tint count = 0;\r\n" +
				"\t\t}\r\n";
			document.TextContent = code;
			markerStrategy = new MarkerStrategy(document);
			
			string xml = "<PartCoverReport>\r\n" +
				"\t<File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\"/>\r\n" +
				"\t<Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"SimpleTest\">\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"1\" sc=\"3\" el=\"1\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"1\" sl=\"2\" sc=\"4\" el=\"2\" ec=\"18\" fid=\"1\" />\r\n" +
				"\t\t\t<pt visit=\"0\" sl=\"3\" sc=\"3\" el=\"3\" ec=\"4\" fid=\"1\" />\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"</PartCoverReport>";
			CodeCoverageResults results = new CodeCoverageResults(new StringReader(xml));
			CodeCoverageMethod method = results.Modules[0].Methods[0];
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.AddMarkers(markerStrategy, method.SequencePoints);	
			
			// Add non-code coverage markers.
			markerStrategy.AddMarker(new TextMarker(0, 2, TextMarkerType.Underlined));
			markerStrategy.AddMarker(new TextMarker(4, 5, TextMarkerType.Underlined));
		}
		
		[Test]
		public void RemoveCodeCoverageMarkers()
		{
			// Check that code coverage markers exist.
			Assert.IsTrue(ContainsCodeCoverageMarkers(markerStrategy));
			
			// Remove code coverage markers.
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.RemoveMarkers(markerStrategy);
			
			// Check that code coverage markers have been removed.
			Assert.IsFalse(ContainsCodeCoverageMarkers(markerStrategy));	
			
			// Check that non-code coverage markers still exist.
			Assert.IsTrue(ContainsNonCodeCoverageMarkers(markerStrategy));
		}
		
		static bool ContainsCodeCoverageMarkers(MarkerStrategy markerStrategy)
		{			
			foreach (TextMarker marker in markerStrategy.TextMarker) {
				if (marker is CodeCoverageTextMarker) {
					return true;
				}
			}
			return false;
		}
		
		static bool ContainsNonCodeCoverageMarkers(MarkerStrategy markerStrategy)
		{			
			int count = 0;
			foreach (TextMarker marker in markerStrategy.TextMarker) {
				if (marker is CodeCoverageTextMarker) {
					return false;
				}
				count++;
			}
			return count > 0;
		}

	}
}
