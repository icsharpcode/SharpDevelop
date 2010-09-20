// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			IWixPropertyValueProvider provider = (IWixPropertyValueProvider)doc;
			Assert.IsNull(provider.GetValue("test"));
		}
	}
}
