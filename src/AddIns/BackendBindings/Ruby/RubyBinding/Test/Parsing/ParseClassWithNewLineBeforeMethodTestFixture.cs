// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using RubyBinding.Tests;

namespace RubyBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseClassWithNewLineBeforeMethodTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		IMethod method;
		FoldMarker methodMarker;
		FoldMarker classMarker;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Test\r\n" +
							"\r\n" +
							"\tdef foo\r\n" +
							"\tend\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);			
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
				if (c.Methods.Count > 0) {
					method = c.Methods[0];
				}
				
				// Get folds.
				ParserFoldingStrategy foldingStrategy = new ParserFoldingStrategy();
				ParseInformation parseInfo = new ParseInformation();
				parseInfo.SetCompilationUnit(compilationUnit);
			
				DocumentFactory docFactory = new DocumentFactory();
				IDocument doc = docFactory.CreateDocument();
				doc.TextContent = Ruby;
				List<FoldMarker> markers = foldingStrategy.GenerateFoldMarkers(doc, @"C:\Temp\test.rb", parseInfo);
			
				if (markers.Count > 0) {
					classMarker = markers[0];
				}
				if (markers.Count > 1) {
					methodMarker = markers[1];
				}
			}
		}
				
		[Test]
		public void ClassBodyRegion()
		{
			int startLine = 1;
			int startColumn = 11;
			int endLine = 5;
			int endColumn = 4;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The class declaration region needs to extend up to and
		/// including the colon.
		/// </summary>
		[Test]
		public void ClassRegion()
		{
			int startLine = 1;
			int startColumn = 1;
			int endLine = 5;
			int endColumn = 4;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.Region.ToString());
		}		
				
		[Test]
		public void MethodBodyRegion()
		{
			int startLine = 3;
			int startColumn = 11; // IronRuby parser includes the as part of the method parameters.
			int endLine = 4;
			int endColumn = 5;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The method region does not include the body.
		/// </summary>
		[Test]
		public void MethodRegion()
		{
			int startLine = 3;
			int startColumn = 2;
			int endLine = 3;
			int endColumn = 11; // IronRuby parser includes the as part of the method parameters
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.Region.ToString());
		}
		
		[Test]
		public void MethodFoldMarkerStartColumn()
		{
			Assert.AreEqual(8, methodMarker.StartColumn);
		}
		
		[Test]
		public void MethodFoldMarkerInnerText()
		{
			Assert.AreEqual("\r\n\tend", methodMarker.InnerText);
		}
		
		/// <summary>
		/// Note that the fold marker locations are zero based.
		/// </summary>
		[Test]
		public void ClassFoldMarkerStartColumn()
		{
			Assert.AreEqual(10, classMarker.StartColumn);
		}
		
		[Test]
		public void ClassFoldMarkerInnerText()
		{
			Assert.AreEqual("\r\n\r\n\tdef foo\r\n\tend\r\nend", classMarker.InnerText);
		}
	}
}

