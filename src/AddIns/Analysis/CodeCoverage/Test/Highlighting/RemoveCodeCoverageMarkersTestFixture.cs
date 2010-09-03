// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Tests.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.CodeCoverage;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Highlighting
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
				if ((Type)marker.Tag == typeof(CodeCoverageHighlighter)) {
					return true;
				}
			}
			return false;
		}
		
		static bool ContainsNonCodeCoverageMarkers(ITextMarkerService markerStrategy)
		{			
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				if ((Type)marker.Tag != typeof(CodeCoverageHighlighter)) {
					return true;
				}
			}
			return false;
		}

	}
}
