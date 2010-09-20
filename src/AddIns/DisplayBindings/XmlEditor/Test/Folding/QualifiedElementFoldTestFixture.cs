// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class QualifiedElementFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<xs:schema xmlns:xs='schema'>\r\n" +
				"</xs:schema>";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
		}
		
		[Test]
		public void GetFolds_QualifiedRootElementWithNamespace_FoldNameContainsFullyQualifiedElementNameExcludingNamespace()
		{
			string name = helper.GetFirstFoldName();
			Assert.AreEqual("<xs:schema>", name);
		}
		
		[Test]
		public void GetFolds_QualifiedRootElementWithNamespace_FoldRegionContainsRootElement()
		{
			DomRegion region = helper.GetFirstFoldRegion();
			
			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 13;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, region);
		}
	}
}
