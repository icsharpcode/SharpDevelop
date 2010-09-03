// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class SystemTypesLoadTest : TestHelper
	{
		[Test]
		public void Int32()
		{
			TestLoading(@"<Int32 xmlns=""clr-namespace:System;assembly=mscorlib"">3</Int32>");
		}
		
		[Test]
		public void Double()
		{
			TestLoading(@"<Double xmlns=""clr-namespace:System;assembly=mscorlib"">3.1</Double>");
		}
		
		[Test]
		public void String()
		{
			TestLoading(@"<String xmlns=""clr-namespace:System;assembly=mscorlib"">text</String>");
		}
	}
}
