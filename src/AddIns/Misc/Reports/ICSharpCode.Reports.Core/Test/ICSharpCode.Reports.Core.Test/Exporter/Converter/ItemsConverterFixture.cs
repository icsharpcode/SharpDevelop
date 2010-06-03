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
		
		[Test]
		public void Check_Location_Of_Item_In_Section ()
		{
			BaseSection section = new BaseSection();
			section.Location = new Point (10,10);
			
			var textItem = CreateSimpeTextItem();
			section.Items.Add(textItem);
			
			Point checkPoint = new Point(section.Location.X + textItem.Location.X,section.Location.Y + textItem.Location.Y);
			
			ReportItemCollection result = Sut.Convert (section,section.Items);
			var checkItem = result[0];
			
			Assert.AreEqual(checkPoint,checkItem.Location);
		}
			
		
		[Test]
		public void Check_Location_After_Convert ()
		{
				BaseSection section = new BaseSection();
			section.Location = new Point (10,10);
			
			var textItem = CreateSimpeTextItem();
			section.Items.Add(textItem);
			
			Point checkPoint = new Point(section.Location.X + section.Size.Width,section.Location.Y + section.Size.Height);
			
			ReportItemCollection result = Sut.Convert (section,section.Items);
			
			Assert.AreEqual(checkPoint,Sut.LocationAfterConvert);
		}
		
		
		private BaseReportItem CreateSimpeTextItem ()
		{
			BaseTextItem bt = new BaseTextItem();
			bt.Location = new Point(10,10);
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
