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
			
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"1\" column=\"3\" endline=\"1\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"2\" column=\"4\" endline=\"2\" endcolumn=\"18\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"3\" column=\"3\" endline=\"3\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
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
