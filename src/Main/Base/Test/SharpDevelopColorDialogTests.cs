// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using System;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class SharpDevelopColorDialogTests
	{
		[Test]
		public void NullString()
		{
			Assert.IsNull(SharpDevelopColorDialog.CustomColorsFromString(null));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.IsNull(SharpDevelopColorDialog.CustomColorsFromString(String.Empty));
		}
		
		[Test]
		public void OneColorInString()
		{
			int[] colors = SharpDevelopColorDialog.CustomColorsFromString("34");
			Assert.AreEqual(1, colors.Length);
			Assert.AreEqual(34, colors[0]);
		}
		
		[Test]
		public void TwoColorsInString()
		{
			int[] colors = SharpDevelopColorDialog.CustomColorsFromString("20|30");
			Assert.AreEqual(2, colors.Length);
			Assert.AreEqual(20, colors[0]);
			Assert.AreEqual(30, colors[1]);
		}
		
		[Test]
		public void SecondColorIsInvalid()
		{
			int[] colors = SharpDevelopColorDialog.CustomColorsFromString("20|Test");
			Assert.AreEqual(1, colors.Length);
			Assert.AreEqual(20, colors[0]);
		}
		
		[Test]
		public void FirstColorIsInvalid()
		{
			int[] colors = SharpDevelopColorDialog.CustomColorsFromString("Test|20");
			Assert.AreEqual(1, colors.Length);
			Assert.AreEqual(20, colors[0]);
		}

		[Test]
		public void NullIntColorsArray()
		{
			Assert.AreEqual(String.Empty, SharpDevelopColorDialog.CustomColorsToString(null));
		}
		
		[Test]
		public void OneCustomColor()
		{
			int[] colors = new int[] { 10 };
			Assert.AreEqual("10", SharpDevelopColorDialog.CustomColorsToString(colors));
		}
		
		[Test]
		public void TwoCustomColors()
		{
			int[] colors = new int[] { 10, 20 };
			Assert.AreEqual("10|20", SharpDevelopColorDialog.CustomColorsToString(colors));
		}
	}
}
