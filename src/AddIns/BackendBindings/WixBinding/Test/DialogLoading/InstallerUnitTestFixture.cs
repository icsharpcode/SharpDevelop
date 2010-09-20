// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;

namespace WixBinding.Tests.DialogLoading
{
	[TestFixture]
	public class InstallerUnitTestFixture
	{
		[Test]
		public void UnitIsOneTwelthTheHeightOf10PointMsSansSerifFont()
		{
			using (Font sansSerifFont = new Font("MS Sans Serif", 10)) {
				double expectedFactor = sansSerifFont.Height / 12.0;
				Assert.AreEqual(expectedFactor, WixDialog.InstallerUnit);
			}
		}
	}
}
