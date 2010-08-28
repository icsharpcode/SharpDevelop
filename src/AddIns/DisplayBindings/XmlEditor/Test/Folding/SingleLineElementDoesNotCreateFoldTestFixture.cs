// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class SingleLineElementDoesNotCreateFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<root>\r\n" +
				"    <child></child>\r\n" +
				"</root>";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
		}
		
		[Test]
		public void GetFolds_ChildElementOnSingleLine_FirstFoldRegionCoversRootElement()
		{
			DomRegion region = helper.GetFirstFoldRegion();
			
			int beginLine = 1;
			int endLine = 3;
			int beginCol = 1;
			int endCol = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginCol, endLine, endCol);
			
			Assert.AreEqual(expectedRegion, region);
		}
		
		[Test]
		public void GetFolds_ChildElementOnSingleLine_ReturnsOneFold()
		{
			int count = helper.Folds.Count;
			Assert.AreEqual(1, count);
		}
	}
}
