// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
