// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			var bt = new BaseTextItem();
			var bec = bt.CreateExportColumn();
			Assert.IsNotNull(bec);
		}
		
		
		[Test]
		public void TypeofExportShouldBeExportText ()
		{
			var bt = new BaseTextItem();
			var bec = bt.CreateExportColumn();
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
