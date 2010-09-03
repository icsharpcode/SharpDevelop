// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class FoldParserParsesInvalidXmlTestFixture
	{
		XmlFoldParserHelper helper;		
		
		[Test]
		public void GetFolds_WhenInvalidXml_DoesNotThrowException()
		{
			string xml =
				"<root\r\n" +
				"    <child>\r\n" +
				"</root>";
	
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			Assert.DoesNotThrow(delegate { helper.GetFolds(xml); });
		}
	}
}
