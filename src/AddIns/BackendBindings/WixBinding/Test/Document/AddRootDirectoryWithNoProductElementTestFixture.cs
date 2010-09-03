// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
