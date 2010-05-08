/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 07.12.2008
 * Zeit: 18:28
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using NUnit.Framework;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseTextItemFixture
	{
		[Test]
		public void DefaultConstructor()
		{
			BaseTextItem bti = new BaseTextItem();
			Assert.IsNotNull (bti);
		}
		
		
		[Test]
		public void DefaultSettings()
		{
			BaseTextItem bti = new BaseTextItem();
			Assert.IsTrue(String.IsNullOrEmpty(bti.Text),"Text should be 'String.IsNullOrEmpty'");
			Assert.AreEqual(ContentAlignment.TopLeft,bti.ContentAlignment);
			Assert.AreEqual(StringTrimming.None,bti.StringTrimming);
		}
		
		
		[Test] 
		public void ToStringOverride ()
		{
			BaseTextItem bti = new BaseTextItem();
			Assert.AreEqual ("BaseTextItem",bti.ToString(),"ToString override");
		}
		
		[Test]
		public void PlainText ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.Text = "abc";
			Assert.AreEqual("abc",bti.Text);
		}
		
		[Test]
		public void GetSetContendAlignment ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.ContentAlignment = System.Drawing.ContentAlignment.BottomCenter;
			Assert.AreEqual(System.Drawing.ContentAlignment.BottomCenter,
			                bti.ContentAlignment,
			                "ContendAlignement should be equal <ContentAlignment.BottomCenter>");
		}
		
		
		[Test]
		public void GetSetStringTrimming ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.StringTrimming = StringTrimming.Character;
			Assert.AreEqual(StringTrimming.Character,bti.StringTrimming,
			                "StringTrimming should be equal <StringTrimming.Character>");
		}
		
		[Test]
		public void GetSetStringFormat ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.StringTrimming = StringTrimming.Character;
			bti.ContentAlignment = System.Drawing.ContentAlignment.BottomCenter;
			StringFormat f = bti.StringFormat;
			Assert.AreEqual(StringAlignment.Center,f.Alignment);
		}
		
		
		[Test]
		public void GetSetFormatString ()
		{
			BaseTextItem bti = new BaseTextItem();
			bti.FormatString = "H";
			Assert.AreEqual("H",bti.FormatString,
			                "StringTrimming should be equal <H>");
		}
		
		
		#region Exporter
		
		[Test]
		public void ExportColumnIsNotNull ()
		{
			BaseTextItem bt = new BaseTextItem();
			BaseExportColumn bec = bt.CreateExportColumn();
			Assert.IsNotNull(bec);
		}
		
		
		[Test]
		public void TypeofExportShouldBeExportText ()
		{
			BaseTextItem bt = new BaseTextItem();
			BaseExportColumn bec = bt.CreateExportColumn();
			Type t = typeof(ExportText);
			Assert.AreEqual(t,bec.GetType(),"Type should be 'ExportText");
		}
		
		[Test]
		public void TextValueEqualExportedText ()
		{
			BaseTextItem bt = new BaseTextItem();
			bt.Text = "Text";
			ExportText bec = (ExportText)bt.CreateExportColumn();
			bec.Text = bt.Text;
		}
		
		#endregion
		
		
	}
}
