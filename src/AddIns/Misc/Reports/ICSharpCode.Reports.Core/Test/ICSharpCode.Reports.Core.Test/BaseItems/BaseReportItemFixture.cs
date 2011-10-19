// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseReportItemFixture
	{
		[Test]
		public void DefaultConstructor()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.IsNotNull(bri);
			Assert.AreEqual(System.Drawing.Color.Black,bri.ForeColor);
		}
		
		[Test]
		public void DefaultForeColor ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual(System.Drawing.Color.Black,bri.ForeColor,"Forecolor should be black");
		}
		
		
		[Test]
		public void DefaultBackColor ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual(GlobalValues.DefaultBackColor,bri.BackColor);
		}
		
		[Test]
		public void DrawBorder()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.IsFalse(bri.DrawBorder,"DrawBorder should BeforePrintEventArgs false");
		}
		
		
		[Test]
		public void DefaultFont ()
		{
			BaseReportItem bri = new BaseReportItem();
			Assert.AreEqual (GlobalValues.DefaultFont,bri.Font);
		}
		
		
	}
}
