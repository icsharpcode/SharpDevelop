// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
