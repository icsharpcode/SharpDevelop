// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
