// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class ShowElementAttributesInFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		string xml =
			"<root a=\"1st\" b=\"2nd\" c='3rd'>\r\n" +
			"</root>";
		
		[Test]
		public void GetFolds_ShowAttributesIsTrue_FoldNameIsElementNameWithAttributes()
		{
			GetFoldsWhenShowAttributesSetToTrue();
			string name = helper.GetFirstFoldName();
			Assert.AreEqual("<root a=\"1st\" b=\"2nd\" c='3rd'>", name);
		}

		[Test]
		public void GetFolds_ShowAttributesIsFalse_FoldNameIsElementNameOnly()
		{
			GetFoldsWhenShowAttributesSetToFalse();
			string name = helper.GetFirstFoldName();
			Assert.AreEqual("<root>", name);
		}
		
		[Test]
		public void GetFolds_ShowAttributesSetToTrue_FirstFoldRegionContainsRootElement()
		{
			GetFoldsWhenShowAttributesSetToTrue();
			
			DomRegion region = helper.GetFirstFoldRegion();

			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 8;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			Assert.AreEqual(expectedRegion, region);
		}
		
		void GetFoldsWhenShowAttributesSetToTrue()
		{
			helper = new XmlFoldParserHelper();
			helper.Options.ShowAttributesWhenFolded = true;
			GetFolds();
		}

		void GetFolds()
		{
			helper.CreateParser();
			helper.GetFolds(xml);
		}

		void GetFoldsWhenShowAttributesSetToFalse()
		{
			helper = new XmlFoldParserHelper();
			helper.Options.ShowAttributesWhenFolded = false;
			GetFolds();
		}
	}
}
