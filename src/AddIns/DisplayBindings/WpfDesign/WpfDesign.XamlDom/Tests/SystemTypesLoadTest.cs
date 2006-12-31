// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>


using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.XamlDom.Tests
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
