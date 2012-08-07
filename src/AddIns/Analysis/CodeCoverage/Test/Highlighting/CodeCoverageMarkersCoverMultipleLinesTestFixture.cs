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
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"2\" sc=\"4\" el=\"9\" ec=\"20\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"10\" sc=\"3\" el=\"10\" ec=\"4\" />\r\n" +
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
