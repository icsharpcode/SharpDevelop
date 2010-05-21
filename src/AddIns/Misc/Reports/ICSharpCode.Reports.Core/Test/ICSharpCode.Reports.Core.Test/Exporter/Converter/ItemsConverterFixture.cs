/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2010
 * Time: 19:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter.Converter;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Exporter.Converter
{
	[TestFixture]
	public class ItemsConverterFixture:ConcernOf<ItemsConverter>
	{
		[Test]
		public void ConvertSimpleItem()
		{
			BaseSection section = new BaseSection();
			section.Items.Add(CreateSimpeTextItem());
			Point point = new Point(1,1);
			ReportItemCollection result = Sut.Convert (section,section.Items);
			Assert.AreEqual(new Point(section.Size.Width,section.Size.Height),Sut.LocationAfterConvert);
			Assert.AreEqual(1,result.Count);
			
		}
		
		
		private BaseReportItem CreateSimpeTextItem ()
		{
			BaseTextItem bt = new BaseTextItem();
			bt.Text = "MyText";
			return bt;
		}
		
		
		public override void Setup()
		{
			Sut = new ItemsConverter();
		}
		
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			// TODO: Add Init code.
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
