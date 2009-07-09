// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class AddCodeCoverageMarkersTestFixture
	{
		ITextMarker markerOne;
		ITextMarker markerTwo;
		ITextMarker markerThree;
		IDocument document;
		ITextMarkerService markerStrategy;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			try {
				string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NCoverAddIn.Tests");
				PropertyService.InitializeService(configFolder, Path.Combine(configFolder, "data"), "NCoverAddIn.Tests");
			} catch (Exception) {}
			
			document = MockTextMarkerService.CreateDocumentWithMockService();
			string code = "\t\t{\r\n" +
				"\t\t\tint count = 0;\r\n" +
				"\t\t}\r\n";
			document.Text = code;
			markerStrategy = document.GetService(typeof(ITextMarkerService)) as ITextMarkerService;
			
			string xml = "<PartCoverReport>\r\n" +
				"<file id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t<type asm=\"Foo.Tests\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"\t\t<method name=\"SimpleTest\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"1\" fid=\"1\" sc=\"3\" el=\"1\" ec=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t\t<pt visit=\"1\" sl=\"2\" fid=\"1\" sc=\"4\" el=\"2\" ec=\"18\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t\t<pt visit=\"0\" sl=\"3\" fid=\"1\" sc=\"3\" el=\"3\" ec=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
				"</PartCoverReport>";
			CodeCoverageResults results = new CodeCoverageResults(new StringReader(xml));
			CodeCoverageMethod method = results.Modules[0].Methods[0];
			CodeCoverageHighlighter highlighter = new CodeCoverageHighlighter();
			highlighter.AddMarkers(document, method.SequencePoints);
			
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				if (markerOne == null) {
					markerOne = marker;
				} else if (markerTwo == null) {
					markerTwo = marker;
				} else if (markerThree == null) {
					markerThree = marker;
				}
			}
		}
		
		[Test]
		public void MarkerCount()
		{
			int count = 0;
			foreach (ITextMarker marker in markerStrategy.TextMarkers) {
				count++;
			}
			
			Assert.AreEqual(3, count);
		}
		
		[Test]
		public void MarkerOneOffset()
		{
			Assert.AreEqual(2, markerOne.StartOffset);
		}
		
		[Test]
		public void MarkerOneLength()
		{
			Assert.AreEqual(1, markerOne.Length);
		}
		
		[Test]
		public void MarkerOneForeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.VisitedForeColor.ToWpf(), markerOne.ForegroundColor);
		}
		
		[Test]
		public void MarkerOneColor()
		{
			Assert.AreEqual(CodeCoverageOptions.VisitedColor.ToWpf(), markerOne.BackgroundColor);
		}
		
		[Test]
		public void MarkerTwoOffset()
		{
			Assert.AreEqual(8, markerTwo.StartOffset);
		}
		
		[Test]
		public void MarkerTwoLength()
		{
			Assert.AreEqual(14, markerTwo.Length);
		}
		
		[Test]
		public void MarkerThreeForeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.NotVisitedForeColor.ToWpf(), markerThree.ForegroundColor);
		}
		
		[Test]
		public void MarkerThreeColor()
		{
			Assert.AreEqual(CodeCoverageOptions.NotVisitedColor.ToWpf(), markerThree.BackgroundColor);
		}
		
		[Test]
		public void MarkerThreeOffset()
		{
			Assert.AreEqual(26, markerThree.StartOffset);
		}
		
		[Test]
		public void MarkerThreeLength()
		{
			Assert.AreEqual(1, markerThree.Length);
		}
	}
}
