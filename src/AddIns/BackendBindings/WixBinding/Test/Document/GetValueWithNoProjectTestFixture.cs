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
	public class GetValueWithNoProjectTestFixture
	{
		[Test]
		public void NullReturned()
		{
			WixDocument doc = new WixDocument();
			Assert.IsNull(doc.GetValue("test"));
		}
	}
}
