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
using ICSharpCode.Reports.Core.Exporter;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.BaseItems
{
	[TestFixture]
	public class BaseRowItemFixture
	{
		[Test]
		public void DefaultConstructur()
		{
			BaseRowItem bri = new BaseRowItem();
			Assert.IsNotNull(bri,"baseRowItem should not be null");
		}
		
		[Test]
		public void DefaultSettings()
		{
			BaseRowItem bri = new BaseRowItem();
			Assert.IsNotNull(bri.Items,"Items should not be 'null'");
			Assert.AreEqual(0,bri.Items.Count,"Items.Count should be '0'");
		}
		
		[Test] 
		public void ToStringOverride ()
		{
			BaseRowItem bri = new BaseRowItem();
			Assert.AreEqual ("BaseRowItem",bri.ToString(),"ToString override");
		}
		
		#region Exporter
		
		[Test]
		public void ExportColumnIsNotNull ()
		{
			BaseRowItem bri = new BaseRowItem();
			var bec = bri.CreateExportColumn();
			Assert.IsNotNull(bec);
		}
		
		[Test]
		public void TypeofExportShouldBeExportContainer ()
		{
			BaseRowItem bri = new BaseRowItem();
			var bec = bri.CreateExportColumn();
			Type t = typeof(ExportContainer);
			Assert.AreEqual(t,bec.GetType(),"Type should be 'ExportContainer");
		}
		
		[Test]
		public void RowValuesEqualExportedText ()
		{
			BaseRowItem bri = new BaseRowItem();
			ExportContainer ec = (ExportContainer)bri.CreateExportColumn();
			Assert.AreEqual (0,ec.Items.Count,"Items.Count should BeforePrintEventArgs '0'");
			Assert.AreEqual(bri.Size,ec.StyleDecorator.Size);                 
		}
		#endregion
	}
}
