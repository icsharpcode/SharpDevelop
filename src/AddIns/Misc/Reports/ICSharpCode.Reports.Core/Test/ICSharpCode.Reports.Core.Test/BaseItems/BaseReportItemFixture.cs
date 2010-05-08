/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.12.2008
 * Zeit: 18:39
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
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
