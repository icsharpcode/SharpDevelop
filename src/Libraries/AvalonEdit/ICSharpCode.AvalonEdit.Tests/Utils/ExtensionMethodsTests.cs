// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit.Utils.Tests
{
	[TestFixture]
	public class ExtensionMethodsTests
	{
		[Test]
		public void ZeroIsNotCloseToOne()
		{
			Assert.IsFalse(0.0.IsClose(1));
		}
		
		[Test]
		public void ZeroIsCloseToZero()
		{
			Assert.IsTrue(0.0.IsClose(0));
		}
		
		[Test]
		public void InfinityIsCloseToInfinity()
		{
			Assert.IsTrue(double.PositiveInfinity.IsClose(double.PositiveInfinity));
		}
		
		[Test]
		public void NaNIsNotCloseToNaN()
		{
			Assert.IsFalse(double.NaN.IsClose(double.NaN));
		}
	}
}
