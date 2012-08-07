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
			
			string xml = "<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"\t<Modules>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>Foo.Tests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"1\" fullPath=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>Foo.Tests.FooTestFixture</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void Foo.Tests.FooTestFixture::SimpleTest()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"1\" sc=\"3\" el=\"1\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"2\" sc=\"4\" el=\"2\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"3\" sc=\"3\" el=\"3\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t</Classes>\r\n" +
				"\t\t</Module>\r\n" +
				"\t</Modules>\r\n" +
				"</CoverageSession>";
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
