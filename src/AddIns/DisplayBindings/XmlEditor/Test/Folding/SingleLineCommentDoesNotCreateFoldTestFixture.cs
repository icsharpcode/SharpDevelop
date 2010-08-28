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
	public class SingleLineCommentDoesNotCreateFoldTestFixture
	{
		XmlFoldParserHelper helper;
		
		[Test]
		public void GetFolds_XmlOnlyHasSingleLineComment_NullReturned()
		{
			string xml = "<!-- single line comment -->";
			helper = new XmlFoldParserHelper();
			helper.CreateParser();
			helper.GetFolds(xml);
			
			Assert.IsNull(helper.Folds);
		}
	}
}
