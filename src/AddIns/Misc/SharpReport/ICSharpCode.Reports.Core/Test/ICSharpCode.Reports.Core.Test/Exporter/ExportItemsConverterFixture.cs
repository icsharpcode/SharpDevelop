/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.05.2010
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Exporter
{
	[TestFixture]
	public class ExportItemsConverterFixture
	{
		[Test]
		public void CanCreateExportItemsConverter()
		{
			IExportItemsConverter sut = new ExportItemsConverter();
			Assert.IsNotNull(sut);
		}
		
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void  ConvertSimpleItems_Throw_On_Null_Items()
		{
			IExportItemsConverter sut = new ExportItemsConverter();
			ExporterCollection ec = sut.ConvertSimpleItems(new Point (10,10),null);
			
		}
		 
		
		[Test]
		public void  ConvertSimpleItems_To_Valid_Collection()
		{
			ReportItemCollection ri = new ReportItemCollection();
			BaseReportItem r = new BaseTextItem(){
				Location = new Point (10,10),
				Size = new Size(20,100)
			};
			ri.Add(r);
			IExportItemsConverter sut = new ExportItemsConverter();
				
			ExporterCollection ec = sut.ConvertSimpleItems(new Point (10,10),ri);
			
			Assert.IsNotNull(ec);
			Assert.AreEqual(1,ec.Count);
		}
		
		
		[Test]
		public void  Convert_SimpleItems_Collection ()
		{
			ReportItemCollection ri = new ReportItemCollection();
			Point itemLocation = new Point (10,10);
			BaseReportItem r = new BaseTextItem(){
				Location = itemLocation,
				Size = new Size(20,100)
			};
			ri.Add(r);
			IExportItemsConverter sut = new ExportItemsConverter();
			
			Point offset = new Point(20,20);
			//
			Rectangle parentRectangle =  new Rectangle (50,50,700,50);
			sut.ParentRectangle = parentRectangle;
			
			ExporterCollection ec = sut.ConvertSimpleItems(offset,ri);
			
			BaseExportColumn be = ec[0];
			
//			this.ParentRectangle.Location.X + lineItem.StyleDecorator.Location.X,
//				                                             lineItem.StyleDecorator.Location.Y + offset.Y);
			
			
			Point resultLocation = new Point (parentRectangle.Location.X + itemLocation.X,itemLocation.Y + offset.Y);
			
			Assert.AreEqual(resultLocation,be.StyleDecorator.Location);
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
