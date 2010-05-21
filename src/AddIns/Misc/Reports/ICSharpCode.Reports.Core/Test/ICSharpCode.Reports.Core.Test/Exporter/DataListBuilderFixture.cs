/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.05.2010
 * Time: 18:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.Project.Interfaces;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Exporter
{
	[TestFixture]
	public class DataListBuilderFixture:ConcernOf<DataExportListBuilder>
	{
		ReportModel reportModel;
		
		[Test]
		public void Can_Create_ExportListBuilder()
		{
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			
			var reportModel = ReportModel.Create();
			
			DataExportListBuilder sut = new DataExportListBuilder(reportModel,dataManager);
			Assert.IsNotNull(sut);
			IDataNavigator n = sut.DataManager.GetNavigator;
			Assert.AreEqual(0,sut.DataManager.GetNavigator.CurrentRow);
		}
		
		
		[Test]
		public void PagesCollection_Should_Be_Not_Null()
		{
			Sut.WritePages();
			Assert.IsNotNull(Sut.Pages);
			Assert.AreEqual(1,Sut.Pages.Count);
			
		}
		
		
		[Test]
		public void Pagenumber_Should_Be_One()
		{
			Sut.WritePages();
			Assert.AreEqual(1,Sut.Pages.Count);
			Assert.AreEqual(1,Sut.Pages[0].PageNumber);
		}
		
		
		[Test]
		public void LocPos()
		{
			Sut.WritePages();
			Point retVal = new Point (50,50 + reportModel.ReportHeader.Size.Height);
			Assert.AreEqual(retVal.Y,Sut.PositionAfterRenderSection.Y);
		}
		
		
		[Test]
		public void Add_One_Item_In_ReportHeader()
		{
			var item = CreateSimpeTextItem();
			reportModel.ReportHeader.Items.Add(item);
			Sut.WritePages();
			BaseReportItem it = Sut.Pages[0].Items[0];
			Assert.IsNotNull(it);
			Assert.AreEqual(1,Sut.Pages[0].Items.Count);
		}
		
		
		[Test]
		public void Add_Container_In_ReportHeader()
		{
			BaseRowItem row = new BaseRowItem() {
				Location = new Point  (5,5)
			};
			row.Items.Add(CreateSimpeTextItem());
			reportModel.ReportHeader.Items.Add(row);
			Sut.WritePages();
			Assert.AreEqual(1,Sut.Pages[0].Items.Count);
			var checkRow = Sut.Pages[0].Items[0];
			Assert.IsAssignableFrom(typeof(BaseRowItem),checkRow);
			
			BaseReportItem checkItem = ((BaseRowItem)checkRow).Items[0];
			Assert.IsNotNull(checkItem);
		}
		
		
		private BaseReportItem CreateSimpeTextItem ()
		{
			BaseTextItem bt = new BaseTextItem();
			bt.Text = "MyText";
			return bt;
		}
		
		public override void Setup()
		{
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			
			 reportModel = ReportModel.Create();
			
			Sut = new DataExportListBuilder(reportModel,dataManager);
		}
		
	}
}
