// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class ShowElementAttributesForElementWithNoAttributesTestFixture
	{
		XmlFoldParserHelper helper;
		
		[Test]
		public void GetFolds_RootElementOnlyWithNoAttributes_FoldNameIsElementNameOnlyWithNoExtraSpaceAtTheEnd()
		{
			string xml = 
				"<root>\r\n" +
				"</root>";
			
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
			
			string name = helper.GetFirstFoldName();
		
			Assert.AreEqual("<root>", name);
		}
	}
}
