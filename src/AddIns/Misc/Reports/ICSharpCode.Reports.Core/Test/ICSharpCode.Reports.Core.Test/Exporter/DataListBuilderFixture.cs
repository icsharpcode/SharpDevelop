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
using ICSharpCode.Reports.Core.Exporter.Converter;
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
		
		#region Render empty ReportModel
		
		[Test]
		public void PagesCollection_Should_Be_Not_Null()
		{
			Sut.WritePages();
			Assert.IsNotNull(Sut.Pages);
			Assert.AreEqual(1,Sut.Pages.Count);
			
		}
		
		
		
		[Test]
		public void PageNumber_Should_Be_One()
		{
			Sut.WritePages();
			Assert.AreEqual(1,Sut.Pages.Count);
			Assert.AreEqual(1,Sut.Pages[0].PageNumber);
		}
		
		
		#endregion
		
		#region Events
		
		[Test]
		public void SectionRenderingEvent_Should_Fire()
		{
			
			bool eventFired = false;
			Sut.SectionRendering += delegate { { eventFired = true;}};
			Sut.WritePages();
			Assert.IsTrue(eventFired);
		}
		
		[Test]
		public void PageCreatedEvent_Should_Fired ()
		{
			bool eventFired = false;
			Sut.PageCreated += delegate { { eventFired = true;}};
			Sut.WritePages();
			
			Assert.IsTrue(eventFired);
		}
		
		
	
		#endregion
		
		
		#region Location after rendering
		
		[Test]
		public void Location_After_Render_Empty_Section()
		{
			Sut.WritePages();
			BaseSection section = reportModel.ReportHeader;
			Point retVal = new Point (section.Location.X + section.Size.Width,section.Location.Y + section.Size.Height);
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
			
			
			var item = Sut.Pages[0].Items[0];
			Assert.IsAssignableFrom(typeof(BaseRowItem),item,"10");
			Assert.AreEqual(1,((BaseRowItem)item).Items.Count,"20");
			
			var textItem = ((BaseRowItem)item).Items[0];
			Assert.IsAssignableFrom(typeof(BaseTextItem),textItem,"30");
			
		}
		
		#endregion
		
		
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
			var itemsConverter = new ItemsConverter();
			
			Sut = new DataExportListBuilder(reportModel,dataManager,itemsConverter);
		}
		
	}
}
