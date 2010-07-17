/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 19.01.2009
 * Zeit: 11:06
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseGraphicItemFixture
	{
		[Test]
		public void DefaultConstructur()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.IsNotNull(bgi,"BaseGraphicItem should not be null");
		}
		
		[Test]
		public void DefaultSettings()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.AreEqual(1,bgi.Thickness,"Thickness should be '1'");
			Assert.AreEqual(System.Drawing.Drawing2D.DashStyle.Solid,bgi.DashStyle,
			                "dashStyle should be 'Solid'");
			Assert.IsFalse(bgi.DrawBorder,"DrawBorder should be 'false'");
		}
		
		[Test] 
		public void ToStringOverride ()
		{
			BaseGraphicItem bgi = new BaseGraphicItem();
			Assert.AreEqual ("BaseGraphicItem",bgi.ToString(),"ToString override");
		}
		
	}
}
