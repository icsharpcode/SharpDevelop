// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Document
{
	[TestFixture]
	public class AddRootDirectoryWithNoProductElementTestFixture
	{
		[Test]
		public void AddRootDirectory()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDirectoryElement element = doc.AddRootDirectory();
			Assert.IsNotNull(element);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"</Wix>";
		}
	}
}
